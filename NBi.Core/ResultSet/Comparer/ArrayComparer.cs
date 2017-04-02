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

        private BaseComparer comparer;

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

                    IList<object> xArray = (x as IEnumerable<object>).ToList();
                    IList<object> yArray = (y as IEnumerable<object>).ToList();

                    //Check the length of the arrays
                    if (xArray.Count() != yArray.Count())
                        return new ComparerResult("(array-size)");

                    switch (columnType)
                    {
                        case ColumnType.Text:
                            comparer = new TextComparer();
                            break;
                        case ColumnType.Numeric:
                            comparer = new NumericComparer();
                            break;
                        case ColumnType.DateTime:
                            comparer = new DateTimeComparer();
                            break;
                        case ColumnType.Boolean:
                            comparer = new BooleanComparer();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (object o in yArray)
                        if (!TryRemove(o, ref xArray))
                            return new ComparerResult("(array-mismatch)");
                }
            }
            return ComparerResult.Equality;
        }

        public bool TryRemove(object o, ref IList<object> list)
        {
            for (int index = 0; index < list.Count; index++)
                if (comparer.Compare(list[index], o).AreEqual)
                {
                    list.RemoveAt(index);
                    return true;
                }
            return false;
        }
    }
}
