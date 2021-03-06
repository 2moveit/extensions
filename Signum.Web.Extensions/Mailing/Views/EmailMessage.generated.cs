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
    
    #line 1 "..\..\Mailing\Views\EmailMessage.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 2 "..\..\Mailing\Views\EmailMessage.cshtml"
    using Signum.Entities.Mailing;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 4 "..\..\Mailing\Views\EmailMessage.cshtml"
    using Signum.Web.Files;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Mailing\Views\EmailMessage.cshtml"
    using Signum.Web.Mailing;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/EmailMessage.cshtml")]
    public partial class EmailMessage : System.Web.Mvc.WebViewPage<dynamic>
    {
        public EmailMessage()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 6 "..\..\Mailing\Views\EmailMessage.cshtml"
Write(Html.ScriptCss("~/Mailing/Content/Mailing.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n\r\n");

            
            #line 9 "..\..\Mailing\Views\EmailMessage.cshtml"
 using (var e = Html.TypeContext<EmailMessageDN>())
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"sf-email-message\"");

WriteLiteral(">\r\n\r\n");

            
            #line 13 "..\..\Mailing\Views\EmailMessage.cshtml"
        
            
            #line default
            #line hidden
            
            #line 13 "..\..\Mailing\Views\EmailMessage.cshtml"
         using (var tabs = Html.Tabs(e))
        {
            if (e.Value.State != EmailMessageState.Created)
            {
                e.ReadOnly = true;
            }


            tabs.Tab("sfEmailMessage", typeof(EmailMessageDN).NiceName(), 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "\r\n\r\n\r\n            <div");

WriteLiteralTo(__razor_template_writer, " class=\"row\"");

WriteLiteralTo(__razor_template_writer, ">\r\n                <div");

WriteLiteralTo(__razor_template_writer, " class=\"repeater-inline form-inline col-sm-8\"");

WriteLiteralTo(__razor_template_writer, ">\r\n");

WriteLiteralTo(__razor_template_writer, "                    ");

            
            #line 26 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityLineDetail(e, f => f.From));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                    ");

            
            #line 27 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityRepeater(e, f => f.Recipients));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                    ");

            
            #line 28 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityRepeater(e, f => f.Attachments));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n                </div>\r\n                <div");

WriteLiteralTo(__razor_template_writer, " class=\"col-sm-4\"");

WriteLiteralTo(__razor_template_writer, ">\r\n                    <fieldset>\r\n                        <legend>Properties</le" +
"gend>\r\n");

WriteLiteralTo(__razor_template_writer, "                        ");

            
            #line 33 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(e, f => f.State));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                        ");

            
            #line 34 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(e, f => f.Sent));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                        ");

            
            #line 35 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityLine(e, f => f.Exception));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                        ");

            
            #line 36 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityLine(e, f => f.Template));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                        ");

            
            #line 37 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityLine(e, f => f.Package));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                        ");

            
            #line 38 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(e, f => f.IsBodyHtml));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                        ");

            
            #line 39 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(e, f => f.BodyHash));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n                    </fieldset>\r\n                </div>\r\n            </div>\r\n\r\n" +
"");

WriteLiteralTo(__razor_template_writer, "            ");

            
            #line 44 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityLine(e, f => f.Target));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "            ");

            
            #line 45 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(e, f => f.Subject));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

            
            #line 46 "..\..\Mailing\Views\EmailMessage.cshtml"
            
            
            #line default
            #line hidden
            
            #line 46 "..\..\Mailing\Views\EmailMessage.cshtml"
             if (e.Value.State == EmailMessageState.Created)
            {
                
            
            #line default
            #line hidden
            
            #line 48 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(e, f => f.Body, vl =>
           {
               vl.ValueLineType = ValueLineType.TextArea;
               vl.ValueHtmlProps["style"] = "width:100%; height:180px;";
           }));

            
            #line default
            #line hidden
            
            #line 52 "..\..\Mailing\Views\EmailMessage.cshtml"
             


            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "<script>\r\n    $(function () {\r\n");

