using System;
using System.Linq;
using System.Collections.Generic;
using NBi.Core.Analysis.Request;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using NBi.Core.Analysis.Member;

namespace NBi.NUnit.Builder
{
	abstract class AbstractMembersBuilder : AbstractTestCaseBuilder
	{
		protected readonly DiscoveryRequestFactory discoveryFactory;

		public AbstractMembersBuilder()
		{
			discoveryFactory = new DiscoveryRequestFactory();
		}

		public AbstractMembersBuilder(DiscoveryRequestFactory factory)
		{
			discoveryFactory = factory;
		}
		
		protected MembersXml SystemUnderTestXml { get; set; }

		protected override void BaseSetup(AbstractSystemUnderTestXml sutXml, AbstractConstraintXml ctrXml)
		{
			if (!(sutXml is MembersXml))
				throw new ArgumentException("Constraint must be a 'MembesrXml'");

			SystemUnderTestXml = (MembersXml)sutXml;
		}

		protected override void BaseBuild()
		{
			SystemUnderTest = InstantiateSystemUnderTest(SystemUnderTestXml);
		}

        protected object InstantiateSystemUnderTest(MembersXml sutXml)
        {
            try 
	        {	        
		        return InstantiateMembersDiscovery(sutXml);
	        }
	        catch (ArgumentOutOfRangeException)
	        {
		
		        throw new ArgumentOutOfRangeException("sutXml", sutXml, "The system-under-test for members must be a hierarchy or a level or a set");
	        }
        }
  
        protected MembersDiscoveryRequest InstantiateMembersDiscovery(MembersXml membersXml)
        {

            string perspective = null, dimension = null, hierarchy = null, level = null, set=null;
            MembersDiscoveryRequest disco = null;

            if (membersXml.Item == null)
                throw new ArgumentNullException();

            if (!(membersXml.Item is DatabaseModelItemXml))
                throw new ArgumentException();

            if (!(membersXml.Item is HierarchyXml || membersXml.Item is LevelXml || membersXml.Item is SetXml))
                throw new ArgumentOutOfRangeException();

            var item = membersXml.Item as DatabaseModelItemXml;

            if (item is HierarchyXml)
            {
                perspective = ((HierarchyXml)item).Perspective;
                dimension = ((HierarchyXml)item).Dimension;
                hierarchy = item.Caption;
            }

            if (item is LevelXml)
            {
                perspective = ((LevelXml)item).Perspective;
                dimension = ((LevelXml)item).Dimension;
                hierarchy = ((LevelXml)item).Hierarchy;
                level = item.Caption;
            }
            if (item is HierarchyXml || item is LevelXml)
            {
                disco = discoveryFactory.Build(
                    item.GetConnectionString(),
                    membersXml.ChildrenOf,
                    membersXml.Exclude.Items,
                    BuildPatterns(membersXml.Exclude.Patterns),
                    perspective,
                    dimension,
                    hierarchy,
                    level);
            }
                
            if (item is SetXml)
            {
                perspective = ((SetXml)item).Perspective;
                set = item.Caption;

                disco = discoveryFactory.Build(
                    item.GetConnectionString(),
                    membersXml.Exclude.Items,
                    BuildPatterns(membersXml.Exclude.Patterns),
                    perspective,
                    set);
            }

            if (disco == null)
                throw new ArgumentException();

            return disco;
        }

		private IEnumerable<PatternValue> BuildPatterns(IEnumerable<PatternXml> patterns)
		{
			var res = new List<PatternValue>();
			if (patterns != null)
			{ 
				foreach (var p in patterns)
				{
					var pv = new PatternValue();
					pv.Pattern = p.Pattern;
					pv.Text = p.Value;
					res.Add(pv);
				}
			}
			return res;
		}
	}
}
