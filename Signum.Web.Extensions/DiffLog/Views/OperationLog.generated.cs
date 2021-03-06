﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.DiffLog.Views
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
    
    #line 1 "..\..\DiffLog\Views\OperationLog.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    
    #line 5 "..\..\DiffLog\Views\OperationLog.cshtml"
    using Signum.Engine.Operations;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 2 "..\..\DiffLog\Views\OperationLog.cshtml"
    using Signum.Entities.Basics;
    
    #line default
    #line hidden
    
    #line 3 "..\..\DiffLog\Views\OperationLog.cshtml"
    using Signum.Entities.DiffLog;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 4 "..\..\DiffLog\Views\OperationLog.cshtml"
    using Signum.Web.Files;
    
    #line default
    #line hidden
    
    #line 6 "..\..\DiffLog\Views\OperationLog.cshtml"
    using Signum.Web.Translation;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/DiffLog/Views/OperationLog.cshtml")]
    public partial class OperationLog : System.Web.Mvc.WebViewPage<dynamic>
    {
        public OperationLog()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 8 "..\..\DiffLog\Views\OperationLog.cshtml"
 using (var e = Html.TypeContext<OperationLogDN>())
{
    e.LabelColumns = new BsColumn(4);

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 13 "..\..\DiffLog\Views\OperationLog.cshtml"
       Write(Html.EntityLine(e, f => f.Operation));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 16 "..\..\DiffLog\Views\OperationLog.cshtml"
       Write(Html.EntityLine(e, f => f.User));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 19 "..\..\DiffLog\Views\OperationLog.cshtml"


            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 22 "..\..\DiffLog\Views\OperationLog.cshtml"
       Write(Html.EntityLine(e, f => f.Target));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\DiffLog\Views\OperationLog.cshtml"
       Write(Html.EntityLine(e, f => f.Origin));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 28 "..\..\DiffLog\Views\OperationLog.cshtml"
    

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 31 "..\..\DiffLog\Views\OperationLog.cshtml"
       Write(Html.ValueLine(e, f => f.Start));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\DiffLog\Views\OperationLog.cshtml"
       Write(Html.ValueLine(e, f => f.End));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 37 "..\..\DiffLog\Views\OperationLog.cshtml"
    
    e.LabelColumns = new BsColumn(2);
    if (e.Value.Exception != null)
    {
    
            
            #line default
            #line hidden
            
            #line 41 "..\..\DiffLog\Views\OperationLog.cshtml"
Write(Html.EntityLine(e, f => f.Exception));

            
            #line default
            #line hidden
            
            #line 41 "..\..\DiffLog\Views\OperationLog.cshtml"
                                         
    }
    

            
            #line default
            #line hidden
WriteLiteral(@"    <style>
        .colorIcon
        {
            color: black;
            padding: 2px;
        }

            .colorIcon.red
            {
                background: #FF8B8B;
            }

            .colorIcon.mini.red
            {
                background: #FFD1D1;
            }

            .colorIcon.green
            {
                background: #72F272;
            }

            .colorIcon.mini.green
            {
                background: #CEF3CE;
            }

        .nav-tabs > li.linkTab > a:hover
        {
            border-color: transparent;
            background-color: transparent;
        }
    </style>
");

            
            #line 77 "..\..\DiffLog\Views\OperationLog.cshtml"

    using (var diff = e.SubContext(a => a.Mixin<DiffLogMixin>()))
    {
        var minMax = Signum.Engine.DiffLog.DiffLogLogic.OperationLogNextPrev(e.Value);
        
        
        using (var tabs = Html.Tabs(e))
        {
            if (diff.Value.InitialState != null)
            {
                var prev = minMax.Min;

                if (prev != null && prev.Mixin<DiffLogMixin>().FinalState != null)
                {
                    tabs.Tab(new Signum.Web.DiffLog.LinkTab( 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, " \r\n<span>");

            
            #line 92 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, DiffLogMessage.PreviousLog.NiceToString());

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n    <span");

WriteLiteralTo(__razor_template_writer, " class=\"glyphicon glyphicon-new-window\"");

WriteLiteralTo(__razor_template_writer, "/></span> \r\n");

WriteLiteralTo(__razor_template_writer, " ");

})
            
            #line 94 "..\..\DiffLog\Views\OperationLog.cshtml"
        , Navigator.NavigateRoute(prev)) {  ToolTip = DiffLogMessage.NavigatesToThePreviousOperationLog.NiceToString() });
                    
                    var eq = prev.Mixin<DiffLogMixin>().FinalState == diff.Value.InitialState;

                    tabs.Tab(new Tab("diffPrev", 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 2716), Tuple.Create("\"", 2791)
, Tuple.Create(Tuple.Create("", 2724), Tuple.Create("glyphicon", 2724), true)
, Tuple.Create(Tuple.Create(" ", 2733), Tuple.Create("glyphicon-fast-backward", 2734), true)
, Tuple.Create(Tuple.Create(" ", 2757), Tuple.Create("colorIcon", 2758), true)
, Tuple.Create(Tuple.Create(" ", 2767), Tuple.Create("red", 2768), true)
            
            #line 99 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 2771), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 2772), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 2811), Tuple.Create("\"", 2888)
, Tuple.Create(Tuple.Create("", 2819), Tuple.Create("glyphicon", 2819), true)
, Tuple.Create(Tuple.Create(" ", 2828), Tuple.Create("glyphicon-step-backward", 2829), true)
, Tuple.Create(Tuple.Create(" ", 2852), Tuple.Create("colorIcon", 2853), true)
, Tuple.Create(Tuple.Create(" ", 2862), Tuple.Create("green", 2863), true)
            
            #line 100 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 2868), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 2869), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    ");

})
            
            #line 101 "..\..\DiffLog\Views\OperationLog.cshtml"
           , 
    
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "<pre>");

            
            #line 102 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, TranslationClient.Diff(prev.Mixin<DiffLogMixin>().FinalState, diff.Value.InitialState));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "</pre>");

})
            
            #line 102 "..\..\DiffLog\Views\OperationLog.cshtml"
                                                                                                       ) { ToolTip = DiffLogMessage.DifferenceBetweenFinalStateOfPreviousLogAndTheInitialState.NiceToString() });
                }

                tabs.Tab(new Tab("initialGraph", Html.PropertyNiceName(() => diff.Value.InitialState), 
    
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "<pre><code>");

            
            #line 106 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, diff.Value.InitialState);

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "</code></pre>");

})
            
            #line 106 "..\..\DiffLog\Views\OperationLog.cshtml"
                                                                  ) { ToolTip = DiffLogMessage.StateWhenTheOperationStarted.NiceToString() });
            }

            if (diff.Value.InitialState != null && diff.Value.FinalState != null)
            {
                var eq = diff.Value.InitialState == diff.Value.FinalState;

                tabs.Tab(new Tab("diff", 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 3647), Tuple.Create("\"", 3722)
, Tuple.Create(Tuple.Create("", 3655), Tuple.Create("glyphicon", 3655), true)
, Tuple.Create(Tuple.Create(" ", 3664), Tuple.Create("glyphicon-step-backward", 3665), true)
, Tuple.Create(Tuple.Create(" ", 3688), Tuple.Create("colorIcon", 3689), true)
, Tuple.Create(Tuple.Create(" ", 3698), Tuple.Create("red", 3699), true)
            
            #line 114 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 3702), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 3703), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 3742), Tuple.Create("\"", 3818)
, Tuple.Create(Tuple.Create("", 3750), Tuple.Create("glyphicon", 3750), true)
, Tuple.Create(Tuple.Create(" ", 3759), Tuple.Create("glyphicon-step-forward", 3760), true)
, Tuple.Create(Tuple.Create(" ", 3782), Tuple.Create("colorIcon", 3783), true)
, Tuple.Create(Tuple.Create(" ", 3792), Tuple.Create("green", 3793), true)
            
            #line 115 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 3798), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 3799), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    ");

})
            
            #line 116 "..\..\DiffLog\Views\OperationLog.cshtml"
           , 
    
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "<pre>");

            
            #line 117 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, TranslationClient.Diff(diff.Value.InitialState, diff.Value.FinalState));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "</pre>");

})
            
            #line 117 "..\..\DiffLog\Views\OperationLog.cshtml"
                                                                                       ) { Active = true, ToolTip = DiffLogMessage.DifferenceBetweenInitialStateAndFinalState.NiceToString() });
            }

            if (diff.Value.FinalState != null)
            {
                tabs.Tab(new Tab("FinalState", Html.PropertyNiceName(() => diff.Value.FinalState), 
    
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "<pre><code>");

            
            #line 123 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, diff.Value.FinalState);

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "</code></pre>");

})
            
            #line 123 "..\..\DiffLog\Views\OperationLog.cshtml"
                                                                ) { ToolTip = DiffLogMessage.StateWhenTheOperationFinished.NiceToString() });

                var next = minMax.Max;

                if (next != null && next.Mixin<DiffLogMixin>().InitialState != null)
                {
                    var eq = diff.Value.FinalState == next.Mixin<DiffLogMixin>().InitialState;

                    tabs.Tab(new Tab("diffNext", 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 4676), Tuple.Create("\"", 4750)
, Tuple.Create(Tuple.Create("", 4684), Tuple.Create("glyphicon", 4684), true)
, Tuple.Create(Tuple.Create(" ", 4693), Tuple.Create("glyphicon-step-forward", 4694), true)
, Tuple.Create(Tuple.Create(" ", 4716), Tuple.Create("colorIcon", 4717), true)
, Tuple.Create(Tuple.Create(" ", 4726), Tuple.Create("red", 4727), true)
            
            #line 132 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 4730), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 4731), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 4770), Tuple.Create("\"", 4846)
, Tuple.Create(Tuple.Create("", 4778), Tuple.Create("glyphicon", 4778), true)
, Tuple.Create(Tuple.Create(" ", 4787), Tuple.Create("glyphicon-fast-forward", 4788), true)
, Tuple.Create(Tuple.Create(" ", 4810), Tuple.Create("colorIcon", 4811), true)
, Tuple.Create(Tuple.Create(" ", 4820), Tuple.Create("green", 4821), true)
            
            #line 133 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 4826), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 4827), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    ");

})
            
            #line 134 "..\..\DiffLog\Views\OperationLog.cshtml"
           , 
    
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "<pre>");

            
            #line 135 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, TranslationClient.Diff(diff.Value.FinalState, next.Mixin<DiffLogMixin>().InitialState));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "</pre>");

})
            
            #line 135 "..\..\DiffLog\Views\OperationLog.cshtml"
                                                                                                       ) { ToolTip = DiffLogMessage.DifferenceBetweenFinalStateAndTheInitialStateOfNextLog.NiceToString() });

                    tabs.Tab(new Signum.Web.DiffLog.LinkTab( 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, " \r\n<span>");

            
            #line 138 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, DiffLogMessage.NextLog.NiceToString());

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n    <span");

WriteLiteralTo(__razor_template_writer, " class=\"glyphicon glyphicon-new-window\"");

WriteLiteralTo(__razor_template_writer, "/></span> \r\n");

WriteLiteralTo(__razor_template_writer, " ");

})
            
            #line 140 "..\..\DiffLog\Views\OperationLog.cshtml"
        , Navigator.NavigateRoute(next)) { ToolTip = DiffLogMessage.NavigatesToTheNextOperationLog.NiceToString() });
                }
                else
                {
                    var entity = (Lite<IdentifiableEntity>)e.Value.Target;

                    var dump = !entity.Exists() ? null : entity.Retrieve().Dump();

                    var eq = diff.Value.FinalState == dump;

                    tabs.Tab(new Tab("diffCurrent", 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 5734), Tuple.Create("\"", 5808)
, Tuple.Create(Tuple.Create("", 5742), Tuple.Create("glyphicon", 5742), true)
, Tuple.Create(Tuple.Create(" ", 5751), Tuple.Create("glyphicon-step-forward", 5752), true)
, Tuple.Create(Tuple.Create(" ", 5774), Tuple.Create("colorIcon", 5775), true)
, Tuple.Create(Tuple.Create(" ", 5784), Tuple.Create("red", 5785), true)
            
            #line 151 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 5788), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 5789), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    <span");

WriteAttributeTo(__razor_template_writer, "class", Tuple.Create(" class=\"", 5828), Tuple.Create("\"", 5904)
, Tuple.Create(Tuple.Create("", 5836), Tuple.Create("glyphicon", 5836), true)
, Tuple.Create(Tuple.Create(" ", 5845), Tuple.Create("glyphicon-fast-forward", 5846), true)
, Tuple.Create(Tuple.Create(" ", 5868), Tuple.Create("colorIcon", 5869), true)
, Tuple.Create(Tuple.Create(" ", 5878), Tuple.Create("green", 5879), true)
            
            #line 152 "..\..\DiffLog\Views\OperationLog.cshtml"
, Tuple.Create(Tuple.Create(" ", 5884), Tuple.Create<System.Object, System.Int32>(eq ? "mini" : ""
            
            #line default
            #line hidden
, 5885), false)
);

WriteLiteralTo(__razor_template_writer, "></span>\r\n    ");

})
            
            #line 153 "..\..\DiffLog\Views\OperationLog.cshtml"
           , 
    
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "<pre>");

            
            #line 154 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, TranslationClient.Diff(diff.Value.FinalState, dump));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "</pre>");

})
            
            #line 154 "..\..\DiffLog\Views\OperationLog.cshtml"
                                                                    ) { ToolTip = DiffLogMessage.DifferenceBetweenFinalStateAndTheCurrentStateOfTheEntity.NiceToString() });

                    tabs.Tab(new Signum.Web.DiffLog.LinkTab( 
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, " \r\n<span>");

            
            #line 157 "..\..\DiffLog\Views\OperationLog.cshtml"
WriteTo(__razor_template_writer, DiffLogMessage.CurrentEntity.NiceToString());

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n    <span");

WriteLiteralTo(__razor_template_writer, " class=\"glyphicon glyphicon-new-window\"");

WriteLiteralTo(__razor_template_writer, "/></span> \r\n");

WriteLiteralTo(__razor_template_writer, " ");

})
            
            #line 159 "..\..\DiffLog\Views\OperationLog.cshtml"
        , Navigator.NavigateRoute(e.Value.Target)) { ToolTip = DiffLogMessage.NavigatesToTheCurrentEntity.NiceToString() });
                }
            }
        }
    }
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
