﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Services;
using System.Windows;
using Signum.Utilities;

namespace Signum.Windows.Authorization
{
    public static class PropertyAuthClient
    {
        static DefaultDictionary<PropertyRoute, PropertyAllowed> propertyRules;

        public static bool Started { get; private set; }

        internal static void Start()
        {
            Started = true;

            Common.RouteTask += Common_RouteTask;
            Common.LabelOnlyRouteTask += Common_RouteTask;
            PropertyRoute.SetIsAllowedCallback(pr => pr.GetAllowedFor(PropertyAllowed.Read));

            AuthClient.UpdateCacheEvent += new Action(AuthClient_UpdateCacheEvent);
        }

        static void AuthClient_UpdateCacheEvent()
        {
            var overrides = Server.Return((IPropertyAuthServer s) => s.OverridenProperties());

            propertyRules = new DefaultDictionary<PropertyRoute, PropertyAllowed>
                (pr => TypeAuthClient.GetAllowed(pr.RootType).MaxUI().ToPropertyAllowed(),
                overrides);
        }

        public static PropertyAllowed GetPropertyAllowed(this PropertyRoute route)
        {
            while (route.PropertyRouteType == PropertyRouteType.MListItems || route.PropertyRouteType == PropertyRouteType.LiteEntity)
                route = route.Parent;

            var propAllowed = propertyRules.GetAllowed(route);

            var typeAllowed = TypeAuthClient.GetAllowed(route.RootType).MaxUI().ToPropertyAllowed();

            return propAllowed < typeAllowed ? propAllowed : typeAllowed;
        }

        public static string GetAllowedFor(this PropertyRoute route, PropertyAllowed requested)
        {
            if (route.PropertyRouteType == PropertyRouteType.MListItems || route.PropertyRouteType == PropertyRouteType.LiteEntity)
                return GetAllowedFor(route.Parent, requested);

            var propAllowed = GetPropertyAllowed(route);
            if (propAllowed < requested)
            {
                var typeAllowed = TypeAuthClient.GetAllowed(route.RootType).MaxUI().ToPropertyAllowed();

                if (typeAllowed < requested)
                    return "Type {0} is set to {1} for {2}".Formato(route.RootType.NiceName(), typeAllowed, RoleDN.Current);

                return "Property {0} is set to {1} for {2}".Formato(route, propAllowed, RoleDN.Current);
            }

            return null;
        }

        static void Common_RouteTask(FrameworkElement fe, string route, PropertyRoute context)
        {
            if (context.PropertyRouteType == PropertyRouteType.FieldOrProperty)
            {
                switch (GetPropertyAllowed(context))
                {
                    case PropertyAllowed.Read: Common.SetIsReadOnly(fe, true); break;
                }
            }
        }
    }
}

