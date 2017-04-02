using System;
using System.Linq;
using NBi.Core.ResultSet.Comparer;
using NUnit.Framework;

namespace NBi.Testing.Unit.Core.ResultSet.Comparer
{
    [TestFixture]
    public class ArrayComparerTest
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
        public void Compare_SameArray_True()
        {
            var comparer = new ArrayComparer();
            var x = new object[] { "alpha", "beta" };
            var result = comparer.Compare(x, x, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.True);
        }

        [Test]
        public void Compare_IdenticalArray_True()
        {
            var comparer = new ArrayComparer();
            var x = new object[] { "alpha", "beta" };
            var y = new object[] { "alpha", "beta" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.True);
        }

        [Test]
        public void Compare_IdenticalArrayChangedOrder_True()
        {
            var comparer = new ArrayComparer();
            var x = new object[] { "alpha", "beta" };
            var y = new object[] { "beta", "alpha" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.True);
        }

        [Test]
        [Ignore("(value) is not supported at the moment in an array")]
        public void Compare_IdenticalArrayWithValue_True()
        {
            var comparer = new ArrayComparer();
            var x = new object[] { "epsilon", "alpha", "delta", "beta", "gamma" };
            var y = new object[] { "beta", "gamma", "(value)", "delta", "epsilon" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.True);
        }

        [Test]
        public void Compare_DifferentArray_False()
        {
            var comparer = new ArrayComparer();
            var x = new object[] { "alpha", "beta" };
            var y = new object[] { "alpha", "gamma" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.False);
        }


        [Test]
        public void Compare_DifferentLengthArray_False()
        {
            var comparer = new ArrayComparer();
            var x = new object[] { "alpha", "beta" };
            var y = new object[] { "alpha", "beta", "gamma" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.False);
        }


        [Test]
        public void Compare_IdenticalNumericArray_True()
        {
            var comparer = new ArrayComparer();
            var x = new object[] { "1.120", "2.35" };
            var y = new object[] { 1.12, 2.35 };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Numeric);
            Assert.That(result.AreEqual, Is.True);
        }

        [Test]
        public void Compare_AnyArray_True()
        {
            var comparer = new ArrayComparer();
            var x = "(any)";
            var y = new object[] { "alpha", "beta" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.True);
        }

        [Test]
        public void Compare_ValueArray_True()
        {
            var comparer = new ArrayComparer();
            var x = "(value)";
            var y = new object[] { "alpha", "beta" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.True);
        }

        [Test]
        public void Compare_NullArray_False()
        {
            var comparer = new ArrayComparer();
            var x = DBNull.Value;
            var y = new object[] { "alpha", "beta" };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.False);
        }

        [Test]
        public void Compare_NullEmptyArray_True()
        {
            var comparer = new ArrayComparer();
            var x = DBNull.Value;
            var y = new object[] { };
            var result = comparer.Compare(x, y, NBi.Core.ResultSet.ColumnType.Text);
            Assert.That(result.AreEqual, Is.True);
        }

    }
}
