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

namespace Signum.Web.Extensions.Help.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    
    #line 4 "..\..\Help\Views\EntityProperty.cshtml"
    using System.Reflection;
    
    #line default
    #line hidden
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
    
    #line 2 "..\..\Help\Views\EntityProperty.cshtml"
    using Signum.Engine.Help;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 1 "..\..\Help\Views\EntityProperty.cshtml"
    using Signum.Entities.Reflection;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 5 "..\..\Help\Views\EntityProperty.cshtml"
    using Signum.Web.Extensions;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Help\Views\EntityProperty.cshtml"
    using Signum.Web.Help;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Help/Views/EntityProperty.cshtml")]
    public partial class EntityProperty : System.Web.Mvc.WebViewPage<dynamic>
    {
        public EntityProperty()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 7 "..\..\Help\Views\EntityProperty.cshtml"
   
    Node<KeyValuePair<string, PropertyHelp>> ep = (Node<KeyValuePair<string, PropertyHelp>>)Model;
    KeyValuePair<string, PropertyHelp> k = ep.Value;

            
            #line default
            #line hidden
WriteLiteral("\r\n<span");

WriteLiteral(" class=\'shortcut\'");

WriteLiteral(">[p:");

            
            #line 11 "..\..\Help\Views\EntityProperty.cshtml"
                      Write(ViewData["EntityName"]);

            
            #line default
            #line hidden
WriteLiteral(".");

            
            #line 11 "..\..\Help\Views\EntityProperty.cshtml"
                                                Write(k.Key);

            
            #line default
            #line hidden
WriteLiteral("]</span>\r\n<dt>");

            
            #line 12 "..\..\Help\Views\EntityProperty.cshtml"
Write(k.Value.PropertyInfo.NiceName());

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n<dd>\r\n");

WriteLiteral("    ");

            
            #line 14 "..\..\Help\Views\EntityProperty.cshtml"
Write(Html.WikiParse(k.Value.Info, HelpClient.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 15 "..\..\Help\Views\EntityProperty.cshtml"
Write(Html.TextArea("p-" + k.Key.Replace("/", "_"), k.Value.UserDescription, new { @class = "editable" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <span");

WriteLiteral(" class=\"editor\"");

WriteAttribute("id", Tuple.Create(" id=\"", 622), Tuple.Create("\"", 660)
, Tuple.Create(Tuple.Create("", 627), Tuple.Create("p-", 627), true)
            
            #line 16 "..\..\Help\Views\EntityProperty.cshtml"
, Tuple.Create(Tuple.Create("", 629), Tuple.Create<System.Object, System.Int32>(k.Key.Replace("/", "_")
            
            #line default
            #line hidden
, 629), false)
, Tuple.Create(Tuple.Create("", 653), Tuple.Create("-editor", 653), true)
);

WriteLiteral(">");

            
            #line 16 "..\..\Help\Views\EntityProperty.cshtml"
                                                           Write(Html.WikiParse(k.Value.UserDescription, HelpClient.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n</dd>\r\n");

            
            #line 18 "..\..\Help\Views\EntityProperty.cshtml"
 if (ep.Children.Count > 0)
{

            
            #line default
            #line hidden
WriteLiteral("    <dl");

WriteLiteral(" class=\"embedded\"");

WriteLiteral(">\r\n");

            
            #line 21 "..\..\Help\Views\EntityProperty.cshtml"
        
            
            #line default
            #line hidden
            
            #line 21 "..\..\Help\Views\EntityProperty.cshtml"
         foreach (var v in ep.Children)
        {
            Html.RenderPartial(HelpClient.ViewEntityPropertyUrl, v);
        }

            
            #line default
            #line hidden
WriteLiteral("    </dl>\r\n");

            
            #line 26 "..\..\Help\Views\EntityProperty.cshtml"
}
            
            #line default
            #line hidden
WriteLiteral(" ");

        }
    }
}
#pragma warning restore 1591
