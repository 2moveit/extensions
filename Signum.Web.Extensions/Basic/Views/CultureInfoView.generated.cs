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

namespace Signum.Web.Extensions.Basic.Views
{
    using System;
    using System.Collections.Generic;
    
    #line 2 "..\..\Basic\Views\CultureInfoView.cshtml"
    using System.Globalization;
    
    #line default
    #line hidden
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
    
    #line 1 "..\..\Basic\Views\CultureInfoView.cshtml"
    using Signum.Entities.Basics;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Basic/Views/CultureInfoView.cshtml")]
    public partial class CultureInfoView : System.Web.Mvc.WebViewPage<dynamic>
    {
        public CultureInfoView()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Basic\Views\CultureInfoView.cshtml"
 using (var tc = Html.TypeContext<CultureInfoDN>())
{
    
            
            #line default
            #line hidden
            
            #line 5 "..\..\Basic\Views\CultureInfoView.cshtml"
Write(Html.ValueLine(tc, t => t.Name, vl => 
    { 
        vl.ValueLineType = ValueLineType.Combo;
        vl.EnumComboItems = CultureInfo.GetCultures(CultureTypes.NeutralCultures | CultureTypes.SpecificCultures)
            .OrderBy(a=>a.Name)
            .Select(ci => new SelectListItem
            {
                Text = "{0} :  {1}".Formato(ci.Name, ci.DisplayName),
                Value = ci.Name,
                Selected = object.Equals(tc.Value.Name, ci.Name),
            }).ToList();
        vl.ValueHtmlProps.Clear();
    }));

            
            #line default
            #line hidden
            
            #line 17 "..\..\Basic\Views\CultureInfoView.cshtml"
      ;
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
