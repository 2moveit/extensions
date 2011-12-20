﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using Signum.Engine.Maps;
using Signum.Engine.Authorization;
using Signum.Engine;
using System.Windows;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using Signum.Engine.Basics;
using Signum.Entities.Basics;
using System.Reflection;

namespace Signum.Entities.Authorization
{
    class TypeDefaultBehaviour
    {
        public TypeAllowed BaseAllowed { get; private set;  }
        public Func<IEnumerable<TypeAllowed>, TypeAllowed> MergeAllowed;

        public TypeDefaultBehaviour(TypeAllowed baseAllowed, Func<IEnumerable<TypeAllowed>, TypeAllowed> merge)
        {
            this.BaseAllowed = baseAllowed;
            this.MergeAllowed = merge;
        }

        public override string ToString()
        {
            return "DefaulBehaviour {0}".Formato(BaseAllowed);
        }
    }

    class TypeAuthCache : IManualAuth<Type, TypeAllowedAndConditions> 
    {
        readonly Lazy<Dictionary<Lite<RoleDN>, RoleAllowedCache>> runtimeRules;

        DefaultBehaviour<TypeAllowedAndConditions> Min;
        DefaultBehaviour<TypeAllowedAndConditions> Max;

        public TypeAuthCache(SchemaBuilder sb, DefaultBehaviour<TypeAllowedAndConditions> max, DefaultBehaviour<TypeAllowedAndConditions> min)
        {
            runtimeRules = Schema.GlobalLazy(this.NewCache);

            this.Max = max;
            this.Min = min;

            sb.Include<RuleTypeDN>();

            sb.Schema.Initializing[InitLevel.Level1SimpleEntities] += Schema_InitializingCache;
            sb.Schema.EntityEvents<RuleTypeDN>().Saving += Schema_Saving;
            AuthLogic.RolesModified += InvalidateCache;

            sb.AddUniqueIndex<RuleTypeDN>(rt => new { rt.Resource, rt.Role });

            sb.Schema.Table<TypeDN>().PreDeleteSqlSync += new Func<IdentifiableEntity, SqlPreCommand>(AuthCache_PreDeleteSqlSync);

            Validator.GetOrCreatePropertyPack((RuleTypeDN r) => r.Conditions).StaticPropertyValidation += TypeAuthCache_StaticPropertyValidation;
        }

        string TypeAuthCache_StaticPropertyValidation(ModifiableEntity sender, PropertyInfo pi)
        {
            RuleTypeDN rt = (RuleTypeDN)sender;
            if (rt.Resource == null)
            {
                if (rt.Conditions.Any())
                    return "Default {0} should not have conditions".Formato(typeof(RuleTypeDN).NiceName());

                return null;
            }
            
            Type type = TypeLogic.DnToType[rt.Resource];
            var conditions = rt.Conditions.Where(a => !TypeConditionLogic.IsDefined(type, EnumLogic<TypeConditionNameDN>.ToEnum(a.Condition)));

            if (conditions.IsEmpty())
                return null;

            return "Type {0} has no definitions for the conditions: {1}".Formato(type.Name, conditions.CommaAnd(a => a.Condition.Name));
        }

        static SqlPreCommand AuthCache_PreDeleteSqlSync(IdentifiableEntity arg)
        {
            var t = Schema.Current.Table<RuleTypeDN>();
            var rec = (FieldReference)t.Fields["resource"].Field;
            var cond = (FieldMList)t.Fields["conditions"].Field;
            var param = SqlParameterBuilder.CreateReferenceParameter("id", false, arg.Id);

            var conditions = new SqlPreCommandSimple("DELETE cond FROM {0} cond INNER JOIN {1} r ON cond.{2} = r.{3} WHERE r.{4} = {5}".Formato(
                cond.RelationalTable.Name.SqlScape(), t.Name.SqlScape(), cond.RelationalTable.BackReference.Name.SqlScape(), "Id", rec.Name.SqlScape(), param.ParameterName.SqlScape()), new List<SqlParameter> { param });
            var rule = new SqlPreCommandSimple("DELETE FROM {0} WHERE {1} = {2}".Formato(t.Name.SqlScape(), rec.Name.SqlScape(), param.ParameterName.SqlScape()), new List<SqlParameter> { param });

            return SqlPreCommand.Combine(Spacing.Simple, conditions, rule); 
        }    

