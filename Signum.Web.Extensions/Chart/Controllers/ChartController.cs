﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Signum.Web.Extensions.Properties;
using Signum.Entities.Chart;
using Signum.Engine.DynamicQuery;
using Signum.Utilities;
using Signum.Entities.Reflection;
using Signum.Entities;
using Signum.Entities.DynamicQuery;
using Signum.Entities.UserQueries;
using System.Collections.Specialized;
using Signum.Engine;
using Signum.Entities.Authorization;
using Signum.Engine.Basics;
using System.Web.Script.Serialization;
using Signum.Engine.Reports;
using Signum.Web.Controllers;
using Signum.Engine.Chart;
using Signum.Entities.Basics;

namespace Signum.Web.Chart
{
    public class ChartController : Controller
    {
        #region chart
        public ActionResult Index(FindOptions findOptions)
        {
            if (!Navigator.IsFindable(findOptions.QueryName))
                throw new UnauthorizedAccessException(Resources.Chart_Query0IsNotAllowed.Formato(findOptions.QueryName));

            Navigator.SetTokens(findOptions.QueryName, findOptions.FilterOptions);

            var request = new ChartRequest(findOptions.QueryName)
            {
                ChartScript = ChartScriptLogic.Scripts.Value.FirstEx(() => "No ChartScript loaded in the database"),
                Filters = findOptions.FilterOptions.Select(fo => fo.ToFilter()).ToList()
            };

            return OpenChartRequest(request,
                findOptions.FilterOptions,
                findOptions.View && IsViewableEntity(request.QueryName));
        }

        public ViewResult FullScreen(string prefix)
        {
            var request = this.ExtractChartRequestCtx(prefix, null).Value;

            return OpenChartRequest(request,
                request.Filters.Select(f => new FilterOption { Token = f.Token, Operation = f.Operation, Value = f.Value }).ToList(),
                IsViewableEntity(request.QueryName));
        }

        public bool IsViewableEntity(object queryName)
        {
            var queryDescription = DynamicQueryManager.Current.QueryDescription(queryName);

            var entityColumn = queryDescription.Columns.SingleEx(a => a.IsEntity);

            Type entitiesType = Lite.Extract(entityColumn.Type);
            Implementations implementations = entityColumn.Implementations.Value;

            return implementations.IsByAll || implementations.Types.Any(t => Navigator.IsViewable(t, EntitySettingsContext.Admin));
        }

        [HttpPost]
        public PartialViewResult UpdateChartBuilder(string prefix)
        {
            string lastToken = Request["lastTokenChanged"];
            
            var request = this.ExtractChartRequestCtx(prefix, lastToken.TryCS(int.Parse)).Value;   

            ViewData[ViewDataKeys.QueryDescription] = DynamicQueryManager.Current.QueryDescription(request.QueryName);
            
            return PartialView(ChartClient.ChartBuilderView, new TypeContext<ChartRequest>(request, prefix));
        }

        [HttpPost]
        public ContentResult NewSubTokensCombo(string webQueryName, string tokenName, string prefix, int index)
        {
            ChartRequest request = this.ExtractChartRequestCtx("", null).Value;

            QueryDescription qd = DynamicQueryManager.Current.QueryDescription(request.QueryName);

            QueryToken token = QueryUtils.Parse(tokenName, qt => qt.SubTokensChart(qd.Columns, request.GroupResults));

            List<QueryToken> subtokens = token.SubTokensChart(qd.Columns, request.GroupResults);

            if (subtokens.IsEmpty())
                return Content("");

            var tokenOptions = SearchControlHelper.TokensCombo(subtokens, null);

            return Content(
                SearchControlHelper.TokenOptionsCombo(
                    SignumController.CreateHtmlHelper(this), request.QueryName, tokenOptions, new Context(null, prefix), index + 1, true).ToHtmlString());
        }

        [HttpPost]
        public ContentResult AddFilter(string webQueryName, string tokenName, int index, string prefix)
        {
            ChartRequest request = this.ExtractChartRequestCtx(prefix, null).Value;

            QueryDescription qd = DynamicQueryManager.Current.QueryDescription(request.QueryName);

            object queryName = Navigator.ResolveQueryName(webQueryName);

            FilterOption fo = new FilterOption(tokenName, null);
            if (fo.Token == null)
            {
                fo.Token = QueryUtils.Parse(tokenName, qt => qt.SubTokensChart(qd.Columns, request.GroupResults));
            }
            fo.Operation = QueryUtils.GetFilterOperations(QueryUtils.GetFilterType(fo.Token.Type)).FirstEx();

            return Content(
                SearchControlHelper.NewFilter(
                    SignumController.CreateHtmlHelper(this), queryName, fo, new Context(null, prefix), index).ToHtmlString());
        }

        [HttpPost]
        public ActionResult Draw(string prefix)
        {
            var requestCtx = this.ExtractChartRequestCtx(prefix, null).ValidateGlobal();

            if (requestCtx.GlobalErrors.Any())
            {
                ModelState.FromContext(requestCtx);
                return JsonAction.ModelState(ModelState);
            }

            var request = requestCtx.Value;

            var resultTable = ChartLogic.ExecuteChart(request);

            var querySettings = Navigator.QuerySettings(request.QueryName);

            ViewData[ViewDataKeys.Results] = resultTable;

            ViewData[ViewDataKeys.View] = IsViewableEntity(request.QueryName);
            ViewData[ViewDataKeys.Formatters] = resultTable.Columns.Select((c, i) => new { c, i }).ToDictionary(c => c.i, c => querySettings.GetFormatter(c.c.Column));

            return PartialView(ChartClient.ChartResultsView, new TypeContext<ChartRequest>(request, prefix));
        }

