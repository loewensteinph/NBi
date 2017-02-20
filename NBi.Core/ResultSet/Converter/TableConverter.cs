using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.ResultSet.Converter
{
    class TableConverter : IConverter<DataTable>
    {
        public virtual DataTable Convert(object value)
        {
            if (value is DataTable && ((value as DataTable).Columns.Count == 0 || (value as DataTable).Rows.Count == 0))
                return null;
            
            if (value is DataTable)
                return (DataTable)value;

            if (value == null || value == DBNull.Value)
                return null;
            
            throw new ArgumentOutOfRangeException();
        }

        public virtual bool IsValid(object value)
        {
            if (value is DataTable || value == null)
                return true;

            return false;
        }
    }
}
