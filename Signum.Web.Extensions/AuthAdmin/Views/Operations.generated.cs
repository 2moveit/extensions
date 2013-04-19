﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.AuthAdmin.Views
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
    using Signum.Engine;
    using Signum.Entities;
    using Signum.Entities.Authorization;
    using Signum.Utilities;
    using Signum.Web;
    using Signum.Web.Auth;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.4.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/AuthAdmin/Views/Operations.cshtml")]
    public partial class Operations : System.Web.Mvc.WebViewPage<dynamic>
    {
        public Operations()
        {
        }
        public override void Execute()
        {

            
            #line 1 "..\..\AuthAdmin\Views\Operations.cshtml"
Write(Html.ScriptCss("~/authAdmin/Content/SF_AuthAdmin.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 2 "..\..\AuthAdmin\Views\Operations.cshtml"
 using (var tc = Html.TypeContext<OperationRulePack>())
{
    
            
            #line default
            #line hidden
            
            #line 4 "..\..\AuthAdmin\Views\Operations.cshtml"
Write(Html.EntityLine(tc, f => f.Role));

            
            #line default
            #line hidden
            
            #line 4 "..\..\AuthAdmin\Views\Operations.cshtml"
                                     
    
            
            #line default
            #line hidden
            
            #line 5 "..\..\AuthAdmin\Views\Operations.cshtml"
Write(Html.ValueLine(tc, f => f.DefaultRule, vl => { vl.UnitText = tc.Value.DefaultLabel; }));

            
            #line default
            #line hidden
            
            #line 5 "..\..\AuthAdmin\Views\Operations.cshtml"
                                                                                           
    
            
            #line default
            #line hidden
            
            #line 6 "..\..\AuthAdmin\Views\Operations.cshtml"
Write(Html.EntityLine(tc, f => f.Type));

            
            #line default
            #line hidden
            
            #line 6 "..\..\AuthAdmin\Views\Operations.cshtml"
                                     


            
            #line default
            #line hidden
WriteLiteral("    <table class=\"sf-auth-rules\" id=\"operations\">\r\n        <thead>\r\n            <" +
"tr>\r\n                <th>\r\n                    ");


            
            #line 12 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(AuthMessage.OperationsAscx_Operation.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");


            
            #line 15 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(AuthMessage.OperationsAscx_Allow.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th> \r\n                <th>\r\n                    ");


            
            #line 18 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(AuthMessage.OperationsAscx_DBOnly.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");


            
            #line 21 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(AuthMessage.OperationsAscx_None.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");


            
            #line 24 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(AuthMessage.OperationsAscx_Overriden.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n");


            
            #line 28 "..\..\AuthAdmin\Views\Operations.cshtml"
         foreach (var item in tc.TypeElementContext(p => p.Rules))
        {

            
            #line default
            #line hidden
WriteLiteral("            <tr>\r\n                <td>\r\n                    ");


            
            #line 32 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(Html.Span(null, item.Value.Resource.Name));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    ");


            
            #line 33 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(Html.HiddenRuntimeInfo(item, i => i.Resource));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    ");


            
            #line 34 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(Html.Hidden(item.Compose("AllowedBase"), item.Value.AllowedBase));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n                 <td>\r\n                    <a class=\"sf-" +
"auth-chooser sf-auth-modify\">\r\n                        ");


            
            #line 38 "..\..\AuthAdmin\Views\Operations.cshtml"
                   Write(Html.RadioButton(item.Compose("Allowed"), "Allow", item.Value.Allowed == OperationAllowed.Allow));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </a>\r\n                </td>\r\n                <td>\r\n        " +
"            <a class=\"sf-auth-chooser sf-auth-read\">\r\n                        ");


            
            #line 43 "..\..\AuthAdmin\Views\Operations.cshtml"
                   Write(Html.RadioButton(item.Compose("Allowed"), "DBOnly", item.Value.Allowed == OperationAllowed.DBOnly));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </a>\r\n                </td>\r\n                <td>\r\n        " +
"            <a class=\"sf-auth-chooser sf-auth-none\">\r\n                        ");


            
            #line 48 "..\..\AuthAdmin\Views\Operations.cshtml"
                   Write(Html.RadioButton(item.Compose("Allowed"), "None", item.Value.Allowed == OperationAllowed.None));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </a>\r\n                </td>\r\n                <td>\r\n        " +
"            ");


            
            #line 52 "..\..\AuthAdmin\Views\Operations.cshtml"
               Write(Html.CheckBox(item.Compose("Overriden"), item.Value.Overriden, new { disabled = "disabled", @class = "sf-auth-overriden" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");


            
            #line 55 "..\..\AuthAdmin\Views\Operations.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </table>\r\n");


            
            #line 57 "..\..\AuthAdmin\Views\Operations.cshtml"
}
            
            #line default
            #line hidden
WriteLiteral(" ");


        }
    }
}
#pragma warning restore 1591
