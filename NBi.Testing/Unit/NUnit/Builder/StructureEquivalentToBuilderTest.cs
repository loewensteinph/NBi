    #region Using directives

using System.Linq;
using Moq;
using NBi.NUnit.Builder;
using NBi.NUnit.Structure;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Settings;
using NBi.Xml.Systems;
using NUnit.Framework;
using NBi.Core.Structure.Olap;
using NBi.Xml.Items.Filters;
using NBi.Core.Structure;

#endregion

namespace NBi.Testing.Unit.NUnit.Builder
{
    [TestFixture]
    public class StructureEquivalentToBuilderTest
    {

        #region SetUp & TearDown
        //Called only at instance creation
        [TestFixtureSetUp]
        public void SetupMethods()
        {

        }

        //Called only at instance destruction
        [TestFixtureTearDown]
        public void TearDownMethods()
        {
        }

        //Called before each test
        [SetUp]
        public void SetupTest()
        {
        }

        //Called after each test
        [TearDown]
        public void TearDownTest()
        {
        }
        #endregion

        //@@@@@@@@@@@@@@@@@@@@@@@@@
        //    GetConstraint()
        //@@@@@@@@@@@@@@@@@@@@@@@@@

        [Test]
        public void GetConstraint_BuildWithWithList_CorrectConstraint()
        {
            //Buiding object used during test
            var sutXml = new StructureXml();
            sutXml.Item = new MeasureGroupsXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective"
            };

            var ctrXml = new EquivalentToXml();
            ctrXml.Items.Add("Search");
            ctrXml.Items.Add("Search 2");

            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var ctr = builder.GetConstraint();

            Assert.That(ctr, Is.InstanceOf<EquivalentToConstraint>());
        }


        //@@@@@@@@@@@@@@@@@@@@@@@@@
        //    GetSystemUnderTest()
        //@@@@@@@@@@@@@@@@@@@@@@@@@


        //**********************
        //       Default ConnectionString
        //**********************

        [Test]
        public void GetSystemUnderTest_ConnectionStringInDefault_CorrectlyInitialized()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();

            sutXml.Item = new MeasureGroupsXml()
            {
                Perspective = "Perspective"
            };

            sutXml.Default = new DefaultXml() { ConnectionString = ConnectionStringReader.GetAdomd() };

            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            //Call the method to test
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());
        }


        //**********************
        //       Pespective
        //**********************

        [Test]
        public void GetSystemUnderTest_CorrectPerspectiveTarget_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new PerspectivesXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd()
            };
            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());
        }
        
        //**********************
        //       Measure-Group
        //**********************

        [Test]
        public void GetSystemUnderTest_CorrectMeasureGroupTarget_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new MeasureGroupsXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective"
            };
            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            //Call the method to test
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());
        }

        [Test]
        public void GetSystemUnderTest_InCorrectMeasureGroupTargetWithoutCaption_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new MeasureGroupsXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective"
            };
            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());
        }
        
        

        //**********************
        //       Measure
        //**********************

        [Test]
        public void GetSystemUnderTest_CorrectMeasureTarget_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new MeasuresXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective",
                MeasureGroup = "MeasureGroup"
            };
            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>()); 
        }

        //**********************
        //       Dimension
        //**********************

        [Test]
        public void GetSystemUnderTest_CorrectDimensionTarget_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new DimensionsXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective"
            };

            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            //Call the method to test
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            var command = sut as OlapCommand;
            Assert.NotNull(command);
            Assert.NotNull(command.Description);
            Assert.IsTrue(command.Description.Filters.Any(f => f is CaptionFilter && (f as CaptionFilter).Caption == "Perspective"));
            Assert.AreEqual(command.Description.Filters.Count(), 1);
        }

        //**********************
        //       Hierarchies
        //**********************

        [Test]
        public void GetSystemUnderTest_CorrectHierarchyTarget_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new DimensionsXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective"
            };

            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            //Call the method to test
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());
        }


        //**********************
        //       Levels
        //**********************


        [Test]
        public void GetSystemUnderTest_CorrectLevelTarget_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new LevelsXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective",
                Dimension = "Dimension",
                Hierarchy = "Hierarchy"
            };

            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());
        }

        //**********************
        //       Properties
        //**********************


        [Test]
        public void GetSystemUnderTest_CorrectPropertyTarget_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new PropertiesXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective",
                Dimension = "Dimension",
                Hierarchy = "Hierarchy",
                Level = "Level"
            };
            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());
        }

        //**********************
        //  Test Filters
        //    IPerspectiveFilter
        //    IMeasureGroupFilter      
        //    IDisplayFolderFilter
        //**********************


        [Test]
        public void GetSystemUndeTest_CorrectFiltersAppliedOnDescriptionPart1_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new MeasuresXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective",
                MeasureGroup = "MeasureGroup",
                DisplayFolder = "DisplayFolder"
            };
            
            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());

            var command = sut as OlapCommand;
            Assert.NotNull(command);
            Assert.NotNull(command.Description);
            Assert.IsTrue(command.Description.Filters.Any(f => f is CaptionFilter && (f as CaptionFilter).Caption == "Perspective"));
            Assert.IsTrue(command.Description.Filters.Any(f => f is CaptionFilter && (f as CaptionFilter).Caption == "MeasureGroup"));
            Assert.IsTrue(command.Description.Filters.Any(f => f is CaptionFilter && (f as CaptionFilter).Caption == "DisplayFolder"));
        }


        //**********************
        //  Test Filters
        //    IDimensionFilter
        //    IHierarchyFilter
        //    ILevelFilter
        //**********************


        [Test]
        public void GetSystemUndeTest_CorrectFiltersAppliedOnDescriptionPart2_Success()
        {
            //Buiding object used during test
            var ctrXmlStubFactory = new Mock<EquivalentToXml>();
            var ctrXml = ctrXmlStubFactory.Object;

            var sutXml = new StructureXml();
            sutXml.Item = new PropertiesXml()
            {
                ConnectionString = ConnectionStringReader.GetAdomd(),
                Perspective = "Perspective",
                Dimension = "Dimension",
                Hierarchy = "Hierarchy",
                Level = "Level"
            };

            var builder = new StructureEquivalentToBuilder();
            builder.Setup(sutXml, ctrXml);
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<OlapCommand>());

            var command = sut as OlapCommand;
            Assert.NotNull(command);
            Assert.NotNull(command.Description);
            Assert.IsTrue(command.Description.Filters.Any(f => f is CaptionFilter && (f as CaptionFilter).Caption == "Dimension"));
            Assert.IsTrue(command.Description.Filters.Any(f => f is CaptionFilter && (f as CaptionFilter).Caption == "Hierarchy"));
            Assert.IsTrue(command.Description.Filters.Any(f => f is CaptionFilter && (f as CaptionFilter).Caption == "Level"));
        }
    }
}
