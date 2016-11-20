using NBi.Core.Structure;
using NBi.Core.Structure.Relational.PostFilters;
using NBi.Xml.Items;
using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Model;

namespace NBi.NUnit.Builder
{
    class FilterObjectDatabaseModelBuilder
    {
        protected bool isSetup;
        protected bool isBuild;
        protected DatabaseModelItemXml item;
        private IEnumerable<IFilter> filters;
        private Target target;

        public void Setup(DatabaseModelItemXml model)
        {
            isBuild = false;
            this.item = model;
            isSetup = true;
        }

        public void Build()
        {
            if (!isSetup)
                throw new InvalidOperationException();

            target = BuildTarget();
            filters = BuildFilters();

            isBuild = true;
        }

        public IEnumerable<IFilter> GetFilters()
        {
            return filters;
        }

        public Target GetTarget()
        {
            return target;
        }

        protected virtual IEnumerable<IFilter> BuildFilters()
        {
            

            if (item is IPerspectiveFilter)
                yield return new CaptionFilter(Target.Perspectives, ((IPerspectiveFilter)item).Perspective);
            if (item is IDimensionFilter)
                yield return new CaptionFilter(Target.Dimensions, ((IDimensionFilter)item).Dimension);
            if (item is IHierarchyFilter)
                yield return new CaptionFilter(Target.Hierarchies, ((IHierarchyFilter)item).Hierarchy);
            if (item is ILevelFilter)
                yield return new CaptionFilter(Target.Levels, ((ILevelFilter)item).Level);
            if (item is IMeasureGroupFilter && !(string.IsNullOrEmpty(((IMeasureGroupFilter)item).MeasureGroup)))
                yield return new CaptionFilter(Target.MeasureGroups, ((IMeasureGroupFilter)item).MeasureGroup);
            if (item is IDisplayFolderFilter && !(string.IsNullOrEmpty(((IDisplayFolderFilter)item).DisplayFolder)))
                yield return new CaptionFilter(Target.DisplayFolders, ((IDisplayFolderFilter)item).DisplayFolder);

            if (item is ITableFilter)
                yield return new CaptionFilter(Target.Tables, ((ITableFilter)item).Table);
            if (item is IRoutineFilter)
                yield return new CaptionFilter(Target.Routines, ((IRoutineFilter)item).Routine);
            if (item is IResultFilter && ((IResultFilter)item).IsResult != IsResultOption.Unspecified)
                yield return new IsResultFilter(((IResultFilter)item).IsResult == IsResultOption.Yes);
            if (item is IParameterDirectionFilter && ((IParameterDirectionFilter)item).Direction != ParameterDirectionOption.Unspecified)
                yield return new ParameterDirectionFilter(((IParameterDirectionFilter)item).Direction.ToString());
            if (item is IOwnerFilter && (!string.IsNullOrEmpty((item as IOwnerFilter).Owner)))
                yield return new OwnerFilter(((IOwnerFilter)item).Owner);

            if (item is IModelSingleItemXml)
                yield return new CaptionFilter(target, ((IModelSingleItemXml)item).Caption);
        }

        protected virtual Target BuildTarget()
        {
            if (!isSetup)
                throw new InvalidOperationException();

            if (item is MeasuresXml || item is MeasureXml)
                return Target.Measures;
            if (item is MeasureGroupsXml || item is MeasureGroupXml)
                return Target.MeasureGroups;
            if (item is ColumnsXml || item is ColumnXml)
                return Target.Columns;
            if (item is TablesXml || item is TableXml)
                return Target.Tables;
            if (item is PropertiesXml || item is PropertyXml)
                return Target.Properties;
            if (item is LevelsXml || item is LevelXml)
                return Target.Levels;
            if (item is HierarchiesXml || item is HierarchyXml)
                return Target.Hierarchies;
            if (item is DimensionsXml || item is DimensionXml)
                return Target.Dimensions;
            if (item is SetsXml || item is SetXml)
                return Target.Sets;
            if (item is RoutineParametersXml || item is RoutineParameterXml)
                return Target.Parameters;
            if (item is RoutinesXml || item is RoutineXml)
                return Target.Routines;
            if (item is PerspectivesXml || item is PerspectiveXml)
                return Target.Perspectives;
            else
                throw new ArgumentException(item.GetType().Name);
        }
    }
}
