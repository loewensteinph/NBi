using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NBi.Core.ResultSet;
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

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(filename))
                {
                    pck.Load(stream);
                }

                var ws = pck.Workbook.Worksheets[ExcelDefinition.SheetName];
                DataTable rawTbl = new DataTable();
                DataTable finalTbl = new DataTable();

                rawTbl.ExtendedProperties.Add("NBi::ResultSetType", "Excel");
                finalTbl.ExtendedProperties.Add("NBi::ResultSetType", "Excel");

                var wsKeyRange = ws.Cells[4, 1, 4, ws.Dimension.End.Column];
                //DataRow row = tbl.Rows.Add();
                foreach (var cell in wsKeyRange)
                {
                    var key = ws.Cells[cell.Start.Row, cell.Start.Column, cell.Start.Row, cell.Start.Column].Value;
                    var name = ws.Cells[cell.Start.Row + 1, cell.Start.Column, cell.Start.Row + 1, cell.Start.Column].Value.ToString();

                    if (key.Equals("*"))
                        keyColumns.Add(name);
                }

                foreach (var firstRowCell in ws.Cells[5, 1, 5, ws.Dimension.End.Column])
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = firstRowCell.Text;
                    if (keyColumns.Contains(col.ColumnName))
                        col.ExtendedProperties.Add("NBi::Role", ColumnRole.Key);

                    rawTbl.Columns.Add(col);
                }

                var startRow = 6;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = rawTbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        var style = cell.Style;
                        decimal numeric;
                        bool formatNumeric = decimal.TryParse(style.Numberformat.Format, out numeric);

                        if (!row.Table.Columns[cell.Start.Column - 1].ExtendedProperties.ContainsKey("NBi::Type"))
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
                            finalTbl.Columns.Add(row.Table.Columns[cell.Start.Column - 1].ColumnName);
                            finalTbl.Columns[cell.Start.Column - 1].ExtendedProperties.Add("NBi::Type", nBIcolType);

                            if (row.Table.Columns[cell.Start.Column - 1].ExtendedProperties.ContainsKey("NBi::Role"))
                                finalTbl.Columns[cell.Start.Column - 1].ExtendedProperties.Add("NBi::Role", row.Table.Columns[cell.Start.Column - 1].ExtendedProperties["NBi::Role"]);

                            finalTbl.Columns[cell.Start.Column - 1].DataType = colType;

                            row.Table.Columns[cell.Start.Column - 1].ExtendedProperties.Add("NBi::Type", nBIcolType);
                        }
                        row[cell.Start.Column - 1] = cell.Text;
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
