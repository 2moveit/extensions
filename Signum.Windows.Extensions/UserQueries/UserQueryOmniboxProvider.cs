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
using System.Windows;

namespace Signum.Windows.UserQueries
{
    public class UserQueryOmniboxProvider: OmniboxProvider<UserQueryOmniboxResult>
    {
        public override OmniboxResultGenerator<UserQueryOmniboxResult> CreateGenerator()
        {
            return new UserQueryOmniboxResultGenerator();
        }

        public override void RenderLines(UserQueryOmniboxResult result, InlineCollection lines)
        {
            lines.AddMatch(result.ToStrMatch);
            
            lines.Add(new Run(" ({0})".Formato(typeof(UserQueryDN).NiceName())) { Foreground = Brushes.DodgerBlue });
        }

        public override void OnSelected(UserQueryOmniboxResult result, Window window)
        {
            UserQueryDN uq = result.UserQuery.RetrieveAndForget();

            var query = QueryClient.queryNames[uq.Query.Key];

            using (UserQueryMenuItem.AutoSet(uq))
            {
                Navigator.Explore(new ExploreOptions(query));
            }
        }

        public override string GetItemStatus(UserQueryOmniboxResult result)
        {
            return "UQ:" + result.UserQuery.Key();
        }
    }
}
