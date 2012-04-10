﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Reports;
using Signum.Engine.DynamicQuery;
using Signum.Entities;
using Signum.Engine.Maps;
using Signum.Engine.Linq;
using Signum.Entities.DynamicQuery;
using Signum.Engine.Extensions.Properties;
using Signum.Engine.Basics;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Signum.Utilities;
using System.IO;

namespace Signum.Engine.Reports
{
    public static class ReportsLogic
    {
        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm, bool excelReport)
        {
            if (excelReport)
            {
                QueryLogic.Start(sb);

                sb.Include<ExcelReportDN>();
                dqm[typeof(ExcelReportDN)] = (from s in Database.Query<ExcelReportDN>()
                                              select new
                                              {
                                                  Entity = s,
                                                  s.Id,
                                                  s.Query,
                                                  s.File.FileName,
                                                  s.DisplayName,
                                                  s.Deleted,
                                              }).ToDynamic();
            }
        }

        public static List<Lite<ExcelReportDN>> GetExcelReports(object queryName)
        {
            return (from er in Database.Query<ExcelReportDN>()
                    where er.Query.Key == QueryUtils.GetQueryUniqueKey(queryName) && !er.Deleted
                    select er.ToLite()).ToList();
        }

        public static byte[] ExecuteExcelReport(Lite<ExcelReportDN> excelReport, QueryRequest request)
        {
            ResultTable queryResult = DynamicQueryManager.Current.ExecuteQuery(request);
            
            ExcelReportDN report = excelReport.RetrieveAndForget();
            string extension = Path.GetExtension(report.File.FileName);
            if (extension != ".xlsx")
                throw new ApplicationException(Resources.ExcelTemplateMustHaveExtensionXLSXandCurrentOneHas0.Formato(extension));

            return ExcelGenerator.WriteDataInExcelFile(queryResult, report.File.BinaryFile);
        }

        public static byte[] ExecutePlainExcel(QueryRequest request)
        {
            ResultTable queryResult = DynamicQueryManager.Current.ExecuteQuery(request);

            return PlainExcelGenerator.WritePlainExcel(queryResult);
        }
    }
}
