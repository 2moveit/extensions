﻿#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using Signum.Entities;
using Signum.Entities.Reflection;
using Signum.Utilities;
using System.Configuration;
using Signum.Web.Properties;
using Signum.Engine;
using Signum.Entities.Files;
#endregion

namespace Signum.Web.Files
{
    public static class FileRepeaterHelper
    {
        private static MvcHtmlString InternalFileRepeater(this HtmlHelper helper, FileRepeater fileRepeater)
        {
            if (!fileRepeater.Visible || (fileRepeater.HideIfNull && fileRepeater.UntypedValue == null))
                return MvcHtmlString.Empty;

            HtmlStringBuilder sb = new HtmlStringBuilder();

            using (sb.Surround(new HtmlTag("fieldset").Class("sf-repeater-field sf-file-repeater-field")))
            {
                using (sb.Surround(new HtmlTag("legend")))
                {
                    sb.AddLine(EntityBaseHelper.BaseLineLabel(helper, fileRepeater));
                    sb.AddLine(ListBaseHelper.CreateButton(helper, fileRepeater, new Dictionary<string, object> { { "title", fileRepeater.AddElementLinkText } }));
                }

                sb.AddLine(helper.HiddenStaticInfo(fileRepeater));
                sb.AddLine(helper.Hidden(fileRepeater.Compose(EntityListBaseKeys.ListPresent), ""));

                sb.AddLine(new HtmlTag("script")
                    .Attr("type", "text/javascript")
                    .InnerHtml(MvcHtmlString.Create("$(function(){ SF.Loader.loadJs('" + RouteHelper.New().Content("~/Files/Scripts/SF_Files.js") + "'); });"))
                    .ToHtml());

                //Write FileLine template
                TypeElementContext<FilePathDN> templateTC = new TypeElementContext<FilePathDN>(null, (TypeContext)fileRepeater.Parent, 0);
                using (FileLine fl = new FileLine(typeof(FilePathDN), templateTC.Value, templateTC, "", templateTC.PropertyRoute) { Remove = false, AsyncUpload = fileRepeater.AsyncUpload, FileType = fileRepeater.FileType })
                    sb.AddLine(EntityBaseHelper.EmbeddedTemplate(fileRepeater, helper.InternalFileLine(fl)));
                
                using (sb.Surround(new HtmlTag("div").IdName(fileRepeater.Compose(EntityRepeaterKeys.ItemsContainer))))
                {
                    if (fileRepeater.UntypedValue != null)
                    {
                        foreach (var itemTC in TypeContextUtilities.TypeElementContext((TypeContext<MList<FilePathDN>>)fileRepeater.Parent))
                            sb.Add(InternalRepeaterElement(helper, itemTC, fileRepeater));
                    }
                }
            }

            return sb.ToHtml();
        }

        private static MvcHtmlString InternalRepeaterElement(this HtmlHelper helper, TypeElementContext<FilePathDN> itemTC, FileRepeater fileRepeater)
        {
            HtmlStringBuilder sb = new HtmlStringBuilder();
            using (sb.Surround(new HtmlTag("fieldset").IdName(itemTC.Compose(EntityRepeaterKeys.RepeaterElement)).Attr("class", "sf-repeater-element sf-file-repeater-element")))
            {
                using (sb.Surround(new HtmlTag("legend")))
                {
                    if (fileRepeater.Remove)
                        sb.AddLine(
                            helper.Href(itemTC.Compose("btnRemove"),
                                          fileRepeater.RemoveElementLinkText,
                                          "javascript:new SF.ERep({0}).remove('{1}');".Formato(fileRepeater.ToJS(), itemTC.ControlID),
                                          fileRepeater.RemoveElementLinkText,
                                          "sf-line-button sf-remove",
                                          new Dictionary<string, object> { { "data-icon", "ui-icon-circle-close" }, { "data-text", false } }));
                }

                sb.AddLine(helper.Hidden(itemTC.Compose(EntityListBaseKeys.Index), itemTC.Index.ToString()));

                //Render FileLine for the current item
                using (sb.Surround(new HtmlTag("div").IdName(itemTC.Compose(EntityBaseKeys.Entity))))
                {
                    TypeContext<FilePathDN> tc = (TypeContext<FilePathDN>)TypeContextUtilities.CleanTypeContext(itemTC);

                    using (FileLine fl = new FileLine(typeof(FilePathDN), tc.Value, itemTC, "", tc.PropertyRoute) { Remove = false })
                        sb.AddLine(helper.InternalFileLine(fl));
                }
            }

            return sb.ToHtml();
        }

        public static MvcHtmlString FileRepeater<T, S>(this HtmlHelper helper, TypeContext<T> tc, Expression<Func<T, MList<S>>> property)
            where S : FilePathDN
        {
            return helper.FileRepeater(tc, property, null);
        }

        public static MvcHtmlString FileRepeater<T, S>(this HtmlHelper helper, TypeContext<T> tc, Expression<Func<T, MList<S>>> property, Action<FileRepeater> settingsModifier)
            where S : FilePathDN
        {
            TypeContext<MList<S>> context = Common.WalkExpression(tc, property);

            FileRepeater fl = new FileRepeater(context.Type, context.UntypedValue, context, null, context.PropertyRoute);

            //Navigator.ConfigureEntityBase(el, Reflector.ExtractLite(typeof(S)) ?? typeof(S), false);
            Common.FireCommonTasks(fl);

            if (settingsModifier != null)
                settingsModifier(fl);

            return helper.InternalFileRepeater(fl);
        }
    }
}