        void Schema_InitializingCache()
        {
            runtimeRules.Load();
        }

        void InvalidateCache()
        {
            runtimeRules.ResetPublicationOnly();
        }

        DefaultRule IManualAuth<Type, TypeAllowedAndConditions>.GetDefaultRule(Lite<RoleDN> role)
        {
            var allowed = Database.Query<RuleTypeDN>().Where(a => a.Resource == null && a.Role == role).Select(a=>a.Allowed).ToList();

            return allowed.IsEmpty() || allowed[0].Equals(Max.BaseAllowed) ? DefaultRule.Max : DefaultRule.Min; 
        }

        void IManualAuth<Type, TypeAllowedAndConditions>.SetDefaultRule(Lite<RoleDN> role, DefaultRule defaultRule)
        {
            if (((IManualAuth<Type, TypeAllowedAndConditions>)this).GetDefaultRule(role) == defaultRule)
                return;

            IQueryable<RuleTypeDN> query = Database.Query<RuleTypeDN>().Where(a => a.Resource == null && a.Role == role);
            if (defaultRule == DefaultRule.Max)
            {
                if (query.UnsafeDelete() == 0)
                    throw new InvalidOperationException("Inconsistency in the data");
            }
            else
            {
                query.UnsafeDelete();

                new RuleTypeDN
                {
                    Role = role,
                    Resource = null,
                    Allowed = Min.BaseAllowed.Fallback,
                }.Save();
            }

            InvalidateCache();
        }

        TypeAllowedAndConditions IManualAuth<Type, TypeAllowedAndConditions>.GetAllowed(Lite<RoleDN> role, Type key)
        {
            TypeDN resource = TypeLogic.TypeToDN[key];

            ManualResourceCache miniCache = new ManualResourceCache(resource, Min, Max);

            return miniCache.GetAllowed(role);
        }

        void IManualAuth<Type, TypeAllowedAndConditions>.SetAllowed(Lite<RoleDN> role, Type key, TypeAllowedAndConditions allowed)
        {
            TypeDN resource = TypeLogic.TypeToDN[key];

            ManualResourceCache miniCache = new ManualResourceCache(resource, Min, Max); 

            if (miniCache.GetAllowed(role).Equals(allowed))
                return;

            IQueryable<RuleTypeDN> query = Database.Query<RuleTypeDN>().Where(a => a.Resource == resource && a.Role == role);
            if (miniCache.GetAllowedBase(role).Equals(allowed))
            {
                if (query.UnsafeDelete() == 0)
                    throw new InvalidOperationException("Inconsistency in the data");
            }
            else
            {
                query.UnsafeDelete();

                allowed.ToRuleType(role, resource).Save();
            }

            InvalidateCache();
        }

        public class ManualResourceCache
        {
            readonly Dictionary<Lite<RoleDN>, TypeAllowedAndConditions> defaultRules;
            readonly Dictionary<Lite<RoleDN>, TypeAllowedAndConditions> specificRules;

            readonly DefaultBehaviour<TypeAllowedAndConditions> Min;
            readonly DefaultBehaviour<TypeAllowedAndConditions> Max;

            public ManualResourceCache(TypeDN resource, DefaultBehaviour<TypeAllowedAndConditions> min, DefaultBehaviour<TypeAllowedAndConditions> max)
            {
                var list =  Database.Query<RuleTypeDN>().Where(r=>r.Resource == resource || r.Resource == null).ToList();

                defaultRules = list.Where(a => a.Resource == null).ToDictionary(a => a.Role, a => a.ToTypeAllowedAndConditions());

                specificRules = list.Where(a => a.Resource != null).ToDictionary(a => a.Role, a => a.ToTypeAllowedAndConditions());

                this.Min = min;
                this.Max = max;
            }