        public ActionResult OpenSubgroup(string prefix)
        {
            var chartRequest = this.ExtractChartRequestCtx(prefix, null).Value;

            if (chartRequest.GroupResults)
            {
                var filters = chartRequest.Filters
                    .Where(a => !(a.Token is AggregateToken))
                    .Select(f => new FilterOption
                    {
                        Token = f.Token,
                        ColumnName = f.Token.FullKey(),
                        Value = f.Value,
                        Operation = f.Operation
                    }).ToList();

                foreach (var column in chartRequest.Columns.Iterate())
                {
                    if (column.Value.ScriptColumn.IsGroupKey)
                        filters.AddRange(GetFilter(column.Value, "c" + column.Position));
                }

                var findOptions = new FindOptions(chartRequest.QueryName)
                {
                    FilterOptions = filters,
                    SearchOnLoad = true
                };

                return Redirect(findOptions.ToString());
            }
            else
            {
                string entity = Request.Params["entity"];
                if (string.IsNullOrEmpty(entity))
                    throw new Exception("If the chart is not grouping, entity must be provided");
                 
                var queryDescription = DynamicQueryManager.Current.QueryDescription(chartRequest.QueryName);
                var querySettings = Navigator.QuerySettings(chartRequest.QueryName);

                var entityColumn = queryDescription.Columns.SingleEx(a => a.IsEntity);
                Type entitiesType = Lite.Extract(entityColumn.Type);

                Lite lite = Lite.Parse(entitiesType, entity);
                return Redirect(Navigator.ViewRoute(lite));
            }
        }

        private FilterOption GetFilter(ChartColumnDN chartToken, string key)
        {
            if (chartToken == null ||  chartToken.Token is AggregateToken)
                return null;


            var token = chartToken.Token;

            bool hasKey = Request.Params.AllKeys.Contains(key);
            var value = !hasKey ? null : 
                FindOptionsModelBinder.Convert(FindOptionsModelBinder.DecodeValue(Request.Params[key]), token.Type);
            
            return new FilterOption
            {
                ColumnName = token.FullKey(),
                Token = token,
                Operation = FilterOperation.EqualTo,
                Value = value,
            };
        }

        [HttpPost]
        public JsonResult Validate(string prefix)
        {
            var requestCtx = this.ExtractChartRequestCtx(prefix, null).ValidateGlobal();

            ModelState.FromContext(requestCtx);
            return JsonAction.ModelState(ModelState);
        }

        [HttpPost]
        public ActionResult ExportData(string prefix)
        {
            var request = ExtractChartRequestCtx(prefix, null).Value;

            if (!Navigator.IsFindable(request.QueryName))
                throw new UnauthorizedAccessException(Resources.Chart_Query0IsNotAllowed.Formato(request.QueryName));

            var resultTable = ChartLogic.ExecuteChart(request);

            byte[] binaryFile = PlainExcelGenerator.WritePlainExcel(resultTable);

            return File(binaryFile, MimeType.FromExtension(".xlsx"), Navigator.ResolveWebQueryName(request.QueryName) + ".xlsx");
        }
        #endregion

        public MappingContext<ChartRequest> ExtractChartRequestCtx(string prefix, int? lastTokenChanged)
        {
            var ctx = new ChartRequest(Navigator.ResolveQueryName(Request.Params["webQueryName"]))
                    .ApplyChanges(ControllerContext, prefix, ChartClient.MappingChartRequest, Request.Params.ToSortedList(prefix));

            ChartRequest chart = ctx.Value;
            if (lastTokenChanged != null)
                chart.Columns[lastTokenChanged.Value].TokenChanged();

            return ctx;
        }

        ViewResult OpenChartRequest(ChartRequest request, List<FilterOption> filterOptions, bool view)
        {
            var queryDescription = DynamicQueryManager.Current.QueryDescription(request.QueryName);

            ViewData[ViewDataKeys.PartialViewName] = ChartClient.ChartControlView;
            ViewData[ViewDataKeys.Title] = Navigator.Manager.SearchTitle(request.QueryName);
            ViewData[ViewDataKeys.QueryDescription] = queryDescription;
            ViewData[ViewDataKeys.FilterOptions] = filterOptions;
            ViewData[ViewDataKeys.View] = view;

            return View(Navigator.Manager.SearchPageView, new TypeContext<ChartRequest>(request, ""));
        }

        #region user chart
        public ActionResult CreateUserChart(string prefix)
        {
            var request = ExtractChartRequestCtx(prefix, null).Value;

            if (!Navigator.IsFindable(request.QueryName))
                throw new UnauthorizedAccessException(Resources.Chart_Query0IsNotAllowed.Formato(request.QueryName));

            var userChart = request.ToUserChart();

            userChart.Related = UserDN.Current.ToLite<IdentifiableEntity>();

            ViewData[ViewDataKeys.QueryDescription] = DynamicQueryManager.Current.QueryDescription(request.QueryName);

            return Navigator.View(this, userChart);
        }

        public ActionResult ViewUserChart(Lite<UserChartDN> lite)
        {
            UserChartDN uc = Database.Retrieve<UserChartDN>(lite);

            ChartRequest request = uc.ToRequest();

            return OpenChartRequest(request,
                request.Filters.Select(f => new FilterOption { Token = f.Token, Operation = f.Operation, Value = f.Value }).ToList(),
                IsViewableEntity(request.QueryName));
        }

        public ActionResult DeleteUserChart(Lite<UserChartDN> lite)
        {
            var queryName = QueryLogic.ToQueryName(lite.InDB().Select(uq => uq.Query.Key).FirstEx());

            Database.Delete<UserChartDN>(lite);

            return Redirect(Navigator.FindRoute(queryName));
        }
        #endregion
    }
}
