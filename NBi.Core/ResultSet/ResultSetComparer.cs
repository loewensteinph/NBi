using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NBi.Core.ResultSet.Comparer;
using System.Text;
using NBi.Core.ResultSet.Converter;

namespace NBi.Core.ResultSet
{
    public class ResultSetComparer : IResultSetComparer
    {
        public ResultSetComparisonSettings Settings { get; set; }

        private readonly Dictionary<KeyCollection, CompareHelper> xDict = new Dictionary<KeyCollection, CompareHelper>();
        private readonly Dictionary<KeyCollection, CompareHelper> yDict = new Dictionary<KeyCollection, CompareHelper>();

        private readonly NumericComparer numericComparer = new NumericComparer();
        private readonly TextComparer textComparer = new TextComparer();
        private readonly DateTimeComparer dateTimeComparer = new DateTimeComparer();
        private readonly BooleanComparer booleanComparer = new BooleanComparer();

        public ResultSetComparer()
        {
        }

        public ResultSetComparer(ResultSetComparisonSettings settings)
        {
            Settings = settings;
        }

        public ResultSetCompareResult Compare(object x, object y)
        {
            if (x is DataTable && y is DataTable)
                return doCompare((DataTable)y, (DataTable)x);

            if (x is ResultSet && y is ResultSet)
                return doCompare(((ResultSet)y).Table, ((ResultSet)x).Table);

            throw new ArgumentException();
        }

        private void BuildRowDictionary(DataTable dt, Dictionary<KeyCollection, CompareHelper> dict, DataRowKeysComparer keyComparer, bool isSystemUnderTest)
        {
            dict.Clear();
            foreach (DataRow row in dt.Rows)
            {
                CompareHelper hlpr = new CompareHelper();

                var ep = dt.ExtendedProperties["NBi::ResultSetType"];
                var keys = keyComparer.GetKeys(row);

                hlpr.Keys = keys;
                hlpr.DataRowObj = row;

                //Check that the rows in the reference are unique
                // All the rows should be unique regardless of whether it is the system under test or the result set.
                if (dict.ContainsKey(keys))
                {
                    throw new ResultSetComparerException(
                        string.Format("The {0} data set has some duplicated keys. Check your keys definition or the result set defined in your {1}. The duplicated hashcode is {2}.\r\nRow to insert:{3}.\r\nRow already inserted:{4}.",
                            isSystemUnderTest ? "actual" : "expected",
                            isSystemUnderTest ? "system-under-test" : "assertion",
                            keys.GetHashCode(),
                            RowToString(row),
                            RowToString(dict[keys].DataRowObj)
                            )
                        );
                }

                dict.Add(keys, hlpr);
            }
        }