            public TypeAllowedAndConditions GetAllowed(Lite<RoleDN> role)
            {
                TypeAllowedAndConditions result;
                if (specificRules.TryGetValue(role, out result))
                    return result;

                return GetAllowedBase(role);
            }

            DefaultBehaviour<TypeAllowedAndConditions> GetBehaviour(Lite<RoleDN> role)
            {
                return defaultRules.TryGet(role, Max.BaseAllowed).Equals(Max.BaseAllowed) ? Max : Min;
            }

            public TypeAllowedAndConditions GetAllowedBase(Lite<RoleDN> role)
            {
                var behaviour = GetBehaviour(role);
                var related = AuthLogic.RelatedTo(role);
                if (related.IsEmpty())
                    return behaviour.BaseAllowed;
                else
                {
                    var baseRules = related.Select(r => GetAllowed(r)).ToList();

                    return behaviour.MergeAllowed(baseRules);
                }     
            }
        }

        Dictionary<Lite<RoleDN>, RoleAllowedCache> NewCache()
        {
            using (AuthLogic.Disable())
            using (new EntityCache(true))
            {
                List<Lite<RoleDN>> roles = AuthLogic.RolesInOrder().ToList();

                var rules = Database.Query<RuleTypeDN>().ToList();

                string errors = rules.Select(a=>a.IntegrityCheck()).Where(StringExtensions.HasText).ToString("\r\n");
                if (errors.HasText())
                    throw new InvalidOperationException(errors); 

                Dictionary<Lite<RoleDN>, TypeAllowedAndConditions> defaultBehaviours =
                    rules.Where(a => a.Resource == null)
                    .ToDictionary(ru => ru.Role, ru => ru.ToTypeAllowedAndConditions());

                Dictionary<Lite<RoleDN>, Dictionary<Type, TypeAllowedAndConditions>> realRules =
                   rules.Where(a => a.Resource != null)
                      .AgGroupToDictionary(ru => ru.Role, gr => gr
                          .ToDictionary(ru => TypeLogic.DnToType[ru.Resource], ru => ru.ToTypeAllowedAndConditions()));

                Dictionary<Lite<RoleDN>, RoleAllowedCache> newRules = new Dictionary<Lite<RoleDN>, RoleAllowedCache>();
                foreach (var role in roles)
                {
                    var related = AuthLogic.RelatedTo(role);

                    var behaviour = defaultBehaviours.TryGet(role, Max.BaseAllowed).Equals(Max.BaseAllowed) ? Max : Min;

                    newRules.Add(role, new RoleAllowedCache(behaviour, related.Select(r => newRules[r]).ToList(), realRules.TryGetC(role)));
                }

                return newRules;
            }
        }

        void Schema_Saving(RuleTypeDN rule)
        {
            Transaction.PostRealCommit += () => InvalidateCache();
        }

        internal void GetRules(BaseRulePack<TypeAllowedRule> rules, IEnumerable<TypeDN> resources)
        {
            RoleAllowedCache cache = runtimeRules.Value[rules.Role];

            rules.SubRoles = AuthLogic.RelatedTo(rules.Role).ToMList();
            rules.DefaultRule = GetDefaultRule(rules.Role);
            rules.Rules = (from r in resources
                           let type = TypeLogic.DnToType[r]
                           select new TypeAllowedRule()
                           {
                               Resource = r,
                               AllowedBase = cache.GetAllowedBase(type),
                               Allowed = cache.GetAllowed(type),
                               AvailableConditions = TypeConditionLogic.ConditionsFor(type).ToReadOnly()
                           }).ToMList();
        }

