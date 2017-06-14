﻿using System;
using System.Linq;

namespace NBi.Core.ResultSet.Comparer
{
    public class DateTimeTolerance : Tolerance
    {
        public TimeSpan TimeSpan {get;set;}

        private DateTimeTolerance()
            : base(0.ToString(), SideTolerance.Both)
        {
            this.TimeSpan = new TimeSpan(0);
        }

        public DateTimeTolerance(TimeSpan value)
            : base(value.ToString(), SideTolerance.Both)
        {
            if (value.Ticks < 0)
                throw new ArgumentException("The parameter 'step' must be a value greater than zero.", "step");

            this.TimeSpan = value;
        }

        public static DateTimeTolerance None
        {
            get
            {
                return new DateTimeTolerance();
            }
        }
    }
}
