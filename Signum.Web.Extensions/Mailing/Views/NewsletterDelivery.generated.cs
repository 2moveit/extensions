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

namespace Signum.Web.Extensions.Mailing.Views
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
    
    #line 1 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
    using Signum.Entities.Mailing;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
    using Signum.Entities.Processes;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/NewsletterDelivery.cshtml")]
    public partial class NewsletterDelivery : System.Web.Mvc.WebViewPage<dynamic>
    {
        public NewsletterDelivery()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
 using (var nc = Html.TypeContext<NewsletterDeliveryDN>()) 
{
	
            
            #line default
            #line hidden
            
            #line 6 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
Write(Html.ValueLine(nc, n => n.Sent));

            
            #line default
            #line hidden
            
            #line 6 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
                                 
	
            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
Write(Html.ValueLine(nc, n => n.SendDate));

            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
                                     
	
            
            #line default
            #line hidden
            
            #line 8 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
Write(Html.EntityLine(nc, n => n.Recipient));

            
            #line default
            #line hidden
            
            #line 8 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
                                       
	
            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
Write(Html.EntityLine(nc, n => n.Newsletter));

            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
                                        

    
            
            #line default
            #line hidden
            
            #line 11 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
Write(Html.SearchControl(new FindOptions(typeof(ProcessExceptionLineDN), "Line", nc.Value), new Context(nc, "Exceptions")));

            
            #line default
            #line hidden
            
            #line 11 "..\..\Mailing\Views\NewsletterDelivery.cshtml"
                                                                                                                         
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