        internal void SetRules(BaseRulePack<TypeAllowedRule> rules)
        {
            if (rules.DefaultRule != GetDefaultRule(rules.Role))
            {
                ((IManualAuth<Type, TypeAllowed>)this).SetDefaultRule(rules.Role, rules.DefaultRule);
                Database.Query<RuleTypeDN>().Where(r => r.Role == rules.Role && r.Resource != null).UnsafeDelete();
                return;
            }

            var current = Database.Query<RuleTypeDN>().Where(r => r.Role == rules.Role && r.Resource != null).ToDictionary(a => a.Resource);
            var should = rules.Rules.Where(a => a.Overriden).ToDictionary(r => r.Resource);

            Synchronizer.Synchronize(current, should,
                (type, pr) => pr.Delete(),
                (type, ar) => ar.Allowed.ToRuleType(rules.Role, type).Save() ,
                (type, pr, ar) =>
                {
                    pr.Allowed = ar.Allowed.Fallback;

                    var shouldConditions = ar.Allowed.Conditions.Select(a => new RuleTypeConditionDN
                    {
                        Allowed = a.Allowed,
                        Condition = EnumLogic<TypeConditionNameDN>.ToEntity(a.ConditionName),
                    }).ToMList();

                    if(!pr.Conditions.SequenceEqual(shouldConditions))
                        pr.Conditions = shouldConditions;

                    if (pr.SelfModified)
                        pr.Save();
                });

            InvalidateCache();
        }

        public DefaultRule GetDefaultRule(Lite<RoleDN> role)
        {
            return runtimeRules.Value[role].GetDefaultRule(Max);
        }

        internal TypeAllowedAndConditions GetAllowed(Lite<RoleDN> role, Type key)
        {
            return runtimeRules.Value[role].GetAllowed(key);
        }

        internal DefaultDictionary<Type, TypeAllowedAndConditions> GetDefaultDictionary()
        {
            return runtimeRules.Value[RoleDN.Current.ToLite()].DefaultDictionary();
        }

        public class RoleAllowedCache
        {
            readonly DefaultBehaviour<TypeAllowedAndConditions> behaviour;
            readonly DefaultDictionary<Type, TypeAllowedAndConditions> rules; 
            readonly List<RoleAllowedCache> baseCaches;

            public RoleAllowedCache(DefaultBehaviour<TypeAllowedAndConditions> behaviour, List<RoleAllowedCache> baseCaches, Dictionary<Type, TypeAllowedAndConditions> newValues)
            {
                this.behaviour = behaviour;

                this.baseCaches = baseCaches;

                TypeAllowedAndConditions defaultAllowed;
                Dictionary<Type, TypeAllowedAndConditions> tmpRules; 

                if(baseCaches.IsEmpty())
                {
                    defaultAllowed = behaviour.BaseAllowed;

                    tmpRules = newValues; 
                }
                else
                {
                    defaultAllowed = behaviour.MergeAllowed(baseCaches.Select(a => a.rules.DefaultAllowed));

                    var keys = baseCaches.Where(b => b.rules.DefaultAllowed.Equals(defaultAllowed) && b.rules != null).SelectMany(a => a.rules.ExplicitKeys).ToHashSet();

                    if (keys != null)
                    {
                        tmpRules = keys.ToDictionary(k => k, k =>
                        {
                            var baseRules = baseCaches.Select(b => b.GetAllowed(k)).ToList();
                            return behaviour.MergeAllowed(baseRules);
                        }); 
                            
                        if (newValues != null)
                            tmpRules.SetRange(newValues);
                    }
                    else
                    {
                        tmpRules = newValues; 
                    }
                }

                tmpRules = Simplify(tmpRules, defaultAllowed);

                rules = new DefaultDictionary<Type, TypeAllowedAndConditions>(defaultAllowed, tmpRules);
            }

