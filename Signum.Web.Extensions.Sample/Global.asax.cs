﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Signum.Test;
using System.Web.Hosting;
using Signum.Web.Extensions.Sample.Properties;
using Signum.Engine.Maps;
using Signum.Web.Operations;
using Signum.Entities.Authorization;
using System.Threading;
using Signum.Engine;
using Signum.Engine.Authorization;

namespace Signum.Web.Extensions.Sample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "v",
               "View/{typeUrlName}/{id}",
               new { controller = "Signum", action = "View", typeFriendlyName = "", id = "" }
           );

            routes.MapRoute(
                "f",
                "Find/{sfQueryUrlName}",
                new { controller = "Signum", action = "Find", sfQueryUrlName = "" }
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            //System.Diagnostics.Debugger.Break();

            RegisterRoutes(RouteTable.Routes);
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new SignumViewEngine());

            HostingEnvironment.RegisterVirtualPathProvider(new AssemblyResourceProvider());

            Signum.Test.Extensions.Starter.Start(Settings.Default.ConnectionString);

            Schema.Current.Initialize();
            LinkTypesAndViews();

            AuthenticationRequiredAttribute.Authenticate = null;
            //Thread.CurrentPrincipal = Database.Query<UserDN>().Where(u => u.UserName == "externo").Single();
        }

        private void LinkTypesAndViews()
        {
            Navigator.Manager = new NavigationManager
            {
                EntitySettings = new Dictionary<Type, EntitySettings>
                {
                    // {typeof(EfColaboradoraDN), new EntitySettings(false){PartialViewName="Views/Home/EfColaboradoraIU" }},
                }
            };

            Constructor.Start(new ConstructorManager
            {
                Constructors = new Dictionary<Type, Func<Controller, object>>
                {
                    //{ typeof(FuturoTomadorDN), c => new FuturoTomadorDN{DatosContacto = new DatosContactoDN().ToLiteFat() }},
                }
            });

            OperationClient.Start(new OperationManager
            {
                Settings = new Dictionary<Enum, WebMenuItem>()
            });

            MusicClient.Start();

            Navigator.Manager.NormalPageUrl = "Views/Shared/NormalPage";
            Navigator.Start();
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            UserDN user = HttpContext.Current.Session == null ? null : (UserDN)HttpContext.Current.Session["usuario"];

            if (user != null)
            {
                Thread.CurrentPrincipal = user;
            }
            else
            {
                using (AuthLogic.Disable())
                {
                    Thread.CurrentPrincipal = Database.Query<UserDN>().Where(u => u.UserName == "test").Single();
                }
            }
        }
    }
}