        private string RowToString(DataRow row)
        {
            var sb = new StringBuilder();
            sb.Append("<");
            foreach (var obj in row.ItemArray)
            {
                if (obj == null)
                    sb.Append("(null)");
                else
                    sb.Append(obj.ToString());
                sb.Append("|");
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append(">");

            return sb.ToString();
        }

        private ResultSetComparisonSettings GetExcelComparisonSettings(DataTable x)
        {
            List<Column> columnList = new List<Column>();


            foreach (DataColumn dataColumn in x.Columns)
            {
                Column column = new Column();

                column.Index = dataColumn.Ordinal;

                column.Role = dataColumn.ExtendedProperties["NBi::Role"].Equals(ColumnRole.Key) ? ColumnRole.Key : ColumnRole.Value;
                switch ((ColumnType)dataColumn.ExtendedProperties["NBi::Type"])
                {
                    case ColumnType.Numeric:
                        column.Type = ColumnType.Numeric;
                        break;
                    case ColumnType.DateTime:
                        column.Type = ColumnType.DateTime;
                        break;
                    case ColumnType.Boolean:
                        column.Type = ColumnType.Boolean;
                        break;
                    default:
                        column.Type = ColumnType.Text;
                        break;
                }
                if (dataColumn.ExtendedProperties.ContainsKey("NBi::Tolerance") && dataColumn.ExtendedProperties["NBi::Tolerance"]!=null && column.Role != ColumnRole.Key)
                {
                    var tolerance = (Tolerance)(dataColumn.ExtendedProperties["NBi::Tolerance"]);
                    column.Tolerance = tolerance.ValueString;
                }

                if (dataColumn.ExtendedProperties.ContainsKey("NBi::Rounding") && dataColumn.ExtendedProperties["NBi::Rounding"] != null)
                {
                    var rounding = (Rounding)(dataColumn.ExtendedProperties["NBi::Rounding"]);
                    column.RoundingStyle = rounding.Style;
                    column.RoundingStep = rounding.Step;
                }
                columnList.Add(column);
            }

            List<IColumnDefinition> columnDefinitions = new List<IColumnDefinition>(columnList);

            ResultSetComparisonSettings ResultSetComparisonSettings = new ResultSetComparisonSettings(
                ResultSetComparisonSettings.KeysChoice.First,
                ResultSetComparisonSettings.ValuesChoice.Last,
                columnDefinitions
            );

            return ResultSetComparisonSettings;
        }

        protected ResultSetCompareResult doCompare(DataTable x, DataTable y)
        {
            var chrono = DateTime.Now;

            var columnsCount = Math.Max(y.Columns.Count, x.Columns.Count);
            if (Settings == null)
                BuildDefaultSettings(columnsCount);
            else
                Settings.ApplyTo(columnsCount);

            Settings.ConsoleDisplay();

            WriteSettingsToDataTableProperties(x, Settings);
            if (x.ExtendedProperties.ContainsKey("NBi::ResultSetType") && Settings.GetMinColumnIndexDefined() == -1)
                Settings = GetExcelComparisonSettings(x);

            WriteSettingsToDataTableProperties(y, Settings);

            CheckSettingsAndDataTable(y, Settings);
            CheckSettingsAndDataTable(x, Settings);

            CheckSettingsAndFirstRow(y, Settings);
            CheckSettingsAndFirstRow(x, Settings);

            var keyComparer = new DataRowKeysComparer(Settings, x.Columns.Count);

            chrono = DateTime.Now;
            BuildRowDictionary(x, xDict, keyComparer, false);
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, string.Format("Building first rows dictionary: {0} [{1}]", x.Rows.Count, DateTime.Now.Subtract(chrono).ToString(@"d\d\.hh\h\:mm\m\:ss\s\ \+fff\m\s")));
            chrono = DateTime.Now;
            BuildRowDictionary(y, yDict, keyComparer, true);
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, string.Format("Building second rows dictionary: {0} [{1}]", y.Rows.Count, DateTime.Now.Subtract(chrono).ToString(@"d\d\.hh\h\:mm\m\:ss\s\ \+fff\m\s")));

            chrono = DateTime.Now;
            List<CompareHelper> missingRows;
            {
                var missingRowKeys = xDict.Keys.Except(yDict.Keys);
                missingRows = new List<CompareHelper>(missingRowKeys.Count());
                foreach (var i in missingRowKeys)
                {
                    missingRows.Add(xDict[i]);
                }
            }
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, string.Format("Missing rows: {0} [{1}]", missingRows.Count(), DateTime.Now.Subtract(chrono).ToString(@"d\d\.hh\h\:mm\m\:ss\s\ \+fff\m\s")));