            internal static Dictionary<Type, TypeAllowedAndConditions> Simplify(Dictionary<Type, TypeAllowedAndConditions> dictionary, TypeAllowedAndConditions defaultAllowed)
            {
                if (dictionary == null || dictionary.Count == 0)
                    return null;

                dictionary.RemoveRange(dictionary.Where(p => p.Value.Equals(defaultAllowed)).Select(p => p.Key).ToList());

                if (dictionary.Count == 0)
                    return null;

                return dictionary;
            }

            public TypeAllowedAndConditions GetAllowed(Type k)
            {
                return rules.GetAllowed(k);
            }

            public TypeAllowedAndConditions GetAllowedBase(Type k)
            {
                if (baseCaches.IsEmpty())
                    return rules.DefaultAllowed;

                var baseRules = baseCaches.Select(b => b.GetAllowed(k)).ToList();

                return behaviour.MergeAllowed(baseRules);
            }

            public DefaultRule GetDefaultRule(DefaultBehaviour<TypeAllowedAndConditions> max)
            {
                return behaviour == max ? DefaultRule.Max : DefaultRule.Min;
            }

            internal DefaultDictionary<Type, TypeAllowedAndConditions> DefaultDictionary()
            {
                return this.rules;
            }
        }

        internal XElement ExportXml()
        {
            var list = Database.RetrieveAll<RuleTypeDN>();

            var defaultRules = list.Where(a => a.Resource == null).ToDictionary(a => a.Role, a => a.ToTypeAllowedAndConditions());
            var specificRules = list.Where(a => a.Resource != null).AgGroupToDictionary(a => a.Role, gr => gr.ToDictionary(a => a.Resource));

            return new XElement("Types",
                (from r in AuthLogic.RolesInOrder()
                 let max = defaultRules.TryGet(r, Max.BaseAllowed).Equals(Max.BaseAllowed)
                 select new XElement("Role",
                     new XAttribute("Name", r.ToStr),
                     max ? null : new XAttribute("Default", "Min"),
                     specificRules.TryGetC(r).TryCC(dic =>
                         from kvp in dic
                         let resource = kvp.Key.CleanName
                         orderby resource
                         select new XElement("Type",
                            new XAttribute("Resource", resource),
                            new XAttribute("Allowed", kvp.Value.Allowed.ToString()),
                            from c in kvp.Value.Conditions
                            select new XElement("Condition",
                                new XAttribute("Name", c.Condition.Key),
                                new XAttribute("Allowed", c.Allowed.ToString()))
                                )
                     ))
                 ));
        }


