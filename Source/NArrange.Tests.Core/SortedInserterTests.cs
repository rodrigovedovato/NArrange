namespace NArrange.Tests.Core
{
    using System;
    using System.ComponentModel;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the SortedInserter class.
    /// </summary>
    [TestFixture]
    public class SortedInserterTests
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a SortedInserter instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            SortBy sortBy = new SortBy();

            SortedInserter sortedInserter = new SortedInserter(ElementType.NotSpecified, sortBy);
        }

        /// <summary>
        /// Test construction with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullTest()
        {
            SortedInserter sortedInserter = new SortedInserter(ElementType.NotSpecified, null);
        }

        /// <summary>
        /// Tests inserting elements by access and name.
        /// </summary>
        [Test]
        public void InsertByAccessAndNameTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Access;
            sortBy.Direction = SortDirection.Ascending;

            SortBy innerSortBy = new SortBy();
            innerSortBy.By = ElementAttributeType.Name;
            innerSortBy.Direction = SortDirection.Ascending;

            sortBy.InnerSortBy = innerSortBy;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert elements with middle access.
            //
            FieldElement field1 = new FieldElement();
            field1.Access = CodeAccess.Protected | CodeAccess.Internal;
            field1.Name = "newField";
            sortedInserter.InsertElement(regionElement, field1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            FieldElement field2 = new FieldElement();
            field2.Access = CodeAccess.Protected | CodeAccess.Internal;
            field2.Name = "gooField";
            sortedInserter.InsertElement(regionElement, field2);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            FieldElement field3 = new FieldElement();
            field3.Access = CodeAccess.Public;
            field3.Name = "zooField";
            sortedInserter.InsertElement(regionElement, field3);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");

            FieldElement field4 = new FieldElement();
            field4.Access = CodeAccess.Public;
            field4.Name = "tooField";
            sortedInserter.InsertElement(regionElement, field4);
            Assert.AreEqual(4, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field4), "Element is not at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            FieldElement field5 = new FieldElement();
            field5.Access = CodeAccess.Private;
            field5.Name = "booField";
            sortedInserter.InsertElement(regionElement, field5);
            Assert.AreEqual(5, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field5), "Element was not inserted at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(field4), "Element is not at the correct index.");
            Assert.AreEqual(4, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");

            FieldElement field6 = new FieldElement();
            field6.Access = CodeAccess.Private;
            field6.Name = "fooField";
            sortedInserter.InsertElement(regionElement, field6);
            Assert.AreEqual(6, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field5), "Element was not inserted at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field6), "Element was not inserted at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");
            Assert.AreEqual(4, regionElement.Children.IndexOf(field4), "Element is not at the correct index.");
            Assert.AreEqual(5, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements by access and name.
        /// </summary>
        [Test]
        public void InsertByAccessDescendingAndNameTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Access;
            sortBy.Direction = SortDirection.Descending;

            SortBy innerSortBy = new SortBy();
            innerSortBy.By = ElementAttributeType.Name;
            innerSortBy.Direction = SortDirection.Ascending;

            sortBy.InnerSortBy = innerSortBy;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert elements with middle access.
            //
            FieldElement field1 = new FieldElement();
            field1.Access = CodeAccess.Protected | CodeAccess.Internal;
            field1.Name = "newField";
            sortedInserter.InsertElement(regionElement, field1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            FieldElement field2 = new FieldElement();
            field2.Access = CodeAccess.Protected | CodeAccess.Internal;
            field2.Name = "gooField";
            sortedInserter.InsertElement(regionElement, field2);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            FieldElement field3 = new FieldElement();
            field3.Access = CodeAccess.Public;
            field3.Name = "zooField";
            sortedInserter.InsertElement(regionElement, field3);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            FieldElement field4 = new FieldElement();
            field4.Access = CodeAccess.Public;
            field4.Name = "tooField";
            sortedInserter.InsertElement(regionElement, field4);
            Assert.AreEqual(4, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field4), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            FieldElement field5 = new FieldElement();
            field5.Access = CodeAccess.Private;
            field5.Name = "booField";
            sortedInserter.InsertElement(regionElement, field5);
            Assert.AreEqual(5, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field4), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");
            Assert.AreEqual(4, regionElement.Children.IndexOf(field5), "Element was not inserted at the correct index.");

            FieldElement field6 = new FieldElement();
            field6.Access = CodeAccess.Private;
            field6.Name = "fooField";
            sortedInserter.InsertElement(regionElement, field6);
            Assert.AreEqual(6, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field4), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field2), "Element was not inserted at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");
            Assert.AreEqual(4, regionElement.Children.IndexOf(field5), "Element was not inserted at the correct index.");
            Assert.AreEqual(5, regionElement.Children.IndexOf(field6), "Element was not inserted at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements by access.
        /// </summary>
        [Test]
        public void InsertByAccessTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Access;
            sortBy.Direction = SortDirection.Ascending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert an element with a middle access.
            //
            FieldElement field1 = new FieldElement();
            field1.Access = CodeAccess.Protected | CodeAccess.Internal;
            sortedInserter.InsertElement(regionElement, field1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            FieldElement field2 = new FieldElement();
            field2.Access = CodeAccess.Public;
            sortedInserter.InsertElement(regionElement, field2);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            FieldElement field3 = new FieldElement();
            field3.Access = CodeAccess.Private;
            sortedInserter.InsertElement(regionElement, field3);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements by element type then by name.
        /// </summary>
        [Test]
        public void InsertByElementTypeAndNameTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.ElementType;
            sortBy.Direction = SortDirection.Ascending;

            sortBy.InnerSortBy = new SortBy();
            sortBy.InnerSortBy.By = ElementAttributeType.Name;
            sortBy.InnerSortBy.Direction = SortDirection.Ascending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.NotSpecified, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert an element with a middle access.
            //
            ConstructorElement constructor = new ConstructorElement();
            constructor.Name = "SomeClass";
            constructor.Access = CodeAccess.Public;
            sortedInserter.InsertElement(regionElement, constructor);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(constructor), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            MethodElement methodElement1 = new MethodElement();
            methodElement1.Name = "SomeMethod";
            methodElement1.Access = CodeAccess.Public;
            sortedInserter.InsertElement(regionElement, methodElement1);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(constructor), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(methodElement1), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "someField";
            fieldElement.Access = CodeAccess.Private;
            sortedInserter.InsertElement(regionElement, fieldElement);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(fieldElement), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(constructor), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(methodElement1), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted by name
            //
            MethodElement methodElement2 = new MethodElement();
            methodElement1.Name = "AnotherMethod";
            methodElement1.Access = CodeAccess.Public;
            sortedInserter.InsertElement(regionElement, methodElement2);
            Assert.AreEqual(4, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(fieldElement), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(constructor), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(methodElement2), "Element is not at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(methodElement1), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements by element type.
        /// </summary>
        [Test]
        public void InsertByElementTypeTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.ElementType;
            sortBy.Direction = SortDirection.Ascending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.NotSpecified, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert an element with a middle access.
            //
            ConstructorElement constructor = new ConstructorElement();
            constructor.Name = "SomeClass";
            constructor.Access = CodeAccess.Public;
            sortedInserter.InsertElement(regionElement, constructor);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(constructor), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            MethodElement methodElement = new MethodElement();
            methodElement.Name = "SomeMethod";
            methodElement.Access = CodeAccess.Public;
            sortedInserter.InsertElement(regionElement, methodElement);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(constructor), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(methodElement), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "someField";
            fieldElement.Access = CodeAccess.Private;
            sortedInserter.InsertElement(regionElement, fieldElement);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(fieldElement), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(constructor), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(methodElement), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements by name in descending order.
        /// </summary>
        [Test]
        public void InsertByNameDescendingTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            sortBy.Direction = SortDirection.Descending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert an element with a mid alphabet name.
            //
            FieldElement field1 = new FieldElement();
            field1.Name = "newField";
            sortedInserter.InsertElement(regionElement, field1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            FieldElement field2 = new FieldElement();
            field2.Name = "zooField";
            sortedInserter.InsertElement(regionElement, field2);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            FieldElement field3 = new FieldElement();
            field3.Name = "booField";
            sortedInserter.InsertElement(regionElement, field3);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements by name.
        /// </summary>
        [Test]
        public void InsertByNameTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            sortBy.Direction = SortDirection.Ascending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert an element with a mid alphabet name.
            //
            FieldElement field1 = new FieldElement();
            field1.Name = "newField";
            sortedInserter.InsertElement(regionElement, field1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            FieldElement field2 = new FieldElement();
            field2.Name = "zooField";
            sortedInserter.InsertElement(regionElement, field2);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            FieldElement field3 = new FieldElement();
            field3.Name = "booField";
            sortedInserter.InsertElement(regionElement, field3);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements without any criteria.
        /// </summary>
        [Test]
        public void InsertByNoneTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.None;
            sortBy.Direction = SortDirection.Ascending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // With no criteria specified, elements should just be inserted
            // at the end of the collection.
            //
            FieldElement field1 = new FieldElement();
            field1.Name = "zooField";
            sortedInserter.InsertElement(regionElement, field1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            FieldElement field2 = new FieldElement();
            field1.Name = "newField";
            sortedInserter.InsertElement(regionElement, field2);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");

            FieldElement field3 = new FieldElement();
            field1.Name = "booField";
            sortedInserter.InsertElement(regionElement, field3);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(field2), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(field3), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting type elements by Type.
        /// </summary>
        [Test]
        public void InsertByTypeElementTypeDescendingTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Type;
            sortBy.Direction = SortDirection.Descending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Type, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert an element with a mid value.
            //
            TypeElement type1 = new TypeElement();
            type1.Name = "Type1";
            type1.Type = TypeElementType.Structure;
            sortedInserter.InsertElement(regionElement, type1);

            //
            // Insert an element that should be sorted toward the end
            //
            TypeElement type2 = new TypeElement();
            type2.Name = "Type2";
            type2.Type = TypeElementType.Class;
            sortedInserter.InsertElement(regionElement, type2);

            //
            // Insert an element that should be sorted toward the middle
            //
            TypeElement type3 = new TypeElement();
            type3.Name = "Type3";
            type3.Type = TypeElementType.Interface;
            sortedInserter.InsertElement(regionElement, type3);

            //
            // Insert an element that should be sorted toward the beginning
            //
            TypeElement type4 = new TypeElement();
            type4.Name = "Type4";
            type4.Type = TypeElementType.Enum;
            sortedInserter.InsertElement(regionElement, type4);

            Assert.AreEqual(4, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(type4), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(type3), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(type1), "Element is not at the correct index.");
            Assert.AreEqual(3, regionElement.Children.IndexOf(type2), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements by type.
        /// </summary>
        [Test]
        public void InsertByTypeTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Type;
            sortBy.Direction = SortDirection.Ascending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Method, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert an element with a mid alphabet return type.
            //
            MethodElement method1 = new MethodElement();
            method1.Name = "DoSomething";
            method1.Type = "Nullable<DateTime>";
            sortedInserter.InsertElement(regionElement, method1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(method1), "Element was not inserted at the correct index.");

            //
            // Insert an element that should be sorted toward the end
            //
            MethodElement method2 = new MethodElement();
            method2.Name = "DoSomething";
            method2.Type = "Type";
            sortedInserter.InsertElement(regionElement, method2);
            Assert.AreEqual(2, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(method1), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(method2), "Element is not at the correct index.");

            //
            // Insert an element that should be sorted toward the beginning
            //
            MethodElement method3 = new MethodElement();
            method3.Name = "DoSomething";
            method3.Type = "IEnumerable";
            sortedInserter.InsertElement(regionElement, method3);
            Assert.AreEqual(3, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(method3), "Element is not at the correct index.");
            Assert.AreEqual(1, regionElement.Children.IndexOf(method1), "Element is not at the correct index.");
            Assert.AreEqual(2, regionElement.Children.IndexOf(method2), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting a null element.
        /// </summary>
        [Test]
        public void InsertNullTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            sortBy.Direction = SortDirection.Ascending;

            SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

            //
            // Create a parent element
            //
            RegionElement regionElement = new RegionElement();
            Assert.AreEqual(0, regionElement.Children.Count, "Parent element should not have any children.");

            //
            // Insert a non-null element
            //
            FieldElement field1 = new FieldElement();
            field1.Name = "newField";
            sortedInserter.InsertElement(regionElement, field1);
            Assert.AreEqual(1, regionElement.Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element was not inserted at the correct index.");

            //
            // Insert a null element
            //
            FieldElement field2 = null;
            sortedInserter.InsertElement(regionElement, field2);
            Assert.AreEqual(1, regionElement.Children.Count, "Element should not have been inserted into the parent.");
            Assert.AreEqual(0, regionElement.Children.IndexOf(field1), "Element is not at the correct index.");
        }

        #endregion Methods
    }
}