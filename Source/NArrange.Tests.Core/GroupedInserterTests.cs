namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the GroupedInserter class.
    /// </summary>
    [TestFixture]
    public class GroupedInserterTests
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a GroupedInserter instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            GroupBy groupBy = new GroupBy();

            GroupedInserter regionedInserter = new GroupedInserter(groupBy);
        }

        /// <summary>
        /// Test construction with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullTest()
        {
            GroupedInserter regionedInserter = new GroupedInserter(null);
        }

        /// <summary>
        /// Tests inserting groups in a sorted manner.
        /// </summary>
        [Test]
        public void GroupSortByAccessTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Access;
            groupBy.Direction = SortDirection.Ascending;

            GroupedInserter groupedInserter = new GroupedInserter(groupBy);

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            // Insert elements
            PropertyElement property1 = new PropertyElement();
            property1.Access = CodeAccess.Internal;
            property1.Name = "Property1";
            property1.Type = "string";
            groupedInserter.InsertElement(groupElement, property1);

            PropertyElement property2 = new PropertyElement();
            property2.Access = CodeAccess.Public;
            property2.Name = "Property2";
            property2.Type = "string";
            groupedInserter.InsertElement(groupElement, property2);

            PropertyElement property3 = new PropertyElement();
            property3.Access = CodeAccess.Protected | CodeAccess.Internal;
            property3.Name = "Property3";
            property3.Type = "string";
            groupedInserter.InsertElement(groupElement, property3);

            PropertyElement property4 = new PropertyElement();
            property4.Access = CodeAccess.Private;
            property4.Name = "Property4";
            property4.Type = "string";
            groupedInserter.InsertElement(groupElement, property4);

            PropertyElement property5 = new PropertyElement();
            property5.Access = CodeAccess.Public;
            property5.Name = "Property5";
            property5.Type = "string";
            groupedInserter.InsertElement(groupElement, property5);

            Assert.AreEqual(4, groupElement.Children.Count, "Unexpected number of child groups.");

            GroupElement childGroup;

            childGroup = groupElement.Children[0] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(1, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("Property4", childGroup.Children[0].Name);

            childGroup = groupElement.Children[1] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(1, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("Property1", childGroup.Children[0].Name);

            childGroup = groupElement.Children[2] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(1, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("Property3", childGroup.Children[0].Name);

            childGroup = groupElement.Children[3] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(2, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("Property2", childGroup.Children[0].Name);
            Assert.AreEqual("Property5", childGroup.Children[1].Name);
        }

        /// <summary>
        /// Tests inserting groups in a sorted manner.
        /// </summary>
        [Test]
        public void GroupSortByNameTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Name;
            groupBy.AttributeCapture = "^(.*?)(\\.|$)";
            groupBy.Direction = SortDirection.Ascending;

            GroupedInserter groupedInserter = new GroupedInserter(groupBy);

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            // Insert elements
            groupedInserter.InsertElement(groupElement, new UsingElement("NUnit.Framework"));
            groupedInserter.InsertElement(groupElement, new UsingElement("NArrange.Core"));
            groupedInserter.InsertElement(groupElement, new UsingElement("NArrange.Core.Configuration"));
            groupedInserter.InsertElement(groupElement, new UsingElement("System"));
            groupedInserter.InsertElement(groupElement, new UsingElement("System.IO"));

            Assert.AreEqual(3, groupElement.Children.Count, "Unexpected number of child groups.");

            GroupElement childGroup;

            // System usings should always come first
            childGroup = groupElement.Children[0] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(2, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("System", childGroup.Children[0].Name);
            Assert.AreEqual("System.IO", childGroup.Children[1].Name);

            childGroup = groupElement.Children[1] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(2, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("NArrange.Core", childGroup.Children[0].Name);
            Assert.AreEqual("NArrange.Core.Configuration", childGroup.Children[1].Name);

            childGroup = groupElement.Children[2] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(1, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("NUnit.Framework", childGroup.Children[0].Name);
        }

        /// <summary>
        /// Tests inserting elements.
        /// </summary>
        [Test]
        public void InsertSortedTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Name;
            groupBy.AttributeCapture = "^(.*?)(\\.|$)";

            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            SortedInserter sortedInserter = new SortedInserter(ElementType.Using, sortBy);

            GroupedInserter groupedInserter = new GroupedInserter(groupBy, sortedInserter);

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            //
            // With no criteria specified, elements should just be inserted
            // at the end of the collection.
            //
            UsingElement using1 = new UsingElement();
            using1.Name = "System.IO";
            groupedInserter.InsertElement(groupElement, using1);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(1, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element was not inserted at the correct index.");

            UsingElement using2 = new UsingElement();
            using2.Name = "System";
            groupedInserter.InsertElement(groupElement, using2);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(2, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index.");

            UsingElement using3 = new UsingElement();
            using3.Name = "System.Text";
            groupedInserter.InsertElement(groupElement, using3);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(3, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index[0].Children.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index.");
            Assert.AreEqual(2, groupElement.Children[0].Children.IndexOf(using3), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements.
        /// </summary>
        [Test]
        public void InsertTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Name;
            groupBy.AttributeCapture = "^(.*?)(\\.|$)";

            GroupedInserter groupedInserter = new GroupedInserter(groupBy);

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            //
            // With no criteria specified, elements should just be inserted
            // at the end of the collection.
            //
            UsingElement using1 = new UsingElement();
            using1.Name = "System.IO";
            groupedInserter.InsertElement(groupElement, using1);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(1, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element was not inserted at the correct index.");

            UsingElement using2 = new UsingElement();
            using2.Name = "System";
            groupedInserter.InsertElement(groupElement, using2);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(2, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index.");

            UsingElement using3 = new UsingElement();
            using3.Name = "System.Text";
            groupedInserter.InsertElement(groupElement, using3);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(3, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index[0].Children.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index.");
            Assert.AreEqual(2, groupElement.Children[0].Children.IndexOf(using3), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting nested groups in a sorted manner.
        /// </summary>
        [Test]
        public void NestedGroupSortTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Type;
            groupBy.Direction = SortDirection.Ascending;

            GroupBy innerGroupBy = new GroupBy();
            innerGroupBy.By = ElementAttributeType.Name;
            innerGroupBy.AttributeCapture = "^(.*?)(\\.|$)";
            innerGroupBy.Direction = SortDirection.Ascending;

            groupBy.InnerGroupBy = innerGroupBy;

            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            SortedInserter sortedInserter = new SortedInserter(ElementType.NotSpecified, sortBy);
            GroupedInserter groupedInserter = new GroupedInserter(
                groupBy,
                new GroupedInserter(groupBy.InnerGroupBy, sortedInserter));

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            // Insert elements
            groupedInserter.InsertElement(groupElement, new UsingElement("NUnit.Framework"));
            groupedInserter.InsertElement(groupElement, new UsingElement("NArrange.Core"));
            groupedInserter.InsertElement(groupElement, new UsingElement("NArrange.CSharp"));
            groupedInserter.InsertElement(groupElement, new UsingElement("NArrange.Core.Configuration"));
            groupedInserter.InsertElement(groupElement, new UsingElement("System"));
            groupedInserter.InsertElement(groupElement, new UsingElement("System.IO"));
            groupedInserter.InsertElement(groupElement, new UsingElement("MyClass2", "NArrange.Core.CodeArranger"));
            groupedInserter.InsertElement(groupElement, new UsingElement("MyClass1", "NArrange.Core.ElementFilter"));

            Assert.AreEqual(2, groupElement.Children.Count, "Unexpected number of child groups.");

            GroupElement childGroup;
            GroupElement grandchildGroup;

            // Namespace usings should come before alias usings
            childGroup = groupElement.Children[0] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(3, childGroup.Children.Count, "Unexpected number of group children.");

            // System usings should always come first
            grandchildGroup = childGroup.Children[0] as GroupElement;
            Assert.IsNotNull(grandchildGroup, "Expected a child group.");
            Assert.AreEqual(2, grandchildGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("System", grandchildGroup.Children[0].Name);
            Assert.AreEqual("System.IO", grandchildGroup.Children[1].Name);

            grandchildGroup = childGroup.Children[1] as GroupElement;
            Assert.IsNotNull(grandchildGroup, "Expected a child group.");
            Assert.AreEqual(3, grandchildGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("NArrange.Core", grandchildGroup.Children[0].Name);
            Assert.AreEqual("NArrange.Core.Configuration", grandchildGroup.Children[1].Name);
            Assert.AreEqual("NArrange.CSharp", grandchildGroup.Children[2].Name);

            grandchildGroup = childGroup.Children[2] as GroupElement;
            Assert.IsNotNull(grandchildGroup, "Expected a child group.");
            Assert.AreEqual(1, grandchildGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("NUnit.Framework", grandchildGroup.Children[0].Name);

            // Alias using directives
            childGroup = groupElement.Children[1] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(2, childGroup.Children.Count, "Unexpected number of group children.");
            grandchildGroup = childGroup.Children[0] as GroupElement;
            Assert.AreEqual(1, grandchildGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("MyClass1", grandchildGroup.Children[0].Name);
            grandchildGroup = childGroup.Children[1] as GroupElement;
            Assert.AreEqual(1, grandchildGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("MyClass2", grandchildGroup.Children[0].Name);
        }

        #endregion Methods
    }
}