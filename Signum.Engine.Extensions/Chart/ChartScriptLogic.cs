﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Maps;
using System.Reflection;
using Signum.Entities.Chart;
using Signum.Engine.Operations;
using Signum.Entities;
using Signum.Utilities;
using System.IO;
using System.Xml.Linq;
using Signum.Engine.Authorization;
using Signum.Engine.Cache;

namespace Signum.Engine.Chart
{
    public static class ChartScriptLogic
    {
        public static ResetLazy<Dictionary<string, ChartScriptDN>> Scripts { get; private set; }

        internal static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                sb.Include<ChartScriptDN>();

                dqm.RegisterQuery(typeof(ChartScriptDN), () =>
                    from uq in Database.Query<ChartScriptDN>()
                    select new
                    {
                        Entity = uq,
                        uq.Id,
                        uq.Name,
                        uq.GroupBy,
                        uq.Columns.Count,
                        uq.Icon,
                    });

                Scripts = sb.GlobalLazy(() => Database.Query<ChartScriptDN>().ToDictionary(a=>a.Name),
                    new InvalidateWith(typeof(ChartScriptDN)));

                RegisterOperations();
            }
        }

        private static void RegisterOperations()
        {
            new Graph<ChartScriptDN>.Execute(ChartScriptOperation.Save)
            {
                AllowsNew = true,
                Lite = false,
                Execute = (cs, _) => { }
            }.Register();

            new Graph<ChartScriptDN>.ConstructFrom<ChartScriptDN>(ChartScriptOperation.Clone)
            {
                Construct = (cs, _) => new ChartScriptDN
                {
                    Name = cs.Name,
                    GroupBy = cs.GroupBy,
                    Icon = cs.Icon,
                    Columns = cs.Columns.Select(col => new ChartScriptColumnDN
                    {
                        ColumnType = col.ColumnType,
                        DisplayName = col.DisplayName,
                        IsGroupKey = col.IsGroupKey,
                        IsOptional = col.IsOptional,
                    }).ToMList(),
                    Script = cs.Script,
                }
            }.Register();


            new Graph<ChartScriptDN>.Delete(ChartScriptOperation.Delete)
            {
                CanDelete = c => Database.Query<UserChartDN>().Any(a => a.ChartScript == c) ? "There are {0} in the database using {1}".Formato(typeof(UserChartDN).NicePluralName(), c) : null,
                Delete = (c, _) => c.Delete(),
            }.Register();
        }

        public static void ImportExportChartScripts()
        {
            ImportExportChartScripts(GetDefaultFolderName());
        }

        public static void ImportExportChartScripts(string folderName)
        {
            Console.WriteLine("You want to export (e), import (i) ChartScripts? (nothing to exit)");

            string answer = Console.ReadLine();

            if (answer.ToLower() == "e")
            {
                ExportAllScripts(folderName);
            }
            else if (answer.ToLower() == "i")
            {
                ImportAllScripts(folderName);
            }
        }

        public static string DefaultFolderDevelopment = @"..\..\..\Extensions\Signum.Engine.Extensions\Chart\ChartScripts";
        public static string DefaultFolderProduction = @"ChartScripts";
        private static string GetDefaultFolderName()
        {
            if (Directory.Exists(DefaultFolderDevelopment))
            {
                if (Directory.Exists(DefaultFolderProduction))
                    return SafeConsole.Ask("In Production?") ? DefaultFolderProduction : DefaultFolderDevelopment;

                return DefaultFolderDevelopment;
            }

            if (Directory.Exists(DefaultFolderProduction))
                return DefaultFolderProduction;

            throw new InvalidOperationException("Default ChartScripts folder not found");
        }

        public static void ExportAllScripts(string folderName)
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            var files = Directory.GetFiles(folderName, "*.xml").ToDictionary(Path.GetFileNameWithoutExtension);

            var charts = Database.Query<ChartScriptDN>().ToDictionary(a => a.Name);

            Options options = new Options();

            Func<ChartScriptDN, string> fileName = cs => Path.Combine(folderName, cs.Name + ".xml");

            Synchronizer.SynchronizeReplacing(new Replacements(), "scripts",
                charts,
                files,
                (name, script) => script.ExportXml().Save(fileName(script)),
                (name, file) =>
                {
                    if (AskYesNoAll("Remove {0} file?".Formato(name), ref options.RemoveOld))
                        File.Delete(file);
                },
                (name, script, file) =>
                {
                    var newFileName = fileName(script);

                    if (file != newFileName)
                        File.Delete(file);

                    if (script.Icon != null)
                        script.Icon.Retrieve();

                    script.ExportXml().Save(newFileName);
                });
        }

        class Options
        {
            public bool RemoveOld;
            public bool OverrideAll;
            public bool ForceAll;
        }

        public static void ImportAllScripts(string folderName)
        {
            var files = Directory.GetFiles(folderName, "*.xml").ToDictionary(Path.GetFileNameWithoutExtension);

            var charts = Database.Query<ChartScriptDN>().ToDictionary(a => a.Name);

            Options options = new Options();

            using (OperationLogic.AllowSave<ChartScriptDN>())
                Synchronizer.SynchronizeReplacing(new Replacements(), "scripts",
                    files,
                    charts,
                    (name, file) =>
                    {
                        var cs = new ChartScriptDN();
                        cs.ImportXml(XDocument.Load(file), name, force: false);
                        cs.Save();

                        Console.WriteLine("{0} entity created.".Formato(name));
                    },
                    (name, script) =>
                    {
                        if (AskYesNoAll("Remove {0} entity?".Formato(name), ref options.RemoveOld))
                        {
                            try
                            {
                                script.Delete();
                                Console.WriteLine("{0} entity removed.".Formato(name));
                            }
                            catch (Exception e)
                            {
                                SafeConsole.WriteLineColor(ConsoleColor.Red, "Error removing {0} entity: {1}".Formato(name, e.Message));
                            }
                        }
                    },
                    (name, file, script) =>
                    {
                        var xDoc = XDocument.Load(file);
                        if (script.Icon != null)
                            script.Icon.Retrieve();
                        try
                        {
                            script.ImportXml(xDoc, name, false);
                        }
                        catch (FormatException f)
                        {
                            SafeConsole.WriteLineColor(ConsoleColor.Yellow, f.Message);
                            if (AskYesNoAll("Force {0}?".Formato(name), ref options.ForceAll))
                                script.ImportXml(xDoc, name, true);
                        }

                        if (script.HasChanges() && AskYesNoAll("Override {0} entity?".Formato(name), ref options.OverrideAll))
                        {
                            script.Save();
                            Console.WriteLine("{0} entity overriden.".Formato(name));
                        }
                    });
        }



        private static bool AskYesNoAll(string message, ref bool all)
        {
            if (all)
                return true;

            while (true)
            {
                Console.Write(message + "(*yes, no, all)");

                var str = Console.ReadLine();

                if (!str.HasText())
                    return true;

                var c = char.ToLower(str[0]);

                if (c == 'a' || c == 'y')
                {
                    all = c == 'a';
                    return true;
                }
                else if (c == 'n')
                    return false;
            }
        }

        public static ChartScriptDN GetChartScript(string chartScriptName)
        {
            return Scripts.Value.GetOrThrow(chartScriptName);
        }
    }
}
