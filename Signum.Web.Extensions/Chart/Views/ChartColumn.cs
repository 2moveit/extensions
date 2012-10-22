﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Signum.Utilities;
    using Signum.Entities;
    using Signum.Web;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Caching;
    using System.Web.DynamicData;
    using System.Web.SessionState;
    using System.Web.Profile;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using System.Xml.Linq;
    using Signum.Web.Extensions.Properties;
    using Signum.Entities.DynamicQuery;
    using Signum.Engine.DynamicQuery;
    using Signum.Entities.Reflection;
    using Signum.Entities.Chart;
    using Signum.Web.Chart;
    using Signum.Engine.Authorization;
    using Signum.Entities.Authorization;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MvcRazorClassGenerator", "1.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Chart/Views/ChartColumn.cshtml")]
    public class _Page_Chart_Views_ChartColumn_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {


public System.Web.WebPages.HelperResult ColorLink(Type type)
    {
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {


     
        var identType = type.IsEnum ? EnumProxy.Generate(type) : type;
        
    
WriteTo(@__razor_helper_writer, Html.Field(Signum.Entities.Extensions.Properties.Resources.ColorsFor0.Formato(type.NiceName()),
                 Html.ActionLink(Signum.Engine.Chart.ChartColorLogic.Colors.Value.ContainsKey(identType) ? Resources.ViewPalette : Resources.CreatePalette,
                          (ColorChartController cc) => cc.Colors(Navigator.ResolveWebTypeName(identType)))));

                                                                                                           ;

});

}


        public _Page_Chart_Views_ChartColumn_cshtml()
        {
        }
        protected System.Web.HttpApplication ApplicationInstance
        {
            get
            {
                return ((System.Web.HttpApplication)(Context.ApplicationInstance));
            }
        }
        public override void Execute()
        {













WriteLiteral("\r\n");


 using (var tc = Html.TypeContext<ChartColumnDN>())
{
    if (tc.Value == null)
    {
        tc.Value = new ChartColumnDN();
    }
    

WriteLiteral("    <tr class=\"sf-chart-token\" data-token=\"");


                                       Write(((TypeElementContext<ChartColumnDN>)tc.Parent).Index);

WriteLiteral("\">\r\n        <td>");


        Write(tc.Value.PropertyLabel + (tc.Value.ScriptColumn.IsOptional?"?":""));

WriteLiteral("\r\n        </td>\r\n        <td>\r\n");


             if (tc.Value.GroupByVisible)
            {
                var groupCheck = new HtmlTag("input")
                    .IdName(tc.Compose("group"))
                    .Attr("type", "checkbox")
                    .Attr("value", "True")
                    .Class("sf-chart-group-trigger");

                if (!tc.Value.GroupByEnabled)
                {
                    groupCheck.Attr("disabled", "disabled");
                }

                bool groupResults = tc.Value.GroupByChecked;
                if (groupResults)
                {
                    groupCheck.Attr("checked", "checked");
                }
                
           Write(groupCheck.ToHtmlSelf());

                                        
                
           Write(Html.Hidden(tc.Compose("group"), groupResults));

                                                               
            }

WriteLiteral("        </td>\r\n        <td>\r\n            <div class=\"sf-query-token\">\r\n          " +
"      ");


           Write(Html.ChartTokenBuilder(tc.Value, tc.Value.ParentChart, (QueryDescription)ViewData[ViewDataKeys.QueryDescription], tc));

WriteLiteral("\r\n            </div>\r\n            <a class=\"sf-chart-token-config-trigger\">");


                                                Write(Resources.Chart_ToggleInfo);

WriteLiteral("</a>\r\n        </td>\r\n    </tr>\r\n");


    
   
    

WriteLiteral("    <tr class=\"sf-chart-token-config\" style=\"display: none\">\r\n        <td>\r\n     " +
"   </td>\r\n        <td colspan=\"2\">\r\n");


             using (Html.FieldInline())
            { 
                
           Write(Html.ValueLine(tc, ct => ct.DisplayName, vl => vl.ValueHtmlProps["class"] = "sf-chart-redraw-onchange"));

                                                                                                                        
                
           Write(Html.ValueLine(tc, ct => ct.Parameter1, vl => ChartClient.SetupParameter(vl, tc.Value, tc.Value.ScriptColumn.Parameter1)));

                                                                                                                                          
                
           Write(Html.ValueLine(tc, ct => ct.Parameter2, vl => ChartClient.SetupParameter(vl, tc.Value, tc.Value.ScriptColumn.Parameter2)));

                                                                                                                                          
                
           Write(Html.ValueLine(tc, ct => ct.Parameter3, vl => ChartClient.SetupParameter(vl, tc.Value, tc.Value.ScriptColumn.Parameter3)));

                                                                                                                                          
                if (tc.Value.Token != null && !Navigator.IsReadOnly(typeof(ChartColorDN), EntitySettingsContext.Admin))
                {
                    var type = tc.Value.Token.Type.CleanType();

                    if (type.IsEnum)
                    {
                
           Write(ColorLink(type));

                                ;
                    }
                    else
                    {
                        var imp = tc.Value.Token.GetImplementations();

                        if (imp != null && !imp.Value.IsByAll)
                        {
                            foreach (var item in imp.Value.Types.Iterate())
                            {
                                if (!item.IsFirst)
                                {

WriteLiteral("                                    ");

WriteLiteral(" | ");

WriteLiteral("\r\n");


                                }
                                
                                 
                            Write(ColorLink(item.Value));

                                                       ;
                            }
                        }
                    }
                }
            }

WriteLiteral("                </td>\r\n            </tr>\r\n");


}

        }
    }
}
