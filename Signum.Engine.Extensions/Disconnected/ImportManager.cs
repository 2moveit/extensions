﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Disconnected;
using Signum.Engine.Maps;
using Signum.Engine.Disconnected;
using Signum.Engine.DynamicQuery;
using Signum.Entities;
using Signum.Entities.Reflection;
using Signum.Utilities;
using System.IO;
using System.Data.Common;
using Signum.Engine.Linq;
using Signum.Utilities.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Linq.Expressions;
using Signum.Engine.Exceptions;
using Signum.Entities.Exceptions;
using Signum.Engine.Authorization;
using System.Threading;
using System.Reflection;
using Signum.Utilities.DataStructures;

namespace Signum.Engine.Disconnected
{
    public class ImportManager
    {
        class UploadTable
        {
            public Type Type;
            public Table Table;
            public IDisconnectedStrategy Strategy;
        }

        public void Initialize()
        {
            var tables = Schema.Current.Tables.Values
                .Select(t => new UploadTable { Type = t.Type, Table = t, Strategy = DisconnectedLogic.GetStrategy(t.Type) })
                .Where(p => p.Strategy.Upload != Upload.None)
                .ToList();

            var dic = tables.ToDictionary(a => a.Table);

            DirectedGraph<Table> graph = DirectedGraph<Table>.Generate(
                dic.Keys,
                t => t.DependentTables().Select(a => a.Key).Where(tab => dic.ContainsKey(tab)));

            var feedback = graph.FeedbackEdgeSet();

            foreach (var edge in feedback.Edges)
            {
                var strategy = dic[edge.From].Strategy;

                if (strategy.DisableForeignKeys == null)
                    strategy.DisableForeignKeys = true;
            }
           
            foreach (var item in dic.Values.Where(a => a.Strategy.DisableForeignKeys == null))
                item.Strategy.DisableForeignKeys = false;

            graph.RemoveAll(feedback.Edges);

            uploadTables = graph.CompilationOrder().Select(t => dic[t]).ToList();
        }

        List<UploadTable> uploadTables;


        class RunningImports
        {
            public Task Task;
            public CancellationTokenSource CancelationSource;
        }

        Dictionary<Lite<DisconnectedImportDN>, RunningImports> runningExports = new Dictionary<Lite<DisconnectedImportDN>, RunningImports>();

        public virtual Lite<DisconnectedImportDN> BeginImportDatabase(DisconnectedMachineDN machine, Stream file)
        {
            Lite<DisconnectedImportDN> import = new DisconnectedImportDN
            {
                Machine = machine.ToLite(),
                Copies = uploadTables.Select(t => new DisconnectedImportTableDN
                {
                    Type = t.Type.ToTypeDN().ToLite(),
                    DisableForeignKeys = t.Strategy.DisableForeignKeys.Value,
                }).ToMList()
            }.Save().ToLite();

            using (FileStream fs = File.OpenWrite(BackupNetworkFileName(machine, import)))
            {
                file.CopyTo(fs);
                file.Close();
            }

            var threadContext = Statics.ExportThreadContext();

            var cancelationSource = new CancellationTokenSource();

            var token = cancelationSource.Token;

            var task = Task.Factory.StartNew(() =>
            {
                Statics.ImportThreadContext(threadContext);

                try
                {
                    string connectionString;

                    using (token.MeasureTime(l => import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { RestoreDatabase = l })))
                    {
                        DropDatabaseIfExists(machine);
                        connectionString = RestoreDatabase(machine, import);
                    }

                    var newDatabase = new SqlConnector(connectionString, Schema.Current, DynamicQueryManager.Current);

                    using (token.MeasureTime(l => import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { SynchronizeSchema = l })))
                    using (Connector.Override(newDatabase))
                    {
                        var script = Administrator.TotalSynchronizeScript(interactive: false);

                        if (script != null)
                        {
                            string fileName = BackupNetworkFileName(machine, import) + ".sql";
                            script.Save(fileName);
                            throw new InvalidOperationException("The schema has changed since the last export. A schema sync script has been saved on: {0}".Formato(fileName));
                        }
                    }

                    try
                    {
                        using (token.MeasureTime(l => import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { DisableForeignKeys = l })))
                            foreach (var item in uploadTables.Where(u => u.Strategy.DisableForeignKeys.Value))
                            {
                                DisableForeignKeys(item.Table);
                            }

                        foreach (var tuple in uploadTables)
                        {
                            ImportResult result = null;
                            using (token.MeasureTime(l =>
                            {
                                if (result != null)
                                    ImportTableQuery(import, tuple.Type.ToTypeDN()).UnsafeUpdate(s => 
                                        new MListElement<DisconnectedImportDN, DisconnectedImportTableDN>
                                        {
                                            Element = new DisconnectedImportTableDN
                                            {
                                                CopyTable = l,
                                                DisableForeignKeys = tuple.Strategy.DisableForeignKeys.Value,
                                                InsertedRows = result.Inserted,
                                                UpdatedRows = result.Updated,
                                            }
                                        });
                            }))
                            {
                                result = tuple.Strategy.Importer.Import(machine, tuple.Table, tuple.Strategy, newDatabase);
                            }
                        }

                        using (token.MeasureTime(l => import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { EnableForeignKeys = l })))
                            DisconnectedLogic.UnsafeUnlock(machine.ToLite());
                    }
                    finally
                    {
                        using (token.MeasureTime(l => import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { EnableForeignKeys = l })))
                            foreach (var item in uploadTables.Where(u => u.Strategy.DisableForeignKeys.Value))
                            {
                                EnableForeignKeys(item.Table);
                            }
                    }

