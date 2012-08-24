﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Omnibox;
using Signum.Entities.DynamicQuery;
using System.Windows;
using Signum.Utilities;
using Signum.Entities;
using System.Windows.Documents;
using System.Windows.Media;
using Signum.Entities.Basics;

namespace Signum.Windows.Omnibox
{
    public class DynamicQueryOmniboxProvider : OmniboxProvider<DynamicQueryOmniboxResult>
    {
        public override OmniboxResultGenerator<DynamicQueryOmniboxResult> CreateGenerator()
        {
            return new DynamicQueryOmniboxResultGenerator(QueryClient.queryNames.Values);
        }

        public override void OnSelected(DynamicQueryOmniboxResult r, Window window)
        {
            Navigator.Explore(new ExploreOptions(r.QueryNameMatch.Value)
            {
                FilterOptions = r.Filters.Select(f =>
                {
                    FilterType ft = QueryUtils.GetFilterType(f.QueryToken.Type);

                    var operation = f.Operation;
                    if (operation != null && !QueryUtils.GetFilterOperations(ft).Contains(f.Operation.Value))
                    {
                        MessageBox.Show(window, "Operation {0} not compatible with {1}".Formato(operation, f.QueryToken.ToString()));
                        operation = FilterOperation.EqualTo;
                    }

                    object value = f.Value;
                    if (value == DynamicQueryOmniboxResultGenerator.UnknownValue)
                    {
                        MessageBox.Show(window, "Unknown value for {0}".Formato(f.QueryToken.ToString()));
                        value = null;
                    }
                    else
                    {
                        if (value is Lite)
                            Server.FillToStr((Lite)value);
                    }

                    return new FilterOption
                    {
                        Token = f.QueryToken,
                        Operation = operation ?? FilterOperation.EqualTo,
                        Value = value,
                    };
                }).ToList(),
                SearchOnLoad = true,
            });
        }

        public override void RenderLines(DynamicQueryOmniboxResult result, InlineCollection lines)
        {
            lines.AddMatch(result.QueryNameMatch);


            foreach (var item in result.Filters)
            {
                lines.Add(" ");

                QueryToken last = null;
                if (item.QueryTokenMatches != null)
                {
                    foreach (var tokenMatch in item.QueryTokenMatches)
                    {
                        if (last != null)
                            lines.Add(".");

                        lines.AddMatch(tokenMatch);

                        last = (QueryToken)tokenMatch.Value;
                    }
                }

                if (item.QueryToken != last)
                {
                    if (last != null)
                        lines.Add(".");

                    lines.Add(new Run(item.QueryToken.Key) { Foreground = Brushes.Gray });
                }

                if (item.CanFilter.HasText())
                {
                    lines.Add(new Run(item.CanFilter) { Foreground = Brushes.Red });
                }
                else if (item.Operation != null)
                {
                    lines.Add(new Bold(new Run(DynamicQueryOmniboxResultGenerator.ToStringOperation(item.Operation.Value))));

                    if (item.Value == DynamicQueryOmniboxResultGenerator.UnknownValue)
                        lines.Add(new Run(Signum.Windows.Extensions.Properties.Resources.Unknown) { Foreground = Brushes.Red });
                    else if (item.ValuePack != null)
                        lines.AddMatch(item.ValuePack);
                    else if (item.Syntax != null && item.Syntax.Completion == FilterSyntaxCompletion.Complete)
                        lines.Add(new Bold(new Run(DynamicQueryOmniboxResultGenerator.ToStringValue(item.Value))));
                    else
                        lines.Add(new Run(DynamicQueryOmniboxResultGenerator.ToStringValue(item.Value)) { Foreground = Brushes.Gray });
                }
            }

            lines.Add(new Run(" ({0})".Formato(typeof(QueryDN).NiceName())) { Foreground = Brushes.Orange });
        }
    }
}
