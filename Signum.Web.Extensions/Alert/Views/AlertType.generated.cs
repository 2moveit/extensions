﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.Alert.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Signum.Entities;
    
    #line 1 "..\..\Alert\Views\AlertType.cshtml"
    using Signum.Entities.Alerts;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Alert/Views/AlertType.cshtml")]
    public partial class AlertType : System.Web.Mvc.WebViewPage<dynamic>
    {
        public AlertType()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 3 "..\..\Alert\Views\AlertType.cshtml"
 using (var cp = Html.TypeContext<AlertTypeDN>())
{
    
            
            #line default
            #line hidden
            
            #line 5 "..\..\Alert\Views\AlertType.cshtml"
Write(Html.ValueLine(cp, d => d.Name));

            
            #line default
            #line hidden
            
            #line 5 "..\..\Alert\Views\AlertType.cshtml"
                                    
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
