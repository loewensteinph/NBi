using NBi.Core.ResultSet.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.ResultSet.Comparer
{
    public class ArrayComparer
    {
        private readonly NumericComparer numericComparer = new NumericComparer();
        private readonly TextComparer textComparer = new TextComparer();
        private readonly DateTimeComparer dateTimeComparer = new DateTimeComparer();
        private readonly BooleanComparer booleanComparer = new BooleanComparer();

        public ComparerResult Compare(object x, object y, ColumnType columnType)
        {
            //Any management
            if (x.ToString() != "(any)" && y.ToString() != "(any)")
            {
                //Null management
                if (DBNull.Value.Equals(x))
                {
                    if (y is IEnumerable<object>)
                    {
                        if (((IEnumerable<object>)y).Count() != 0)
                            return new ComparerResult("(null)");
                    }
                    else if (!DBNull.Value.Equals(y) && y.ToString() != "(null)")
                        return new ComparerResult("(null)");
                }
                else if (DBNull.Value.Equals(y))
                {
                    if (x is IEnumerable<object>)
                    {
                        if (((IEnumerable<object>)x).Count() != 0)
                            return new ComparerResult("(array)");
                    }
                    if (!DBNull.Value.Equals(x) && x.ToString() != "(null)" && x.ToString() != "(blank)")
                        return new ComparerResult(x.ToString());
                }
                //(value) management
                else if (x.ToString() == "(value)" || y.ToString() == "(value)")
                {
                    if (DBNull.Value.Equals(x) || DBNull.Value.Equals(y))
                        return new ComparerResult(DBNull.Value.Equals(y) ? "(null)" : x.ToString());
                    if ((x is IEnumerable<object>) && ((IEnumerable<object>)x).Count() == 0)
                        return new ComparerResult("(null)");
                    if ((y is IEnumerable<object>) && ((IEnumerable<object>)y).Count() == 0)
                        return new ComparerResult("(array)");
                }
                //Not Null management
                else
                {
                    //Check the array
                    if (!(x is IEnumerable<object>))
                        return new ComparerResult("(cell)");
                    if (!(y is IEnumerable<object>))
                        return new ComparerResult("(array)");

                    var xArray = x as IEnumerable<object>;
                    var yArray = y as IEnumerable<object>;

                    //Check the length of the arrays
                    if (xArray.Count() != yArray.Count())
                        return new ComparerResult("(array-size)");

                    Func<object, bool> filter = (o => o.ToString() != "(any)" && o.ToString() != "(value)");

                    //Numeric
                    if (columnType == ColumnType.Numeric)
                    {
                        var converter = new NumericConverter();
                        foreach (var item in xArray.Union(yArray))
                            if (!converter.IsValid(item))
                                return new ComparerResult("(array-type-mismatch)");

                        for (int i = 0; i < xArray.Count(); i++)
                        {
                            var res = numericComparer.Compare(xArray.ElementAt(i), yArray.ElementAt(i));
                            if (!res.AreEqual)
                                return res;
                        }
                    }
                    //Date and Time
                    else if (columnType == ColumnType.DateTime)
                    {
                        var converter = new DateTimeConverter();
                        foreach (var item in xArray.Union(yArray))
                            if (!converter.IsValid(item))
                                return new ComparerResult("(array-type-mismatch)");

                        for (int i = 0; i < xArray.Count(); i++)
                        {
                            var res = dateTimeComparer.Compare(xArray.ElementAt(i), yArray.ElementAt(i));
                            if (!res.AreEqual)
                                return res;
                        }
                    }
                    //Boolean
                    else if (columnType == ColumnType.Boolean)
                    {
                        var converter = new BooleanConverter();
                        foreach (var item in xArray.Union(yArray))
                            if (!converter.IsValid(item))
                                return new ComparerResult("(array-type-mismatch)");

                        for (int i = 0; i < xArray.Count(); i++)
                        {
                            var res = booleanComparer.Compare(xArray.ElementAt(i), yArray.ElementAt(i));
                            if (!res.AreEqual)
                                return res;
                        }
                    }
                    //Text
                    else
                    {
                        var converter = new TextConverter();
                        foreach (var item in xArray.Union(yArray))
                            if (!converter.IsValid(item))
                                return new ComparerResult("(array-type-mismatch)");

                        for (int i = 0; i < xArray.Count(); i++)
                        {
                            var res = textComparer.Compare(xArray.ElementAt(i), yArray.ElementAt(i));
                            if (!res.AreEqual)
                                return res;
                        }
                    }
                }
            }
            return ComparerResult.Equality;
        }
    }
}
