﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using Signum.Entities.Reports;

namespace Signum.Entities.ControlPanel
{
    [Serializable]
    public class PanelPart : EmbeddedEntity
    {
        string title;
        [StringLengthValidator(AllowNulls=false, Min=1)]
        public string Title
        {
            get { return title; }
            set { Set(ref title, value, () => Title); }
        }

        int row = 1;
        [NumberIsValidator(ComparisonType.GreaterThan, 0)]
        public int Row
        {
            get { return row; }
            set { Set(ref row, value, () => Row); }
        }

        int column = 1;
        [NumberIsValidator(ComparisonType.GreaterThan, 0)]
        public int Column
        {
            get { return column; }
            set { Set(ref column, value, () => Column); }
        }

        bool fill;
        public bool Fill
        {
            get { return fill; }
            set { Set(ref fill, value, () => Fill); }
        }

        [ImplementedBy(typeof(UserQueryDN), typeof(CountSearchControlPartDN), typeof(LinkListPartDN))]
        IIdentifiable content;
        public IIdentifiable Content
        {
            get { return content; }
            set { Set(ref content, value, () => Content); }
        }
    }

    [Serializable]
    public class CountSearchControlPartDN : Entity
    {
        MList<CountUserQueryElement> userQueries = new MList<CountUserQueryElement>();
        public MList<CountUserQueryElement> UserQueries
        {
            get { return userQueries; }
            set { Set(ref userQueries, value, () => UserQueries); }
        }

        public override string ToString()
        {
            return UserQueries.ToString(uq => uq.Label, ",");
        }
    }

    [Serializable]
    public class CountUserQueryElement : EmbeddedEntity
    {
        string label;
        public string Label
        {
            get { return label ?? UserQuery.TryCC(uq => uq.DisplayName); }
            set { Set(ref label, value, () => Label); }
        }

        UserQueryDN userQuery;
        [NotNullValidator]
        public UserQueryDN UserQuery
        {
            get { return userQuery; }
            set { Set(ref userQuery, value, () => UserQuery); }
        }
    }

    [Serializable]
    public class LinkListPartDN : Entity
    {
        MList<LinkElement> links = new MList<LinkElement>();
        public MList<LinkElement> Links
        {
            get { return links; }
            set { Set(ref links, value, () => Links); }
        }

        public override string ToString()
        {
            return Links.ToString(uq => uq.Label, ",");
        }
    }

    [Serializable]
    public class LinkElement : EmbeddedEntity
    {
        string label;
        [NotNullValidator]
        public string Label
        {
            get { return label; }
            set { Set(ref label, value, () => Label); }
        }

        string link;
        [StringLengthValidator(AllowNulls=false, Min=2)]
        public string Link
        {
            get { return link; }
            set { Set(ref link, value, () => Link); }
        }
    }
}