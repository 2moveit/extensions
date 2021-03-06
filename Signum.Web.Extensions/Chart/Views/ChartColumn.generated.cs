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

namespace Signum.Web.Extensions.Chart.Views
{
    using System;
    using System.Collections.Generic;
    
    #line 4 "..\..\Chart\Views\ChartColumn.cshtml"
    using System.Configuration;
    
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
    
    #line 9 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Engine.Authorization;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Engine.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Entities;
    
    #line default
    #line hidden
    
    #line 10 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Entities.Authorization;
    
    #line default
    #line hidden
    
    #line 7 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Entities.Chart;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Entities.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Entities.Reflection;
    
    #line default
    #line hidden
    using Signum.Utilities;
    
    #line 1 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Web;
    
    #line default
    #line hidden
    
    #line 8 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Web.Chart;
    
    #line default
    #line hidden
    
    #line 11 "..\..\Chart\Views\ChartColumn.cshtml"
    using Signum.Web.UserAssets;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Chart/Views/ChartColumn.cshtml")]
    public partial class ChartColumn : System.Web.Mvc.WebViewPage<dynamic>
    {

#line 14 "..\..\Chart\Views\ChartColumn.cshtml"
public System.Web.WebPages.HelperResult ColorLink(TypeContext tc, Type type)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {

#line 15 "..\..\Chart\Views\ChartColumn.cshtml"
 
    var identType = type.IsEnum ? EnumEntity.Generate(type) : type;


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    <div");

WriteLiteralTo(__razor_helper_writer, " class=\"col-sm-4\"");

WriteLiteralTo(__razor_helper_writer, ">\r\n");

WriteLiteralTo(__razor_helper_writer, "        ");


#line 18 "..\..\Chart\Views\ChartColumn.cshtml"
WriteTo(__razor_helper_writer, Html.FormGroup(tc, null, ChartMessage.ColorsFor0.NiceToString().Formato(type.NiceName()),
                    Html.ActionLink(Signum.Engine.Chart.ChartColorLogic.Colors.Value.ContainsKey(identType) ? ChartMessage.ViewPalette.NiceToString() : ChartMessage.CreatePalette.NiceToString(),
                                    (ColorChartController cc) => cc.Colors(Navigator.ResolveWebTypeName(identType)), new { @class = "form-control" })));


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\r\n    </div>\r\n");


#line 22 "..\..\Chart\Views\ChartColumn.cshtml"


#line default
#line hidden
});

#line 22 "..\..\Chart\Views\ChartColumn.cshtml"
}
#line default
#line hidden

        public ChartColumn()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n");

WriteLiteral("\r\n");

            
            #line 24 "..\..\Chart\Views\ChartColumn.cshtml"
 using (var tc = Html.TypeContext<ChartColumnDN>())
{

    if (tc.Value == null)
    {
        tc.Value = new ChartColumnDN();
    }
    

            
            #line default
            #line hidden
WriteLiteral("    <tr");

WriteLiteral(" class=\"sf-chart-token\"");

WriteLiteral(" data-token=\"");

            
            #line 32 "..\..\Chart\Views\ChartColumn.cshtml"
                                       Write(((TypeElementContext<ChartColumnDN>)tc.Parent).Index);

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">\r\n        <th>");

            
            #line 33 "..\..\Chart\Views\ChartColumn.cshtml"
        Write(tc.Value.PropertyLabel + (tc.Value.ScriptColumn.IsOptional ? "?" : ""));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </th>\r\n        <td");

WriteLiteral(" style=\"text-align: center\"");

WriteLiteral(">\r\n");

            
            #line 36 "..\..\Chart\Views\ChartColumn.cshtml"
            
            
            #line default
            #line hidden
            
            #line 36 "..\..\Chart\Views\ChartColumn.cshtml"
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
                
            
            #line default
            #line hidden
            
            #line 54 "..\..\Chart\Views\ChartColumn.cshtml"
           Write(groupCheck.ToHtmlSelf());

            
            #line default
            #line hidden
            
            #line 54 "..\..\Chart\Views\ChartColumn.cshtml"
                                        
                
            
            #line default
            #line hidden
            
            #line 55 "..\..\Chart\Views\ChartColumn.cshtml"
           Write(Html.Hidden(tc.Compose("group"), groupResults));

            
            #line default
            #line hidden
            
            #line 55 "..\..\Chart\Views\ChartColumn.cshtml"
                                                               
            }

            
            #line default
            #line hidden
WriteLiteral("        </td>\r\n        <td>\r\n            <div");

