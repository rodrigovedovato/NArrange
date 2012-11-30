namespace NArrange.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;
    using NArrange.CSharp;
    using NArrange.Tests.CSharp;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the CodeArranger class.
    /// </summary>
    [TestFixture]
    public class CodeArrangerTests
    {
        #region Fields

        /// <summary>
        /// Set of test elements for arranging.
        /// </summary>
        private ReadOnlyCollection<ICodeElement> _testElements;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Tests arrangement with nested regions.
        /// </summary>
        [Test]
        public void ArrangeNestedRegionTest()
        {
            List<ICodeElement> elements = new List<ICodeElement>();

            TypeElement type = new TypeElement();
            type.Type = TypeElementType.Class;
            type.Name = "TestClass";

            FieldElement field = new FieldElement();
            field.Name = "val";
            field.Type = "int";

            type.AddChild(field);
            elements.Add(type);

            // Create a configuration with a nested region
            CodeConfiguration codeConfiguration = new CodeConfiguration();

            ElementConfiguration typeConfiguration = new ElementConfiguration();
            typeConfiguration.ElementType = ElementType.Type;

            RegionConfiguration regionConfiguration1 = new RegionConfiguration();
            regionConfiguration1.Name = "Region1";

            RegionConfiguration regionConfiguration2 = new RegionConfiguration();
            regionConfiguration2.Name = "Region2";

            ElementConfiguration fieldConfiguration = new ElementConfiguration();
            fieldConfiguration.ElementType = ElementType.Field;

            regionConfiguration2.Elements.Add(fieldConfiguration);
            regionConfiguration1.Elements.Add(regionConfiguration2);
            typeConfiguration.Elements.Add(regionConfiguration1);
            codeConfiguration.Elements.Add(typeConfiguration);

            CodeArranger arranger = new CodeArranger(codeConfiguration);

            ReadOnlyCollection<ICodeElement> arrangedElements = arranger.Arrange(elements.AsReadOnly());

            Assert.AreEqual(1, arrangedElements.Count, "Unexpected number of arranged elements.");

            TypeElement arrangedType = arrangedElements[0] as TypeElement;
            Assert.IsNotNull(arrangedType, "Expected a type element after arranging.");
            Assert.AreEqual(1, arrangedType.Children.Count, "Unexpected number of arranged child elements.");

            RegionElement arrangedRegion1 = arrangedType.Children[0] as RegionElement;
            Assert.IsNotNull(arrangedRegion1, "Expected a region element after arranging.");
            Assert.AreEqual(regionConfiguration1.Name, arrangedRegion1.Name);
            Assert.AreEqual(1, arrangedRegion1.Children.Count, "Unexpected number of arranged child elements.");

            RegionElement arrangedRegion2 = arrangedRegion1.Children[0] as RegionElement;
            Assert.IsNotNull(arrangedRegion2, "Expected a region element after arranging.");
            Assert.AreEqual(regionConfiguration2.Name, arrangedRegion2.Name);
            Assert.AreEqual(1, arrangedRegion2.Children.Count, "Unexpected number of arranged child elements.");

            FieldElement arrangedFieldElement = arrangedRegion2.Children[0] as FieldElement;
            Assert.IsNotNull(arrangedFieldElement, "Expected a field element after arranging.");
        }

        /// <summary>
        /// Tests arranging static fields with and without dependencies.
        /// </summary>
        [Test]
        public void ArrangeStaticFieldsTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            TypeElement classElement = new TypeElement();
            classElement.Type = TypeElementType.Class;
            classElement.Access = CodeAccess.Public;
            classElement.Name = "TestClass";

            FieldElement fieldElement1 = new FieldElement();
            fieldElement1.MemberModifiers = MemberModifiers.Static;
            fieldElement1.Access = CodeAccess.Protected;
            fieldElement1.Type = "object";
            fieldElement1.Name = "_obj";
            fieldElement1.InitialValue = "typeof(int).ToString();";
            classElement.AddChild(fieldElement1);

            // This field has a static dependency.  Normally it would be sorted first
            // due to its access, but we want to make sure it gets added after the
            // field for which it is dependent.
            FieldElement fieldElement2 = new FieldElement();
            fieldElement2.MemberModifiers = MemberModifiers.Static;
            fieldElement2.Access = CodeAccess.Public;
            fieldElement2.Type = "bool";
            fieldElement2.Name = "Initialized";
            fieldElement2.InitialValue = "_initializationString != null";
            classElement.AddChild(fieldElement2);

            FieldElement fieldElement3 = new FieldElement();
            fieldElement3.MemberModifiers = MemberModifiers.Static;
            fieldElement3.Access = CodeAccess.Private;
            fieldElement3.Type = "string";
            fieldElement3.Name = "_initializationString";
            fieldElement3.InitialValue = "_obj";
            classElement.AddChild(fieldElement3);

            codeElements.Add(classElement);

            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");
            TypeElement typeElement = arranged[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Expected a type element.");

            List<FieldElement> staticFields = new List<FieldElement>();
            Action<ICodeElement> findStaticFields = delegate(ICodeElement codeElement)
            {
                FieldElement fieldElement = codeElement as FieldElement;
                if (fieldElement != null && fieldElement.MemberModifiers == MemberModifiers.Static)
                {
                    staticFields.Add(fieldElement);
                }
            };

            ElementUtilities.ProcessElementTree(typeElement, findStaticFields);

            Assert.AreEqual(3, staticFields.Count, "Unexpected number of static fields after arranging.");
            Assert.AreEqual("_obj", staticFields[0].Name);
            Assert.AreEqual("_initializationString", staticFields[1].Name);
            Assert.AreEqual("Initialized", staticFields[2].Name);

            //
            // Remove the dependency
            //
            fieldElement2.InitialValue = "true";
            fieldElement3.InitialValue = "\"test\"";

            arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");
            typeElement = arranged[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Expected a type element.");

            staticFields.Clear();
            ElementUtilities.ProcessElementTree(typeElement, findStaticFields);

            Assert.AreEqual(3, staticFields.Count, "Unexpected number of static fields after arranging.");
            Assert.AreEqual("Initialized", staticFields[0].Name);
            Assert.AreEqual("_obj", staticFields[1].Name);
            Assert.AreEqual("_initializationString", staticFields[2].Name);
        }

        /// <summary>
        /// Test the construction with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullTest()
        {
            CodeArranger arranger = new CodeArranger(null);
        }

        /// <summary>
        /// Tests arranging a condition directive.
        /// </summary>
        [Test]
        public void DefaultArrangeConditionDirectiveTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            ConditionDirectiveElement ifCondition = new ConditionDirectiveElement();
            ifCondition.ConditionExpression = "DEBUG";

            FieldElement field1 = new FieldElement();
            field1.Name = "zField";
            field1.Type = "int";

            FieldElement field2 = new FieldElement();
            field2.Name = "aField";
            field2.Type = "int";

            ifCondition.AddChild(field1);
            ifCondition.AddChild(field2);

            ifCondition.ElseCondition = new ConditionDirectiveElement();

            FieldElement field3 = new FieldElement();
            field3.Name = "testField";
            field3.Type = "int";

            FieldElement field1Clone = field1.Clone() as FieldElement;
            FieldElement field2Clone = field2.Clone() as FieldElement;

            TypeElement classElement = new TypeElement();
            classElement.Name = "TestClass";
            classElement.AddChild(field1Clone);
            classElement.AddChild(field2Clone);

            ifCondition.ElseCondition.AddChild(field3);
            ifCondition.ElseCondition.AddChild(classElement);

            codeElements.Add(ifCondition);

            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            ReadOnlyCollection<ICodeElement> arranged =
                arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");
            ConditionDirectiveElement ifConditionTest = arranged[0] as ConditionDirectiveElement;
            Assert.IsNotNull(ifConditionTest, "Expected a condition directive element.");

            Assert.AreEqual(2, ifConditionTest.Children.Count, "After arranging, an unexpected number of nested elements were returned.");
            Assert.AreEqual(field2.Name, ifConditionTest.Children[0].Name);
            Assert.AreEqual(field1.Name, ifConditionTest.Children[1].Name);

            ConditionDirectiveElement elseConditionTest = ifConditionTest.ElseCondition;
            Assert.IsNotNull(elseConditionTest, "Expected a condition directive element.");
            Assert.AreEqual(2, ifConditionTest.Children.Count, "After arranging, an unexpected number of nested elements were returned.");
            Assert.AreEqual(field3.Name, elseConditionTest.Children[0].Name);
            Assert.AreEqual(classElement.Name, elseConditionTest.Children[1].Name);

            TypeElement classElementTest = elseConditionTest.Children[1] as TypeElement;
            Assert.IsNotNull(classElementTest, "Expected a type element.");
            Assert.AreEqual(1, classElementTest.Children.Count);
            Assert.AreEqual("Fields", classElementTest.Children[0].Name);
        }

        /// <summary>
        /// Tests arranging an enumeration.
        /// </summary>
        [Test]
        public void DefaultArrangeEnumerationTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            UsingElement usingElement = new UsingElement();
            usingElement.Name = "System";

            TypeElement enumElement = new TypeElement();
            enumElement.Type = TypeElementType.Enum;
            enumElement.Access = CodeAccess.Public;
            enumElement.Name = "TestEnum";
            enumElement.BodyText = "Value1 = 1,\r\nValue2 = 2";

            NamespaceElement namespaceElement = new NamespaceElement();
            namespaceElement.Name = "TestNamespace";
            namespaceElement.AddChild(usingElement);
            namespaceElement.AddChild(enumElement);

            codeElements.Add(namespaceElement);

            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            ReadOnlyCollection<ICodeElement> arranged =
                arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");
            NamespaceElement namespaceElementTest = arranged[0] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");

            Assert.AreEqual(2, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            Assert.AreEqual(ElementType.Using, namespaceElement.Children[0].ElementType);

            RegionElement regionElement = namespaceElementTest.Children[1] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element.");
            Assert.AreEqual("Enumerations", regionElement.Name, "Unexpected region name.");

            Assert.AreEqual(1, regionElement.Children.Count, "After arranging, an unexpected number of region elements were returned.");
            TypeElement typeElement = regionElement.Children[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Expected a type element.");

            Assert.AreEqual(TypeElementType.Enum, typeElement.Type, "Unexpected type element type.");
            Assert.AreEqual(enumElement.Name, typeElement.Name, "Unexpected type element name.");
        }

        /// <summary>
        /// Tests arranging a nested class.
        /// </summary>
        [Test]
        public void DefaultArrangeNestedClassTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            TypeElement parentClassElement = new TypeElement();
            parentClassElement.Type = TypeElementType.Class;
            parentClassElement.Access = CodeAccess.Public;
            parentClassElement.Name = "ParentClass";

            TypeElement classElement = new TypeElement();
            classElement.Type = TypeElementType.Class;
            classElement.Access = CodeAccess.Private;
            classElement.Name = "NestedClass";
            parentClassElement.AddChild(classElement);

            NamespaceElement namespaceElement = new NamespaceElement();
            namespaceElement.Name = "TestNamespace";
            namespaceElement.AddChild(parentClassElement);

            MethodElement methodElement = new MethodElement();
            methodElement.Type = "void";
            methodElement.Access = CodeAccess.Public;
            methodElement.Name = "DoSomething";
            classElement.AddChild(methodElement);

            FieldElement fieldElement = new FieldElement();
            fieldElement.Type = "bool";
            fieldElement.Access = CodeAccess.Private;
            fieldElement.Name = "_val";
            classElement.AddChild(fieldElement);

            PropertyElement propertyElement = new PropertyElement();
            propertyElement.Type = "bool";
            propertyElement.Access = CodeAccess.Public;
            propertyElement.Name = "Value";
            propertyElement.BodyText = "return _val";
            classElement.AddChild(propertyElement);

            codeElements.Add(namespaceElement);

            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            ReadOnlyCollection<ICodeElement> arranged =
                arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");
            NamespaceElement namespaceElementTest = arranged[0] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");

            Assert.AreEqual(1, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            RegionElement typeRegionElement = namespaceElementTest.Children[0] as RegionElement;
            Assert.IsNotNull(typeRegionElement, "Expected a region element.");
            Assert.AreEqual("Types", typeRegionElement.Name);

            Assert.AreEqual(1, typeRegionElement.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            TypeElement parentTypeElement = typeRegionElement.Children[0] as TypeElement;
            Assert.IsNotNull(parentTypeElement, "Expected a type element.");

            Assert.AreEqual(TypeElementType.Class, parentTypeElement.Type, "Unexpected type element type.");
            Assert.AreEqual(parentClassElement.Name, parentTypeElement.Name, "Unexpected type element name.");

            Assert.AreEqual(1, parentTypeElement.Children.Count, "After arranging, an unexpected number of parent class elements were returned.");
            RegionElement nestedTypeRegionElement = parentTypeElement.Children[0] as RegionElement;
            Assert.IsNotNull(nestedTypeRegionElement, "Expected a region element.");
            Assert.AreEqual("Nested Types", nestedTypeRegionElement.Name, "Unexpected region name.");

            Assert.AreEqual(1, nestedTypeRegionElement.Children.Count, "After arranging, an unexpected number of parent class region elements were returned.");
            TypeElement nestedTypeElement = nestedTypeRegionElement.Children[0] as TypeElement;
            Assert.IsNotNull(nestedTypeElement, "Expected a type element.");

            Assert.AreEqual(TypeElementType.Class, nestedTypeElement.Type, "Unexpected type element type.");
            Assert.AreEqual(classElement.Name, nestedTypeElement.Name, "Unexpected type element name.");

            Assert.AreEqual(3, nestedTypeElement.Children.Count, "An unexpected number of class child elements were returned.");
            List<RegionElement> nestedRegionElements = new List<RegionElement>();
            foreach (ICodeElement classChildElement in nestedTypeElement.Children)
            {
                RegionElement nestedRegionElement = classChildElement as RegionElement;
                Assert.IsNotNull(
                    nestedRegionElement,
                    "Expected a region element but was {0}.",
                    classChildElement.ElementType);
                nestedRegionElements.Add(nestedRegionElement);
            }

            Assert.AreEqual("Fields", nestedRegionElements[0].Name, "Unexpected region element name.");
            Assert.AreEqual("Properties", nestedRegionElements[1].Name, "Unexpected region element name.");
            Assert.AreEqual("Methods", nestedRegionElements[2].Name, "Unexpected region element name.");

            GroupElement fieldGroupElement = nestedRegionElements[0].Children[0].Children[0] as GroupElement;
            Assert.IsNotNull(fieldGroupElement, "Expected a group element for fields.");

            foreach (ICodeElement codeElement in fieldGroupElement.Children)
            {
                FieldElement fieldElementTest = codeElement as FieldElement;
                Assert.IsNotNull(
                    fieldElementTest,
                    "Expected a field element but was type {0}: {1}",
                    codeElement.ElementType,
                    codeElement);
            }

            Assert.AreEqual(1, nestedRegionElements[1].Children.Count);
            foreach (ICodeElement codeElement in nestedRegionElements[1].Children[0].Children)
            {
                PropertyElement propertyElementTest = codeElement as PropertyElement;
                Assert.IsNotNull(
                    propertyElementTest,
                    "Expected a property element but was type {0}: {1}",
                     codeElement.ElementType,
                     codeElement);
            }

            Assert.AreEqual(1, nestedRegionElements[2].Children.Count);
            foreach (ICodeElement codeElement in nestedRegionElements[2].Children[0].Children)
            {
                MethodElement methodElementTest = codeElement as MethodElement;
                Assert.IsNotNull(
                    methodElementTest,
                    "Expected a method element but was type {0}: {1}",
                     codeElement.ElementType,
                     codeElement);
            }
        }

        /// <summary>
        /// Tests arranging with the default configuration except moving using directivesNo.
        /// </summary>
        [Test]
        public void DefaultArrangeNoUsingMoveTest()
        {
            CodeConfiguration configuration = CodeConfiguration.Default.Clone() as CodeConfiguration;
            configuration.Formatting.Usings.MoveTo = CodeLevel.None;

            CodeArranger arranger = new CodeArranger(configuration);

            ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(_testElements);

            //
            // Verify using statements were grouped and sorted correctly
            //
            Assert.AreEqual(3, arranged.Count, "An unexpected number of root elements were returned from Arrange.");

            RegionElement regionElement = arranged[0] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element.");
            Assert.AreEqual("Header", regionElement.Name);

            GroupElement groupElement = arranged[1] as GroupElement;
            Assert.IsNotNull(groupElement, "Expected a group element.");
            Assert.AreEqual("Namespace", groupElement.Name, "Unexpected group name.");
            Assert.AreEqual(1, groupElement.Children.Count, "Group contains an unexpected number of child elements.");

            groupElement = groupElement.Children[0] as GroupElement;
            Assert.IsNotNull(groupElement, "Expected a group element.");
            Assert.AreEqual("System", groupElement.Name, "Unexpected group name.");
            Assert.AreEqual(7, groupElement.Children.Count, "Group contains an unexpected number of child elements.");

            string lastUsingName = null;
            foreach (CodeElement groupedElement in groupElement.Children)
            {
                UsingElement usingElement = groupedElement as UsingElement;
                Assert.IsNotNull(usingElement, "Expected a using element.");

                string usingName = usingElement.Name;
                if (lastUsingName != null)
                {
                    Assert.AreEqual(
                        -1, lastUsingName.CompareTo(usingName), "Expected using statements to be sorted by name.");
                }
            }

            //
            // Verify the namespace arrangement
            //
            NamespaceElement namespaceElement = arranged[2] as NamespaceElement;
            Assert.IsNotNull(namespaceElement, "Expected a namespace element.");
        }

        /// <summary>
        /// Tests arranging a simple class.
        /// </summary>
        [Test]
        public void DefaultArrangeSimpleClassTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            TypeElement classElement = new TypeElement();
            classElement.Type = TypeElementType.Class;
            classElement.Access = CodeAccess.Public;
            classElement.Name = "TestClass";

            NamespaceElement namespaceElement = new NamespaceElement();
            namespaceElement.Name = "TestNamespace";
            namespaceElement.AddChild(classElement);

            MethodElement methodElement = new MethodElement();
            methodElement.Type = "void";
            methodElement.Access = CodeAccess.Public;
            methodElement.Name = "DoSomething";
            classElement.AddChild(methodElement);

            FieldElement fieldElement = new FieldElement();
            fieldElement.Type = "bool";
            fieldElement.Access = CodeAccess.Private;
            fieldElement.Name = "_val";
            classElement.AddChild(fieldElement);

            PropertyElement propertyElement = new PropertyElement();
            propertyElement.Type = "bool";
            propertyElement.Access = CodeAccess.Public;
            propertyElement.Name = "Value";
            propertyElement.BodyText = "return _val";
            classElement.AddChild(propertyElement);

            codeElements.Add(namespaceElement);

            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            ReadOnlyCollection<ICodeElement> arranged =
                arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");
            NamespaceElement namespaceElementTest = arranged[0] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");

            Assert.AreEqual(1, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            RegionElement typeRegionElement = namespaceElementTest.Children[0] as RegionElement;
            Assert.IsNotNull(typeRegionElement, "Expected a region element.");
            Assert.AreEqual("Types", typeRegionElement.Name);

            Assert.AreEqual(1, typeRegionElement.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            TypeElement typeElement = typeRegionElement.Children[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Expected a type element.");

            Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected type element type.");
            Assert.AreEqual(classElement.Name, typeElement.Name, "Unexpected type element name.");

            Assert.AreEqual(3, typeElement.Children.Count, "An unexpected number of class child elements were returned.");
            List<RegionElement> regionElements = new List<RegionElement>();
            foreach (ICodeElement classChildElement in typeElement.Children)
            {
                RegionElement regionElement = classChildElement as RegionElement;
                Assert.IsNotNull(
                    regionElement, "Expected a region element but was {0}.", classChildElement.ElementType);
                regionElements.Add(regionElement);
            }

            Assert.AreEqual("Fields", regionElements[0].Name, "Unexpected region element name.");
            Assert.AreEqual("Properties", regionElements[1].Name, "Unexpected region element name.");
            Assert.AreEqual("Methods", regionElements[2].Name, "Unexpected region element name.");

            GroupElement fieldGroupElement = regionElements[0].Children[0].Children[0] as GroupElement;
            Assert.IsNotNull(fieldGroupElement, "Expected a group element for fields.");

            foreach (ICodeElement codeElement in fieldGroupElement.Children)
            {
                FieldElement fieldElementTest = codeElement as FieldElement;
                Assert.IsNotNull(
                    fieldElementTest,
                    "Expected a field element but was type {0}: {1}",
                    codeElement.ElementType,
                    codeElement);
            }

            Assert.AreEqual(1, regionElements[1].Children.Count);
            foreach (ICodeElement codeElement in regionElements[1].Children[0].Children)
            {
                PropertyElement propertyElementTest = codeElement as PropertyElement;
                Assert.IsNotNull(
                    propertyElementTest,
                    "Expected a property element but was type {0}: {1}",
                     codeElement.ElementType,
                     codeElement);
            }

            Assert.AreEqual(1, regionElements[2].Children.Count);
            foreach (ICodeElement codeElement in regionElements[2].Children[0].Children)
            {
                MethodElement methodElementTest = codeElement as MethodElement;
                Assert.IsNotNull(
                    methodElementTest,
                    "Expected a method element but was type {0}: {1}",
                     codeElement.ElementType,
                     codeElement);
            }
        }

        /// <summary>
        /// Tests arranging a structure with the StructLayout attribute.
        /// </summary>
        [Test]
        public void DefaultArrangeStructLayoutTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            TypeElement structElement = new TypeElement();
            structElement.Type = TypeElementType.Structure;
            structElement.Access = CodeAccess.Public;
            structElement.Name = "TestStructure";
            structElement.AddAttribute(new AttributeElement("System.Runtime.InteropServices.StructLayout"));

            NamespaceElement namespaceElement = new NamespaceElement();
            namespaceElement.Name = "TestNamespace";
            namespaceElement.AddChild(structElement);

            FieldElement fieldElement1 = new FieldElement();
            fieldElement1.Type = "int";
            fieldElement1.Access = CodeAccess.Public;
            fieldElement1.Name = "z";
            structElement.AddChild(fieldElement1);

            FieldElement fieldElement2 = new FieldElement();
            fieldElement2.Type = "int";
            fieldElement2.Access = CodeAccess.Public;
            fieldElement2.Name = "x";
            structElement.AddChild(fieldElement2);

            FieldElement fieldElement3 = new FieldElement();
            fieldElement3.Type = "int";
            fieldElement3.Access = CodeAccess.Public;
            fieldElement3.Name = "y";
            structElement.AddChild(fieldElement3);

            codeElements.Add(namespaceElement);

            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            ReadOnlyCollection<ICodeElement> arranged =
                arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");
            NamespaceElement namespaceElementTest = arranged[0] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");

            Assert.AreEqual(1, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            RegionElement typeRegionElement = namespaceElementTest.Children[0] as RegionElement;
            Assert.IsNotNull(typeRegionElement, "Expected a region element.");
            Assert.AreEqual("Types", typeRegionElement.Name);

            Assert.AreEqual(1, typeRegionElement.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            TypeElement typeElement = typeRegionElement.Children[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Expected a type element.");
            Assert.AreEqual(TypeElementType.Structure, typeElement.Type, "Unexpected type element type.");
            Assert.AreEqual(structElement.Name, typeElement.Name, "Unexpected type element name.");

            Assert.AreEqual(1, typeElement.Children.Count, "An unexpected number of class child elements were returned.");
            RegionElement regionElement = typeElement.Children[0] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element but was {0}.", regionElement.ElementType);
            Assert.AreEqual("Fixed Fields", regionElement.Name, "Unexpected region name.");

            Assert.AreEqual(3, regionElement.Children.Count, "Unexpected number of region child elements.");

            // The fields should not have been sorted
            Assert.AreEqual(fieldElement1.Name, regionElement.Children[0].Name);
            Assert.AreEqual(fieldElement2.Name, regionElement.Children[1].Name);
            Assert.AreEqual(fieldElement3.Name, regionElement.Children[2].Name);
        }

        /// <summary>
        /// Tests arranging with the default configuration.
        /// </summary>
        [Test]
        public void DefaultArrangeTest()
        {
            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(_testElements);

            //
            // Verify using statements were grouped and sorted correctly
            //
            Assert.AreEqual(2, arranged.Count, "An unexpected number of root elements were returned from Arrange.");

            RegionElement regionElement = arranged[0] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element.");
            Assert.AreEqual("Header", regionElement.Name);

            //
            // Verify the namespace arrangement
            //
            NamespaceElement namespaceElement = arranged[1] as NamespaceElement;
            Assert.IsNotNull(namespaceElement, "Expected a namespace element.");
            Assert.IsTrue(namespaceElement.Children.Count > 0);

            GroupElement groupElement = namespaceElement.Children[0] as GroupElement;
            Assert.IsNotNull(groupElement, "Expected a group element.");
            Assert.AreEqual("Namespace", groupElement.Name, "Unexpected group name.");
            Assert.AreEqual(1, groupElement.Children.Count, "Group contains an unexpected number of child elements.");

            groupElement = groupElement.Children[0] as GroupElement;
            Assert.IsNotNull(groupElement, "Expected a group element.");
            Assert.AreEqual("System", groupElement.Name, "Unexpected group name.");
            Assert.AreEqual(8, groupElement.Children.Count, "Group contains an unexpected number of child elements.");

            string lastUsingName = null;
            foreach (CodeElement groupedElement in groupElement.Children)
            {
                UsingElement usingElement = groupedElement as UsingElement;
                Assert.IsNotNull(usingElement, "Expected a using element.");

                string usingName = usingElement.Name;
                if (lastUsingName != null)
                {
                    Assert.AreEqual(
                        -1, lastUsingName.CompareTo(usingName), "Expected using statements to be sorted by name.");
                }
            }
        }

        /// <summary>
        /// Tests arranging using statements in a region with the default configuration.
        /// </summary>
        [Test]
        public void DefaultArrangeUsingsInRegionTest()
        {
            CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

            List<ICodeElement> codeElements = new List<ICodeElement>();

            RegionElement regionElement = new RegionElement();
            regionElement.Name = "Using Directives";

            UsingElement usingElement1 = new UsingElement();
            usingElement1.Name = "System";
            regionElement.AddChild(usingElement1);

            UsingElement usingElement2 = new UsingElement();
            usingElement2.Name = "System.Text";
            regionElement.AddChild(usingElement2);

            codeElements.Add(regionElement);

            ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(codeElements.AsReadOnly());

            //
            // Verify using statements were stripped from the region
            //
            Assert.AreEqual(1, arranged.Count, "An unexpected number of root elements were returned from Arrange.");
            GroupElement groupElement = arranged[0] as GroupElement;
            Assert.IsNotNull(groupElement, "Expected a group element.");
            Assert.AreEqual("Namespace", groupElement.Name);

            groupElement = groupElement.Children[0] as GroupElement;
            Assert.IsNotNull(groupElement, "Expected a group element.");
            Assert.AreEqual("System", groupElement.Name);
            foreach (ICodeElement arrangedElement in groupElement.Children)
            {
                Assert.IsTrue(arrangedElement is UsingElement, "Expected a using element.");
            }
        }

        /// <summary>
        /// Tests moving usings to the namespace level.
        /// </summary>
        [Test]
        public void MoveUsingsBasicTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            UsingElement using1 = new UsingElement();
            using1.Name = "System";
            using1.IsMovable = true;

            UsingElement using2 = new UsingElement();
            using2.Name = "System.IO";
            using2.IsMovable = true;

            UsingElement using3 = new UsingElement();
            using3.Name = "System.Collections";
            using3.IsMovable = true;

            codeElements.Add(using1);
            codeElements.Add(using2);

            NamespaceElement namespaceElement = new NamespaceElement();
            namespaceElement.Name = "TestNamespace";
            namespaceElement.AddChild(using3);

            codeElements.Add(namespaceElement);

            CodeConfiguration configuration = CodeConfiguration.Default.Clone() as CodeConfiguration;
            CodeArranger arranger;

            //
            // Do not move.
            //
            configuration.Formatting.Usings.MoveTo = CodeLevel.None;
            arranger = new CodeArranger(configuration);
            ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(2, arranged.Count, "After arranging, an unexpected number of elements were returned.");

            GroupElement fileGroup = arranged[0] as GroupElement;
            Assert.IsNotNull(fileGroup);
            GroupElement innerGroup = fileGroup.Children[0] as GroupElement;
            Assert.AreEqual("System", innerGroup.Children[0].Name);
            Assert.AreEqual("System.IO", innerGroup.Children[1].Name);

            NamespaceElement namespaceElementTest = arranged[1] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");
            Assert.AreEqual(1, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            GroupElement namespaceGroup = namespaceElementTest.Children[0] as GroupElement;
            Assert.IsNotNull(namespaceGroup);
            innerGroup = namespaceGroup.Children[0] as GroupElement;
            Assert.AreEqual("System.Collections", innerGroup.Children[0].Name);

            //
            // Move to file level;
            //
            configuration.Formatting.Usings.MoveTo = CodeLevel.File;
            arranger = new CodeArranger(configuration);
            arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(2, arranged.Count, "After arranging, an unexpected number of elements were returned.");

            fileGroup = arranged[0] as GroupElement;
            Assert.IsNotNull(fileGroup);
            innerGroup = fileGroup.Children[0] as GroupElement;
            Assert.AreEqual("System", innerGroup.Children[0].Name);
            Assert.AreEqual("System.Collections", innerGroup.Children[1].Name);
            Assert.AreEqual("System.IO", innerGroup.Children[2].Name);

            namespaceElementTest = arranged[1] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");

            //
            // Move to namespace.
            //
            configuration.Formatting.Usings.MoveTo = CodeLevel.Namespace;
            arranger = new CodeArranger(configuration);
            arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(1, arranged.Count, "After arranging, an unexpected number of elements were returned.");

            namespaceElementTest = arranged[0] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");
            Assert.AreEqual(1, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            namespaceGroup = namespaceElementTest.Children[0] as GroupElement;
            Assert.IsNotNull(namespaceGroup);
            innerGroup = namespaceGroup.Children[0] as GroupElement;
            Assert.AreEqual("System", innerGroup.Children[0].Name);
            Assert.AreEqual("System.Collections", innerGroup.Children[1].Name);
            Assert.AreEqual("System.IO", innerGroup.Children[2].Name);

            //
            // Move back to file level;
            //
            configuration.Formatting.Usings.MoveTo = CodeLevel.File;
            arranger = new CodeArranger(configuration);
            arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(2, arranged.Count, "After arranging, an unexpected number of elements were returned.");

            fileGroup = arranged[0] as GroupElement;
            Assert.IsNotNull(fileGroup);
            innerGroup = fileGroup.Children[0] as GroupElement;
            Assert.AreEqual("System", innerGroup.Children[0].Name);
            Assert.AreEqual("System.Collections", innerGroup.Children[1].Name);
            Assert.AreEqual("System.IO", innerGroup.Children[2].Name);

            namespaceElementTest = arranged[1] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");
        }

        /// <summary>
        /// Tests moving usings to the file level.
        /// </summary>
        [Test]
        public void MoveUsingsToFileTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            UsingElement using1 = new UsingElement();
            using1.Name = "System";
            using1.IsMovable = true;

            codeElements.Add(using1);

            NamespaceElement namespaceElement = new NamespaceElement();
            namespaceElement.Name = "TestNamespace";
            codeElements.Add(namespaceElement);

            // Nested region and groups
            RegionElement region = new RegionElement();
            region.Name = "Region";
            namespaceElement.AddChild(region);
            GroupElement group = new GroupElement();
            group.Name = "Group";
            region.AddChild(group);

            UsingElement using2 = new UsingElement();
            using2.Name = "System.IO";
            using2.IsMovable = true;

            group.AddChild(using2);

            UsingElement using3 = new UsingElement();
            using3.Name = "System.Collections";
            using3.IsMovable = true;
            namespaceElement.AddChild(using3);

            TypeElement class1 = new TypeElement();
            class1.Name = "Class1";
            namespaceElement.AddChild(class1);

            TypeElement class2 = new TypeElement();
            class2.Name = "Class2";
            namespaceElement.AddChild(class2);

            CodeConfiguration configuration = CodeConfiguration.Default.Clone() as CodeConfiguration;
            CodeArranger arranger;

            //
            // Move to file.
            //
            configuration.Formatting.Usings.MoveTo = CodeLevel.File;
            arranger = new CodeArranger(configuration);
            ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(2, arranged.Count, "After arranging, an unexpected number of elements were returned.");

            GroupElement fileGroup = arranged[0] as GroupElement;
            Assert.IsNotNull(fileGroup);
            GroupElement innerGroup = fileGroup.Children[0] as GroupElement;
            Assert.AreEqual("System", innerGroup.Children[0].Name);
            Assert.AreEqual("System.Collections", innerGroup.Children[1].Name);
            Assert.AreEqual("System.IO", innerGroup.Children[2].Name);

            NamespaceElement namespaceElementTest = arranged[1] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");
            Assert.AreEqual(2, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");

            RegionElement typeRegion = namespaceElementTest.Children[1] as RegionElement;
            Assert.IsNotNull(typeRegion);
            Assert.AreEqual("Class1", typeRegion.Children[0].Name);
            Assert.AreEqual("Class2", typeRegion.Children[1].Name);
        }

        /// <summary>
        /// Tests moving usings to the namespace level.
        /// </summary>
        [Test]
        public void MoveUsingsToNamespaceTest()
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();

            UsingElement using1 = new UsingElement();
            using1.Name = "System";
            using1.IsMovable = true;

            codeElements.Add(using1);

            // Nested region and groups
            RegionElement region = new RegionElement();
            region.Name = "Region";
            codeElements.Add(region);
            GroupElement group = new GroupElement();
            group.Name = "Group";
            region.AddChild(group);

            UsingElement using2 = new UsingElement();
            using2.Name = "System.IO";
            using2.IsMovable = true;

            group.AddChild(using2);

            NamespaceElement namespaceElement = new NamespaceElement();
            namespaceElement.Name = "TestNamespace";
            codeElements.Add(namespaceElement);

            UsingElement using3 = new UsingElement();
            using3.Name = "System.Collections";
            using3.IsMovable = true;
            namespaceElement.AddChild(using3);

            TypeElement class1 = new TypeElement();
            class1.Name = "Class1";
            namespaceElement.AddChild(class1);

            TypeElement class2 = new TypeElement();
            class2.Name = "Class2";
            namespaceElement.AddChild(class2);

            CodeConfiguration configuration = CodeConfiguration.Default.Clone() as CodeConfiguration;
            CodeArranger arranger;

            //
            // Move to namespace.
            //
            configuration.Formatting.Usings.MoveTo = CodeLevel.Namespace;
            arranger = new CodeArranger(configuration);
            ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(codeElements.AsReadOnly());

            Assert.AreEqual(2, arranged.Count, "After arranging, an unexpected number of elements were returned.");

            NamespaceElement namespaceElementTest = arranged[1] as NamespaceElement;
            Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");
            Assert.AreEqual(2, namespaceElementTest.Children.Count, "After arranging, an unexpected number of namespace elements were returned.");
            GroupElement namespaceGroup = namespaceElementTest.Children[0] as GroupElement;
            Assert.IsNotNull(namespaceGroup);
            GroupElement innerGroup = namespaceGroup.Children[0] as GroupElement;
            Assert.AreEqual("System", innerGroup.Children[0].Name);
            Assert.AreEqual("System.Collections", innerGroup.Children[1].Name);
            Assert.AreEqual("System.IO", innerGroup.Children[2].Name);

            RegionElement typeRegion = namespaceElementTest.Children[1] as RegionElement;
            Assert.IsNotNull(typeRegion);
            Assert.AreEqual("Class1", typeRegion.Children[0].Name);
            Assert.AreEqual("Class2", typeRegion.Children[1].Name);
        }

        /// <summary>
        /// Performs setup for this test fixture.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                CSharpParser parser = new CSharpParser();
                _testElements = parser.Parse(reader);

                Assert.IsTrue(_testElements.Count > 0, "Test file does not contain any elements.");
            }
        }

        #endregion Methods
    }
}