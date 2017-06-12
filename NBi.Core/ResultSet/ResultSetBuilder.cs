using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NBi.Core.Query;

namespace NBi.Core.ResultSet
{
    public class ResultSetBuilder : IResultSetBuilder
    {
        private readonly CsvProfile profile;

        private readonly ExcelDefinition excelProfile;
        //private readonly ExcelProfile excelProfile;
        public ResultSetBuilder()
            : this(CsvProfile.SemiColumnDoubleQuote)
        {
        }

        public ResultSetBuilder(CsvProfile profile)
        {
            this.profile = profile;
        }
        public ResultSetBuilder(ExcelDefinition profile)
        {
            this.excelProfile = profile;
        }
        public virtual ResultSet Build(Object obj)
        {
            //Console.WriteLine("Debug: {0} {1}", obj.GetType(), obj.ToString()); 
            
            if (obj is ResultSet)
                return Build((ResultSet)obj);
            else if (obj is IList<IRow>)
                return Build((IList<IRow>)obj);
            else if (obj is IDbCommand)
                return Build((IDbCommand)obj);
            else if (obj is string)
                return Build((string)obj);
            else if (obj is object[])
                return Build((object[])obj);

            throw new ArgumentOutOfRangeException(string.Format("Type '{0}' is not expected when building a ResultSet", obj.GetType()));
        }
        
        public virtual ResultSet Build(ResultSet resultSet)
        {
            return resultSet;
        }

        public virtual ResultSet Build(IList<IRow> rows)
        {
            var rs = new ResultSet();
            rs.Load(rows);
            return rs;
        }
        
        public virtual ResultSet Build(IDbCommand cmd)
        {
            var qe = new QueryEngineFactory().GetExecutor(cmd);
            var ds = qe.Execute();
            var rs = new ResultSet();
            rs.Load(ds);
            return rs;
        }

        public virtual ResultSet Build(string path)
        {
            var rs = new ResultSet();

            if (!System.IO.File.Exists(path))
                throw new ExternalDependencyNotFoundException(path);
            if (path.EndsWith(".csv"))
            {
                CsvReader reader = new CsvReader(profile);
                DataTable dataTable = reader.Read(path, false);
                rs.Load(dataTable);
            }
            if (path.EndsWith(".xlsx"))
            {
                ExcelReader reader = new ExcelReader(excelProfile);
                DataTable dataTable = reader.Read(path, false);
                rs.Load(dataTable);
            }

            return rs;
        }

        public virtual ResultSet Build(object[] objects)
        {
            var rows = new List<IRow>();
            foreach (var obj in objects)
            {
                var items = obj as List<object>;
                var row = new Row();
                foreach (var item in items)
                {
                    var cell = new Cell();
                    cell.Value = item.ToString();
                    row.Cells.Add(cell);
                }
                rows.Add(row);
            }
            return Build(rows);
        }

        private class Row : IRow
        {
            private readonly IList<ICell> cells = new List<ICell>();

            public IList<ICell> Cells
            {
                get { return cells; }
            }
        }

        private class Cell : ICell
        {
            private string cellValue;
            public string Value
            {
                get { return cellValue; }
                set { cellValue = value; }
            }
        }

    }
}
