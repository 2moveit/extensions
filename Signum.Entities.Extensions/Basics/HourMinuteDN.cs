﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using System.Linq.Expressions;
using System.ComponentModel;
using Signum.Entities.Extensions.Properties;

namespace Signum.Entities
{
    [Serializable]
    public class HourMinuteDN : EmbeddedEntity
    {
        int hour = 0;
        [NumberBetweenValidator(0, 23)]
        public int Hour
        {
            get { return hour; }
            set { SetToStr(ref hour, value, () => Hour); }
        }

        int minute = 0;
        [NumberBetweenValidator(0, 59)]
        public int Minute
        {
            get { return minute; }
            set { SetToStr(ref minute, value, () => Minute); }
        }

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(hour, minute, 0);
        }

        public override string ToString()
        {
            return ToTimeSpan().ToString();
        }
    }
}