            chrono = DateTime.Now;
            List<CompareHelper> unexpectedRows;
            {
                var unexpectedRowKeys = yDict.Keys.Except(xDict.Keys);
                unexpectedRows = new List<CompareHelper>(unexpectedRowKeys.Count());
                foreach (var i in unexpectedRowKeys)
                {
                    unexpectedRows.Add(yDict[i]);
                }
            }
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, string.Format("Unexpected rows: {0} [{1}]", unexpectedRows.Count(), DateTime.Now.Subtract(chrono).ToString(@"d\d\.hh\h\:mm\m\:ss\s\ \+fff\m\s")));

            chrono = DateTime.Now;
            List<CompareHelper> keyMatchingRows;
            {
                var keyMatchingRowKeys = xDict.Keys.Intersect(yDict.Keys);
                keyMatchingRows = new List<CompareHelper>(keyMatchingRowKeys.Count());
                foreach (var i in keyMatchingRowKeys)
                {
                    keyMatchingRows.Add(xDict[i]);
                }
            }
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, string.Format("Rows with a matching key and not duplicated: {0}  [{1}]", keyMatchingRows.Count(), DateTime.Now.Subtract(chrono).ToString(@"d\d\.hh\h\:mm\m\:ss\s\ \+fff\m\s")));

            chrono = DateTime.Now;
            var nonMatchingValueRows = new List<DataRow>();

            // If all of the columns make up the key, then we already know which rows match and which don't.
            //  So there is no need to continue testing
            CompareValues(keyMatchingRows, nonMatchingValueRows);
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, string.Format("Rows with a matching key but without matching value: {0} [{1}]", nonMatchingValueRows.Count(), DateTime.Now.Subtract(chrono).ToString(@"d\d\.hh\h\:mm\m\:ss\s\ \+fff\m\s")));

            var duplicatedRows = new List<DataRow>(); // Dummy place holder

            return ResultSetCompareResult.Build(
                missingRows.Select(a => a.DataRowObj).ToList(),
                unexpectedRows.Select(a => a.DataRowObj).ToList(),
                duplicatedRows,
                keyMatchingRows.Select(a => a.DataRowObj).ToList(),
                nonMatchingValueRows
                );
        }

        private void CompareValues(List<CompareHelper> keyMatchingRows, List<DataRow> nonMatchingValueRows)
        {
            if (Settings.KeysDef != ResultSetComparisonSettings.KeysChoice.All)
            {
                foreach (var rxHelper in keyMatchingRows)
                {
                    var ryHelper = yDict[rxHelper.Keys];

                    //if (ryHelper.ValuesHashed == rxHelper.ValuesHashed)
                    //{
                    //    // quick shortcut. If the hash of the values matches, then there is no further need to test
                    //    continue;
                    //}

                    var rx = rxHelper.DataRowObj;
                    var ry = ryHelper.DataRowObj;

                    var isRowOnError = false;

                    for (int i = 0; i < rx.Table.Columns.Count; i++)
                    {
                        if (Settings.GetColumnRole(i) == ColumnRole.Value)
                        {
                            //Any management
                            if (rx[i].ToString() != "(any)" && ry[i].ToString() != "(any)")
                            {
                                //Null management
                                if (rx.IsNull(i) || ry.IsNull(i))
                                {
                                    if ((!rx.IsNull(i) && rx[i].ToString() != "(blank)") || (!ry.IsNull(i) && ry[i].ToString() != "(blank)"))
                                    {
                                        ry.SetColumnError(i, ry.IsNull(i) ? rx[i].ToString() : "(null)");
                                        if (!isRowOnError)
                                        {
                                            isRowOnError = true;
                                            nonMatchingValueRows.Add(ry);
                                        }

                                    }
                                }
                                //(value) management
                                else if (rx[i].ToString() == "(value)" || ry[i].ToString() == "(value)")
                                {
                                    if (rx.IsNull(i) || ry.IsNull(i))
                                    {
                                        ry.SetColumnError(i, rx[i].ToString());
                                        if (!isRowOnError)
                                        {
                                            isRowOnError = true;
                                            nonMatchingValueRows.Add(ry);
                                        }
                                    }
                                }
                                //Not Null management
                                else
                                {
                                    ComparerResult result = null;

                                    //Numeric
                                    if (Settings.GetColumnType(i) == ColumnType.Numeric)
                                    {
                                        //Convert to decimal
                                        if (Settings.IsRounding(i))
                                            result = numericComparer.Compare(rx[i], ry[i], Settings.GetRounding(i));
                                        else
                                            result = numericComparer.Compare(rx[i], ry[i], Settings.GetTolerance(i));
                                    }
                                    //Date and Time
                                    else if (Settings.GetColumnType(i) == ColumnType.DateTime)
                                    {
                                        //Convert to dateTime
                                        if (Settings.IsRounding(i))
                                            result = dateTimeComparer.Compare(rx[i], ry[i], Settings.GetRounding(i));
                                        else
                                            result = dateTimeComparer.Compare(rx[i], ry[i], Settings.GetTolerance(i));
                                    }
                                    //Boolean
                                    else if (Settings.GetColumnType(i) == ColumnType.Boolean)
                                    {
                                        //Convert to bool
                                        result = booleanComparer.Compare(rx[i], ry[i]);
                                    }
                                    //Text
                                    else
                                    {
                                        result = textComparer.Compare(rx[i], ry[i]);
                                    }

                                    //If are not equal then we need to set the message in the ColumnError.
                                    if (!result.AreEqual)
                                    {
                                        ry.SetColumnError(i, result.Message);
                                        if (!isRowOnError)
                                        {
                                            isRowOnError = true;
                                            nonMatchingValueRows.Add(ry);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void WriteSettingsToDataTableProperties(DataTable dt, ResultSetComparisonSettings settings)
        {
            foreach (DataColumn column in dt.Columns)
            {
                if (!dt.ExtendedProperties.ContainsKey("NBi::ResultSetType"))
                {
                    if (column.ExtendedProperties.ContainsKey("NBi::Type"))
                        column.ExtendedProperties["NBi::Type"] = settings.GetColumnType(column.Ordinal);
                    else
                        column.ExtendedProperties.Add("NBi::Type", settings.GetColumnType(column.Ordinal));
                }
                if (column.ExtendedProperties.ContainsKey("NBi::Role") && !dt.ExtendedProperties["NBi::ResultSetType"].Equals("Excel") && !column.ExtendedProperties["NBi::Role"].Equals(ColumnRole.Key))
                    column.ExtendedProperties["NBi::Role"] = settings.GetColumnRole(column.Ordinal);
                if (!column.ExtendedProperties.ContainsKey("NBi::Role"))
                    column.ExtendedProperties.Add("NBi::Role", settings.GetColumnRole(column.Ordinal));
                if (column.ExtendedProperties.ContainsKey("NBi::Tolerance") && !dt.ExtendedProperties["NBi::ResultSetType"].Equals("Excel"))
                    column.ExtendedProperties["NBi::Tolerance"] = settings.GetTolerance(column.Ordinal);
                if (!column.ExtendedProperties.ContainsKey("NBi::Tolerance"))
                    column.ExtendedProperties.Add("NBi::Tolerance", settings.GetTolerance(column.Ordinal));
                if (column.ExtendedProperties.ContainsKey("NBi::Rounding") && !dt.ExtendedProperties["NBi::ResultSetType"].Equals("Excel"))
                    column.ExtendedProperties["NBi::Rounding"] = settings.GetRounding(column.Ordinal);
                if (!column.ExtendedProperties.ContainsKey("NBi::Rounding"))
                    column.ExtendedProperties.Add("NBi::Rounding", settings.GetRounding(column.Ordinal));
            }
        }


        protected void CheckSettingsAndDataTable(DataTable dt, ResultSetComparisonSettings settings)
        {
            var max = settings.GetMaxColumnIndexDefined();
            if (dt.Columns.Count <= max)
            {
                var exception = string.Format("You've defined a column with an index of {0}, meaning that your result set would have at least {1} columns but your result set has only {2} columns."
                    , max
                    , max + 1
                    , dt.Columns.Count);

                if (dt.Columns.Count == max && settings.GetMinColumnIndexDefined() == 1)
                    exception += " You've no definition for a column with an index of 0. Are you sure you'vent started to index at 1 in place of 0?";

                throw new ResultSetComparerException(exception);
            }
        }

        protected void CheckSettingsAndFirstRow(DataTable dt, ResultSetComparisonSettings settings)
        {
            if (dt.Rows.Count == 0)
                return;

            var dr = dt.Rows[0];
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                if (!dr.IsNull(i))
                {
                    if (settings.GetColumnType(i) == ColumnType.Numeric && IsNumericField(dr.Table.Columns[i]))
                        continue;

                    var numericConverter = new NumericConverter();
                    if (settings.GetColumnType(i) == ColumnType.Numeric && !(numericConverter.IsValid(dr[i]) || BaseComparer.IsValidInterval(dr[i])))
                    {
                        var exception = string.Format("The column with an index of {0} is expecting a numeric value but the first row of your result set contains a value '{1}' not recognized as a valid numeric value or a valid interval."
                            , i, dr[i].ToString());

                        if (numericConverter.IsValid(dr[i].ToString().Replace(",", ".")))
                            exception += " Aren't you trying to use a comma (',' ) as a decimal separator? NBi requires that the decimal separator must be a '.'.";

                        throw new ResultSetComparerException(exception);
                    }

                    if (settings.GetColumnType(i) == ColumnType.DateTime && IsDateTimeField(dr.Table.Columns[i]))
                        return;

                    if (settings.GetColumnType(i) == ColumnType.DateTime && !BaseComparer.IsValidDateTime(dr[i].ToString()))
                    {
                        throw new ResultSetComparerException(
                            string.Format("The column with an index of {0} is expecting a date & time value but the first row of your result set contains a value '{1}' not recognized as a valid date & time value."
                                , i, dr[i].ToString()));
                    }
                }
            }
        }

        private bool IsNumericField(DataColumn dataColumn)
        {
            return
                dataColumn.DataType == typeof(Byte) ||
                dataColumn.DataType == typeof(Decimal) ||
                dataColumn.DataType == typeof(Double) ||
                dataColumn.DataType == typeof(Int16) ||
                dataColumn.DataType == typeof(Int32) ||
                dataColumn.DataType == typeof(Int64) ||
                dataColumn.DataType == typeof(SByte) ||
                dataColumn.DataType == typeof(Single) ||
                dataColumn.DataType == typeof(UInt16) ||
                dataColumn.DataType == typeof(UInt32) ||
                dataColumn.DataType == typeof(UInt64);
        }

        private bool IsDateTimeField(DataColumn dataColumn)
        {
            return
                dataColumn.DataType == typeof(DateTime);
        }

        protected void BuildDefaultSettings(int columnsCount)
        {
            Settings = new ResultSetComparisonSettings(
                columnsCount,
                ResultSetComparisonSettings.KeysChoice.AllExpectLast,
                ResultSetComparisonSettings.ValuesChoice.Last);
        }

    }
}
