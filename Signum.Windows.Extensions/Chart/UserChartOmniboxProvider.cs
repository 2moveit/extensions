﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Windows.Omnibox;
using Signum.Entities.UserQueries;
using Signum.Entities.Omnibox;
using System.Windows.Documents;
using System.Windows.Media;
using Signum.Utilities;
using Signum.Windows.Authorization;
using Signum.Entities.Chart;
using Signum.Entities.DynamicQuery;
using System.Windows;

namespace Signum.Windows.Chart
{
    public class UserChartOmniboxProvider : OmniboxProvider<UserChartOmniboxResult>
    {
        public override OmniboxResultGenerator<UserChartOmniboxResult> CreateGenerator()
        {
            return new UserChartOmniboxResultGenerator();
        }

        public override void RenderLines(UserChartOmniboxResult result, InlineCollection lines)
        {
            lines.AddMatch(result.ToStrMatch);

            lines.Add(new Run(" ({0})".Formato(typeof(UserChartDN).NiceName())) { Foreground = Brushes.DarkViolet });
        }

        public override void OnSelected(UserChartOmniboxResult result, Window window)
        {
            UserChartDN uq = result.UserChart.RetrieveAndForget();

            var query = QueryClient.queryNames[uq.Query.Key];

            using (UserChartMenuItem.AutoSet(uq))
            {
                ChartRequestWindow cw = new ChartRequestWindow()
                {
                    DataContext = new ChartRequest(query)
                };

                cw.Show();
            }
        }

        public override string GetItemStatus(UserChartOmniboxResult result)
        {
            return "UC:" + result.UserChart.Key();
        }
    }
}