                    using (token.MeasureTime(l => import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { DropDatabase = l })))
                        DropDatabase(newDatabase);

                    token.ThrowIfCancellationRequested();

                    import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { State = DisconnectedImportState.Completed, Total = s.CalculateTotal() });

                    machine.IsOffline = false;
                    machine.Save();
                }
                catch (Exception e)
                {
                    var ex = e.LogException();

                    import.InDB().UnsafeUpdate(s => new DisconnectedImportDN { Exception = ex.ToLite(), State = DisconnectedImportState.Error });
                }
                finally
                {
                    runningExports.Remove(import);
                }
            });


            runningExports.Add(import, new RunningImports { Task = task, CancelationSource = cancelationSource });

            return import;
        }

        private void DropDatabaseIfExists(DisconnectedMachineDN machine)
        {
            DisconnectedTools.DropIfExists(DatabaseName(machine));
        }

        private void DropDatabase(SqlConnector newDatabase)
        {
            DisconnectedTools.DropDatabase(newDatabase.DatabaseName());
        }
        
        protected virtual void EnableForeignKeys(Table table)
        {
            DisconnectedTools.EnableForeignKeys(table);

            foreach (var rt in table.RelationalTables())
                DisconnectedTools.EnableForeignKeys(rt);
        }

        protected virtual void DisableForeignKeys(Table table)
        {
            DisconnectedTools.DisableForeignKeys(table);

            foreach (var rt in table.RelationalTables())
                DisconnectedTools.DisableForeignKeys(rt);
        }

        private string RestoreDatabase(DisconnectedMachineDN machine, Lite<DisconnectedImportDN> import)
        {
            string backupFileName = Path.Combine(DisconnectedLogic.BackupFolder, BackupFileName(machine, import));

            string databaseName = DatabaseName(machine);

            DisconnectedTools.RestoreDatabase(databaseName,
                backupFileName,
                DatabaseFileName(machine),
                DatabaseLogFileName(machine));

            return ((SqlConnector)Connector.Current).ConnectionString.Replace(Connector.Current.DatabaseName(), databaseName);
        }

        protected virtual string DatabaseFileName(DisconnectedMachineDN machine)
        {
            return Path.Combine(DisconnectedLogic.DatabaseFolder, Connector.Current.DatabaseName() + "_Import_" + machine.MachineName + ".mdf");
        }

        protected virtual string DatabaseLogFileName(DisconnectedMachineDN machine)
        {
            return Path.Combine(DisconnectedLogic.DatabaseFolder, Connector.Current.DatabaseName() + "_Import_" + machine.MachineName + "_Log.ldf");
        }

        protected virtual string DatabaseName(DisconnectedMachineDN machine)
        {
            return Connector.Current.DatabaseName() + "_Import_" + machine.MachineName;
        }

        public virtual string BackupNetworkFileName(DisconnectedMachineDN machine, Lite<DisconnectedImportDN> import)
        {
            return Path.Combine(DisconnectedLogic.BackupNetworkFolder, BackupFileName(machine, import));
        }

        protected virtual string BackupFileName(DisconnectedMachineDN machine, Lite<DisconnectedImportDN> import)
        {
            return "{0}.{1}.Import.{2}.bak".Formato(Connector.Current.DatabaseName(), machine.MachineName.ToString(), import.Id);
        }

        private IQueryable<MListElement<DisconnectedImportDN, DisconnectedImportTableDN>> ImportTableQuery(Lite<DisconnectedImportDN> import, TypeDN type)
        {
            return Database.MListQuery((DisconnectedImportDN s) => s.Copies).Where(dst => dst.Parent.ToLite() == import && dst.Element.Type.RefersTo(type));
        }
    }

    public interface ICustomImporter
    {
        ImportResult Import(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase);
    }

    public class BasicImporter<T> : ICustomImporter where T : IdentifiableEntity
    {
        public virtual ImportResult Import(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase)
        {
            int inserts = Insert(machine, table, strategy, newDatabase);

            return new ImportResult { Inserted = inserts, Updated = 0 };
        }

        protected virtual int Insert(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase)
        {
            var interval = GetNewIdsInterval(table, machine, newDatabase);

            if (interval == null)
                return 0;

            using (Transaction tr = new Transaction())
            {
                int result;

                using (DisconnectedTools.DisableIdentityRestoreSeed(table))
                {
                    SqlPreCommandSimple sql = InsertTableScript(table, newDatabase, interval);

                    result = Executor.ExecuteNonQuery(sql);
                }

                foreach (var rt in table.RelationalTables())
                {
                    SqlPreCommandSimple sql = InsertRelationalTableScript(table, newDatabase, interval, rt);

                    Executor.ExecuteNonQuery(sql);
                }

                return tr.Commit(result);
            }
        }

        protected virtual SqlPreCommandSimple InsertRelationalTableScript(Table table, SqlConnector newDatabase, Interval<int>? interval, RelationalTable rt)
        {
            ParameterBuilder pb = Connector.Current.ParameterBuilder;
            var columns = rt.Columns.Values.Where(c => !c.Identity);

            string command = @"INSERT INTO {0} ({1})
SELECT {2}
FROM {3} as [relationalTable]
JOIN {4} [table] on [relationalTable].{5} = [table].Id
WHERE @min <= [table].Id AND [table].Id < @max".Formato(
rt.Name.SqlScape(),
columns.ToString(c => c.Name.SqlScape(), ", "),
columns.ToString(c => "[relationalTable]." + c.Name.SqlScape(), ", "),
Prefix(newDatabase) + rt.Name.SqlScape(),
Prefix(newDatabase) + table.Name.SqlScape(),
rt.BackReference.Name.SqlScape());

            var sql = new SqlPreCommandSimple(command, new List<DbParameter>()
            {
                pb.CreateParameter("@min", interval.Value.Min, typeof(int)),
                pb.CreateParameter("@max", interval.Value.Max, typeof(int)),
            });
            return sql;
        }

        protected virtual SqlPreCommandSimple InsertTableScript(Table table, SqlConnector newDatabase, Interval<int>? interval)
        {
            ParameterBuilder pb = Connector.Current.ParameterBuilder;
            string command = @"INSERT INTO {0} ({1})
SELECT {2}
FROM {3} as [table]
WHERE @min <= [table].Id AND [table].Id < @max".Formato(
table.Name.SqlScape(),
table.Columns.Keys.ToString(a => a.SqlScape(), ", "),
table.Columns.Keys.ToString(a => "[table]." + a.SqlScape(), ", "),
Prefix(newDatabase) + table.Name.SqlScape());

            var sql = new SqlPreCommandSimple(command, new List<DbParameter>()
            {
                pb.CreateParameter("@min", interval.Value.Min, typeof(int)),
                pb.CreateParameter("@max", interval.Value.Max, typeof(int)),
            });
            return sql;
        }

        protected virtual string Prefix(SqlConnector newDatabase)
        {
            return newDatabase.DatabaseName().SqlScape() + ".dbo.".Formato();
        }

        protected virtual Interval<int>? GetNewIdsInterval(Table table, DisconnectedMachineDN machine, SqlConnector newDatabase)
        {
            int? maxOther;
            using (Connector.Override(newDatabase))
                maxOther = DisconnectedTools.MaxIdInRange(table, machine.SeedMin, machine.SeedMax);

            if (maxOther == null)
                return null;

            int? max = DisconnectedTools.MaxIdInRange(table, machine.SeedMin, machine.SeedMax);
            if (max != null && max == maxOther)
                return null;

            return new Interval<int>((max + 1) ?? machine.SeedMin, machine.SeedMax);
        }
    }

    public class UpdateImporter<T> : BasicImporter<T> where T : IdentifiableEntity, IDisconnectedEntity
    {
        public override ImportResult Import(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase)
        {
            int inserts = Insert(machine, table, strategy, newDatabase);

            int update = strategy.Upload == Upload.Subset ? Update(machine, table, strategy, newDatabase) : 0;

            return new ImportResult { Inserted = inserts, Updated = update };
        }

        protected virtual int Update(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase)
        {
            using (Transaction tr = new Transaction())
            {
                SqlPreCommandSimple command = UpdateTableScript(machine, table, newDatabase);

                int result = Executor.ExecuteNonQuery(command);

                foreach (var rt in table.RelationalTables())
                {
                    SqlPreCommandSimple delete = DeleteUpdatedRelationalTableScript(machine, table, rt, newDatabase);

                    Executor.ExecuteNonQuery(delete);

                    SqlPreCommandSimple insert = InsertUpdatedRelationalTableScript(rt, machine, table, newDatabase);

                    Executor.ExecuteNonQuery(insert);
                }

                return tr.Commit(result);
            }
        }

        protected virtual SqlPreCommandSimple InsertUpdatedRelationalTableScript(RelationalTable rt, DisconnectedMachineDN machine, Table table, SqlConnector newDatabase)
        {
            string prefix = Prefix(newDatabase);
            ParameterBuilder pb = Connector.Current.ParameterBuilder;
            var columns = rt.Columns.Values.Where(c => !c.Identity);

            var insert = new SqlPreCommandSimple(@"INSERT INTO {0} ({1})
SELECT {2}
FROM {3} as [relationalTable]
INNER JOIN {4} as [table] ON [relationalTable].{5} = [table].id".Formato(
            rt.Name.SqlScape(),
            columns.ToString(c => c.Name.SqlScape(), ", "),
            columns.ToString(c => "[relationalTable]." + c.Name.SqlScape(), ", "),
            prefix + rt.Name.SqlScape(),
            prefix + table.Name.SqlScape(),
            rt.BackReference.Name.SqlScape()) + GetUpdateWhere(table), new List<DbParameter> { pb.CreateParameter("@machineId", machine.Id, typeof(int)) });
            return insert;
        }

        protected virtual SqlPreCommandSimple DeleteUpdatedRelationalTableScript(DisconnectedMachineDN machine, Table table, RelationalTable rt, SqlConnector newDatabase)
        {
            ParameterBuilder pb = Connector.Current.ParameterBuilder;

            var delete = new SqlPreCommandSimple(@"DELETE {0}
FROM {0}
INNER JOIN {1} as [table] ON {0}.{2} = [table].id".Formato(
                rt.Name.SqlScape(),
                Prefix(newDatabase) + table.Name.SqlScape(),
                rt.BackReference.Name.SqlScape()) + GetUpdateWhere(table), new List<DbParameter> { pb.CreateParameter("@machineId", machine.Id, typeof(int)) });
            return delete;
        }

        protected virtual SqlPreCommandSimple UpdateTableScript(DisconnectedMachineDN machine, Table table, SqlConnector newDatabase)
        {
            ParameterBuilder pb = Connector.Current.ParameterBuilder;

            var command = new SqlPreCommandSimple(@"UPDATE {0} SET
{2}
FROM {0}
INNER JOIN {1} as [table] ON {0}.id = [table].id
".Formato(
 table.Name.SqlScape(),
 Prefix(newDatabase) + table.Name.SqlScape(),
 table.Columns.Values.Where(c => !c.PrimaryKey).ToString(c => "   {0}.{1} = [table].{1}".Formato(table.Name.SqlScape(), c.Name), ",\r\n")) + GetUpdateWhere(table),
 new List<DbParameter> { pb.CreateParameter("@machineId", machine.Id, typeof(int)) });
            return command;
        }

        protected virtual string GetUpdateWhere(Table table)
        {
            var where = "\r\nWHERE [table].{0} = @machineId AND [table].{1} != [table].{2}".Formato(
                ((FieldReference)table.GetField(piDisconnectedMachine)).Name.SqlScape(),
                ((FieldValue)table.GetField(piTicks)).Name.SqlScape(),
                ((FieldValue)table.GetField(piLastOnlineTicks)).Name.SqlScape());
            return where;
        }

        static PropertyInfo piDisconnectedMachine = ReflectionTools.GetPropertyInfo((IDisconnectedEntity de) => de.DisconnectedMachine);
        static PropertyInfo piTicks = ReflectionTools.GetPropertyInfo((IDisconnectedEntity de) => de.Ticks);
        static PropertyInfo piLastOnlineTicks = ReflectionTools.GetPropertyInfo((IDisconnectedEntity de) => de.LastOnlineTicks);
    }

    public class ImportResult
    {
        public int Inserted;
        public int Updated;
    }
}
