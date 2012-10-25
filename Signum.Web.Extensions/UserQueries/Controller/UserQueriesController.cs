﻿#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Signum.Utilities;
using Signum.Engine;
using Signum.Entities;
using Signum.Engine.Maps;
using Signum.Web.Properties;
using Signum.Engine.DynamicQuery;
using Signum.Entities.Reflection;
using Signum.Entities.DynamicQuery;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using Signum.Entities.Reports;
using Signum.Entities.Basics;
using Signum.Engine.Basics;
using Signum.Entities.Authorization;
using Signum.Entities.UserQueries;
using Signum.Engine.UserQueries;
#endregion

namespace Signum.Web.UserQueries
{
    public class UserQueriesController : Controller
    {
        public ActionResult View(Lite<UserQueryDN> lite)
        {
            UserQueryDN uq = Database.Retrieve<UserQueryDN>(lite);

            FindOptions fo = uq.ToFindOptions();
           
            return Navigator.Find(this, fo);
        }

        public ActionResult Create(QueryRequest request)
        {
            if (!Navigator.IsFindable(request.QueryName))
                throw new UnauthorizedAccessException(Resources.ViewForType0IsNotAllowed.Formato(request.QueryName));

            var userQuery = ToUserQuery(request);

            userQuery.Related = UserDN.Current.ToLite<IdentifiableEntity>();

            return Navigator.View(this, userQuery);
        }

        public static UserQueryDN ToUserQuery(QueryRequest request)
        {
            return request.ToUserQuery(
                DynamicQueryManager.Current.QueryDescription(request.QueryName),
                QueryLogic.RetrieveOrGenerateQuery(request.QueryName), FindOptions.DefaultElementsPerPage);
        }

        public ActionResult Delete(Lite<UserQueryDN> lite)
        {
            var queryName = QueryLogic.ToQueryName(lite.InDB().Select(uq => uq.Query.Key).FirstEx());

            Database.Delete<UserQueryDN>(lite);

            return Redirect(Navigator.FindRoute(queryName));
        }

        public ActionResult Save()
        {
            UserQueryDN userQuery = null;
            
            try
            {
                userQuery = this.ExtractEntity<UserQueryDN>();
            }
            catch(Exception ex){}

            var context = userQuery.ApplyChanges(this.ControllerContext, null, true).ValidateGlobal();

            if (context.GlobalErrors.Any())
            {
                ModelState.FromContext(context);
                return JsonAction.ModelState(ModelState);
            }

            userQuery = context.Value.Save();
            return JsonAction.Redirect(Navigator.ViewRoute(userQuery.ToLite()));
        }
    }
}
