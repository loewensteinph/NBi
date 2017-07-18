using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using NBi.Core.ResultSet;
using NBi.Core.ResultSet.Comparer;
using OfficeOpenXml;

namespace NBi.Core
{
    public class ExcelReader
    {
        public ExcelReader(ExcelDefinition excelProfile)
        {
            ExcelDefinition = new ExcelDefinition();
            ExcelDefinition = excelProfile;

            _patterns = new HashSet<string>();
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                _patterns.UnionWith(culture.DateTimeFormat.GetAllDateTimePatterns());
            }
        }
        public ExcelDefinition ExcelDefinition { get; set; }
        public event ProgressStatusHandler ProgressStatusChanged;
        public void RaiseProgressStatus(string status)
        {
            if (ProgressStatusChanged != null)
                ProgressStatusChanged(this, new ProgressStatusEventArgs(status));
        }
        public void RaiseProgressStatus(string status, int current, int total)
        {
            if (ProgressStatusChanged != null)
                ProgressStatusChanged(this,
                    new ProgressStatusEventArgs(string.Format(status, current, total), current, total));
        }
        public DataTable Read(string filename, bool firstLineIsColumnName)
        {
            var keyColumns = new List<string>();
            var tolerances = new Dictionary<string, object>();
            var rounding = new Dictionary<string, Rounding>();
            var datatypes = new Dictionary<string, object>();

            using (var pck = new ExcelPackage())
            {
                using (var stream = File.OpenRead(filename))
                {
                    pck.Load(stream);
                }

                var ws = pck.Workbook.Worksheets[ExcelDefinition.SheetName];

                if(ws == null)
                    ws = pck.Workbook.Worksheets[0];

                if (ws == null)
                    throw new NullReferenceException();
                
                var rawTbl = new DataTable();
                var finalTbl = new DataTable();

                var wsIsIdentityInsert = ws.Cells["D1"].Value;

                finalTbl.ExtendedProperties.Add("NBi::BulkIdentityInsert", Boolean.Parse(wsIsIdentityInsert.ToString()));
                finalTbl.ExtendedProperties.Add("NBi::ResultSetType", "Excel");

                //Tolerances
                ExcelRange wsToleranceRange = ws.Cells[6, 2, 6, ws.Dimension.End.Column];
                GetTolerances(wsToleranceRange, ws, tolerances);

                //Rounding
                ExcelRange wsRounding = ws.Cells[4, 2, 4, ws.Dimension.End.Column];
                GetRounding(wsRounding, ws, rounding);

                //Key Column Indicators
                ExcelRange wsKeyRange = ws.Cells[7, 2, 7, ws.Dimension.End.Column];
                GetKeyColumns(wsKeyRange, ws, keyColumns);

                var startRow = 8;

                foreach (var firstRowCell in ws.Cells[startRow, 2, startRow, ws.Dimension.End.Column])
                {
                    var col = new DataColumn();
                    col.ColumnName = firstRowCell.Text;
                    if (keyColumns.Contains(col.ColumnName))
                        col.ExtendedProperties.Add("NBi::Role", ColumnRole.Key);

                    object tc;
                    if (tolerances.TryGetValue(col.ColumnName, out tc))
                        col.ExtendedProperties.Add("NBi::Tolerance", tc);

                    Rounding rs;
                    if (rounding.TryGetValue(col.ColumnName, out rs))
                        col.ExtendedProperties.Add("NBi::Rounding", rs);

                    rawTbl.Columns.Add(col);
                }

                startRow = 9;

                // Check Datatypes
                for (var colNum = 2; colNum <= rawTbl.Columns.Count + 1; colNum++)
                {
                    string colName = rawTbl.Columns[colNum-2].ColumnName;
                    ExcelRange wsCol = ws.Cells[startRow, colNum, ws.Dimension.End.Row, colNum];
                    object currentDataType = typeof(String);
                    bool first = true;
                    foreach (var wsCell in wsCol)
                    {
                        if (!wsCell.Text.Equals(string.Empty))
                        {
                            if (currentDataType != ParseString(wsCell.Text) && !first)
                            {
                                if (currentDataType.Equals(typeof(Decimal)) &&
                                    ParseString(wsCell.Text).Equals(typeof(Int64)))
                                    break;
                                currentDataType = typeof(String);
                                break;
                            }
                            currentDataType = ParseString(wsCell.Text);
                        }
                        first = false;
                    }
                    datatypes.Add(colName, currentDataType);
                }

                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 2, rowNum, rawTbl.Columns.Count + 1];
                    bool hascontent = false;
                    foreach (var cell in wsRow)
                    {
                        if (!cell.Text.Equals(string.Empty))
                            hascontent = true;
                    }
                    if (hascontent)
                    {
                        var row = rawTbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            if (!row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.ContainsKey("NBi::Type"))
                            {
                                var nBIcolType = ColumnType.Text;
                                object colType;

                                datatypes.TryGetValue(row.Table.Columns[cell.Start.Column - 2].ColumnName,
                                    out colType);

                                if(colType.Equals(typeof(Decimal)))
                                    nBIcolType = ColumnType.Numeric;
                                if (colType.Equals(typeof(Int64)))
                                    nBIcolType = ColumnType.Numeric;
                                if (colType.Equals(typeof(DateTime)))
                                    nBIcolType = ColumnType.DateTime;
                                if (colType.Equals(typeof(Boolean)))
                                    nBIcolType = ColumnType.Boolean;

                                finalTbl.Columns.Add(row.Table.Columns[cell.Start.Column - 2].ColumnName);
                                finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Type", nBIcolType);

                                if (row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.ContainsKey("NBi::Role"))
                                    finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Role",
                                        row.Table.Columns[cell.Start.Column - 2].ExtendedProperties["NBi::Role"]);

                                if (row.Table.Columns[cell.Start.Column - 2].ExtendedProperties
                                    .ContainsKey("NBi::Tolerance"))
                                    finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Tolerance",
                                        row.Table.Columns[cell.Start.Column - 2].ExtendedProperties["NBi::Tolerance"]);

                                if (row.Table.Columns[cell.Start.Column - 2].ExtendedProperties
                                    .ContainsKey("NBi::Rounding"))
                                    finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Rounding",
                                        row.Table.Columns[cell.Start.Column - 2].ExtendedProperties["NBi::Rounding"]);