WriteLiteralTo(__razor_template_writer, "        ");

            
            #line 56 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, MailingClient.Module["initHtmlEditor"](e.Compose("Body")));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n    });\r\n                </script>\r\n");

            
            #line 59 "..\..\Mailing\Views\EmailMessage.cshtml"
            }
            else
            {
                var body = MailingClient.GetWebMailBody(e.Value.Body, new WebMailOptions
                {
                    Attachments = e.Value.Attachments,
                    UntrustedImage = null,
                    Url = Url,
                });

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "<h3>");

            
            #line 68 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, EmailMessageMessage.Message.NiceToString());

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, ":</h3>\r\n");

WriteLiteralTo(__razor_template_writer, "<div>\r\n");

            
            #line 70 "..\..\Mailing\Views\EmailMessage.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 70 "..\..\Mailing\Views\EmailMessage.cshtml"
                         if (e.Value.IsBodyHtml)
                        {            

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "<iframe");

WriteAttributeTo(__razor_template_writer, "id", Tuple.Create(" id=\"", 2465), Tuple.Create("\"", 2490)
            
            #line 72 "..\..\Mailing\Views\EmailMessage.cshtml"
, Tuple.Create(Tuple.Create("", 2470), Tuple.Create<System.Object, System.Int32>(e.Compose("iframe")
            
            #line default
            #line hidden
, 2470), false)
);

WriteLiteralTo(__razor_template_writer, " style=\"width:90%\"");

WriteLiteralTo(__razor_template_writer, ">\r\n");

WriteLiteralTo(__razor_template_writer, "                                ");

            
            #line 73 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.Raw(body));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n                            </iframe>\r\n");

WriteLiteralTo(__razor_template_writer, "<script>\r\n    $(function () {\r\n        var iframe = $(\"");

            
            #line 77 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, e.Compose("iframe"));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\");\r\n");

WriteLiteralTo(__razor_template_writer, "        ");

            
            #line 78 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, MailingClient.Module["activateIFrame"](new Newtonsoft.Json.Linq.JRaw("iframe")));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n    });\r\n                            </script>\r\n");

            
            #line 81 "..\..\Mailing\Views\EmailMessage.cshtml"
                        }
                        else
                        {

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "<pre>\r\n");

WriteLiteralTo(__razor_template_writer, "                            ");

            
            #line 85 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.Raw(HttpUtility.HtmlEncode(e.Value.Body)));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n                        </pre>\r\n");

            
            #line 87 "..\..\Mailing\Views\EmailMessage.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "            </div>\r\n");

            
            #line 89 "..\..\Mailing\Views\EmailMessage.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "            ");

})
            
            #line 90 "..\..\Mailing\Views\EmailMessage.cshtml"
                   );



            if (e.Value.Mixins.OfType<EmailReceptionMixin>().Any() && e.Value.Mixin<EmailReceptionMixin>().ReceptionInfo != null)
            {
                using (var ri = e.SubContext(f => f.Mixin<EmailReceptionMixin>().ReceptionInfo))
                {
                    tabs.Tab("sfEmailReceptionInfo", ri.PropertyRoute.PropertyInfo.NiceName(), 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "\r\n            <fieldset>\r\n                <legend>Properties</legend>\r\n\r\n");

WriteLiteralTo(__razor_template_writer, "                ");

            
            #line 102 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.EntityLine(ri, f => f.Reception));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                ");

            
            #line 103 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(ri, f => f.UniqueId));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                ");

            
            #line 104 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(ri, f => f.SentDate));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                ");

            
            #line 105 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(ri, f => f.ReceivedDate));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                ");

            
            #line 106 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, Html.ValueLine(ri, f => f.DeletionDate));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n\r\n            </fieldset>\r\n\r\n            <pre>");

            
            #line 110 "..\..\Mailing\Views\EmailMessage.cshtml"
WriteTo(__razor_template_writer, ri.Value.RawContent);

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "</pre>\r\n            ");

})
            
            #line 111 "..\..\Mailing\Views\EmailMessage.cshtml"
                   );
                }
            }
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n");

            
            #line 116 "..\..\Mailing\Views\EmailMessage.cshtml"
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
