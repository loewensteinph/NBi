using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Model;

namespace NBi.Core.Report
{
    public class CaptionFilter : ICaptionFilter
    {
        private readonly string caption;
        private readonly Target target;

        public Target Target { get { return target; } }
        public Enum UntypedTarget { get { return target; } }
        public string Caption { get { return caption; } }

        public CaptionFilter(Target target, string caption)
        {
            this.target = target;
            this.caption = caption;
        }
    }
}
