using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core.ResultSet.Comparer;
using System.Collections.ObjectModel;

namespace NBi.Core.ResultSet
{
    public class SettingsResultSetComparisonByName : SettingsResultSetComparison<string>
    {
        public readonly string ParentColumnName; 
        protected readonly IReadOnlyCollection<SettingsResultSetComparisonByName> subSettings;

        protected override bool IsKey(string name)
        {
            if (ColumnsDef.Any(c => c.Name == name && c.Role != ColumnRole.Key))
                return false;

            if (ColumnsDef.Any(c => c.Name == name && c.Role == ColumnRole.Key))
                return true;
            
            return false;
        }

        protected override bool IsValue(string name)
        {
            if (ColumnsDef.Any(c => c.Name == name && c.Role != ColumnRole.Value))
                return false;

            if (ColumnsDef.Any(c => c.Name == name && c.Role == ColumnRole.Value))
                return true;

            return false;
        }

        public override bool IsArray(string name)
        {
            var isArray = ColumnsDef.Any(
                    c => c.Name == name
                    && c.IsArray);

            if (IsKey(name) && isArray)
                throw new InvalidOperationException(String.Format("The column with name '{0}' cannot be defined as a key and as an array at the same time.", name));

            return isArray;
        }

        public override bool IsRounding(string name)
        {
            return ColumnsDef.Any(
                    c => c.Name == name
                    && c.Role == ColumnRole.Value
                    && c.RoundingStyle != Comparer.Rounding.RoundingStyle.None
                    && !string.IsNullOrEmpty(c.RoundingStep));
        }

        public override Rounding GetRounding(string name)
        {
            if (!IsRounding(name))
                return null;

            return RoundingFactory.Build(ColumnsDef.Single(
                    c => c.Name == name
                    && c.Role == ColumnRole.Value));
        }

        public override ColumnRole GetColumnRole(string name)
        {
            if (!cacheRole.ContainsKey(name))
            {
                if (IsKey(name))
                    cacheRole.Add(name, ColumnRole.Key);
                else if (IsValue(name))
                    cacheRole.Add(name, ColumnRole.Value);
                else
                    cacheRole.Add(name, ColumnRole.Ignore);
            }

            return cacheRole[name];
        }

        public override ColumnType GetColumnType(string name)
        {
            if (!cacheType.ContainsKey(name))
            {
                if (IsNumeric(name))
                    cacheType.Add(name, ColumnType.Numeric);
                else if (IsDateTime(name))
                    cacheType.Add(name, ColumnType.DateTime);
                else if (IsBoolean(name))
                    cacheType.Add(name, ColumnType.Boolean);
                else if (IsTable(name))
                    cacheType.Add(name, ColumnType.Table);
                else
                    cacheType.Add(name, ColumnType.Text);
            }
            return cacheType[name];
        }

        protected override bool IsType(string name, ColumnType type)
        {
            if (ColumnsDef.Any(c => c.Name == name && c.Type != type))
                return false;

            if (ColumnsDef.Any(c => c.Name == name && c.Type == type))
                return true;

            if (IsKey(name))
                return type == ColumnType.Text;

            return (IsValue(name) && ValuesDefaultType == type);
        }

        public override Tolerance GetTolerance(string name)
        {
            if (GetColumnType(name) != ColumnType.Numeric && GetColumnType(name) != ColumnType.DateTime)
                return null;

            var col = ColumnsDef.FirstOrDefault(c => c.Name == name);
            if (col == null || !col.IsToleranceSpecified)
            {
                if (IsNumeric(name))
                    return DefaultTolerance;
                else
                    return DateTimeTolerance.None;
            }

            return ToleranceFactory.Instantiate(col);
        }

        public SettingsResultSetComparisonByName(IEnumerable<IColumnDefinition> columnsDef)
            : this(string.Empty, columnsDef, ColumnType.Numeric, NumericAbsoluteTolerance.None, Enumerable.Empty<SettingsResultSetComparisonByName>()) { }


        public SettingsResultSetComparisonByName(IEnumerable<IColumnDefinition> columnsDef, ColumnType valuesDefaultType, NumericTolerance defaultTolerance)
            : this(string.Empty, columnsDef, valuesDefaultType, defaultTolerance, Enumerable.Empty<SettingsResultSetComparisonByName>()) { }

        public SettingsResultSetComparisonByName(IEnumerable<IColumnDefinition> columnsDef, ColumnType valuesDefaultType, NumericTolerance defaultTolerance, IEnumerable<SettingsResultSetComparisonByName> subSettings)
            : this(string.Empty, columnsDef, valuesDefaultType, defaultTolerance, subSettings) { }

        public SettingsResultSetComparisonByName(string parent, IEnumerable<IColumnDefinition> columnsDef, ColumnType valuesDefaultType, NumericTolerance defaultTolerance)
            : this(parent, columnsDef, valuesDefaultType, defaultTolerance, Enumerable.Empty<SettingsResultSetComparisonByName>()) { }

        public SettingsResultSetComparisonByName(string parent, IEnumerable<IColumnDefinition> columnsDef, ColumnType valuesDefaultType, NumericTolerance defaultTolerance, IEnumerable<SettingsResultSetComparisonByName> subSettings)
            : base(valuesDefaultType, defaultTolerance, new ReadOnlyCollection<IColumnDefinition>(columnsDef.ToList()))
        {
            ParentColumnName = parent;
            this.subSettings = new ReadOnlyCollection<SettingsResultSetComparisonByName>(subSettings.ToList());
        }


        public IEnumerable<string> GetKeyNames()
        {
            return ColumnsDef.Where(c => c.Role == ColumnRole.Key).Select(c => c.Name).OrderBy(x => x);
        }

        public IEnumerable<string> GetValueNames()
        {
            return ColumnsDef.Where(c => c.Role == ColumnRole.Value).Select(c => c.Name);
        }

        public IEnumerable<string> GetColumnNames()
        {
            return ColumnsDef.Where(c => c.Role != ColumnRole.Ignore).Select(c => c.Name);
        }

        public SettingsResultSetComparisonByName GetSubSettings(string parentColumName)
        {
            if (!IsType(parentColumName, ColumnType.Table))
                throw new ArgumentException();

            return subSettings.Where(s => s.ParentColumnName == parentColumName).Single();
        }

    }
}
