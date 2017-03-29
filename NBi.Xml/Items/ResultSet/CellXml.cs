using System.Xml.Serialization;
using NBi.Core.ResultSet;
using System.Collections.Generic;
using System.Linq;

namespace NBi.Xml.Items.ResultSet
{
    public class CellXml : ICell
    {
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute("column-name")]
        public string ColumnName { get; set; }

        public override string ToString()
        {
            return Value;
        }

        [XmlElement("row")]
        public List<RowXml> _rows { get; set; }

        public IList<IRow> Rows
        {
            get { return _rows.Cast<IRow>().ToList(); }
        }

        [XmlElement("item")]
        public List<ItemXml> Items { get; set; }

        [XmlIgnore]
        public IEnumerable<string> Values
        {
            get
            {
                if (Items.Count == 0 && Value=="(null)")
                    return null; 
                else if (Items.Count == 0 && Value!="(null)")
                    return Enumerable.Repeat(Value, 1);
                else
                    return Items.Select(x => x.Value).ToList();
            }
            set
            {
                Items.Clear();
                foreach (var v in value)
                    Items.Add(new ItemXml() { Value = v });
            }
        }

        public CellXml()
        {
            _rows = new List<RowXml>();
            Items = new List<ItemXml>();
        }
    }
}
