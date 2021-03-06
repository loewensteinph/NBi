﻿using NBi.Service;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Service
{
    public class TestCaseCollectionManagerTest
    {
        private void Load(DataTable table, string[] rows, string columnNames)
        {
            var columns = columnNames.Split(',');
            for (int i = 0; i < columns.Length; i++)
                table.Columns.Add(new DataColumn(columns[i]));

            foreach (var row in rows)
            {
                var newRow = table.NewRow();
                newRow.ItemArray = row.Split(',');
                table.Rows.Add(newRow);
            }

            table.AcceptChanges();
        }
        
        [Test]
        public void Item_EmptyCollection_ElementCreated()
        {
            var manager = new TestCaseCollectionManager();
            var tc = manager.Item("alpha");
            Assert.That(tc, Is.Not.Null);
            Assert.That(tc, Is.TypeOf<TestCaseManager>());
        }

        [Test]
        public void Item_EmptyCollectionWithNoName_ElementCreated()
        {
            var manager = new TestCaseCollectionManager();
            var tc = manager.Item(string.Empty);
            Assert.That(tc, Is.Not.Null);
            Assert.That(tc, Is.TypeOf<TestCaseManager>());
        }

        [Test]
        public void Item_GetTwice_SameElement()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            var tc2 = manager.Item("alpha");

            Assert.That(tc1, Is.EqualTo(tc2));
        }

        [Test]
        public void Item_GetTwoDistinct_DifferentElements()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            var tc2 = manager.Item("beta");

            Assert.That(tc1, Is.Not.EqualTo(tc2));
        }

        [Test]
        public void Item_NullName_DifferentElements()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item(null);

            Assert.That(tc1, Is.Not.Null);
        }

        [Test]
        public void Focus_TwoElementsCreatedSetFocusNeverCalled_FirstElement()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            var tc2 = manager.Item("beta");

            var focus = manager.Scope;

            Assert.That(focus, Is.EqualTo(tc1));
        }

        [Test]
        public void Focus_TwoElementsCreatedSetFocusCalledForSecond_SecondElement()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            var tc2 = manager.Item("beta");

            manager.SetFocus("beta");
            var focus = manager.Scope;

            Assert.That(focus, Is.EqualTo(tc2));
        }

        [Test]
        public void Cross_ThreeTimesTwo_SixRowsFiveColumns()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            Load(tc1.Content, new string[] { "a11,a12", "a21,a22", "a31,a32" }, "alpha1,alpha2");
            var tc2 = manager.Item("beta");
            Load(tc2.Content, new string[] { "b11,b12,b13", "b21,b22,b23" }, "beta1,beta2,beta3");

            manager.SetFocus("gamma");
            manager.Cross("alpha", "beta");

            var focus = manager.Scope;

            Assert.That(focus.Content.Rows, Has.Count.EqualTo(6));
            Assert.That(focus.Content.Columns, Has.Count.EqualTo(5));
        }

        [Test]
        public void Cross_ThreeTimesTwoOnItself_SixRowsFiveColumns()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            Load(tc1.Content, new string[] { "a11,a12", "a21,a22", "a31,a32" }, "alpha1,alpha2");
            var tc2 = manager.Item("beta");
            Load(tc2.Content, new string[] { "b11,b12,b13", "b21,b22,b23" }, "beta1,beta2,beta3");

            manager.Cross("alpha", "beta");
            //The focus hasn't moved ... so still on tc1
            Assert.That(tc1.Content.Rows, Has.Count.EqualTo(6));
            Assert.That(tc1.Content.Columns, Has.Count.EqualTo(5));
        }

        [Test]
        public void Cross_ThreeTimesTwoWithOneCommonColumnName_SixRowsFourColumns()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            Load(tc1.Content, new string[] { "a11,a12", "a21,a22", "a31,a32" }, "alpha1,alpha2");
            var tc2 = manager.Item("beta");
            Load(tc2.Content, new string[] { "b11,b12,b13", "b21,b22,b23" }, "alpha1,beta2,beta3");

            manager.SetFocus("gamma");
            manager.Cross("alpha", "beta");

            var focus = manager.Scope;

            Assert.That(focus.Content.Rows, Has.Count.EqualTo(6));
            Assert.That(focus.Content.Columns, Has.Count.EqualTo(4));
        }

        [Test]
        public void Cross_ThreeTimesTwoOnMatchingColumn_FourRowsFourColumns()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            Load(tc1.Content, new string[] { "a11,a12", "a21,a22", "a31,a32" }, "alpha1,alpha2");
            var tc2 = manager.Item("beta");
            Load(tc2.Content, new string[] { "a11,b12,b13", "a21,b22,b23", "a21,b32,b33", "a21,b42,b43", "a41,b52,b53" }, "alpha1,beta2,beta3");

            manager.SetFocus("gamma");
            manager.Cross("alpha", "beta", "alpha1");

            var focus = manager.Scope;

            Assert.That(focus.Content.Rows, Has.Count.EqualTo(4));
            Assert.That(focus.Content.Columns, Has.Count.EqualTo(4));
        }

        [Test]
        public void Cross_ThreeTimesTwoOnMatchingColumnWithoutPrimaryKey_FiveRowsFourColumns()
        {
            var manager = new TestCaseCollectionManager();
            var tc1 = manager.Item("alpha");
            Load(tc1.Content, new string[] { "a11,a12", "a11,a22", "a21,a32" }, "alpha1,alpha2");
            var tc2 = manager.Item("beta");
            Load(tc2.Content, new string[] { "a11,b12,b13", "a21,b22,b23", "a21,b32,b33", "a21,b42,b43", "a41,b52,b53" }, "alpha1,beta2,beta3");

            manager.SetFocus("gamma");
            manager.Cross("alpha", "beta", "alpha1");

            var focus = manager.Scope;

            Assert.That(focus.Content.Rows, Has.Count.EqualTo(5));
            Assert.That(focus.Content.Columns, Has.Count.EqualTo(4));
        }

        [Test]
        public void Copy_SimpleMaster_CopyIsEffectivelyDone()
        {
            var manager = new TestCaseCollectionManager();
            var master = manager.Item("master");
            Load(master.Content, new string[] { "a11,a12", "a11,a22", "a21,a32" }, "alpha1,alpha2");
           
            manager.Copy("master", "copied");
            var copied = manager.Item("copied");

            for (int i = 0; i < master.Content.Rows.Count; i++)
                Assert.That(copied.Content.Rows[i].ItemArray, Is.EqualTo(master.Content.Rows[i].ItemArray)); 
            
            Assert.That(copied.Content.Rows, Has.Count.EqualTo(master.Content.Rows.Count));
        }

        [Test]
        public void Copy_SimpleMaster_CopyIsNotReferenceCopy()
        {
            var manager = new TestCaseCollectionManager();
            var master = manager.Item("master");
            Load(master.Content, new string[] { "a11,a12", "a11,a22", "a21,a32" }, "alpha1,alpha2");

            manager.Copy("master", "copied");
            var copied = manager.Item("copied");
            manager.Item("master").Content.Clear();

            Assert.That(master.Content.Rows, Has.Count.EqualTo(0));
            Assert.That(copied.Content.Rows, Has.Count.GreaterThan(0));
        }

        [Test]
        public void Copy_SimpleMasterWithCopiedAlreadyLoaded_CopyIsNotAllowed()
        {
            var manager = new TestCaseCollectionManager();
            var master = manager.Item("master");
            Load(master.Content, new string[] { "a11,a12", "a11,a22", "a21,a32" }, "alpha1,alpha2");
            
            var copied = manager.Item("copied");
            Load(copied.Content, new string[] { "b11,b12", "b11,b22" }, "beta1,beta2");

            Assert.Throws<ArgumentException>(delegate { manager.Copy("master", "copied"); });
        }
    }
}