        internal SqlPreCommand ImportXml(XElement element, Dictionary<string, Lite<RoleDN>> roles)
        {
            var current = Database.RetrieveAll<RuleTypeDN>().GroupToDictionary(a => a.Role);
            var should = element.Element("Types").Elements("Role").ToDictionary(x => roles[x.Attribute("Name").Value]);

            Table table = Schema.Current.Table(typeof(RuleTypeDN));

            return Synchronizer.SynchronizeScript(current, should,
                (role, listRules) => listRules.Select(rt => table.DeleteSqlSync(rt)).Combine(Spacing.Simple),
                (role, x) =>
                {
                    var max = x.Attribute("Default") == null || x.Attribute("Default").Value != "Min";
                    SqlPreCommand defSql = SetDefault(table, null, max, role);

                    var dic = x.Elements("Type").ToDictionary(
                        xr => TypeLogic.TypeToDN[TypeLogic.GetType(xr.Attribute("Resource").Value)],
                        xr => new
                        {
                            Allowed = xr.Attribute("Allowed").Value.ToEnum<TypeAllowed>(),
                            Condition = Conditions(xr)
                        }, "Type rules for {0}".Formato(role));

                    SqlPreCommand restSql = dic.Select(kvp => table.InsertSqlSync(new RuleTypeDN
                    {
                        Resource = kvp.Key,
                        Role = role,
                        Allowed = kvp.Value.Allowed,
                        Conditions =  kvp.Value.Condition
                    }, Comment(role, kvp.Key, kvp.Value.Allowed))).Combine(Spacing.Simple);

                    return SqlPreCommand.Combine(Spacing.Simple, defSql, restSql);
                },
                (role, list, x) =>
                {
                    var def = list.SingleOrDefaultEx(a => a.Resource == null);
                    var max = x.Attribute("Default") == null || x.Attribute("Default").Value != "Min";
                    SqlPreCommand defSql = SetDefault(table, def, max, role);

                    SqlPreCommand restSql = Synchronizer.SynchronizeScript(
                        list.Where(a => a.Resource != null).ToDictionary(a => a.Resource),
                        x.Elements("Type").ToDictionary(a => TypeLogic.TypeToDN[TypeLogic.GetType(a.Attribute("Resource").Value)], "Type rules for {0}".Formato(role)),
                        (r, rt) => table.DeleteSqlSync(rt, Comment(role, r, rt.Allowed)),
                        (r, xr) =>
                        {
                            var a = xr.Attribute("Allowed").Value.ToEnum<TypeAllowed>();
                            var conditions = Conditions(xr);

                            return table.InsertSqlSync(new RuleTypeDN { Resource = r, Role = role, Allowed = a, Conditions = conditions}, Comment(role, r, a));
                        },
                        (r, pr, xr) =>
                        {
                            var oldA = pr.Allowed;
                            pr.Allowed = xr.Attribute("Allowed").Value.ToEnum<TypeAllowed>();
                            var conditions = Conditions(xr);

                            if (!pr.Conditions.SequenceEqual(conditions))
                                pr.Conditions = conditions;

                            return table.UpdateSqlSync(pr, Comment(role, r, oldA, pr.Allowed));
                        }, Spacing.Simple);

                    return SqlPreCommand.Combine(Spacing.Simple, defSql, restSql);
                }, Spacing.Double);
        }

        private static MList<RuleTypeConditionDN> Conditions(XElement xr)
        {
            return xr.Descendants("Condition").Select(xc => new RuleTypeConditionDN
            {
                Condition = EnumLogic<TypeConditionNameDN>.ToEntity(xc.Attribute("Name").Value),
                Allowed = xc.Attribute("Allowed").Value.ToEnum<TypeAllowed>()
            }).ToMList();
        }


        internal static string Comment(Lite<RoleDN> role, TypeDN resource, TypeAllowed allowed)
        {
            return "{0} {1} for {2} ({3})".Formato(typeof(TypeDN).NiceName(), resource.ToStr, role, allowed);
        }

        internal static string Comment(Lite<RoleDN> role, TypeDN resource, TypeAllowed from, TypeAllowed to)
        {
            return "{0} {1} for {2} ({3} -> {4})".Formato(typeof(TypeDN).NiceName(), resource.ToStr, role, from, to);
        }

        private SqlPreCommand SetDefault(Table table, RuleTypeDN def, bool max, Lite<RoleDN> role)
        {
            string comment = "Default {0} for {1}".Formato(typeof(TypeDN).NiceName(), role);

            if (max)
            {
                if (def != null)
                    return table.DeleteSqlSync(def, comment + " ({0})".Formato(def.Allowed));

                return null;
            }
            else
            {
                if (def == null)
                {
                    return table.InsertSqlSync(new RuleTypeDN()
                    {
                        Role = role,
                        Resource = null,
                        Allowed = Min.BaseAllowed.Fallback
                    }, comment + " ({0})".Formato(Min.BaseAllowed));
                }
                else if (!def.Allowed.Equals(Min.BaseAllowed))
                {
                    var old = def.Allowed;
                    def.Allowed = Min.BaseAllowed.Fallback;
                    return table.UpdateSqlSync(def, comment + "({0} -> {1})".Formato(old, Min.BaseAllowed));
                }

                return null;
            }
        }
    }
}