                                finalTbl.Columns[cell.Start.Column - 2].DataType = colType as Type;

                                row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Type", nBIcolType);
                            }

                            if (cell.Text.Equals("NULL"))
                            {
                                row[cell.Start.Column - 2] = DBNull.Value;
                            }
                            else
                            {
                                row[cell.Start.Column - 2] = cell.Text;
                            }
                        }
                    }
                }
                foreach (DataRow dr in rawTbl.Rows)
                    finalTbl.ImportRow(dr);
                return finalTbl;
            }
        }
        private static void GetKeyColumns(ExcelRange wsKeyRange, ExcelWorksheet ws, List<string> keyColumns)
        {
            foreach (var cell in wsKeyRange)
            {
                var key = ws.Cells[cell.Start.Row, cell.Start.Column, cell.Start.Row, cell.Start.Column].Value;
                var name = ws.Cells[cell.Start.Row + 1, cell.Start.Column, cell.Start.Row + 1, cell.Start.Column]
                    .Value;
                if (key != null && name != null)
                    if (key.Equals("*"))
                        keyColumns.Add(name.ToString());
            }
        }
        private static void GetRounding(ExcelRange wsRounding, ExcelWorksheet ws, Dictionary<string, Rounding> rounding)
        {
            foreach (var cell in wsRounding)
            {
                var roundingStyle = ws.Cells[cell.Start.Row, cell.Start.Column, cell.Start.Row, cell.Start.Column]
                    .Value;
                var roundingStep = ws.Cells[cell.Start.Row + 1, cell.Start.Column, cell.Start.Row + 1,
                    cell.Start.Column].Value;
                var name = ws.Cells[cell.Start.Row + 4, cell.Start.Column, cell.Start.Row + 4, cell.Start.Column]
                    .Value;

                var rs = Rounding.RoundingStyle.None;
                var rsp = string.Empty;

                if (roundingStyle != null)
                    rs = (Rounding.RoundingStyle) Enum.Parse(typeof(Rounding.RoundingStyle),
                        roundingStyle.ToString());

                if (roundingStep != null)
                    rsp = roundingStep.ToString();

                if (roundingStep != null || roundingStyle != null)
                    rounding.Add(name.ToString(), new Rounding(rsp, rs));
            }
        }
        private static void GetTolerances(ExcelRange wsToleranceRange, ExcelWorksheet ws, Dictionary<string, object> tolerances)
        {
            foreach (var cell in wsToleranceRange)
            {
                var tolerance = ws.Cells[cell.Start.Row, cell.Start.Column, cell.Start.Row, cell.Start.Column]
                    .Value;
                var name = ws.Cells[cell.Start.Row + 2, cell.Start.Column, cell.Start.Row + 2, cell.Start.Column]
                    .Value;
                if (tolerance != null)
                {
                    var tol = tolerance.ToString();

                    var nt = new NumericToleranceFactory();
                    object toler = nt.Instantiate(tol);

                    tolerances.Add(name.ToString(), toler);
                }
            }
        }
        private static HashSet<string> _patterns;
        public object ParseString(string str)
        {
            Int64 intValue;
            Decimal decimalValue;
            Guid guidValue;
            bool boolValue;
            DateTime datetimeValue;
            // Place checks higher if if-else statement to give higher priority to type.
             if (Int64.TryParse(str, out intValue))
                return intValue.GetType();
            else if (DateTime.TryParseExact(str, _patterns.ToArray(), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out datetimeValue))
                return datetimeValue.GetType();
            else if (Decimal.TryParse(str, out decimalValue))
                return decimalValue.GetType();
             else if (DateTime.TryParse(str, out datetimeValue))
                 return datetimeValue.GetType();
            else if (Guid.TryParse(str, out guidValue))
                return guidValue.GetType();
            else if (Boolean.TryParse(str, out boolValue))
                return boolValue.GetType();
            return typeof(String);
        }
        public bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }
    }
}