WriteLiteral(" class=\"sf-query-token\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 60 "..\..\Chart\Views\ChartColumn.cshtml"
           Write(Html.QueryTokenDNBuilder(tc.SubContext(a => a.Token), new QueryTokenBuilderSettings((QueryDescription)ViewData[ViewDataKeys.QueryDescription],
               SubTokensOptions.CanElement | (tc.Value.ParentChart.GroupResults && !tc.Value.IsGroupKey == true ? SubTokensOptions.CanAggregate : 0))
                {
                    ControllerUrl = null,
                }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <a");

WriteLiteral(" class=\"sf-chart-token-config-trigger\"");

WriteLiteral(">");

            
            #line 66 "..\..\Chart\Views\ChartColumn.cshtml"
                                                Write(ChartMessage.Chart_ToggleInfo.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n        </td>\r\n    </tr>\r\n");

            
            #line 69 "..\..\Chart\Views\ChartColumn.cshtml"
    
   
    

            
            #line default
            #line hidden
WriteLiteral("    <tr");

WriteLiteral(" class=\"sf-chart-token-config\"");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n        <td />\r\n\r\n        <td />\r\n        <td");

WriteLiteral(" colspan=\"1\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"form-vertical\"");

WriteLiteral(">\r\n");

            
            #line 78 "..\..\Chart\Views\ChartColumn.cshtml"
                
            
            #line default
            #line hidden
            
            #line 78 "..\..\Chart\Views\ChartColumn.cshtml"
                 using (var sc = tc.SubContext())
                {
                    sc.FormGroupSize = FormGroupSize.Small;
                    sc.FormGroupStyle = FormGroupStyle.Basic;

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"col-sm-4\"");

WriteLiteral(">");

            
            #line 83 "..\..\Chart\Views\ChartColumn.cshtml"
                                         Write(Html.ValueLine(sc, ct => ct.DisplayName, vl => vl.ValueHtmlProps["class"] = "sf-chart-redraw-onchange"));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

            
            #line 84 "..\..\Chart\Views\ChartColumn.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 84 "..\..\Chart\Views\ChartColumn.cshtml"
                         if (sc.Value.ScriptColumn.Parameter1 != null)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <div");

WriteLiteral(" class=\"col-sm-4\"");

WriteLiteral(">");

            
            #line 86 "..\..\Chart\Views\ChartColumn.cshtml"
                                             Write(Html.ValueLine(sc, ct => ct.Parameter1, vl => ChartClient.SetupParameter(vl, sc.Value, sc.Value.ScriptColumn.Parameter1)));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

            
            #line 87 "..\..\Chart\Views\ChartColumn.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                        ");

            
            #line 88 "..\..\Chart\Views\ChartColumn.cshtml"
                         if (sc.Value.ScriptColumn.Parameter2 != null)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <div");

WriteLiteral(" class=\"col-sm-4\"");

WriteLiteral(">");

            
            #line 90 "..\..\Chart\Views\ChartColumn.cshtml"
                                             Write(Html.ValueLine(sc, ct => ct.Parameter2, vl => ChartClient.SetupParameter(vl, sc.Value, sc.Value.ScriptColumn.Parameter2)));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

            
            #line 91 "..\..\Chart\Views\ChartColumn.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                        ");

            
            #line 92 "..\..\Chart\Views\ChartColumn.cshtml"
                         if (sc.Value.ScriptColumn.Parameter3 != null)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <div");

WriteLiteral(" class=\"col-sm-4\"");

WriteLiteral(">");

            
            #line 94 "..\..\Chart\Views\ChartColumn.cshtml"
                                             Write(Html.ValueLine(sc, ct => ct.Parameter3, vl => ChartClient.SetupParameter(vl, sc.Value, sc.Value.ScriptColumn.Parameter3)));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

            
            #line 95 "..\..\Chart\Views\ChartColumn.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 97 "..\..\Chart\Views\ChartColumn.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 97 "..\..\Chart\Views\ChartColumn.cshtml"
                         if (sc.Value.Token != null && !Navigator.IsReadOnly(typeof(ChartColorDN)))
                        {
                            var type = sc.Value.Token.Token.Type.CleanType();

                            if (type.UnNullify().IsEnum && Signum.Engine.Maps.Schema.Current.Tables.ContainsKey(EnumEntity.Generate(type.UnNullify())))
                            {
                            
            
            #line default
            #line hidden
            
            #line 103 "..\..\Chart\Views\ChartColumn.cshtml"
                       Write(ColorLink(sc, type.UnNullify()));

            
            #line default
            #line hidden
            
            #line 103 "..\..\Chart\Views\ChartColumn.cshtml"
                                                            ;
                            }
                            else
                            {
                                var imp = sc.Value.Token.Token.GetImplementations();

                                if (imp != null && !imp.Value.IsByAll)
                                {
                                    foreach (var item in imp.Value.Types.Iterate())
                                    {
                            
            
            #line default
            #line hidden
            
            #line 113 "..\..\Chart\Views\ChartColumn.cshtml"
                       Write(ColorLink(sc, item.Value));

            
            #line default
            #line hidden
            
            #line 113 "..\..\Chart\Views\ChartColumn.cshtml"
                                                      ;
                                    }
                                }
                            }
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n");

            
            #line 120 "..\..\Chart\Views\ChartColumn.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </div>\r\n        </td>\r\n    </tr>\r\n");

            
            #line 124 "..\..\Chart\Views\ChartColumn.cshtml"
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
