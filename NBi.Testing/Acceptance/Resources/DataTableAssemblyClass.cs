using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NBi.Testing.Acceptance.Resources
{
    /// <summary>
    /// This class is only used for acceptance testing purpose
    /// </summary>
    class DataTableAssemblyClass
    {
        public DataTableAssemblyClass()
        {

        }

        public DataTable GeDataTable()
        {
            DataTable dt = new DataTable("Test");
            dt.Columns.Add(new DataColumn("col1", typeof(String)));
            dt.Rows.Add("1");
            return dt;
        }
    }
}
