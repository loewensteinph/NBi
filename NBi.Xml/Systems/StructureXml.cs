using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using NBi.Xml.Items;

namespace NBi.Xml.Systems
{
    public class StructureXml: AbstractSystemUnderTestXml
    {
        [XmlElement(Type = typeof(PerspectiveXml), ElementName = "perspective"),
        XmlElement(Type = typeof(MeasureGroupXml), ElementName = "measure-group"),
        XmlElement(Type = typeof(MeasureXml), ElementName = "measure"),
        XmlElement(Type = typeof(DimensionXml), ElementName = "dimension"),
        XmlElement(Type = typeof(HierarchyXml), ElementName = "hierarchy"),
        XmlElement(Type = typeof(LevelXml), ElementName = "level"),
        XmlElement(Type = typeof(PropertyXml), ElementName = "property"),
        XmlElement(Type = typeof(PerspectivesXml), ElementName = "perspectives"),
        XmlElement(Type = typeof(MeasureGroupsXml), ElementName = "measure-groups"),
        XmlElement(Type = typeof(MeasuresXml), ElementName = "measures"),
        XmlElement(Type = typeof(DimensionsXml), ElementName = "dimensions"),
        XmlElement(Type = typeof(HierarchiesXml), ElementName = "hierarchies"),
        XmlElement(Type = typeof(LevelsXml), ElementName = "levels"),
        XmlElement(Type = typeof(PropertiesXml), ElementName = "properties"),
        XmlElement(Type = typeof(TableXml), ElementName = "table"),
        XmlElement(Type = typeof(ColumnXml), ElementName = "column"),
        XmlElement(Type = typeof(TablesXml), ElementName = "tables"),
        XmlElement(Type = typeof(ColumnsXml), ElementName = "columns"),
        XmlElement(Type = typeof(SetXml), ElementName = "set"),
        XmlElement(Type = typeof(SetsXml), ElementName = "sets"),
        XmlElement(Type = typeof(RoutineXml), ElementName = "routine"),
        XmlElement(Type = typeof(RoutinesXml), ElementName = "routines"),
        XmlElement(Type = typeof(RoutineParameterXml), ElementName = "parameter"),
        XmlElement(Type = typeof(RoutineParametersXml), ElementName = "parameters"),
        XmlElement(Type = typeof(ReportParameterXml), ElementName = "report-parameter"),
        XmlElement(Type = typeof(ReportParametersXml), ElementName = "report-parameters")
        ]
        public override ModelItemXml Item { get; set; }

        internal override Dictionary<string, string> GetRegexMatch()
        {
            if (Item is IAutoCategorize)
                return ((IAutoCategorize)Item).GetRegexMatch();
            else
                return new Dictionary<string, string>();
        }

        public override ICollection<string> GetAutoCategories()
        {
            ICollection<string> values = new List<string>();
            if (Item is IAutoCategorize)
                values = ((IAutoCategorize)Item).GetAutoCategories();

            values.Add("Structure");
            return values;
        }
    }
}
