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

namespace Signum.Web.Extensions.Note.Views
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
    
    #line 1 "..\..\Note\Views\Note.cshtml"
    using Signum.Entities.Notes;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Note/Views/Note.cshtml")]
    public partial class Note : System.Web.Mvc.WebViewPage<dynamic>
    {
        public Note()
        {
        }
        public override void Execute()
        {
            
            #line 2 "..\..\Note\Views\Note.cshtml"
 using (var cp = Html.TypeContext<NoteDN>())
{
    
            
            #line default
            #line hidden
            
            #line 4 "..\..\Note\Views\Note.cshtml"
Write(Html.ValueLine(cp, d => d.CreationDate, vl => vl.ReadOnly = true));

            
            #line default
            #line hidden
            
            #line 4 "..\..\Note\Views\Note.cshtml"
                                                                      
    
            
            #line default
            #line hidden
            
            #line 5 "..\..\Note\Views\Note.cshtml"
Write(Html.EntityLine(cp, d => d.Target, vl => vl.ReadOnly = true));

            
            #line default
            #line hidden
            
            #line 5 "..\..\Note\Views\Note.cshtml"
                                                                 
    
            
            #line default
            #line hidden
            
            #line 6 "..\..\Note\Views\Note.cshtml"
Write(Html.ValueLine(cp, d => d.Title));

            
            #line default
            #line hidden
            
            #line 6 "..\..\Note\Views\Note.cshtml"
                                     
    
            
            #line default
            #line hidden
            
            #line 7 "..\..\Note\Views\Note.cshtml"
Write(Html.EntityCombo(cp, d => d.NoteType, ec=>ec.Remove = true));

            
            #line default
            #line hidden
            
            #line 7 "..\..\Note\Views\Note.cshtml"
                                                                
    
            
            #line default
            #line hidden
            
            #line 8 "..\..\Note\Views\Note.cshtml"
Write(Html.ValueLine(cp, d => d.Text, vl =>
    {
        vl.ValueLineType = ValueLineType.TextArea;
        vl.ValueHtmlProps["rows"] = 6;
        vl.ValueHtmlProps["cols"] = 100;
    }));

            
            #line default
            #line hidden
            
            #line 13 "..\..\Note\Views\Note.cshtml"
      
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
