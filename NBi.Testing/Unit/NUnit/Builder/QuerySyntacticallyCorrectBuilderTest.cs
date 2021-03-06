﻿#region Using directives
using System.Data;
using Moq;
using NBi.NUnit.Builder;
using NBi.NUnit.Query;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using NUnit.Framework;
#endregion

namespace NBi.Testing.Unit.NUnit.Builder
{
    [TestFixture]
    public class QuerySyntacticallyCorrectBuilderTest
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

        [Test]
        public void GetConstraint_Build_CorrectConstraint()
        {
            //Buiding object used during test
            var sutXmlStubFactory = new Mock<ExecutionXml>();
            var itemXmlStubFactory = new Mock<QueryableXml>();
            itemXmlStubFactory.Setup(i => i.GetQuery()).Returns("query");
            sutXmlStubFactory.Setup(s => s.Item).Returns(itemXmlStubFactory.Object);
            var sutXml = sutXmlStubFactory.Object;
            sutXml.Item = itemXmlStubFactory.Object;

            var ctrXml = new SyntacticallyCorrectXml();

            var builder = new ExecutionSyntacticallyCorrectBuilder();
            builder.Setup(sutXml, ctrXml);
            //Call the method to test
            builder.Build();
            var ctr = builder.GetConstraint();

            //Assertion
            Assert.That(ctr, Is.InstanceOf<SyntacticallyCorrectConstraint>());
        }

        [Test]
        public void GetSystemUnderTest_Build_CorrectIDbCommand()
        {
            //Buiding object used during test
            var sutXmlStubFactory = new Mock<ExecutionXml>();
            var itemXmlStubFactory = new Mock<QueryableXml>();
            itemXmlStubFactory.Setup(i => i.GetQuery()).Returns("query");
            sutXmlStubFactory.Setup(s => s.Item).Returns(itemXmlStubFactory.Object);
            var sutXml = sutXmlStubFactory.Object;
            sutXml.Item = itemXmlStubFactory.Object;

            var ctrXml = new SyntacticallyCorrectXml();

            var builder = new ExecutionSyntacticallyCorrectBuilder();
            builder.Setup(sutXml, ctrXml);
            //Call the method to test
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<IDbCommand>());
        }

        [Test]
        public void GetSystemUnderTest_Build_CorrectIDbCommandForSProc()
        {
            //Buiding object used during test
            var sutXmlStubFactory = new Mock<ExecutionXml>();
            var itemXmlStubFactory = new Mock<ReportXml>();
            itemXmlStubFactory.Setup(i => i.GetQuery()).Returns("query");
            itemXmlStubFactory.Setup(i => i.GetCommandType()).Returns(CommandType.StoredProcedure);
            sutXmlStubFactory.Setup(s => s.Item).Returns(itemXmlStubFactory.Object);
            sutXmlStubFactory.Setup(s => s.BaseItem).Returns(itemXmlStubFactory.Object);
            var sutXml = sutXmlStubFactory.Object;
            sutXml.Item = itemXmlStubFactory.Object;

            var ctrXml = new SyntacticallyCorrectXml();

            var builder = new ExecutionSyntacticallyCorrectBuilder();
            builder.Setup(sutXml, ctrXml);
            //Call the method to test
            builder.Build();
            var sut = builder.GetSystemUnderTest();

            //Assertion
            Assert.That(sut, Is.InstanceOf<IDbCommand>());
            Assert.That((sut as IDbCommand).CommandType, Is.EqualTo(CommandType.StoredProcedure));
        }

    }
}
