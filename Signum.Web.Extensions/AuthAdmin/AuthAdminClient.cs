﻿#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using Signum.Entities.Authorization;
using Signum.Engine.Authorization;
using System.Reflection;
using Signum.Web.Operations;
using Signum.Entities;
using System.Web.Mvc;
using Signum.Web.Properties;
using System.Diagnostics;
using Signum.Engine;
using Signum.Entities.Basics;
using Signum.Entities.Reflection;
using Signum.Entities.Operations;
using System.Linq.Expressions;
using Signum.Engine.Maps;
using System.Web.Routing;
#endregion

namespace Signum.Web.Authorization
{
    public static class AuthAdminClient
    {
        public static string ViewPrefix = "authAdmin/Views/";

        public static void Start(bool types, bool properties, bool queries, bool operations, bool permissions, bool facadeMethods, bool entityGroups)
        {
            if (Navigator.Manager.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                AssemblyResourceManager.RegisterAreaResources(
                    new AssemblyResourceStore(typeof(AuthClient), "/authAdmin/", "Signum.Web.Extensions.AuthAdmin."));

                RouteTable.Routes.InsertRouteAt0("authAdmin/{resourcesFolder}/{*resourceName}",
                    new { controller = "Resources", action = "Index", area = "authAdmin" },
                    new { resourcesFolder = new InArray(new string[] { "Scripts", "Content", "Images" }) });

                if (Navigator.Manager.EntitySettings.ContainsKey(typeof(UserDN)))
                    Navigator.EntitySettings<UserDN>().PartialViewName = _ => ViewPrefix + "User";
                else
                    Navigator.AddSetting(new EntitySettings<UserDN>(EntityType.Default) { PartialViewName = _ => ViewPrefix + "User" });

                if (Navigator.Manager.EntitySettings.ContainsKey(typeof(RoleDN)))
                    Navigator.EntitySettings<RoleDN>().PartialViewName = _ => ViewPrefix + "Role";
                else
                    Navigator.AddSetting(new EntitySettings<RoleDN>(EntityType.Admin) { PartialViewName = _ => ViewPrefix + "Role" });

                Navigator.RegisterTypeName<IUserRelatedDN>();

                if (types)
                    Register<TypeRulePack, TypeAllowedRule, TypeDN, TypeAllowed, TypeDN>("types", a => a.Resource, "Resource", false);

                if (properties)
                    Register<PropertyRulePack, PropertyAllowedRule, PropertyDN, PropertyAllowed, string>("properties", a => a.Resource.Path, "Resource_Path", true);

                if (queries)
                    Register<QueryRulePack, QueryAllowedRule, QueryDN, bool, string>("queries", a => a.Resource.Key, "Resource_Key", true);

                if (operations)
                    Register<OperationRulePack, OperationAllowedRule, OperationDN, bool, OperationDN>("operations", a => a.Resource, "Resource", true);

                if (permissions)
                    Register<PermissionRulePack, PermissionAllowedRule, PermissionDN, bool, PermissionDN>("permissions", a => a.Resource, "Resource", false);

                if (facadeMethods)
                    Register<FacadeMethodRulePack, FacadeMethodAllowedRule, FacadeMethodDN, bool, string>("facadeMethods", a => a.Resource.Name, "Resource_Name", false);

                if (entityGroups)
                {
                    Register<EntityGroupRulePack, EntityGroupAllowedRule, EntityGroupDN, EntityGroupAllowed, EntityGroupDN>("entityGroups", a => a.Resource, "Resource", false);

                    Navigator.EntitySettings<EntityGroupRulePack>().MappingAdmin
                        .GetProperty(m => m.Rules, rul =>
                        ((EntityMapping<EntityGroupAllowedRule>)((MListDictionaryMapping<EntityGroupAllowedRule, EntityGroupDN>)rul).ElementMapping)
                                .RemoveProperty(a => a.Allowed)
                                .SetProperty(a => a.In, new ValueMapping<TypeAllowed>(), null)
                                .SetProperty(a => a.Out, new ValueMapping<TypeAllowed>(), null));
                }
            }
        }

        public static void StartUserGraph()
        {
            if (Navigator.Manager.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                Navigator.EntitySettings<UserDN>().ShowOkSave = admin => false;

                OperationClient.Manager.Settings.AddRange(new Dictionary<Enum, OperationSettings>
                    {
                        { UserOperation.SaveNew, new EntityOperationSettings { IsVisible = ctx => ctx.Entity.IsNew }},
                        { UserOperation.Save, new EntityOperationSettings { IsVisible = ctx => !ctx.Entity.IsNew }},
                        { UserOperation.Disable, new EntityOperationSettings { IsVisible = ctx => !ctx.Entity.IsNew }},
                        { UserOperation.Enable, new EntityOperationSettings { IsVisible = ctx => !ctx.Entity.IsNew }}
                    });
            }
        }

        static void Register<T, AR, R, A, K>(string partialViewName, Func<AR, K> getKey, string getKeyRoute, bool embedded)
            where T : BaseRulePack<AR>
            where AR: AllowedRule<R, A>, new()
            where R : IdentifiableEntity
            where A : struct
        {
            if (!Navigator.Manager.EntitySettings.ContainsKey(typeof(R)))
                Navigator.AddSetting(new EntitySettings<R>(EntityType.ServerOnly));

            Navigator.AddSetting(new EntitySettings<T>(EntityType.NotSaving)
            {
                PartialViewName = e => ViewPrefix + partialViewName,
                MappingAdmin = new EntityMapping<T>(false)
                    .SetProperty(m => m.Rules,
                    new MListDictionaryMapping<AR, K>(getKey, getKeyRoute)
                    {
                        ElementMapping = new EntityMapping<AR>(false)
                                .SetProperty(p => p.Allowed, new ValueMapping<A>(), null)
                    }, null)
            });

            ButtonBarEntityHelper.RegisterEntityButtons<T>((ControllerContext controllerContext, T entity, string viewName, string prefix) =>
                new[] { new ToolBarButton { 
                    OnClick = (embedded ? "postDialog('{0}', '{1}')" :  "Submit('{0}', '{1}')").Formato(
                        new UrlHelper(controllerContext.RequestContext).Action((embedded? "save" : "") +  partialViewName, "AuthAdmin"), prefix), 
                    Text = Resources.Save,
                    DivCssClass = ToolBarButton.DefaultEntityDivCssClass 
                } 
                });
        }
    }
}