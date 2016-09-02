using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Report.Result
{
    public class ReportParameter
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string DataType { get; set; }
        public bool IsVisible { get; set; }
    }
}
