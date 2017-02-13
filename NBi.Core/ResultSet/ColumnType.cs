using System.Xml.Serialization;

namespace NBi.Core.ResultSet
{
    public enum ColumnType
    {
        [XmlEnum(Name = "text")]
        Text = 0,
        [XmlEnum(Name = "numeric")]
        Numeric = 1,
        [XmlEnum(Name = "dateTime")]
        DateTime = 2,
        [XmlEnum(Name = "boolean")]
        Boolean = 3,
        [XmlEnum(Name = "table")]
        Table = 10
    }
}
