using NBi.Core.ResultSet.Converter;
using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace NBi.Core.ResultSet.Comparer
{
    class TableComparer : BaseComparer
    {
        private readonly IConverter<DataTable> converter;
        private readonly SettingsResultSetComparisonByName settings;

        public TableComparer(SettingsResultSetComparisonByName settings)
        {
            converter = new TableConverter();
            this.settings = settings;
        }

        protected override ComparerResult CompareObjects(object x, object y)
        {
            var xTable = converter.Convert(x);
            var yTable = converter.Convert(y);

            if (IsEqual(xTable, yTable))
                return ComparerResult.Equality;

            return new ComparerResult("(MISMATCH)");
        }

        protected override ComparerResult CompareObjects(object x, object y, Tolerance tolerance)
        {
            throw new NotImplementedException("You cannot compare two tables with a tolerance");
        }

        protected override ComparerResult CompareObjects(object x, object y, Rounding rounding)
        {
            throw new NotImplementedException("You cannot compare two tables with a rounding.");
        }

        protected bool IsEqual(DataTable x, DataTable y)
        {
            if (EqualByNull(x, y) != null)
                return true;

            if ((x == null && y != null) || (x != null && y == null))
                return false;

            var builder = new ResultSetComparisonBuilder();
            builder.Setup(settings);
            builder.Build();
            var engine = builder.GetComparer();
            var result = engine.Compare(x, y);
            return result.Difference == ResultSetDifferenceType.None;
        }

        protected override ComparerResult EqualByNull(object x, object y)
        {
            if (x == null && y == null)
                return ComparerResult.Equality;

            return null;
        }

        protected override bool IsValidObject(object x)
        {
            return converter.IsValid(x);
        }
    }
}
