using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NBi.Core.ResultSet;
using NBi.Core.ResultSet.Comparer;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace NBi.Core
{
    public class ExcelReader
    {
        public event ProgressStatusHandler ProgressStatusChanged;
        public ExcelReader(ExcelDefinition excelProfile)
        {
            ExcelDefinition = new ExcelDefinition();
            ExcelDefinition = excelProfile;
        }

        public ExcelDefinition ExcelDefinition { get; set; }

        public void RaiseProgressStatus(string status)
        {
            if (ProgressStatusChanged != null)
                ProgressStatusChanged(this, new ProgressStatusEventArgs(status));
        }

        public void RaiseProgressStatus(string status, int current, int total)
        {
            if (ProgressStatusChanged != null)
                ProgressStatusChanged(this, new ProgressStatusEventArgs(string.Format(status, current, total), current, total));
        }
        public DataTable Read(string filename, bool firstLineIsColumnName)
        {
            List<string> keyColumns = new List<string>();
            Dictionary<string, object> tolerances = new Dictionary<string, object>();
            Dictionary<string, Rounding> rounding = new Dictionary<string, Rounding>();

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(filename))
                {
                    pck.Load(stream);
                }

                var ws = pck.Workbook.Worksheets[ExcelDefinition.SheetName];
                DataTable rawTbl = new DataTable();
                DataTable finalTbl = new DataTable();

                var wsGlobalTolerance = ws.Cells[3, 2, 3, 2].Value;

                finalTbl.ExtendedProperties.Add("NBi::ResultSetType", "Excel");
                finalTbl.ExtendedProperties.Add("NBi::Tolerance", wsGlobalTolerance);

                var wsToleranceRange = ws.Cells[7, 2, 7, ws.Dimension.End.Column];
                //Key Column Indicators
                foreach (var cell in wsToleranceRange)
                {
                    var tolerance = ws.Cells[cell.Start.Row, cell.Start.Column, cell.Start.Row, cell.Start.Column].Value;
                    var name = ws.Cells[cell.Start.Row + 2, cell.Start.Column, cell.Start.Row + 2, cell.Start.Column].Value;
                    if (tolerance != null)
                    {
                        var tol = tolerance.ToString();

                        NumericToleranceFactory nt = new NumericToleranceFactory();
                        object toler = nt.Instantiate(tol);

                        tolerances.Add(name.ToString(), toler);
                    }
                }
                var wsRounding = ws.Cells[5, 2, 5, ws.Dimension.End.Column];
                //Rounding Style
                foreach (var cell in wsRounding)
                {
                    var roundingStyle = ws.Cells[cell.Start.Row, cell.Start.Column, cell.Start.Row, cell.Start.Column].Value;
                    var roundingStep = ws.Cells[cell.Start.Row + 1, cell.Start.Column, cell.Start.Row + 1, cell.Start.Column].Value;
                    var name = ws.Cells[cell.Start.Row + 4, cell.Start.Column, cell.Start.Row + 4, cell.Start.Column].Value;

                    Rounding.RoundingStyle rs = Rounding.RoundingStyle.None;
                    string rsp = string.Empty;

                    if (roundingStyle != null)
                    {
                        rs = (Rounding.RoundingStyle)Enum.Parse(typeof(Rounding.RoundingStyle), roundingStyle.ToString());
                    }

                    if (roundingStep != null)
                    {
                        rsp = roundingStep.ToString();
                    }

                    if(roundingStep!=null || roundingStyle !=null)
                        rounding.Add(name.ToString(), new Rounding(rsp, rs));
                }
                var wsKeyRange  = ws.Cells[8, 2, 8, ws.Dimension.End.Column];
                //Key Column Indicators
                foreach (var cell in wsKeyRange)
                {
                    var key = ws.Cells[cell.Start.Row, cell.Start.Column, cell.Start.Row, cell.Start.Column].Value;
                    var name = ws.Cells[cell.Start.Row + 1, cell.Start.Column, cell.Start.Row + 1, cell.Start.Column].Value.ToString();
                    if (key != null)
                    {
                        if (key.Equals("*"))
                            keyColumns.Add(name);
                    }
                }

                foreach (var firstRowCell in ws.Cells[9, 2, 9, ws.Dimension.End.Column])
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = firstRowCell.Text;
                    if (keyColumns.Contains(col.ColumnName))
                        col.ExtendedProperties.Add("NBi::Role", ColumnRole.Key);

                    object tc;
                    if(tolerances.TryGetValue(col.ColumnName, out tc))
                        col.ExtendedProperties.Add("NBi::Tolerance", tc);

                    Rounding rs;
                    if (rounding.TryGetValue(col.ColumnName, out rs))
                        col.ExtendedProperties.Add("NBi::Rounding", rs);

                    rawTbl.Columns.Add(col);
                }

                var startRow = 10;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 2, rowNum, ws.Dimension.End.Column];
                    DataRow row = rawTbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        var style = cell.Style;
                        decimal numeric;
                        bool formatNumeric = decimal.TryParse(style.Numberformat.Format, out numeric);

                        if (!row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.ContainsKey("NBi::Type"))
                        {
                            var nBIcolType = ColumnType.Text;
                            var colType = Type.GetType("System.String");

                            if (style.Numberformat.Format.Contains("mm-dd-yy"))
                            {
                                nBIcolType = ColumnType.DateTime;
                                colType = Type.GetType("System.DateTime");
                            }
                            if (formatNumeric)
                            {
                                nBIcolType = ColumnType.Numeric;
                                colType = Type.GetType("System.Decimal");
                            }
                            finalTbl.Columns.Add(row.Table.Columns[cell.Start.Column - 2].ColumnName);
                            finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Type", nBIcolType);

                            if (row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.ContainsKey("NBi::Role"))
                                finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Role", row.Table.Columns[cell.Start.Column - 2].ExtendedProperties["NBi::Role"]);

                            if (row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.ContainsKey("NBi::Tolerance"))
                                finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Tolerance", row.Table.Columns[cell.Start.Column - 2].ExtendedProperties["NBi::Tolerance"]);

                            if (row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.ContainsKey("NBi::Rounding"))
                                finalTbl.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Rounding", row.Table.Columns[cell.Start.Column - 2].ExtendedProperties["NBi::Rounding"]);

                            finalTbl.Columns[cell.Start.Column - 2].DataType = colType;

                            row.Table.Columns[cell.Start.Column - 2].ExtendedProperties.Add("NBi::Type", nBIcolType);
                        }
                        row[cell.Start.Column - 2] = cell.Text;
                    }
                }
                foreach (DataRow dr in rawTbl.Rows)
                {
                    finalTbl.ImportRow(dr);
                }
                return finalTbl;
            }
        }
    }
}
