namespace NArrange.Tests.VisualBasic
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.VisualBasic;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the VBParser class.
    /// </summary>
    [TestFixture]
    public class VBParserTests
    {
        #region Fields

        /// <summary>
        /// Constructor region index in ClassMembers.vb.
        /// </summary>
        private const int ConstructorRegionIndex = 1;

        /// <summary>
        /// Delegate region index in ClassMembers.vb.
        /// </summary>
        private const int DelegateRegionIndex = 4;

        /// <summary>
        /// Event region index in ClassMembers.vb.
        /// </summary>
        private const int EventRegionIndex = 5;

        /// <summary>
        /// Field region index in ClassMembers.vb.
        /// </summary>
        private const int FieldRegionIndex = 0;

        /// <summary>
        /// Method region index in ClassMembers.vb.
        /// </summary>
        private const int MethodRegionIndex = 3;

        /// <summary>
        /// Nested type region index in ClassMembers.vb.
        /// </summary>
        private const int NestedTypeRegionIndex = 6;

        /// <summary>
        /// Property region index in ClassMembers.vb.
        /// </summary>
        private const int PropertyRegionIndex = 2;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Tests that when the end of a block is expected, the appropriate
        /// error is thrown.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException), ExpectedMessage = "Expected End Namespace: Line 1, Column 26")]
        public void ExpectedBlockCloseTest()
        {
            StringReader reader = new StringReader(
                "namespace SampleNamespace");

            VBParser parser = new VBParser();
            parser.Parse(reader);
        }

        /// <summary>
        /// Tests that when an initial value is expected, the appropriate
        /// error is thrown.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            ExpectedMessage = "Expected an initial value",
            MatchType = MessageMatch.Contains)]
        public void ExpectedFieldInitialValueTest()
        {
            StringReader reader = new StringReader(
                "namespace SampleNamespace\r\n" +
                "    public class SampleClass\r\n" +
                "        private test as string = \r\n" +
                "    end class\r\n" +
                "end namespace");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests the parsing of assembly attributes.
        /// </summary>
        [Test]
        public void ParseAssemblyAttributesTest()
        {
            VBParser parser = new VBParser();

            VBTestFile testFile = VBTestUtilities.GetAssemblyAttributesFile();
            using (TextReader reader = testFile.GetReader())
            {
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.IsNotNull(elements, "Code element collection should not be null.");
                Assert.AreEqual(15, elements.Count, "An unexpected number of elements were parsed.");

                //
                // Using statements
                //
                UsingElement usingElement;

                usingElement = elements[0] as UsingElement;
                Assert.IsNotNull(usingElement, "Element is not a UsingElement.");
                Assert.AreEqual("System.Reflection", usingElement.Name, "Unexpected name.");

                usingElement = elements[1] as UsingElement;
                Assert.IsNotNull(usingElement, "Element is not a UsingElement.");
                Assert.AreEqual("System.Runtime.CompilerServices", usingElement.Name, "Unexpected name.");

                usingElement = elements[2] as UsingElement;
                Assert.IsNotNull(usingElement, "Element is not a UsingElement.");
                Assert.AreEqual("System.Runtime.InteropServices", usingElement.Name, "Unexpected name.");

                //
                // Attributes
                //
                AttributeElement attributeElement;

                attributeElement = elements[3] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyTitle", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"NArrange.Core.Tests\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(3, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[4] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyDescription", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[5] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyConfiguration", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[6] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyCompany", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[7] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyProduct", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"NArrange.Core.Tests\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[8] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyCopyright", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"Copyright ©  2007\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[9] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyTrademark", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[10] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyCulture", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[11] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("ComVisible", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("False", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(3, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[12] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("Guid", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"def01aba-79c5-4082-9522-e570c52a2df1\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(1, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[13] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyVersion", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"1.0.0.0\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(9, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");

                attributeElement = elements[14] as AttributeElement;
                Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
                Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("AssemblyFileVersion", attributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"1.0.0.0\"", attributeElement.BodyText, "Unexpected attribute text.");
                Assert.AreEqual(0, attributeElement.HeaderComments.Count, "An unexpected number of header comment lines were parsed.");
            }
        }

        /// <summary>
        /// Tests parsing an attribute where the closing is expected.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
           MatchType = MessageMatch.Contains,
          ExpectedMessage = "Expected >")]
        public void ParseAttributeCloseExpectedTest()
        {
            StringReader reader = new StringReader(
                "<assembly: AssemblyDescription(\"SampleAssembly\")");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing an attribute list.
        /// </summary>
        [Test]
        public void ParseAttributeListTest()
        {
            StringReader reader = new StringReader(
                "<assembly: AssemblyDescription(\"SampleAssembly\"), _\r\nComVisible(false),\tAssemblyConfiguration(\"\"), TestAttribute>");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            AttributeElement attributeElement;

            attributeElement = elements[0] as AttributeElement;
            Assert.IsNotNull(attributeElement, "Element is not an AttributeElement.");
            Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
            Assert.AreEqual("AssemblyDescription", attributeElement.Name, "Unexpected attribute name.");
            Assert.AreEqual("\"SampleAssembly\"", attributeElement.BodyText, "Unexpected attribute text.");

            Assert.AreEqual(3, attributeElement.Children.Count, "An unexpected number of child elements were parsed.");

            AttributeElement attributeChildElement = attributeElement.Children[0] as AttributeElement;
            Assert.IsNotNull(attributeChildElement, "Element is not an AttributeElement.");
            Assert.AreEqual("assembly", attributeChildElement.Target, "Unexpected attribute target.");
            Assert.AreEqual("ComVisible", attributeChildElement.Name, "Unexpected attribute name.");
            Assert.AreEqual("false", attributeChildElement.BodyText, "Unexpected attribute text.");

            attributeChildElement = attributeElement.Children[1] as AttributeElement;
            Assert.IsNotNull(attributeChildElement, "Element is not an AttributeElement.");
            Assert.AreEqual("assembly", attributeChildElement.Target, "Unexpected attribute target.");
            Assert.AreEqual("AssemblyConfiguration", attributeChildElement.Name, "Unexpected attribute name.");
            Assert.AreEqual("\"\"", attributeChildElement.BodyText, "Unexpected attribute text.");

            attributeChildElement = attributeElement.Children[2] as AttributeElement;
            Assert.IsNotNull(attributeChildElement, "Element is not an AttributeElement.");
            Assert.AreEqual("assembly", attributeChildElement.Target, "Unexpected attribute target.");
            Assert.AreEqual("TestAttribute", attributeChildElement.Name, "Unexpected attribute name.");
            Assert.IsNull(attributeChildElement.BodyText, "Unexpected attribute text.");
        }

        /// <summary>
        /// Tests parsing an attribute.
        /// </summary>
        [Test]
        public void ParseAttributeTest()
        {
            StringReader reader = new StringReader(
                "<assembly: AssemblyDescription(\"SampleAssembly\")>");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            AttributeElement attributeElement = elements[0] as AttributeElement;
            Assert.IsNotNull(attributeElement, "Element is not a AttributeElement.");
            Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
            Assert.AreEqual("AssemblyDescription", attributeElement.Name, "Unexpected attribute name.");
            Assert.AreEqual("\"SampleAssembly\"", attributeElement.BodyText, "Unexpected attribute text.");
        }

        /// <summary>
        /// Tests parsing an attribute that contains an attribute
        /// character in a string.
        /// </summary>
        [Test]
        public void ParseAttributeWithAttributeCharacterTest()
        {
            StringReader reader = new StringReader(
                "<assembly: AssemblyDescription(\"SampleAssembly>\")>");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            AttributeElement attributeElement = elements[0] as AttributeElement;
            Assert.IsNotNull(attributeElement, "Element is not a AttributeElement.");
            Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
            Assert.AreEqual("AssemblyDescription", attributeElement.Name, "Unexpected attribute name.");
            Assert.AreEqual("\"SampleAssembly>\"", attributeElement.BodyText, "Unexpected attribute text.");
        }

        /// <summary>
        /// Tests parsing an attribute with a line continuation.
        /// </summary>
        [Test]
        public void ParseAttributeWithLineContinuationTest()
        {
            StringReader reader = new StringReader(
                "<assembly: _\r\nAssemblyDescription(\"SampleAssembly\")>");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            AttributeElement attributeElement = elements[0] as AttributeElement;
            Assert.IsNotNull(attributeElement, "Element is not a AttributeElement.");
            Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
            Assert.AreEqual("AssemblyDescription", attributeElement.Name, "Unexpected attribute name.");
            Assert.AreEqual("\"SampleAssembly\"", attributeElement.BodyText, "Unexpected attribute text.");
        }

        /// <summary>
        /// Tests parsing a method whose body text has a comment containing the end block text.
        /// </summary>
        [Test]
        public void ParseBlockEndInCommentTest()
        {
            StringReader reader;
            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements;
            TextCodeElement textCodeElement;

            reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "'End Sub\r\n" +
                "End Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("'End Sub", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "REM End Sub\r\n" +
                "End Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("REM End Sub", textCodeElement.BodyText, "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing a method without a closing name.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected End")]
        public void ParseBlockExpectedEndElementTest()
        {
            VBParser parser = new VBParser();

            StringReader reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "_\r\n" +
                "End");

            parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a method whose body text uses the line continuation character.
        /// </summary>
        [Test]
        public void ParseBlockLineContinuationTest()
        {
            StringReader reader;
            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements;
            TextCodeElement textCodeElement;

            reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "_\r\n" +
                "End Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("_", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private Sub DoSomething()\r\n" +
               "_  \r\n" +
               "End Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("_", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private Sub DoSomething()\r\n" +
               "_\r\n" +
               "_\r\n" +
               "End Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("_\r\n_", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private Sub DoSomething()\r\n" +
               "_\t\r\n" +
               "_\r\n" +
               "End Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("_\t\r\n_", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private Sub DoSomething()\r\n" +
               "_\r\n" +
               "End _\r\n" +
               "Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("_", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private Sub DoSomething()\r\n" +
               "_\r\n" +
               "End _ \t\r\n" +
               "Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("_", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private Sub DoSomething()\r\n" +
               "End some text\r\n" +
               "End Sub");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("End some text", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private ReadOnly Property SomeProperty() As String\r\n" +
               "\tGet\r\n" +
               "\t\tReturn Nothing\r\n" +
               "\tEnd _\r\n" +
               "\tGet\r\n" +
               "End Property");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("Get\r\n\t\tReturn Nothing\r\n\tEnd _\r\n\tGet", textCodeElement.BodyText, "Unexpected body text.");

            reader = new StringReader(
               "Private ReadOnly Property SomeProperty() As String\r\n" +
               "\tGet\r\n" +
               "\t\tReturn Nothing\r\n" +
               "\tEnd _ \t\r\n" +
               "\tGet\r\n" +
               "End Property");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("Get\r\n\t\tReturn Nothing\r\n\tEnd _ \t\r\n\tGet", textCodeElement.BodyText, "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing a method whose block has a closing comment.
        /// </summary>
        [Test]
        public void ParseBlockWithClosingCommentTest()
        {
            StringReader reader;
            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements;
            TextCodeElement textCodeElement;

            reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "\t'Comment here\r\n" +
                "End Sub \t'New");

            elements = parser.Parse(reader);
            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            textCodeElement = elements[0] as TextCodeElement;
            Assert.AreEqual("'Comment here", textCodeElement.BodyText, "Unexpected body text.");
        }

        /// <summary>
        /// Tests the parsing of a single namespace with a single class
        /// definition.
        /// </summary>
        [Test]
        public void ParseClassDefinitionTest()
        {
            VBParser parser = new VBParser();

            VBTestFile testFile = VBTestUtilities.GetClassDefinitionFile();
            using (TextReader reader = testFile.GetReader())
            {
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.IsNotNull(elements, "Code element collection should not be null.");
                Assert.AreEqual(7, elements.Count, "An unexpected number of elements were parsed.");

                CommentElement commentElement1 = elements[0] as CommentElement;
                Assert.IsNotNull(commentElement1, "Expected a CommentElement.");
                Assert.AreEqual(" This is comment line 1", commentElement1.Text, "Unexpected comment text.");

                CommentElement commentElement2 = elements[1] as CommentElement;
                Assert.IsNotNull(commentElement2, "Expected a CommentElement.");
                Assert.AreEqual(" This is comment line 2", commentElement2.Text, "Unexpected comment text.");

                CommentElement commentElement3 = elements[2] as CommentElement;
                Assert.IsNotNull(commentElement3, "Expected a CommentElement.");
                Assert.AreEqual(" This is comment line 3", commentElement3.Text, "Unexpected comment text.");

                UsingElement using1 = elements[3] as UsingElement;
                Assert.IsNotNull(using1, "Expected a UsingElement.");
                Assert.AreEqual("System", using1.Name, "Unexpected using name.");

                UsingElement using2 = elements[4] as UsingElement;
                Assert.IsNotNull(using2, "Expected a UsingElement.");
                Assert.AreEqual("System.Collections.Generic", using2.Name, "Unexpected using name.");

                UsingElement using3 = elements[5] as UsingElement;
                Assert.IsNotNull(using3, "Expected a UsingElement.");
                Assert.AreEqual("System.Text", using3.Name, "Unexpected using name.");

                NamespaceElement namespaceElement = elements[6] as NamespaceElement;
                Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
                Assert.AreEqual("SampleNamespace", namespaceElement.Name, "Unexpected namespace name.");

                Assert.IsNotNull(namespaceElement.Children, "Namespace Children collection should not be null.");
                Assert.AreEqual(1, namespaceElement.Children.Count, "An unexpected number of namespace child elements were parsed.");

                TypeElement classElement = namespaceElement.Children[0] as TypeElement;
                Assert.IsNotNull(classElement, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass", classElement.Name, "Unexpected class name.");
                Assert.AreEqual(3, classElement.HeaderComments.Count, "An unexpected number of class header comment lines were parsed.");
                foreach (ICommentElement comment in
                    classElement.HeaderComments)
                {
                    Assert.AreEqual(CommentType.XmlLine, comment.Type, "Class header comment should be an XML comment.");
                }
                Assert.AreEqual(CodeAccess.Public, classElement.Access, "Unexpected class code access level.");
            }
        }

        /// <summary>
        /// Tests parsing a class where a type implementation specification is expected.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
          ExpectedMessage = "Expected )")]
        public void ParseClassExpectedTypeImplementsTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test(Of T");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class where a type parameter constraint is expected.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
        ExpectedMessage = "Expected type parameter constraint")]
        public void ParseClassExpectedTypeParameterConstraintTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test(Of T As)");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with a missing endregion tag.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
           MatchType = MessageMatch.Contains,
          ExpectedMessage = "Missing end region directive for 'Fields'")]
        public void ParseClassMissingEndregionTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test\r\n" +
                "\t#Region \"Fields\"\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with a missing region name.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
           MatchType = MessageMatch.Contains,
           ExpectedMessage = "Expected region name")]
        public void ParseClassMissingRegionNameTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test\r\n" +
                "\t#Region\r\n" +
                "\t#Endregion\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with invalid multiple type parameters.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            ExpectedMessage = "Expected , or )",
            MatchType = MessageMatch.Contains)]
        public void ParseClassMultipleTypeParameterInvalidTest1()
        {
            StringReader reader = new StringReader(
                "Partial Public Class NewClass(Of T as new IDisposable)\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with invalid multiple type parameters.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            ExpectedMessage = "Expected , or }",
            MatchType = MessageMatch.Contains)]
        public void ParseClassMultipleTypeParameterInvalidTest2()
        {
            StringReader reader = new StringReader(
                "Partial Public Class NewClass(Of T as new,  S as {IDisposable)\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with invalid multiple type parameters.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            ExpectedMessage = "Expected , or )",
            MatchType = MessageMatch.Contains)]
        public void ParseClassMultipleTypeParameterInvalidTest3()
        {
            StringReader reader = new StringReader(
                "Partial Public Class NewClass(Of T as new, IDisposable, S as new, IComparable})\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.Fail();
        }

        /// <summary>
        /// Tests parsing a class with invalid multiple type parameters.
        /// </summary>
        [Test]
        public void ParseClassMultipleTypeParameterTest1()
        {
            string[] variations = new string[]
            {
                "Partial Public Class NewClass(Of T as new,R,S as new,Q)\r\n" +
                "End Class",
                "Partial Public Class NewClass(Of T as new, R, S as new, Q)\r\n" +
                "End Class"
            };

            foreach (string variation in variations)
            {
                StringReader reader = new StringReader(variation);

                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                TypeElement classElement = elements[0] as TypeElement;
                Assert.IsNotNull(classElement, "Expected a class element");
                Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class.");
                Assert.AreEqual("NewClass", classElement.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Public, classElement.Access, "Unexpected access.");
                Assert.IsTrue(classElement.IsPartial, "Expected a partial class.");
                Assert.AreEqual(4, classElement.TypeParameters.Count, "Unexpected number of type parameters.");
                Assert.AreEqual("T", classElement.TypeParameters[0].Name);
                Assert.AreEqual(1, classElement.TypeParameters[0].Constraints.Count);
                Assert.AreEqual("new", classElement.TypeParameters[0].Constraints[0]);
                Assert.AreEqual("R", classElement.TypeParameters[1].Name);
                Assert.AreEqual(0, classElement.TypeParameters[1].Constraints.Count);
                Assert.AreEqual("S", classElement.TypeParameters[2].Name);
                Assert.AreEqual(1, classElement.TypeParameters[2].Constraints.Count);
                Assert.AreEqual("new", classElement.TypeParameters[2].Constraints[0]);
                Assert.AreEqual("Q", classElement.TypeParameters[3].Name);
                Assert.AreEqual(0, classElement.TypeParameters[3].Constraints.Count);
            }
        }

        /// <summary>
        /// Tests parsing a class with multiple type parameters and constraints.
        /// </summary>
        [Test]
        public void ParseClassMultipleTypeParameterTest2()
        {
            StringReader reader = new StringReader(
                "Partial Public Class NewClass(Of T as {new, IDisposable}, S as {new, IComparable})\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            TypeElement classElement = elements[0] as TypeElement;
            Assert.IsNotNull(classElement, "Expected a class element");
            Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class.");
            Assert.AreEqual("NewClass", classElement.Name, "Unexpected class name.");
            Assert.AreEqual(CodeAccess.Public, classElement.Access, "Unexpected access.");
            Assert.IsTrue(classElement.IsPartial, "Expected a partial class.");
            Assert.AreEqual(2, classElement.TypeParameters.Count, "Unexpected number of type parameters.");
            Assert.AreEqual("T", classElement.TypeParameters[0].Name);
            Assert.AreEqual(2, classElement.TypeParameters[0].Constraints.Count);
            Assert.AreEqual("new", classElement.TypeParameters[0].Constraints[0]);
            Assert.AreEqual("IDisposable", classElement.TypeParameters[0].Constraints[1]);
            Assert.AreEqual("S", classElement.TypeParameters[1].Name);
            Assert.AreEqual(2, classElement.TypeParameters[0].Constraints.Count);
            Assert.AreEqual("new", classElement.TypeParameters[1].Constraints[0]);
            Assert.AreEqual("IComparable", classElement.TypeParameters[1].Constraints[1]);
        }

        /// <summary>
        /// Tests parsing a class with multiple type parameters and constraints.
        /// </summary>
        [Test]
        public void ParseClassMultipleTypeParameterTest3()
        {
            StringReader reader = new StringReader(
                "Partial Public Class NewClass(Of T as {new, IDisposable}, S as new, R)\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            TypeElement classElement = elements[0] as TypeElement;
            Assert.IsNotNull(classElement, "Expected a class element");
            Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class.");
            Assert.AreEqual("NewClass", classElement.Name, "Unexpected class name.");
            Assert.AreEqual(CodeAccess.Public, classElement.Access, "Unexpected access.");
            Assert.IsTrue(classElement.IsPartial, "Expected a partial class.");
            Assert.AreEqual(3, classElement.TypeParameters.Count, "Unexpected number of type parameters.");
            Assert.AreEqual("T", classElement.TypeParameters[0].Name);
            Assert.AreEqual(2, classElement.TypeParameters[0].Constraints.Count);
            Assert.AreEqual("new", classElement.TypeParameters[0].Constraints[0]);
            Assert.AreEqual("IDisposable", classElement.TypeParameters[0].Constraints[1]);
            Assert.AreEqual("S", classElement.TypeParameters[1].Name);
            Assert.AreEqual(1, classElement.TypeParameters[1].Constraints.Count);
            Assert.AreEqual("new", classElement.TypeParameters[1].Constraints[0]);
            Assert.AreEqual("R", classElement.TypeParameters[2].Name);
            Assert.AreEqual(0, classElement.TypeParameters[2].Constraints.Count);
        }

        /// <summary>
        /// Tests parsing a class with unspecified access.
        /// </summary>
        [Test]
        public void ParseClassPartialUnspecifiedAccessTest()
        {
            StringReader reader = new StringReader(
                "partial class Test\r\nend class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            TypeElement typeElement = elements[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
            Assert.AreEqual("Test", typeElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.None, typeElement.Access, "Unexpected code access.");
            Assert.IsTrue(typeElement.IsPartial, "Expected a partial class.");
            Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected type element type.");
        }

        /// <summary>
        /// Tests parsing a simple class.
        /// </summary>
        [Test]
        public void ParseClassSimpleTest()
        {
            StringReader reader = new StringReader(
                "public class Test\r\n" +
                "end class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            TypeElement typeElement = elements[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
            Assert.AreEqual("Test", typeElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, typeElement.Access, "Unexpected code access.");
            Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected type element type.");
        }

        /// <summary>
        /// Tests parsing a class with an unclosed type parameter constraint.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
           ExpectedMessage = "Expected )")]
        public void ParseClassUnclosedTypeParameterConstraintTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test(Of T As IComparable(Of T");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with an unclosed type parameter.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
           ExpectedMessage = "Expected ,")]
        public void ParseClassUnclosedTypeParameterTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test(Of T\r\nEnd Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with an invalid element.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unhandled element")]
        public void ParseClassUnhandledElementTest1()
        {
            StringReader reader = new StringReader(
                "public class Test\r\ntest\r\nend class");
            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with an invalid element.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unhandled element")]
        public void ParseClassUnhandledElementTest2()
        {
            StringReader reader = new StringReader(
                "public class Test\r\ntest\r\n\r\nPublic Test as String\r\nend class");
            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with an invalid element.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unhandled element")]
        public void ParseClassUnhandledElementTest3()
        {
            StringReader reader = new StringReader(
                "public class Test\r\n;\r\n\r\nPublic Test as String\r\nend class");
            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a class with unspecified access.
        /// </summary>
        [Test]
        public void ParseClassUnspecifiedAccessTest()
        {
            StringReader reader = new StringReader(
                "class Test\r\n" +
                "class Nested\r\n" +
                "end class\r\n" +
                "end class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            TypeElement typeElement = elements[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
            Assert.AreEqual("Test", typeElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.None, typeElement.Access, "Unexpected code access.");
            Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected type element type.");

            Assert.AreEqual(1, typeElement.Children.Count, "An unexpected number of child elements were parsed.");
            TypeElement nestedtypeElement = typeElement.Children[0] as TypeElement;
            Assert.IsNotNull(nestedtypeElement, "Element is not a TypeElement.");
            Assert.AreEqual("Nested", nestedtypeElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.None, nestedtypeElement.Access, "Unexpected code access.");
            Assert.AreEqual(TypeElementType.Class, nestedtypeElement.Type, "Unexpected type element type.");
        }

        /// <summary>
        /// Tests parsing a commented member.
        /// </summary>
        [Test]
        public void ParseCommentedMemberTest()
        {
            StringReader reader = new StringReader(
                "'Private Sub DoSomething()\r\n" +
                "'End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");
            Assert.AreEqual("Private Sub DoSomething()", ((ICommentElement)elements[0]).Text, "Unexpected comment text.");
            Assert.AreEqual("End Sub", ((ICommentElement)elements[1]).Text, "Unexpected comment text.");
        }

        /// <summary>
        /// Tests parsing an invalid comment line.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unhandled element text")]
        public void ParseCommentLineInvalidTest()
        {
            StringReader reader = new StringReader("REMBlah Blah");

            VBParser parser = new VBParser();
            parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a single comment line.
        /// </summary>
        [Test]
        public void ParseCommentLineTest()
        {
            string[] variations =
                {
                    "' Imports System.Text",
                    "REM Imports System.Text"
                };

            foreach (string variation in variations)
            {
                StringReader reader = new StringReader(variation);

                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
                CommentElement commentElement = elements[0] as CommentElement;
                Assert.IsNotNull(commentElement, "Element is not a CommentElement.");
                Assert.AreEqual(CommentType.Line, commentElement.Type, "Unexpected comment type.");
                Assert.AreEqual(" Imports System.Text", commentElement.Text, "Unexpected comment text.");
            }
        }

        /// <summary>
        /// Tests parsing an invalid condition directive.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
           ExpectedMessage = "Cannot arrange files with preprocessor directives containing attributes unassociated to an element")]
        public void ParseConditionDirectiveAttributeUnhandledTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "<Obsolete()>\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a condition directive with an else if condition.
        /// </summary>
        [Test]
        public void ParseConditionDirectiveElseIfTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#ElseIf TEST\r\n" +
                "Protected Sub New()\r\n" +
                "End Sub\r\n" +
                "#Else\r\n" +
                "Private Sub New()\r\n" +
                "End Sub\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

            ConditionDirectiveElement conditionElement =
                elements[0] as ConditionDirectiveElement;
            Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual("DEBUG", conditionElement.ConditionExpression, "Unexpected condition expression.");
            Assert.AreEqual(1, conditionElement.Children.Count, "Unexpected number of child elements.");

            ConstructorElement constructorElement =
                conditionElement.Children[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

            Assert.IsNotNull(conditionElement.ElseCondition, "Expected an Else condition.");
            Assert.AreEqual("TEST", conditionElement.ElseCondition.ConditionExpression, "Unexpected condition expression.");
            Assert.AreEqual(1, conditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

            ConstructorElement elseIfConstructorElement =
                conditionElement.ElseCondition.Children[0] as ConstructorElement;
            Assert.IsNotNull(elseIfConstructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Protected, elseIfConstructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, elseIfConstructorElement.Parameters, "Unexpected parameter string.");

            Assert.IsNotNull(conditionElement.ElseCondition.ElseCondition, "Expected an Else condition.");
            Assert.AreEqual(1, conditionElement.ElseCondition.ElseCondition.Children.Count, "Unexpected number of child elements.");

            ConstructorElement elseConstructorElement =
                conditionElement.ElseCondition.ElseCondition.Children[0] as ConstructorElement;
            Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Private, elseConstructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, elseConstructorElement.Parameters, "Unexpected parameter string.");
        }

        /// <summary>
        /// Tests parsing a condition directive with an else condition.
        /// </summary>
        [Test]
        public void ParseConditionDirectiveElseTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#Else\r\n" +
                "Private Sub New()\r\n" +
                "End Sub\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

            ConditionDirectiveElement conditionElement =
                elements[0] as ConditionDirectiveElement;
            Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual(1, conditionElement.Children.Count, "Unexpected number of child elements.");

            ConstructorElement constructorElement =
                conditionElement.Children[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

            Assert.IsNotNull(conditionElement.ElseCondition, "Expected an Else condition.");
            Assert.AreEqual(1, conditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

            ConstructorElement elseConstructorElement =
                conditionElement.ElseCondition.Children[0] as ConstructorElement;
            Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Private, elseConstructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, elseConstructorElement.Parameters, "Unexpected parameter string.");
        }

        /// <summary>
        /// Tests parsing an invalid condition directive.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected a condition expression")]
        public void ParseConditionDirectiveMissingConditionTest()
        {
            StringReader reader = new StringReader(
                "#If \r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing an invalid condition directive.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected #End If")]
        public void ParseConditionDirectiveMissingEndTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "Public Sub New()\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing an invalid condition directive.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected #End If")]
        public void ParseConditionDirectiveNestedMissingEndTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "#If TEST\r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a condition directive with a nested region.
        /// </summary>
        [Test]
        public void ParseConditionDirectiveNestedRegionTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "#Region \"Constructors\"\r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#End Region\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

            ConditionDirectiveElement conditionElement = elements[0] as ConditionDirectiveElement;
            Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual(1, conditionElement.Children.Count, "Unexpected number of child elements.");
            Assert.IsNull(conditionElement.ElseCondition);

            RegionElement regionElement = conditionElement.Children[0] as RegionElement;
            Assert.IsNotNull(regionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual(1, regionElement.Children.Count, "Unexpected number of child elements.");

            ConstructorElement constructorElement =
                regionElement.Children[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");
        }

        /// <summary>
        /// Tests parsing a nested condition directive.
        /// </summary>
        [Test]
        public void ParseConditionDirectiveNestedTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "#If TEST\r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#Else\r\n" +
                "Private Sub New()" +
                "End Sub\r\n" +
                "#End If\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

            ConditionDirectiveElement conditionElement = elements[0] as ConditionDirectiveElement;
            Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual(1, conditionElement.Children.Count, "Unexpected number of child elements.");
            Assert.IsNull(conditionElement.ElseCondition);

            ConditionDirectiveElement nestedConditionElement = conditionElement.Children[0] as ConditionDirectiveElement;
            Assert.IsNotNull(nestedConditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual(1, nestedConditionElement.Children.Count, "Unexpected number of child elements.");

            ConstructorElement constructorElement =
                nestedConditionElement.Children[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

            Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
            Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

            Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
            Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

            ConstructorElement elseConstructorElement =
                nestedConditionElement.ElseCondition.Children[0] as ConstructorElement;
            Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Private, elseConstructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, elseConstructorElement.Parameters, "Unexpected parameter string.");
        }

        /// <summary>
        /// Tests parsing a nested condition directive.
        /// </summary>
        [Test]
        public void ParseConditionDirectiveNestedWithChildTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#If TEST\r\n" +
                "Protected Sub New()\r\n" +
                "End Sub\r\n" +
                "#Else\r\n" +
                "Private Sub New()\r\n" +
                "End Sub\r\n" +
                "#End If\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

            ConditionDirectiveElement conditionElement = elements[0] as ConditionDirectiveElement;
            Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual(2, conditionElement.Children.Count, "Unexpected number of child elements.");
            Assert.IsNull(conditionElement.ElseCondition);

            ConstructorElement constructorElement =
                conditionElement.Children[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

            ConditionDirectiveElement nestedConditionElement = conditionElement.Children[1] as ConditionDirectiveElement;
            Assert.IsNotNull(nestedConditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual(1, nestedConditionElement.Children.Count, "Unexpected number of child elements.");

            constructorElement =
                nestedConditionElement.Children[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Protected, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

            Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
            Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

            Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
            Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

            ConstructorElement elseConstructorElement =
                nestedConditionElement.ElseCondition.Children[0] as ConstructorElement;
            Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Private, elseConstructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, elseConstructorElement.Parameters, "Unexpected parameter string.");
        }

        /// <summary>
        /// Tests parsing a simple condition directive.
        /// </summary>
        [Test]
        public void ParseConditionDirectiveSimpleTest()
        {
            StringReader reader = new StringReader(
                "#If DEBUG\r\n" +
                "Public Sub New()\r\n" +
                "End Sub\r\n" +
                "#End If");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

            ConditionDirectiveElement conditionElement = elements[0] as ConditionDirectiveElement;
            Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
            Assert.AreEqual("DEBUG", conditionElement.ConditionExpression, "Unexpected condition expression.");
            Assert.AreEqual(1, conditionElement.Children.Count, "Unexpected number of child elements.");

            ConstructorElement constructorElement = conditionElement.Children[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

            Assert.IsNull(conditionElement.ElseCondition);
        }

        /// <summary>
        /// Tests parsing a constructor with parameters.
        /// </summary>
        [Test]
        public void ParseConstructorParametersTest()
        {
            StringReader reader = new StringReader(
                "public sub New(ByVal value As Integer, ByVal max As Integer)\r\n" +
                "end sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            ConstructorElement constructorElement = elements[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual("New", constructorElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual("ByVal value As Integer, ByVal max As Integer", constructorElement.Parameters, "Unexpected parameter string.");
        }

        /// <summary>
        /// Verifies the parsing of constructor members from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParseConstructorsTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                ConstructorElement constructor;

                RegionElement regionElement = classElement.Children[ConstructorRegionIndex] as RegionElement;
                Assert.IsNotNull(regionElement, "Expected a region element.");

                constructor = regionElement.Children[0] as ConstructorElement;
                Assert.IsNotNull(constructor, "Expected a constructor.");
                Assert.AreEqual("New", constructor.Name, "Unexpected constructor name.");
                Assert.AreEqual(CodeAccess.Public, constructor.Access, "Unexpected access level.");
                Assert.AreEqual(3, constructor.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(constructor.IsStatic, "Constructor should not be static.");
                Assert.IsEmpty(constructor.Parameters, "Parameter string should be empty.");

                constructor = regionElement.Children[1] as ConstructorElement;
                Assert.IsNotNull(constructor, "Expected a constructor.");
                Assert.AreEqual("New", constructor.Name, "Unexpected constructor name.");
                Assert.AreEqual(CodeAccess.Internal, constructor.Access, "Unexpected access level.");
                Assert.AreEqual(4, constructor.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(constructor.IsStatic, "Constructor should not be static.");
                Assert.AreEqual("ByVal arrayParam As String()", constructor.Parameters, "Unexpected parameters string.");

                constructor = regionElement.Children[2] as ConstructorElement;
                Assert.IsNotNull(constructor, "Expected a constructor.");
                Assert.AreEqual("New", constructor.Name, "Unexpected constructor name.");
                Assert.AreEqual(CodeAccess.None, constructor.Access, "Unexpected access level.");
                Assert.AreEqual(4, constructor.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.AreEqual(CommentType.Line, constructor.HeaderComments[0].Type);
                Assert.AreEqual(CommentType.Line, constructor.HeaderComments[1].Type);
                Assert.AreEqual(CommentType.Line, constructor.HeaderComments[2].Type);
                Assert.AreEqual(CommentType.XmlLine, constructor.HeaderComments[3].Type);
                Assert.IsTrue(constructor.IsStatic, "Constructor should be static.");
                Assert.IsEmpty(constructor.Parameters, "Parameter string should be empty.");
            }
        }

        /// <summary>
        /// Tests parsing a constructor.
        /// </summary>
        [Test]
        public void ParseConstructorTest()
        {
            StringReader reader = new StringReader(
                "public Sub New()\r\n" +
                "end sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            ConstructorElement constructorElement = elements[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual("New", constructorElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");
        }

        /// <summary>
        /// Verifies the parsing of delegates from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParseDelegatesTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                DelegateElement delegateElement;

                RegionElement regionElement = classElement.Children[DelegateRegionIndex] as RegionElement;
                Assert.IsNotNull(regionElement, "Expected a region element.");

                delegateElement = regionElement.Children[0] as DelegateElement;
                Assert.IsNotNull(delegateElement, "Expected a delegate.");
                Assert.AreEqual("SampleEventHandler", delegateElement.Name, "Unexpected delegate name.");
                Assert.AreEqual(CodeAccess.Public, delegateElement.Access, "Unexpected access level.");
                Assert.AreEqual(5, delegateElement.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(delegateElement.IsStatic, "Delegate should not be static.");
                Assert.IsNull(delegateElement.Type, "Unexpected return type.");
                Assert.IsFalse(delegateElement.IsAbstract, "Delegate should not be abstract.");
                Assert.IsFalse(delegateElement.IsOverride, "Delegate should not be an override.");
                Assert.IsFalse(delegateElement.IsSealed, "Delegate should not be sealed.");
                Assert.IsFalse(delegateElement.IsVirtual, "Delegate should not be virtual.");
                Assert.IsFalse(delegateElement.IsNew, "Delegate should not be new.");
                Assert.AreEqual(0, delegateElement.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual("ByVal sender As Object, ByVal boolParam As Boolean", delegateElement.Parameters, "Unexpected parameter string.");

                delegateElement = regionElement.Children[1] as DelegateElement;
                Assert.IsNotNull(delegateElement, "Expected a delegate.");
                Assert.AreEqual("Compare", delegateElement.Name, "Unexpected delegate name.");
                Assert.AreEqual(CodeAccess.Private, delegateElement.Access, "Unexpected access level.");
                Assert.AreEqual(7, delegateElement.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(delegateElement.IsStatic, "Delegate should not be static.");
                Assert.AreEqual("Integer", delegateElement.Type, "Unexpected return type.");
                Assert.IsFalse(delegateElement.IsAbstract, "Delegate should not be abstract.");
                Assert.IsFalse(delegateElement.IsOverride, "Delegate should not be an override.");
                Assert.IsFalse(delegateElement.IsSealed, "Delegate should not be sealed.");
                Assert.IsFalse(delegateElement.IsVirtual, "Delegate should not be virtual.");
                Assert.IsFalse(delegateElement.IsNew, "Delegate should not be new.");
                Assert.AreEqual(0, delegateElement.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual("ByVal t1 As T, ByVal t2 As T", delegateElement.Parameters, "Unexpected parameter string.");
                Assert.AreEqual(1, delegateElement.TypeParameters.Count, "Unexpected number of type parameters");
                Assert.AreEqual("T", delegateElement.TypeParameters[0].Name, "Unexpected type parameter name.");
                Assert.AreEqual(1, delegateElement.TypeParameters[0].Constraints.Count, "Unexpected number of type parameter constraints.");
                Assert.AreEqual("Class", delegateElement.TypeParameters[0].Constraints[0], "Unexpected type parameter constraint.");
            }
        }

        /// <summary>
        /// Tests parsing an enum.
        /// </summary>
        [Test]
        public void ParseEnumTest()
        {
            StringReader reader = new StringReader(
                "Public Enum TestEnum\r\n" +
                "\tOff = 0\r\n" +
                "\tOn = 1\r\n" +
                "End Enum");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            TypeElement typeElement = elements[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
            Assert.AreEqual("TestEnum", typeElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, typeElement.Access, "Unexpected code access.");
            Assert.AreEqual(TypeElementType.Enum, typeElement.Type, "Unexpected type element type.");
            Assert.IsTrue(
                typeElement.BodyText.Contains("On") &&
                typeElement.BodyText.Contains("Off"),
                "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing an enum with a type specified.
        /// </summary>
        [Test]
        public void ParseEnumTypeTest()
        {
            StringReader reader = new StringReader(
                "Enum TestEnum As Integer\r\n" +
                "\tOff = 0\r\n" +
                "\tOn = 1\r\n" +
                "End Enum");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            TypeElement typeElement = elements[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
            Assert.AreEqual("TestEnum", typeElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.None, typeElement.Access, "Unexpected code access.");
            Assert.AreEqual(TypeElementType.Enum, typeElement.Type, "Unexpected type element type.");
            Assert.IsTrue(
                typeElement.BodyText.Contains("On") &&
                typeElement.BodyText.Contains("Off"),
                "Unexpected body text.");
            Assert.AreEqual(1, typeElement.Interfaces.Count);
            Assert.AreEqual("Integer", typeElement.Interfaces[0].Name);
        }

        /// <summary>
        /// Verifies the parsing of events from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParseEventsTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                EventElement eventElement;

                RegionElement regionElement = classElement.Children[EventRegionIndex] as RegionElement;
                Assert.IsNotNull(regionElement, "Expected a region element.");

                eventElement = regionElement.Children[0] as EventElement;
                Assert.IsNotNull(eventElement, "Expected an event.");
                Assert.AreEqual("SimpleEvent", eventElement.Name, "Unexpected event name.");
                Assert.AreEqual(CodeAccess.Public, eventElement.Access, "Unexpected access level.");
                Assert.AreEqual(3, eventElement.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(eventElement.IsStatic, "Delegate should not be static.");
                Assert.AreEqual("SampleEventHandler", eventElement.Type, "Unexpected return type.");
                Assert.IsFalse(eventElement.IsAbstract, "Event should not be abstract.");
                Assert.IsFalse(eventElement.IsOverride, "Event should not be an override.");
                Assert.IsFalse(eventElement.IsSealed, "Event should not be sealed.");
                Assert.IsFalse(eventElement.IsVirtual, "Event should not be virtual.");
                Assert.IsFalse(eventElement.IsNew, "Event should not be new.");
                Assert.AreEqual(0, eventElement.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsNull(eventElement.BodyText, "Unexpected body text.");

                eventElement = regionElement.Children[1] as EventElement;
                Assert.IsNotNull(eventElement, "Expected an event.");
                Assert.AreEqual("GenericEvent", eventElement.Name, "Unexpected event name.");
                Assert.AreEqual(CodeAccess.Public, eventElement.Access, "Unexpected access level.");
                Assert.AreEqual(3, eventElement.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(eventElement.IsStatic, "Delegate should not be static.");
                Assert.AreEqual("EventHandler(Of EventArgs)", eventElement.Type, "Unexpected return type.");
                Assert.IsFalse(eventElement.IsAbstract, "Event should not be abstract.");
                Assert.IsFalse(eventElement.IsOverride, "Event should not be an override.");
                Assert.IsFalse(eventElement.IsSealed, "Event should not be sealed.");
                Assert.IsFalse(eventElement.IsVirtual, "Event should not be virtual.");
                Assert.IsFalse(eventElement.IsNew, "Event should not be new.");
                Assert.AreEqual(0, eventElement.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsNull(eventElement.BodyText, "Unexpected body text.");

                eventElement = regionElement.Children[2] as EventElement;
                Assert.IsNotNull(eventElement, "Expected an event.");
                Assert.AreEqual("AnotherEvent", eventElement.Name, "Unexpected event name.");
                Assert.AreEqual(CodeAccess.Public, eventElement.Access, "Unexpected access level.");
                Assert.AreEqual(3, eventElement.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(eventElement.IsStatic, "Delegate should not be static.");
                Assert.IsNull(eventElement.Type, "Unexpected return type.");
                Assert.AreEqual("ByVal args As EventArgs", eventElement.Parameters, "Unexpected parameters.");
                Assert.IsFalse(eventElement.IsAbstract, "Event should not be abstract.");
                Assert.IsFalse(eventElement.IsOverride, "Event should not be an override.");
                Assert.IsFalse(eventElement.IsSealed, "Event should not be sealed.");
                Assert.IsFalse(eventElement.IsVirtual, "Event should not be virtual.");
                Assert.IsFalse(eventElement.IsNew, "Event should not be new.");
                Assert.AreEqual(0, eventElement.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsNull(eventElement.BodyText, "Unexpected body text.");

                eventElement = regionElement.Children[3] as EventElement;
                Assert.IsNotNull(eventElement, "Expected an event.");
                Assert.AreEqual("ExplicitEvent", eventElement.Name, "Unexpected event name.");
                Assert.AreEqual(CodeAccess.Public, eventElement.Access, "Unexpected access level.");
                Assert.AreEqual(3, eventElement.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(eventElement.IsStatic, "Delegate should not be static.");
                Assert.AreEqual("SampleEventHandler", eventElement.Type, "Unexpected return type.");
                Assert.IsFalse(eventElement.IsAbstract, "Event should not be abstract.");
                Assert.IsFalse(eventElement.IsOverride, "Event should not be an override.");
                Assert.IsFalse(eventElement.IsSealed, "Event should not be sealed.");
                Assert.IsFalse(eventElement.IsVirtual, "Event should not be virtual.");
                Assert.IsFalse(eventElement.IsNew, "Event should not be new.");
                Assert.AreEqual(0, eventElement.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsTrue(eventElement.BodyText.Contains("AddHandler"), "Unexpected body text.");
                Assert.IsTrue(eventElement.BodyText.Contains("RemoveHandler"), "Unexpected body text.");
                Assert.IsTrue(eventElement.BodyText.Contains("RaiseEvent"), "Unexpected body text.");
            }
        }

        /// <summary>
        /// Tests parsing a generic field.
        /// </summary>
        [Test]
        public void ParseFieldGenericTest()
        {
            StringReader reader = new StringReader(
                "Private Shared _dictionary As Dictionary(Of String, Integer) = new Dictionary(Of String, Integer)");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("_dictionary", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.IsTrue(fieldElement.IsStatic, "Unexpected value for IsStatic.");
            Assert.AreEqual("Dictionary(Of String, Integer)", fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("new Dictionary(Of String, Integer)", fieldElement.InitialValue, "Unexpected initial value.");
            Assert.IsFalse((bool)fieldElement[VBExtendedProperties.WithEvents], "Unexpected value for WithEvents.");
        }

        /// <summary>
        /// Tests parsing multiple fields from a single statement.
        /// </summary>
        [Test]
        public void ParseFieldMultiTest1()
        {
            string[] fieldDefinitions = new string[]
            {
                "Private val1, val2 As Integer",
                "Private val1 , val2 As Integer",
                "Private val1 ,val2 As Integer"
            };

            foreach (string fieldDefinition in fieldDefinitions)
            {
                StringReader reader = new StringReader(fieldDefinition);

                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");
                FieldElement fieldElement1 = elements[0] as FieldElement;
                Assert.IsNotNull(fieldElement1, "Element is not a FieldElement.");
                Assert.AreEqual("val1", fieldElement1.Name, "Unexpected name.");
                Assert.AreEqual(CodeAccess.Private, fieldElement1.Access, "Unexpected code access.");
                Assert.AreEqual("Integer", fieldElement1.Type, "Unexpected member type.");
                Assert.IsNull(fieldElement1.InitialValue, "Unexpected initial value.");

                FieldElement fieldElement2 = elements[1] as FieldElement;
                Assert.IsNotNull(fieldElement2, "Element is not a FieldElement.");
                Assert.AreEqual("val2", fieldElement2.Name, "Unexpected name.");
                Assert.AreEqual(CodeAccess.Private, fieldElement2.Access, "Unexpected code access.");
                Assert.AreEqual("Integer", fieldElement2.Type, "Unexpected member type.");
                Assert.IsNull(fieldElement2.InitialValue, "Unexpected initial value.");
            }
        }

        /// <summary>
        /// Tests parsing multiple fields declared on one line.
        /// </summary>
        [Test]
        public void ParseFieldMultiTest2()
        {
            StringReader reader = new StringReader(
                "Private val1 As Integer, val2 As String");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement1 = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement1, "Element is not a FieldElement.");
            Assert.AreEqual("val1", fieldElement1.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement1.Access, "Unexpected code access.");
            Assert.AreEqual("Integer", fieldElement1.Type, "Unexpected member type.");
            Assert.IsNull(fieldElement1.InitialValue, "Unexpected initial value.");

            FieldElement fieldElement2 = elements[1] as FieldElement;
            Assert.IsNotNull(fieldElement2, "Element is not a FieldElement.");
            Assert.AreEqual("val2", fieldElement2.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement2.Access, "Unexpected code access.");
            Assert.AreEqual("String", fieldElement2.Type, "Unexpected member type.");
            Assert.IsNull(fieldElement2.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Tests parsing a simple field with an underscore at the beginning of the
        /// name.
        /// </summary>
        [Test]
        public void ParseFieldNameWithUnderscoreTest()
        {
            StringReader reader = new StringReader(
                "Private _val As Integer");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("_val", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("Integer", fieldElement.Type, "Unexpected member type.");
            Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Tests parsing a simple field.
        /// </summary>
        [Test]
        public void ParseFieldSimpleTest()
        {
            StringReader reader = new StringReader(
                "Private val As Integer");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("Integer", fieldElement.Type, "Unexpected member type.");
            Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Verifies the parsing of field members from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParseFieldsTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                FieldElement field;

                RegionElement regionElement = classElement.Children[FieldRegionIndex] as RegionElement;
                Assert.IsNotNull(regionElement, "Expected a region element.");

                int fieldIndex = 0;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_simpleField", field.Name, "Unexpected field name.");
                Assert.AreEqual("Boolean", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.None, field.Access, "Unexpected field access level.");
                Assert.AreEqual(true, field[VBExtendedProperties.Dim], "Expected field to be declared Dim.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(1, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be readonly.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_fieldWithInitialVal", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.AreEqual("1", field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_dimField", field.Name, "Unexpected field name.");
                Assert.AreEqual("Boolean", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.AreEqual(true, field[VBExtendedProperties.Dim], "Expected field to be declared Dim.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be readonly.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("StaticStr", field.Name, "Unexpected field name.");
                Assert.AreEqual("String", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Protected, field.Access, "Unexpected field access level.");
                Assert.AreEqual("\"static; string;\"", field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(3, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsTrue(field.IsStatic, "Field should be static.");
                Assert.IsTrue(field.IsReadOnly, "Field should be readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_genericField", field.Name, "Unexpected field name.");
                Assert.AreEqual("Nullable(Of Integer)", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_arrayField", field.Name, "Unexpected field name.");
                Assert.AreEqual("String()", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Protected | CodeAccess.Internal, field.Access, "Unexpected field access level.");
                Assert.AreEqual("{}", field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("internal", field.Name, "Unexpected field name.");
                Assert.AreEqual("Boolean", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Internal, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_attributedField", field.Name, "Unexpected field name.");
                Assert.AreEqual("String", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.AreEqual("Nothing", field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(3, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsTrue(field.IsStatic, "Field should be static.");
                Assert.AreEqual(1, field.Attributes.Count, "Unexpected number of attributes.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("ConstantStr", field.Name, "Unexpected field name.");
                Assert.AreEqual("String", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Public, field.Access, "Unexpected field access level.");
                Assert.IsTrue(field.InitialValue.Contains("\"constant string\""), "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsTrue(field.IsConstant, "Field should be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val1", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val2", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val3", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsTrue(field.IsStatic, "Field should be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val4", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsTrue(field.IsStatic, "Field should be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val5", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsTrue(field.IsStatic, "Field should be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val6", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsTrue(field.IsStatic, "Field should be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val7", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;

                field = regionElement.Children[fieldIndex] as FieldElement;
                Assert.IsNotNull(field, "Expected a field.");
                Assert.AreEqual("_val8", field.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", field.Type, "Unexpected field type.");
                Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
                Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
                Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
                Assert.IsFalse(field.IsStatic, "Field should not be static.");
                Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
                Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
                fieldIndex++;
            }
        }

        /// <summary>
        /// Tests parsing a field with a trailing comment.
        /// </summary>
        [Test]
        public void ParseFieldTrailingCommentTest()
        {
            StringReader reader = new StringReader(
                "Private _commented As Integer  'This is a comment");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("_commented", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("Integer", fieldElement.Type, "Unexpected member type.");
            Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
            Assert.IsNotNull(fieldElement.TrailingComment, "Expected a trailing comment.");
            Assert.AreEqual("This is a comment", fieldElement.TrailingComment.Text);

            reader = new StringReader(
                "Private _commented As Integer = 1 'This is a comment");

            elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("_commented", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("Integer", fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("1", fieldElement.InitialValue, "Unexpected initial value.");
            Assert.IsNotNull(fieldElement.TrailingComment, "Expected a trailing comment.");
            Assert.AreEqual("This is a comment", fieldElement.TrailingComment.Text);
        }

        /// <summary>
        /// Tests parsing a field with an initial value that contains
        /// character symbols.
        /// </summary>
        [Test]
        public void ParseFieldWithCharInitialValueTest()
        {
            StringReader reader = new StringReader(
                "private val As Char = \"X\"");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("Char", fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("\"X\"", fieldElement.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Tests parsing a field with a constructed initial value.
        /// </summary>
        [Test]
        public void ParseFieldWithConstructedGenericValueTest()
        {
            StringReader reader = new StringReader(
                "Dim m_myVar as New Nullable(Of Integer)()");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("m_myVar", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(true, fieldElement[VBExtendedProperties.Dim]);
            Assert.AreEqual(CodeAccess.None, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual(null, fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("New Nullable(Of Integer)()", fieldElement.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Tests parsing a field with a constructed initial value.
        /// </summary>
        [Test]
        public void ParseFieldWithConstructedValueTest()
        {
            StringReader reader = new StringReader(
                "Dim m_myVar as New MyObject(\"A string parameter\")");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("m_myVar", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(true, fieldElement[VBExtendedProperties.Dim]);
            Assert.AreEqual(CodeAccess.None, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual(null, fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("New MyObject(\"A string parameter\")", fieldElement.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Tests parsing a field with the WithEvents attribute.
        /// </summary>
        [Test]
        public void ParseFieldWithEventsTest()
        {
            StringReader reader = new StringReader(
                "Public WithEvents button1 As Button = New Button()");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("button1", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, fieldElement.Access, "Unexpected code access.");
            Assert.IsFalse(fieldElement.IsStatic, "Unexpected value for IsStatic.");
            Assert.AreEqual("Button", fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("New Button()", fieldElement.InitialValue, "Unexpected initial value.");
            Assert.IsTrue((bool)fieldElement[VBExtendedProperties.WithEvents], "Unexpected value for WithEvents.");
        }

        /// <summary>
        /// Tests parsing a field with an initial value.
        /// </summary>
        [Test]
        public void ParseFieldWithInitialValueTest()
        {
            StringReader reader = new StringReader(
                "private custom as Integer = 17");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("custom", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("Integer", fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("17", fieldElement.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Tests parsing a field with a multiline initial value.
        /// </summary>
        [Test]
        public void ParseFieldWithMultipleLineInitialValueTest()
        {
            StringReader reader = new StringReader(
                "Private val As String = _\r\n" +
                "\"This\" & _\r\n" +
                "\"Is\" & _\r\n" +
                "\"a Test\" 'Comment here");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("String", fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("\"This\" & \"Is\" & \"a Test\"", fieldElement.InitialValue, "Unexpected initial value.");
            Assert.AreEqual(0, fieldElement.HeaderComments.Count);
            Assert.IsNotNull(fieldElement.TrailingComment);
            Assert.AreEqual("Comment here", fieldElement.TrailingComment.Text);
        }

        /// <summary>
        /// Tests parsing a field with an initial value that contains
        /// special character symbols.
        /// </summary>
        [Test]
        public void ParseFieldWithSpecialCharInitialValueTest()
        {
            string fieldText = "private val As String = \"Quote\"\"here\"";
            StringReader reader = new StringReader(fieldText);

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            FieldElement fieldElement = elements[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
            Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
            Assert.AreEqual("String", fieldElement.Type, "Unexpected member type.");
            Assert.AreEqual("\"Quote\"\"here\"", fieldElement.InitialValue, "Unexpected initial value.");
        }

        /// <summary>
        /// Tests parsing an external function.
        /// </summary>
        [Test]
        public void ParseFunctionExternalTest()
        {
            string[] variations =
            {
                "Public Declare Ansi Function ExternalFunction Lib \"Some.dll\" Alias \"doit\" (ByVal filename As String) As String",
                "Public Declare Ansi Function _\r\nExternalFunction Lib _\r\n\"Some.dll\" Alias _\r\n \"doit\" (ByVal filename As String) As String"
            };

            foreach (string variation in variations)
            {
                StringReader reader = new StringReader(variation);

                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
                MethodElement methodElement = elements[0] as MethodElement;
                Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
                Assert.AreEqual("ExternalFunction", methodElement.Name, "Unexpected name.");
                Assert.AreEqual(CodeAccess.Public, methodElement.Access, "Unexpected code access.");
                Assert.AreEqual("ByVal filename As String", methodElement.Parameters, "Unexpected parameters.");
                Assert.AreEqual("String", methodElement.Type, "Unexpected return type.");
                Assert.AreEqual("Ansi", methodElement[VBExtendedProperties.ExternalModifier], "Unexpected external modifier.");
                Assert.AreEqual("Some.dll", methodElement[VBExtendedProperties.ExternalLibrary], "Unexpected external library.");
                Assert.AreEqual("doit", methodElement[VBExtendedProperties.ExternalAlias], "Unexpected external alias.");
            }
        }

        /// <summary>
        /// Tests parsing a function.
        /// </summary>
        [Test]
        public void ParseFunctionTest()
        {
            StringReader reader = new StringReader(
                "Private Function GetSomething() As Boolean\r\n" +
                "\tReturn False\r\n" +
                "End Function");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("GetSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
            Assert.AreEqual("Boolean", methodElement.Type, "Unexpected member type.");
        }

        /// <summary>
        /// Tests parsing a function.
        /// </summary>
        [Test]
        public void ParseFunctionUntypedTest()
        {
            StringReader reader = new StringReader(
                "Private Function GetSomething()\r\n" +
                "\tReturn False\r\n" +
                "End Function");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("GetSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, methodElement.Type, "Unexpected member type.");
        }

        /// <summary>
        /// Tests parsing header comments.
        /// </summary>
        [Test]
        public void ParseHeaderCommentsTest()
        {
            StringReader reader = new StringReader(
                "'Comment1\r\n'Comment2\r\nPublic Sub New()\r\nEnd Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            ConstructorElement constructorElement = elements[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual("New", constructorElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");
            Assert.AreEqual(2, constructorElement.HeaderComments.Count, "Unexpected number of header comment lines.");
            Assert.AreEqual("Comment1", constructorElement.HeaderComments[0].Text);
            Assert.AreEqual(CommentType.Line, constructorElement.HeaderComments[0].Type);
            Assert.AreEqual("Comment2", constructorElement.HeaderComments[1].Text);
            Assert.AreEqual(CommentType.Line, constructorElement.HeaderComments[1].Type);
        }

        /// <summary>
        /// Tests parsing header comments.
        /// </summary>
        [Test]
        public void ParseHeaderCommentsXmlTest()
        {
            StringReader reader = new StringReader(
                "'''<summary>Comment1\r\n" +
                "'''Comment2</summary>\r\n" +
                "Public Sub New()\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            ConstructorElement constructorElement = elements[0] as ConstructorElement;
            Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
            Assert.AreEqual("New", constructorElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
            Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");
            Assert.AreEqual(2, constructorElement.HeaderComments.Count, "Unexpected number of header comment lines.");
            Assert.AreEqual("<summary>Comment1", constructorElement.HeaderComments[0].Text);
            Assert.AreEqual(CommentType.XmlLine, constructorElement.HeaderComments[0].Type);
            Assert.AreEqual("Comment2</summary>", constructorElement.HeaderComments[1].Text);
            Assert.AreEqual(CommentType.XmlLine, constructorElement.HeaderComments[1].Type);
        }

        /// <summary>
        /// Tests the parsing of a single namespace with a single interface
        /// definition.
        /// </summary>
        [Test]
        public void ParseInterfaceDefinitionTest()
        {
            VBParser parser = new VBParser();

            VBTestFile testFile = VBTestUtilities.GetInterfaceDefinitionFile();
            using (TextReader reader = testFile.GetReader())
            {
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.IsNotNull(elements, "Code element collection should not be null.");
                Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");

                UsingElement using1 = elements[0] as UsingElement;
                Assert.IsNotNull(using1, "Expected a UsingElement.");
                Assert.AreEqual("System", using1.Name, "Unexpected using name.");

                NamespaceElement namespaceElement = elements[1] as NamespaceElement;
                Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
                Assert.AreEqual("SampleNamespace", namespaceElement.Name, "Unexpected namespace name.");

                Assert.IsNotNull(namespaceElement.Children, "Namespace Children collection should not be null.");
                Assert.AreEqual(1, namespaceElement.Children.Count, "An unexpected number of namespace child elements were parsed.");

                TypeElement interfaceElement = namespaceElement.Children[0] as TypeElement;
                Assert.IsNotNull(interfaceElement, "Expected a TypeElement.");
                Assert.AreEqual(TypeElementType.Interface, interfaceElement.Type, "Expected type to be an interface.");
                Assert.IsFalse(interfaceElement.IsStatic, "Structure should not be static.");
                Assert.IsFalse(interfaceElement.IsSealed, "Interfaces should not be considered sealed.");
                Assert.AreEqual("SampleInterface", interfaceElement.Name, "Unexpected interface name.");
                Assert.AreEqual(3, interfaceElement.HeaderComments.Count, "An unexpected number of class header comment lines were parsed.");
                foreach (ICommentElement comment in
                    interfaceElement.HeaderComments)
                {
                    Assert.AreEqual(CommentType.XmlLine, comment.Type, "Interface header comment should be an XML comment.");
                }
                Assert.AreEqual(CodeAccess.Public, interfaceElement.Access, "Unexpected code access level.");
            }
        }

        /// <summary>
        /// Verifies the parsing of an interface implementation.
        /// </summary>
        [Test]
        public void ParseInterfaceImplementationTest()
        {
            VBTestFile testFile = VBTestUtilities.GetInterfaceImplementationFile();
            using (TextReader reader = testFile.GetReader())
            {
                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(5, elements.Count, "Unexpected number of top-level elements.");

                NamespaceElement namespaceElement = elements[4] as NamespaceElement;
                Assert.IsNotNull(namespaceElement, "Expected a namespace element.");

                Assert.AreEqual(1, namespaceElement.Children.Count, "Unexpected number of namespace elements.");

                TypeElement classElement = namespaceElement.Children[0] as TypeElement;
                Assert.IsNotNull(classElement, "Expected a type element.");
                Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class type.");
                Assert.AreEqual("InterfaceImplementation", classElement.Name, "Unexpected type name.");
                Assert.AreEqual(CodeAccess.Public, classElement.Access, "Unexpected access level.");
                Assert.IsFalse(classElement.IsStatic, "Type should not be static.");
                Assert.IsFalse(classElement.IsAbstract, "Type should not be abstract.");
                Assert.IsFalse(classElement.IsSealed, "Type should not be sealed.");
                Assert.AreEqual(1, classElement.TypeParameters.Count, "Unexpected number of type parameters.");
                Assert.AreEqual("T", classElement.TypeParameters[0].Name, "Unexpected type parameter name.");
                Assert.AreEqual("Component", classElement.Interfaces[0].Name);
                Assert.AreEqual("IList(Of T)", classElement.Interfaces[1].Name);
                Assert.AreEqual("IList", classElement.Interfaces[2].Name);
                Assert.AreEqual("ICollection", classElement.Interfaces[3].Name);
                Assert.AreEqual("IBindingList", classElement.Interfaces[4].Name);
                Assert.AreEqual("ITypedList", classElement.Interfaces[5].Name);

                Assert.AreEqual(41, classElement.Children.Count, "Unexpected number of child elements.");

                InterfaceMemberElement memberElement = null;

                Action<string[]> assertMemberImplements = delegate(string[] interfaceMembers)
                {
                    Assert.AreEqual(interfaceMembers.Length, memberElement.Implements.Count, "Unexpected number of interface implementations.");
                    for (int interfaceIndex = 0; interfaceIndex < interfaceMembers.Length; interfaceIndex++)
                    {
                        Assert.AreEqual(interfaceMembers[interfaceIndex], memberElement.Implements[interfaceIndex].Name, "Unexpected interface implementation specification.");
                    }
                };

                int index = 0;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Add", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection(Of T).Add" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Clear", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection(Of T).Clear", "IList.Clear" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Contains", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection(Of T).Contains" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("CopyTo", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection(Of T).CopyTo" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("CopyTo", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection.CopyTo" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Count", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Integer", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection(Of T).Count", "ICollection.Count" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("IsReadOnly", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection(Of T).IsReadOnly", "IList.IsReadOnly" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Remove", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                Assert.IsTrue((bool)memberElement[VBExtendedProperties.Overloads]);
                assertMemberImplements(new string[] { "System.Collections.Generic.ICollection(Of T).Remove" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("GetEnumerator", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("IEnumerator(Of T)", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IEnumerable(Of T).GetEnumerator" });
                Assert.IsTrue(memberElement.BodyText.Contains("Return Nothing"), "Unexpected body text.");
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("IndexOf", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Integer", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList(Of T).IndexOf" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Insert", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList(Of T).Insert" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Item", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("T", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList(Of T).Item" });
                Assert.IsTrue(memberElement.BodyText.Contains("Set(ByVal value As T)"), "Unexpected body text.");
                Assert.AreEqual(true, memberElement[VBExtendedProperties.Default], "Expected a default property.");
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("RemoveAt", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList(Of T).RemoveAt", "IList.RemoveAt" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("IsSynchronized", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection.IsSynchronized" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("SyncRoot", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Object", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ICollection.SyncRoot" });
                Assert.IsTrue(memberElement.BodyText.Contains("Return Nothing"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("AddObject", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Integer", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList.Add" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("ContainsObject", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList.Contains" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("IndexOfObject", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Integer", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList.IndexOf" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("InsertObject", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList.Insert" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("GetObjectEnumerator", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("IEnumerator", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IEnumerable.GetEnumerator" });
                Assert.IsTrue(memberElement.BodyText.Contains("Return Nothing"), "Unexpected body text.");
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("IsFixedSize", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList.IsFixedSize" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("ObjectItem", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Object", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList.Item" });
                Assert.IsTrue(memberElement.BodyText.Contains("Set(ByVal value As Object)"), "Unexpected body text.");
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Remove", memberElement.Name, "Unexpected member name.");
                Assert.IsTrue((bool)memberElement[VBExtendedProperties.Overloads]);
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IList.Remove" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("AddIndex", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.AddIndex" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("AddNew", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Object", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.AddNew" });
                Assert.IsTrue(memberElement.BodyText.Contains("Return Nothing"), "Unexpected body text.");
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("AllowEdit", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.AllowEdit" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("AllowNew", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.AllowNew" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("AllowRemove", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.AllowRemove" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("ApplySort", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.ApplySort" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("Find", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Integer", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.Find" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("IsSorted", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.IsSorted" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("ListChanged", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.ListChanged" });
                EventElement eventElement = memberElement as EventElement;
                Assert.AreEqual(
                    "ByVal sender As Object, ByVal e As ListChangedEventArgs",
                    eventElement.Parameters,
                    "Unexpected parameters.");
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("RemoveIndex", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.RemoveIndex" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("RemoveSort", memberElement.Name, "Unexpected member name.");
                Assert.IsNull(memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.RemoveSort" });
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("SortDirection", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("ListSortDirection", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.SortDirection" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("SortProperty", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("PropertyDescriptor", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.SortProperty" });
                Assert.IsTrue(memberElement.BodyText.Contains("Return Nothing"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("SupportsChangeNotification", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.SupportsChangeNotification" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("SupportsSearching", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.SupportsSearching" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("SupportsSorting", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("Boolean", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "IBindingList.SupportsSorting" });
                Assert.IsTrue(memberElement.BodyText.Contains("End Get"), "Unexpected body text.");
                Assert.AreEqual(VBKeyword.ReadOnly, memberElement[VBExtendedProperties.AccessModifier]);
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("GetItemProperties", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("PropertyDescriptorCollection", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ITypedList.GetItemProperties" });
                Assert.IsTrue(memberElement.BodyText.Contains("Return Nothing"), "Unexpected body text.");
                index++;

                memberElement = classElement.Children[index] as InterfaceMemberElement;
                Assert.IsNotNull(memberElement);
                Assert.AreEqual("GetListName", memberElement.Name, "Unexpected member name.");
                Assert.AreEqual("String", memberElement.Type, "Unexpected return type.");
                assertMemberImplements(new string[] { "ITypedList.GetListName" });
                Assert.IsTrue(memberElement.BodyText.StartsWith("'Comment here"), "Unexpected body text.");
                Assert.IsTrue(memberElement.BodyText.Contains("Return Nothing"), "Unexpected body text.");
                index++;
            }
        }

        /// <summary>
        /// Tests parsing an invalid type definition.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
           ExpectedMessage = "Expected End Class")]
        public void ParseInvalidTypeDefinitionTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test\r\n" +
                "End Structure");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Verifies that the correct number of members are parsed from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParseMembersCountTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                Assert.AreEqual(
                    NestedTypeRegionIndex + 1,
                    classElement.Children.Count,
                    "Unexpected number of class members.");
            }
        }

        /// <summary>
        /// Tests parsing an abstract method.
        /// </summary>
        [Test]
        public void ParseMethodAbstractTest()
        {
            StringReader reader = new StringReader(
                "Public MustOverride Sub DoSomething()");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, methodElement.Access, "Unexpected code access.");
            Assert.AreEqual(MemberModifiers.Abstract, methodElement.MemberModifiers, "Unexpected member modifiers.");
            Assert.IsNull(methodElement.Type, "Unexpected member type.");
            Assert.IsNull(methodElement.BodyText, "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing a method without a block closing.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unexpected end of file. Expected End Sub")]
        public void ParseMethodBodyUnexpectedEndOfFileTest()
        {
            StringReader reader = new StringReader(
                "private sub DoSomething()");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a method that handles events with a reference whose
        /// name contains an underscore.
        /// </summary>
        [Test]
        public void ParseMethodHandlesNameWithUnderscoreTest()
        {
            StringReader reader = new StringReader(
                "Private Sub selectedControl_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _currentlySelectedControl.LocationChanged\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Expected a method element.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected access.");
            Assert.AreEqual("selectedControl_LocationChanged", methodElement.Name, "Unexpected member name.");
            Assert.IsNull(methodElement.Type, "Unexpected return type.");

            string[] handles = methodElement[VBExtendedProperties.Handles] as string[];
            Assert.IsNotNull(handles, "Handles extended property was not set.");
            Assert.AreEqual(1, handles.Length, "Unexpected number of Handles declarations.");
            Assert.AreEqual("_currentlySelectedControl.LocationChanged", handles[0], "Unexpected Handles declaration.");
        }

        /// <summary>
        /// Tests parsing a method that handles events.
        /// </summary>
        [Test]
        public void ParseMethodHandlesTest()
        {
            StringReader reader = new StringReader(
                "Public Sub Button_Click() _\r\n" +
                "\tHandles Button1.Click, _\r\n" +
                "\t\tButton2.Click\r\n" +
                "\t'Body text line 1\r\n" +
                "\t'Body text line 2\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Expected a method element.");
            Assert.AreEqual(CodeAccess.Public, methodElement.Access, "Unexpected access.");
            Assert.AreEqual("Button_Click", methodElement.Name, "Unexpected member name.");
            Assert.IsNull(methodElement.Type, "Unexpected return type.");

            string[] handles = methodElement[VBExtendedProperties.Handles] as string[];
            Assert.IsNotNull(handles, "Handles extended property was not set.");
            Assert.AreEqual(2, handles.Length, "Unexpected number of Handles declarations.");
            Assert.AreEqual("Button1.Click", handles[0], "Unexpected Handles declaration.");
            Assert.AreEqual("Button2.Click", handles[1], "Unexpected Handles declaration.");

            Assert.AreEqual(
                "'Body text line 1\r\n\t'Body text line 2",
                methodElement.BodyText,
                "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing a method that impelements an interface.
        /// </summary>
        [Test]
        public void ParseMethodImplementsTest()
        {
            StringReader reader = new StringReader(
                "Public Sub Clear() _\r\n" +
                "\tImplements ICollection(Of T).Clear, _\r\n" +
                "\t\tIList.Clear\r\n" +
                "\t'Body text line 1\r\n" +
                "\t'Body text line 2\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Expected a method element.");
            Assert.AreEqual(CodeAccess.Public, methodElement.Access, "Unexpected access.");
            Assert.AreEqual("Clear", methodElement.Name, "Unexpected member name.");
            Assert.IsNull(methodElement.Type, "Unexpected return type.");

            Assert.AreEqual(2, methodElement.Implements.Count, "Unexpected number of interface implementations.");
            Assert.AreEqual(
                "ICollection(Of T).Clear",
                methodElement.Implements[0].Name,
                "Unexpected interface implementation.");
            Assert.AreEqual(
                "IList.Clear",
                methodElement.Implements[1].Name,
                "Unexpected interface implementation.");

            Assert.AreEqual(
                "'Body text line 1\r\n\t'Body text line 2",
                methodElement.BodyText,
                "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing a method with an underscore at the beginning of the name.
        /// </summary>
        [Test]
        public void ParseMethodNameWithUnderscoreTest()
        {
            StringReader reader = new StringReader(
                "Public MustOverride Sub _DoSomething()");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("_DoSomething", methodElement.Name, "Unexpected name.");
        }

        /// <summary>
        /// Tests parsing a method declared as an overload.
        /// </summary>
        [Test]
        public void ParseMethodOverloadTest()
        {
            StringReader reader = new StringReader(
                "Private Overloads Sub DoSomething()\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
            Assert.IsNull(methodElement.Type, "Unexpected member type.");
            Assert.AreEqual(string.Empty, methodElement.BodyText, "Unexpected body text.");
            Assert.AreEqual(true, methodElement[VBExtendedProperties.Overloads], "Unexpected value for extended property Overloads.");
        }

        /// <summary>
        /// Tests parsing a partial method declaration.
        /// </summary>
        [Test]
        public void ParseMethodPartialDeclarationTest()
        {
            StringReader reader = new StringReader(
                "Partial Private Sub DoSomething()\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
            Assert.IsNull(methodElement.Type, "Unexpected member type.");
            Assert.IsTrue(methodElement.IsPartial, "Expected a partial method.");
            Assert.AreEqual(string.Empty, methodElement.BodyText, "Unexpected body text.");
        }

        /// <summary>
        /// Verifies the parsing of methods from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParseMethodsTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                MethodElement method;

                RegionElement regionElement = classElement.Children[MethodRegionIndex] as RegionElement;
                Assert.IsNotNull(regionElement, "Expected a region element.");

                method = regionElement.Children[0] as MethodElement;
                Assert.IsNotNull(method, "Expected a method.");
                Assert.AreEqual("Finalize", method.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Protected, method.Access, "Unexpected access level.");
                Assert.AreEqual(4, method.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(method.IsStatic, "Method should not be static.");
                Assert.IsEmpty(method.Parameters, "Parameter string should be empty.");

                method = regionElement.Children[1] as MethodElement;
                Assert.IsNotNull(method, "Expected a method.");
                Assert.AreEqual("DoSomething", method.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
                Assert.AreEqual(1, method.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(method.IsStatic, "Method should not be static.");
                Assert.IsNull(method.Type, "Unexpected return type.");
                Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
                Assert.IsFalse(method.IsOverride, "Method should not be an override.");
                Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
                Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
                Assert.IsFalse(method.IsNew, "Method should not be new.");
                Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(string.Empty, method.Parameters, "Unexpected parameter string.");

                method = regionElement.Children[2] as MethodElement;
                Assert.IsNotNull(method, "Expected a method.");
                Assert.AreEqual("ToString", method.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
                Assert.AreEqual(3, method.HeaderComments.Count, "Unexpecte number of comments.");
                Assert.IsFalse(method.IsStatic, "Method should not be static.");
                Assert.AreEqual("String", method.Type, "Unexpected return type.");
                Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
                Assert.IsTrue(method.IsOverride, "Method should be an override.");
                Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
                Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
                Assert.IsFalse(method.IsNew, "Method should not be new.");
                Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(string.Empty, method.Parameters, "Unexpected parameter string.");

                method = regionElement.Children[3] as MethodElement;
                Assert.IsNotNull(method, "Expected a method.");
                Assert.AreEqual("GetBoolValue", method.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Private, method.Access, "Unexpected access level.");
                Assert.AreEqual(6, method.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(method.IsStatic, "Method should not be static.");
                Assert.AreEqual("Boolean", method.Type, "Unexpected return type.");
                Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
                Assert.IsFalse(method.IsOverride, "Method should not be an override.");
                Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
                Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
                Assert.IsFalse(method.IsNew, "Method should not be new.");
                Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual("ByVal intParam As Integer, ByVal stringParam As String", method.Parameters, "Unexpected parameter string.");
                Assert.IsTrue(method.BodyText.Contains("Return True"), "Unexpected body text.");

                method = regionElement.Children[4] as MethodElement;
                Assert.IsNotNull(method, "Expected a method.");
                Assert.AreEqual("GetWithParamAttributes", method.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Internal, method.Access, "Unexpected access level.");
                Assert.AreEqual(5, method.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsTrue(method.IsStatic, "Method should be static.");
                Assert.AreEqual("Nullable(Of Integer)", method.Type, "Unexpected return type.");
                Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
                Assert.IsFalse(method.IsOverride, "Method should not be an override.");
                Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
                Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
                Assert.IsFalse(method.IsNew, "Method should not be new.");
                Assert.AreEqual(1, method.Attributes.Count, "Unexpected number of attributes.");
                Assert.IsTrue(
                    method.Parameters.Contains(
                    "<Description(\"Int parameter\")>"),
                    "Unexpected params string.");
                Assert.IsTrue(
                    method.Parameters.Contains(
                    "ByVal intParam As Integer"),
                    "Unexpected params string.");
                Assert.IsTrue(
                    method.Parameters.Contains(
                    "<Description(\"String parameter\")>"),
                    "Unexpected params string.");
                Assert.IsTrue(
                    method.Parameters.Contains(
                    "ByVal stringParam As String"),
                    "Unexpected params string.");

                method = regionElement.Children[5] as MethodElement;
                Assert.IsNotNull(method, "Expected a method.");
                Assert.AreEqual("GetWithTypeParameters", method.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
                Assert.AreEqual(3, method.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(method.IsStatic, "Method should not be static.");
                Assert.AreEqual("Boolean", method.Type, "Unexpected return type.");
                Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
                Assert.IsFalse(method.IsOverride, "Method should not be an override.");
                Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
                Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
                Assert.IsFalse(method.IsNew, "Method should not be new.");
                Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(
                    "ByVal typeParam1 As Action(Of T1), ByVal typeParam2 As Action(Of T2)",
                    method.Parameters.Trim(),
                    "Unexpected params string.");
                Assert.AreEqual(2, method.TypeParameters.Count, "Unexpected number of type parameters.");
                Assert.AreEqual("T1", method.TypeParameters[0].Name, "Unexpected type parameter name.");
                Assert.AreEqual(2, method.TypeParameters[0].Constraints.Count, "Unexpected type parameter constraints.");
                Assert.AreEqual("T2", method.TypeParameters[1].Name, "Unexpected type parameter name.");
                Assert.AreEqual(2, method.TypeParameters[1].Constraints.Count, "Unexpected type parameter constraints.");

                //
                // Import method
                //
                method = regionElement.Children[6] as MethodElement;
                Assert.IsNotNull(method, "Expected a method.");
                Assert.AreEqual("MessageBox", method.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
                Assert.AreEqual(0, method.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsTrue(method.IsStatic, "Method should be static.");
                Assert.AreEqual("Integer", method.Type, "Unexpected return type.");
                Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
                Assert.IsFalse(method.IsExternal, "Method should not be external.");
                Assert.IsFalse(method.IsOverride, "Method should not be an override.");
                Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
                Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
                Assert.IsFalse(method.IsNew, "Method should not be new.");
                Assert.AreEqual(1, method.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(
                    "ByVal h As Integer, ByVal m As String, ByVal c As String, ByVal type As Integer",
                    method.Parameters.Trim(),
                    "Unexpected params string.");
                Assert.AreEqual(0, method.TypeParameters.Count, "Unexpected number of type parameters.");
            }
        }

        /// <summary>
        /// Tests parsing a method with a block close in a comment of the 
        /// body text.
        /// </summary>
        [Test]
        public void ParseMethodWithEndBlockLineCommentTest()
        {
            StringReader reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "\t'Test End Block\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
            Assert.IsNull(methodElement.Type, "Unexpected member type.");
            Assert.AreEqual("'Test End Block", methodElement.BodyText, "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing a method with a block close in the body text.
        /// </summary>
        [Test]
        public void ParseMethodWithEndBlockTextTest()
        {
            StringReader reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "\tConsole.WriteLine(\"End Sub\")\r\n" +
                "\tConsole.WriteLine()\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
            Assert.IsNull(methodElement.Type, "Unexpected member type.");
            Assert.AreEqual("Console.WriteLine(\"End Sub\")\r\n\tConsole.WriteLine()", methodElement.BodyText, "Unexpected body text.");
        }

        /// <summary>
        /// Tests parsing a simple module.
        /// </summary>
        [Test]
        public void ParseModuleSimpleTest()
        {
            StringReader reader = new StringReader(
                "public module Test\r\n" +
                "end module");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            TypeElement typeElement = elements[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
            Assert.AreEqual("Test", typeElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Public, typeElement.Access, "Unexpected code access.");
            Assert.AreEqual(TypeElementType.Module, typeElement.Type, "Unexpected type element type.");
        }

        /// <summary>
        /// Tests the parsing of a single namespace with a multiple class
        /// definitions.
        /// </summary>
        [Test]
        public void ParseMultiClassDefinitionTest()
        {
            VBParser parser = new VBParser();

            VBTestFile testFile = VBTestUtilities.GetMultiClassDefinitionFile();
            using (TextReader reader = testFile.GetReader())
            {
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.IsNotNull(elements, "Code element collection should not be null.");
                Assert.AreEqual(6, elements.Count, "An unexpected number of elements were parsed.");

                UsingElement using1 = elements[0] as UsingElement;
                Assert.IsNotNull(using1, "Expected a UsingElement.");
                Assert.AreEqual("System", using1.Name, "Unexpected using name.");

                UsingElement using2 = elements[1] as UsingElement;
                Assert.IsNotNull(using2, "Expected a UsingElement.");
                Assert.AreEqual("System.Collections.Generic", using2.Name, "Unexpected using name.");

                UsingElement using3 = elements[2] as UsingElement;
                Assert.IsNotNull(using3, "Expected a UsingElement.");
                Assert.AreEqual("System.Text", using3.Name, "Unexpected using name.");

                UsingElement using4 = elements[3] as UsingElement;
                Assert.IsNotNull(using4, "Expected a UsingElement.");
                Assert.AreEqual("System.ComponentModel", using4.Name, "Unexpected using name.");

                NamespaceElement namespaceElement = elements[4] as NamespaceElement;
                Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
                Assert.AreEqual("SampleNamespace", namespaceElement.Name, "Unexpected namespace name.");

                Assert.IsNotNull(namespaceElement.Children, "Namespace Children collection should not be null.");
                Assert.AreEqual(9, namespaceElement.Children.Count, "An unexpected number of namespace child elements were parsed.");

                //
                // Sample class 1
                //
                TypeElement classElement1 = namespaceElement.Children[0] as TypeElement;
                Assert.IsNotNull(classElement1, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass1", classElement1.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Public, classElement1.Access, "Unexpected class code access level.");
                Assert.IsFalse(classElement1.IsStatic, "Class should not be static.");
                Assert.IsFalse(classElement1.IsSealed, "Class should not be sealed.");

                //
                // Sample class 2
                //
                TypeElement classElement2 = namespaceElement.Children[1] as TypeElement;
                Assert.IsNotNull(classElement2, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass2", classElement2.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.None, classElement2.Access, "Unexpected class code access level.");
                Assert.AreEqual(1, classElement2.Interfaces.Count, "Unexpected number of implemented interfaces.");
                Assert.AreEqual("Exception", classElement2.Interfaces[0].Name, "Unexpected implemented interface name.");
                Assert.IsFalse(classElement2.IsStatic, "Class should not be static.");
                Assert.IsFalse(classElement2.IsSealed, "Class should not be sealed.");

                //
                // Sample class 3
                //
                TypeElement classElement3 = namespaceElement.Children[2] as TypeElement;
                Assert.IsNotNull(classElement3, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass3", classElement3.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Internal, classElement3.Access, "Unexpected class code access level.");
                Assert.AreEqual(3, classElement3.Interfaces.Count, "Unexpected base class name.");
                Assert.AreEqual("List(Of Integer)", classElement3.Interfaces[0].Name, "Unexpected interface name.");
                Assert.AreEqual("IDisposable", classElement3.Interfaces[1].Name, "Unexpected interface name.");
                Assert.AreEqual("IComparable", classElement3.Interfaces[2].Name, "Unexpected interface name.");
                Assert.IsFalse(classElement3.IsStatic, "Class should not be static.");
                Assert.IsFalse(classElement3.IsSealed, "Class should not be sealed.");

                MethodElement disposeMethod = classElement3.Children[0] as MethodElement;
                Assert.IsNotNull(disposeMethod, "Expected a method element.");
                Assert.AreEqual("Dispose", disposeMethod.Name, "Unexpected method name.");
                Assert.AreEqual(CodeAccess.Public, disposeMethod.Access, "Unexpected method access.");
                Assert.IsNull(disposeMethod.Type, "Unexpected return type.");
                Assert.AreEqual(1, disposeMethod.Implements.Count, "Unexpected number of implemenation declarations.");
                Assert.AreEqual("IDisposable.Dispose", disposeMethod.Implements[0].Name, "Unexpected implementation declaration.");

                //
                // Sample class 4
                //
                TypeElement classElement4 = namespaceElement.Children[3] as TypeElement;
                Assert.IsNotNull(classElement4, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass4", classElement4.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Internal, classElement4.Access, "Unexpected class code access level.");
                Assert.IsFalse(classElement4.IsStatic, "Class should not be static.");
                Assert.IsTrue(classElement4.IsSealed, "Class should be sealed.");
                Assert.AreEqual(2, classElement4.Interfaces.Count, "Unexpected base class name.");
                Assert.AreEqual("IComparable", classElement4.Interfaces[0].Name, "Unexpected interface name.");
                Assert.AreEqual("IDisposable", classElement4.Interfaces[1].Name, "Unexpected interface name.");

                Assert.AreEqual(2, classElement4.TypeParameters.Count, "Unexpected number of type parameters.");

                TypeParameter parameter1 = classElement4.TypeParameters[0];
                Assert.AreEqual("T1", parameter1.Name, "Unexpected type parameter name.");

                // where T1 : IComparable, IConvertible, new()
                Assert.AreEqual(3, parameter1.Constraints.Count, "Unexpected number of type parameter constraints.");
                Assert.AreEqual("IComparable", parameter1.Constraints[0], "Unexpected type parameter contraint.");
                Assert.AreEqual("IConvertible", parameter1.Constraints[1], "Unexpected type parameter contraint.");
                Assert.AreEqual("New", parameter1.Constraints[2], "Unexpected type parameter contraint.");

                TypeParameter parameter2 = classElement4.TypeParameters[1];
                Assert.AreEqual("T2", parameter2.Name, "Unexpected type parameter name.");

                // where T2 : class, IComparable, IConvertible, new()
                Assert.AreEqual(4, parameter2.Constraints.Count, "Unexpected number of type parameter constraints.");
                Assert.AreEqual("Class", parameter2.Constraints[0], "Unexpected type parameter contraint.");
                Assert.AreEqual("IComparable(Of T2)", parameter2.Constraints[1], "Unexpected type parameter contraint.");
                Assert.AreEqual("Global.System.IConvertible", parameter2.Constraints[2], "Unexpected type parameter contraint.");
                Assert.AreEqual("New", parameter2.Constraints[3], "Unexpected type parameter contraint.");

                //
                // Sample class 5
                //
                TypeElement classElement5 = namespaceElement.Children[4] as TypeElement;
                Assert.IsNotNull(classElement5, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass5", classElement5.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Internal, classElement5.Access, "Unexpected class code access level.");
                Assert.IsFalse(classElement5.IsStatic, "Class should not be static.");
                Assert.IsTrue(classElement5.IsSealed, "Class should be sealed.");

                //
                // Sample class 6
                //
                TypeElement classElement6 = namespaceElement.Children[5] as TypeElement;
                Assert.IsNotNull(classElement6, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass6", classElement6.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Public, classElement6.Access, "Unexpected class code access level.");
                Assert.IsFalse(classElement6.IsStatic, "Class should not be static.");
                Assert.IsTrue(classElement6.IsSealed, "Class should be sealed.");

                //
                // Sample class 7
                //
                TypeElement classElement7 = namespaceElement.Children[6] as TypeElement;
                Assert.IsNotNull(classElement7, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass7", classElement7.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Public, classElement7.Access, "Unexpected class code access level.");
                Assert.IsFalse(classElement7.IsStatic, "Class should not be static.");
                Assert.IsTrue(classElement7.IsSealed, "Class should be sealed.");
                Assert.AreEqual(2, classElement7.Interfaces.Count, "Unexpected number of interfaces.");
                Assert.AreEqual("Global.System.IDisposable", classElement7.Interfaces[0].Name, "Unexpected interface name.");
                Assert.AreEqual("IComparable(Of Integer)", classElement7.Interfaces[1].Name, "Unexpected interface name.");

                //
                // Sample class 8
                //
                TypeElement classElement8 = namespaceElement.Children[7] as TypeElement;
                Assert.IsNotNull(classElement8, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass8", classElement8.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Public, classElement8.Access, "Unexpected class code access level.");
                Assert.IsFalse(classElement8.IsStatic, "Class should not be static.");
                Assert.IsFalse(classElement8.IsSealed, "Class should not be sealed.");
                Assert.IsTrue(classElement8.IsAbstract, "Class should be abstract.");

                //
                // Sample class 9
                //
                TypeElement classElement9 = namespaceElement.Children[8] as TypeElement;
                Assert.IsNotNull(classElement9, "Expected a TypeElement.");
                Assert.AreEqual("SampleClass9", classElement9.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Internal, classElement9.Access, "Unexpected class code access level.");
                Assert.IsFalse(classElement9.IsStatic, "Class should not be static.");
                Assert.IsTrue(classElement9.IsSealed, "Class should be sealed.");
                Assert.AreEqual(0, classElement9.Interfaces.Count, "Unexpected base class name.");
                Assert.AreEqual(2, classElement9.TypeParameters.Count, "Unexpected number of type parameters.");

                //
                // Global class
                //
                TypeElement globalClassElement = elements[5] as TypeElement;
                Assert.IsNotNull(globalClassElement, "Expected a TypeElement.");
                Assert.AreEqual("GlobalClass", globalClassElement.Name, "Unexpected class name.");
                Assert.AreEqual(CodeAccess.Public, globalClassElement.Access, "Unexpected class code access level.");
                Assert.IsFalse(globalClassElement.IsStatic, "Class should not be static.");
                Assert.IsFalse(globalClassElement.IsSealed, "Class should not be sealed.");
                Assert.AreEqual(0, globalClassElement.Interfaces.Count, "Unexpected base class name.");
                Assert.AreEqual(0, globalClassElement.TypeParameters.Count, "Unexpected number of type parameters.");
            }
        }

        /// <summary>
        /// Tests the parsing of multiple namespaces.
        /// </summary>
        [Test]
        public void ParseMultipleNamespaceTest()
        {
            VBParser parser = new VBParser();

            VBTestFile testFile = VBTestUtilities.GetMultipleNamespaceFile();
            using (TextReader reader = testFile.GetReader())
            {
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.IsNotNull(elements, "Code element collection should not be null.");
                Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");

                NamespaceElement namespaceElement1 = elements[0] as NamespaceElement;
                Assert.IsNotNull(namespaceElement1, "Expected a NamespaceElement.");
                Assert.AreEqual("SampleNamespace1", namespaceElement1.Name, "Unexpected namespace name.");

                NamespaceElement namespaceElement2 = elements[1] as NamespaceElement;
                Assert.IsNotNull(namespaceElement2, "Expected a NamespaceElement.");
                Assert.AreEqual("SampleNamespace2", namespaceElement2.Name, "Unexpected namespace name.");
                Assert.IsTrue(namespaceElement2.HeaderComments.Count > 0);
            }
        }

        /// <summary>
        /// Tests parsing of a namepsace where an opening 
        /// brace is expected.";
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected End")]
        public void ParseNamespaceExpectedEnd()
        {
            StringReader reader = new StringReader(
                "Namespace TestNamespace");

            VBParser parser = new VBParser();
            parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing of a namepsace where a closing 
        /// brace is expected.";
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected End Namespace")]
        public void ParseNamespaceExpectedNamespace()
        {
            StringReader reader = new StringReader(
                "Namespace TestNamespace\r\n" +
                "End");

            VBParser parser = new VBParser();
            parser.Parse(reader);
        }

        /// <summary>
        /// Verifies the parsing of nested types from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParseNestedTypesTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                TypeElement type;

                RegionElement regionElement = classElement.Children[NestedTypeRegionIndex] as RegionElement;
                Assert.IsNotNull(regionElement, "Expected a region element.");

                type = regionElement.Children[0] as TypeElement;
                Assert.IsNotNull(type, "Expected a type.");
                Assert.AreEqual(TypeElementType.Enum, type.Type, "Unexpected type element type.");
                Assert.AreEqual("SampleEnum", type.Name, "Unexpected type name.");
                Assert.AreEqual(CodeAccess.Private, type.Access, "Unexpected access level.");
                Assert.AreEqual(3, type.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(type.IsStatic, "Type should not be static.");
                Assert.IsFalse(type.IsAbstract, "Type should not be abstract.");
                Assert.IsFalse(type.IsSealed, "Type should not be sealed.");
                Assert.AreEqual(1, type.Attributes.Count, "Unexpected number of attributes.");

                type = regionElement.Children[1] as TypeElement;
                Assert.IsNotNull(type, "Expected a type.");
                Assert.AreEqual(TypeElementType.Structure, type.Type, "Unexpected type element type.");
                Assert.AreEqual("SampleStructure", type.Name, "Unexpected type name.");
                Assert.AreEqual(CodeAccess.Public, type.Access, "Unexpected access level.");
                Assert.AreEqual(3, type.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(type.IsStatic, "Type should not be static.");
                Assert.IsFalse(type.IsAbstract, "Type should not be abstract.");
                Assert.IsFalse(type.IsSealed, "Type should not be sealed.");
                Assert.AreEqual(0, type.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(2, type.Children.Count, "Unexpected number of child elements.");

                type = regionElement.Children[2] as TypeElement;
                Assert.IsNotNull(type, "Expected a type.");
                Assert.AreEqual(TypeElementType.Class, type.Type, "Unexpected type element type.");
                Assert.AreEqual("SampleNestedClass", type.Name, "Unexpected type name.");
                Assert.AreEqual(CodeAccess.Private, type.Access, "Unexpected access level.");
                Assert.AreEqual(3, type.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(type.IsStatic, "Type should not be static.");
                Assert.IsFalse(type.IsAbstract, "Type should not be abstract.");
                Assert.IsFalse(type.IsSealed, "Type should not be sealed.");
                Assert.AreEqual(0, type.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(1, type.Children.Count, "Unexpected number of child elements.");

                type = regionElement.Children[3] as TypeElement;
                Assert.IsNotNull(type, "Expected a type.");
                Assert.AreEqual(TypeElementType.Class, type.Type, "Unexpected type element type.");
                Assert.AreEqual("SampleNestedStaticClass", type.Name, "Unexpected type name.");
                Assert.AreEqual(CodeAccess.Internal, type.Access, "Unexpected access level.");
                Assert.AreEqual(3, type.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(type.IsStatic, "Type should not be static.");
                Assert.IsFalse(type.IsAbstract, "Type should not be abstract.");
                Assert.IsTrue(type.IsSealed, "Type should be sealed.");
                Assert.AreEqual(0, type.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(2, type.Children.Count, "Unexpected number of child elements.");
            }
        }

        /// <summary>
        /// Tests parsing a null stream.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullStreamTest()
        {
            VBParser parser = new VBParser();
            parser.Parse(null);
        }

        /// <summary>
        /// Verifies the parsing of operators.
        /// </summary>
        [Test]
        public void ParseOperatorsTest()
        {
            VBTestFile testFile = VBTestUtilities.GetOperatorsFile();
            using (TextReader reader = testFile.GetReader())
            {
                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(2, elements.Count, "Unexpected number of top-level elements.");

                NamespaceElement namespaceElement = elements[1] as NamespaceElement;
                Assert.IsNotNull(namespaceElement, "Expected a namespace element.");

                Assert.AreEqual(1, namespaceElement.Children.Count, "Unexpected number of namespace elements.");

                TypeElement classElement = namespaceElement.Children[0] as TypeElement;
                Assert.IsNotNull(classElement, "Expected a type element.");
                Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class type.");
                Assert.AreEqual("Fraction", classElement.Name, "Unexpected type name.");
                Assert.AreEqual(CodeAccess.Public, classElement.Access, "Unexpected access level.");
                Assert.AreEqual(3, classElement.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(classElement.IsStatic, "Type should not be static.");
                Assert.IsFalse(classElement.IsAbstract, "Type should not be abstract.");
                Assert.IsFalse(classElement.IsSealed, "Type should not be sealed.");
                Assert.AreEqual(0, classElement.Attributes.Count, "Unexpected number of attributes.");

                Assert.AreEqual(12, classElement.Children.Count, "Unexpected number of child elements.");

                FieldElement fieldElement = classElement.Children[0] as FieldElement;
                Assert.IsNotNull(fieldElement, "Expected a field element.");
                Assert.AreEqual("num", fieldElement.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", fieldElement.Type, "Unexpected field type.");

                fieldElement = classElement.Children[1] as FieldElement;
                Assert.IsNotNull(fieldElement, "Expected a field element.");
                Assert.AreEqual("den", fieldElement.Name, "Unexpected field name.");
                Assert.AreEqual("Integer", fieldElement.Type, "Unexpected field type.");

                ConstructorElement constructorElement = classElement.Children[2] as ConstructorElement;
                Assert.IsNotNull(constructorElement, "Expected a constructor element.");
                Assert.AreEqual("New", constructorElement.Name, "Unexpected constructor name.");
                Assert.AreEqual("ByVal num As Integer, ByVal den As Integer", constructorElement.Parameters, "Unexpected constructor parameters.");

                MethodElement operatorElement = classElement.Children[3] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("+", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Fraction", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal a As Fraction, ByVal b As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Return"), "Unexpected operator body text.");

                operatorElement = classElement.Children[4] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("*", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Fraction", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal a As Fraction, ByVal b As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Return"), "Unexpected operator body text.");

                operatorElement = classElement.Children[5] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("/", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Fraction", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal a As Fraction, ByVal b As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Return"), "Unexpected operator body text.");

                operatorElement = classElement.Children[6] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("=", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Boolean", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal a As Fraction, ByVal b As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Return"), "Unexpected operator body text.");

                operatorElement = classElement.Children[7] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("<>", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Boolean", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal a As Fraction, ByVal b As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Return"), "Unexpected operator body text.");

                operatorElement = classElement.Children[8] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("<=", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Boolean", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal a As Fraction, ByVal b As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Throw"), "Unexpected operator body text.");

                operatorElement = classElement.Children[9] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual(">=", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Boolean", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal a As Fraction, ByVal b As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Throw"), "Unexpected operator body text.");

                operatorElement = classElement.Children[10] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("CType", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.Implicit, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Double", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal f As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Return"), "Unexpected operator body text.");

                operatorElement = classElement.Children[11] as MethodElement;
                Assert.IsNotNull(operatorElement, "Expected a method element.");
                Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
                Assert.AreEqual("CType", operatorElement.Name, "Unexpected operator name.");
                Assert.AreEqual(OperatorType.Explicit, operatorElement.OperatorType, "Unexpected operator attributes.");
                Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
                Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
                Assert.AreEqual("Decimal", operatorElement.Type, "Unexpected operator return type.");
                Assert.AreEqual("ByVal f As Fraction", operatorElement.Parameters, "Unexpected operator parameters.");
                Assert.IsTrue(operatorElement.BodyText.Contains("Return"), "Unexpected operator body text.");
            }
        }

        /// <summary>
        /// Tests parsing an option statement.
        /// </summary>
        [Test]
        public void ParseOptionTest()
        {
            string[] optionVariations = new string[]
            {
                "Option Explicit On",
                "Option _\r\nExplicit _\r\nOn"
            };

            foreach (string optionStr in optionVariations)
            {
                StringReader reader = new StringReader(optionStr);

                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(
                    1, elements.Count, "An unexpected number of elements were parsed from '{0}'", optionStr);
                AttributeElement optionElement = elements[0] as AttributeElement;
                Assert.IsNotNull(optionElement, "Element is not a AttributeElement.  '{0}'", optionStr);
                Assert.AreEqual(
                    "Option Explicit On",
                    optionElement.BodyText,
                    "Unexpected text from '{0}'",
                    optionStr);
            }
        }

        /// <summary>
        /// Verifies the parsing of properties members from the 
        /// sample class.
        /// </summary>
        [Test]
        public void ParsePropertiesTest()
        {
            VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
            using (TextReader reader = testFile.GetReader())
            {
                TypeElement classElement = GetMembersTestClass(reader);

                PropertyElement property;

                RegionElement regionElement = classElement.Children[PropertyRegionIndex] as RegionElement;
                Assert.IsNotNull(regionElement, "Expected a region element.");

                property = regionElement.Children[0] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("SimpleProperty", property.Name, "Unexpected property name.");
                Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
                Assert.AreEqual(3, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(property.IsStatic, "Property should not be static.");
                Assert.AreEqual("Boolean", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
                Assert.IsFalse((bool)property["Default"], "Property should not be a default property.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsTrue(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");

                property = regionElement.Children[1] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("ProtectedProperty", property.Name, "Unexpected property name.");
                Assert.AreEqual(CodeAccess.Protected, property.Access, "Unexpected access level.");
                Assert.AreEqual(3, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(property.IsStatic, "Property should not be static.");
                Assert.AreEqual("Integer", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsTrue(property.IsVirtual, "Property should be virtual.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsFalse(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");

                property = regionElement.Children[2] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("StaticProperty", property.Name, "Unexpected property name.");
                Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
                Assert.AreEqual(4, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsTrue(property.IsStatic, "Property should be static.");
                Assert.AreEqual("String", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsFalse(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");
                Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");

                property = regionElement.Children[3] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("AttributedProperty", property.Name, "Unexpected property name.");
                Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
                Assert.AreEqual(2, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsTrue(property.IsStatic, "Property should be static.");
                Assert.AreEqual("String", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsFalse(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");
                Assert.AreEqual(2, property.Attributes.Count, "Unexpected number of attributes.");

                property = regionElement.Children[4] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("GenericProperty", property.Name, "Unexpected property name.");
                Assert.AreEqual(CodeAccess.Internal, property.Access, "Unexpected access level.");
                Assert.AreEqual(6, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(property.IsStatic, "Property should not be static.");
                Assert.AreEqual("Nullable(Of Integer)", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
                Assert.IsFalse(property.IsNew, "Property should not be new.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsTrue(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");
                Assert.AreEqual(1, property.Attributes.Count, "Unexpected number of attributes.");
                IAttributeElement attributeElement = property.Attributes[0];
                Assert.IsNull(attributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("Obsolete", property.Attributes[0].Name);
                Assert.AreEqual(string.Empty, attributeElement.BodyText, "Unexpected attribute body text.");
                IAttributeElement childAttributeElement = attributeElement.Children[0] as IAttributeElement;
                Assert.IsNull(childAttributeElement.Target, "Unexpected attribute target.");
                Assert.AreEqual("Description", childAttributeElement.Name, "Unexpected attribute name.");
                Assert.AreEqual("\"Multiple attribute property.\"", childAttributeElement.BodyText, "Unexpected attribute text.");

                property = regionElement.Children[5] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("ArrayProperty", property.Name, "Unexpected property name.");
                Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
                Assert.AreEqual(3, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(property.IsStatic, "Property should not be static.");
                Assert.AreEqual("String()", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
                Assert.IsTrue(property.IsNew, "Property should be new.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsFalse(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");
                Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");

                //
                // Indexer property
                //
                property = regionElement.Children[6] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("Item", property.Name, "Unexpected property name.");
                Assert.AreEqual("ByVal index As Integer", property.IndexParameter, "Unexpected index parameter.");
                Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
                Assert.AreEqual(5, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(property.IsStatic, "Property should not be static.");
                Assert.AreEqual("String", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
                Assert.IsFalse(property.IsNew, "Property should not be new.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsTrue(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");
                Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(true, property[VBExtendedProperties.Default]);

                property = regionElement.Children[7] as PropertyElement;
                Assert.IsNotNull(property, "Expected a property.");
                Assert.AreEqual("Item", property.Name, "Unexpected property name.");
                Assert.AreEqual("ByVal string1 As String, ByVal string2 As String", property.IndexParameter, "Unexpected index parameter.");
                Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
                Assert.AreEqual(0, property.HeaderComments.Count, "Unexpected number of header comment lines.");
                Assert.IsFalse(property.IsStatic, "Property should not be static.");
                Assert.AreEqual("Integer", property.Type, "Unexpected property type.");
                Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
                Assert.IsFalse(property.IsOverride, "Property should not be an override.");
                Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
                Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
                Assert.IsFalse(property.IsNew, "Property should not be new.");
                Assert.IsTrue(
                    property.BodyText.Contains("Get"), "Unexpected body text.");
                Assert.IsTrue(
                    property.BodyText.Contains("Set"), "Unexpeced body text.");
                Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");
                Assert.AreEqual(true, property[VBExtendedProperties.Default]);
            }
        }

        /// <summary>
        /// Tests parsing unmatched nested regions that use comment directives.  
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Missing end region directive for 'Fields'")]
        public void ParseRegionCommentDirectiveMismatchedTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test\r\n" +
                "\t' $(Begin) Fields\r\n" +
                "\t'$( Begin )  Private\r\n" +
                "\tPrivate _test As Boolean = false\r\n" +
                "\t' $(End) Private\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing nested regions that use comment directives.  
        /// </summary>
        [Test]
        public void ParseRegionCommentDirectiveTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test\r\n" +
                "\t' $(Begin) Fields\r\n" +
                "\t'$( Begin )  Private\r\n" +
                "\tPrivate _test As Boolean = false\r\n" +
                "\t' $(End) Private\r\n" +
                "\t'$(End)\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            Assert.AreEqual(1, elements[0].Children.Count, "An unexpected number of child elements were parsed.");

            RegionElement regionElement = elements[0].Children[0] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element.");
            Assert.AreEqual("Fields", regionElement.Name, "Unexpected region name.");
            Assert.AreEqual(1, regionElement.Children.Count, "Unexpected number of region child elements.");

            RegionElement childRegionElement = regionElement.Children[0] as RegionElement;
            Assert.IsNotNull(childRegionElement, "Expected a region element.");
            Assert.AreEqual("Private", childRegionElement.Name, "Unexpected region name.");
            Assert.AreEqual(1, childRegionElement.Children.Count, "Unexpected number of region child elements.");

            FieldElement fieldElement = childRegionElement.Children[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Expected a field element.");
        }

        /// <summary>
        /// Tests comment scenarios with region preprocessor directives. 
        /// </summary>
        [Test]
        public void ParseRegionCommentsTest()
        {
            StringReader reader = new StringReader(
                "'Leading\r\n" +
                "#Region \"Comments\"\r\n" +
                "'Enclosed\r\n" +
                "#End Region");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");

            CommentElement leadingComment = elements[0] as CommentElement;
            Assert.IsNotNull(leadingComment, "Expected a comment element.");
            Assert.AreEqual("Leading", leadingComment.Text);

            RegionElement regionElement = elements[1] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element.");
            Assert.AreEqual("Comments", regionElement.Name, "Unexpected region name.");
            Assert.AreEqual(1, regionElement.Children.Count, "Unexpected number of region child elements.");

            CommentElement enclosedComment = regionElement.Children[0] as CommentElement;
            Assert.IsNotNull(enclosedComment, "Expected a comment element.");
            Assert.AreEqual("Enclosed", enclosedComment.Text);
        }

        /// <summary>
        /// Tests parsing nested regions.  
        /// </summary>
        [Test]
        public void ParseRegionNestedTest()
        {
            StringReader reader = new StringReader(
                "public class Test\r\n" +
                "\t#region \"Fields\"\r\n" +
                "\t#region \"Private\"\r\n" +
                "\tprivate _test As Boolean = False\r\n" +
                "\t#end region\r\n" +
                "\t#end region\r\n" +
                "end class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            Assert.AreEqual(1, elements[0].Children.Count, "An unexpected number of child elements were parsed.");

            RegionElement regionElement = elements[0].Children[0] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element.");
            Assert.AreEqual("Fields", regionElement.Name, "Unexpected region name.");
            Assert.AreEqual(1, regionElement.Children.Count, "Unexpected number of region child elements.");

            RegionElement childRegionElement = regionElement.Children[0] as RegionElement;
            Assert.IsNotNull(childRegionElement, "Expected a region element.");
            Assert.AreEqual("Private", childRegionElement.Name, "Unexpected region name.");
            Assert.AreEqual(1, childRegionElement.Children.Count, "Unexpected number of region child elements.");

            FieldElement fieldElement = childRegionElement.Children[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Expected a field element.");
        }

        /// <summary>
        /// Tests parsing a region preprocessor directive. 
        /// </summary>
        [Test]
        public void ParseRegionTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test\r\n" +
                "\r\n" +
                "\t#Region \"Fields\"\r\n" +
                "\tPrivate _test As Boolean = false\r\n" +
                "\t#End Region\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            Assert.AreEqual(1, elements[0].Children.Count, "An unexpected number of child elements were parsed.");

            RegionElement regionElement = elements[0].Children[0] as RegionElement;
            Assert.IsNotNull(regionElement, "Expected a region element.");
            Assert.AreEqual("Fields", regionElement.Name, "Unexpected region name.");
            Assert.AreEqual(1, regionElement.Children.Count, "Unexpected number of region child elements.");

            FieldElement fieldElement = regionElement.Children[0] as FieldElement;
            Assert.IsNotNull(fieldElement, "Expected a field element.");
        }

        /// <summary>
        /// Tests an unmatched end region directive.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unmatched end region directive")]
        public void ParseRegionUnmatchedEndDirectiveTest()
        {
            StringReader reader = new StringReader("\t#End Region \"Fields\"");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests the parsing of a single namespace.
        /// </summary>
        [Test]
        public void ParseSingleNamespaceTest()
        {
            VBParser parser = new VBParser();

            VBTestFile testFile = VBTestUtilities.GetSingleNamespaceFile();
            using (TextReader reader = testFile.GetReader())
            {
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.IsNotNull(elements, "Code element collection should not be null.");
                Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

                NamespaceElement namespaceElement = elements[0] as NamespaceElement;
                Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
                Assert.AreEqual("SampleNamespace", namespaceElement.Name, "Unexpected namespace name.");
                Assert.IsNotNull(namespaceElement.Children, "Children collection should not be null.");
                Assert.AreEqual(0, namespaceElement.Children.Count, "Children collection should not be null.");
            }
        }

        /// <summary>
        /// Tests the parsing of a single namespace with a single structure
        /// definition.
        /// </summary>
        [Test]
        public void ParseStructDefinitionTest()
        {
            VBParser parser = new VBParser();

            VBTestFile testFile = VBTestUtilities.GetStructDefinitionFile();
            using (TextReader reader = testFile.GetReader())
            {
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.IsNotNull(elements, "Code element collection should not be null.");
                Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");

                UsingElement using1 = elements[0] as UsingElement;
                Assert.IsNotNull(using1, "Expected a UsingElement.");
                Assert.AreEqual("System", using1.Name, "Unexpected using name.");

                NamespaceElement namespaceElement = elements[1] as NamespaceElement;
                Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
                Assert.AreEqual("SampleNamespace", namespaceElement.Name, "Unexpected namespace name.");

                Assert.IsNotNull(namespaceElement.Children, "Namespace Children collection should not be null.");
                Assert.AreEqual(1, namespaceElement.Children.Count, "An unexpected number of namespace child elements were parsed.");

                TypeElement structElement = namespaceElement.Children[0] as TypeElement;
                Assert.IsNotNull(structElement, "Expected a TypeElement.");
                Assert.AreEqual(TypeElementType.Structure, structElement.Type, "Expected type to be a structure.");
                Assert.IsFalse(structElement.IsStatic, "Structure should not be static.");
                Assert.IsFalse(structElement.IsSealed, "Structures should not be sealed.");
                Assert.AreEqual("SampleStruct", structElement.Name, "Unexpected structure name.");
                Assert.AreEqual(3, structElement.HeaderComments.Count, "An unexpected number of class header comment lines were parsed.");
                foreach (ICommentElement comment in structElement.HeaderComments)
                {
                    Assert.AreEqual(CommentType.XmlLine, comment.Type, "Structure header comment should be an XML comment.");
                }
                Assert.AreEqual(CodeAccess.Public, structElement.Access, "Unexpected code access level.");
            }
        }

        /// <summary>
        /// Tests parsing an external subroutine.
        /// </summary>
        [Test]
        public void ParseSubExternalTest()
        {
            string[] variations =
            {
                "Public Declare Ansi Sub ExternalSub Lib \"Some.dll\" Alias \"doit\" (ByVal filename As String)",
                "Public Declare Ansi Sub _\r\nExternalSub Lib _\r\n\"Some.dll\" Alias _\r\n \"doit\" (ByVal filename As String)"
            };

            foreach (string variation in variations)
            {
                StringReader reader = new StringReader(variation);

                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
                MethodElement methodElement = elements[0] as MethodElement;
                Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
                Assert.AreEqual("ExternalSub", methodElement.Name, "Unexpected name.");
                Assert.AreEqual(CodeAccess.Public, methodElement.Access, "Unexpected code access.");
                Assert.AreEqual("ByVal filename As String", methodElement.Parameters, "Unexpected parameters.");
                Assert.IsNull(methodElement.Type, "Unexpected return type.");
                Assert.AreEqual("Ansi", methodElement[VBExtendedProperties.ExternalModifier], "Unexpected external modifier.");
                Assert.AreEqual("Some.dll", methodElement[VBExtendedProperties.ExternalLibrary], "Unexpected external library.");
                Assert.AreEqual("doit", methodElement[VBExtendedProperties.ExternalAlias], "Unexpected external alias.");
            }
        }

        /// <summary>
        /// Tests parsing a subroutine.
        /// </summary>
        [Test]
        public void ParseSubTest()
        {
            StringReader reader = new StringReader(
                "Private Sub DoSomething()\r\n" +
                "End Sub");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            MethodElement methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
            Assert.IsNull(methodElement.Type, "Unexpected member type.");

            reader = new StringReader(
                "Sub DoSomething()\r\n" +
                "End Sub");

            elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            methodElement = elements[0] as MethodElement;
            Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
            Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
            Assert.AreEqual(CodeAccess.None, methodElement.Access, "Unexpected code access.");
            Assert.IsNull(methodElement.Type, "Unexpected member type.");
        }

        /// <summary>
        /// Tests parsing a type that implements interfaces.
        /// </summary>
        [Test]
        public void ParseTypeImplementsTest()
        {
            StringReader reader = new StringReader(
                "Public Class TestClass : Implements IList\r\n" +
                "\tImplements IDisposable, IBindingList\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
            TypeElement typeElement = elements[0] as TypeElement;
            Assert.IsNotNull(typeElement, "Expected a type element.");
            Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected type element type.");
            Assert.AreEqual(3, typeElement.Interfaces.Count);
            Assert.AreEqual("IList", typeElement.Interfaces[0].Name);
            Assert.AreEqual("IDisposable", typeElement.Interfaces[1].Name);
            Assert.AreEqual("IBindingList", typeElement.Interfaces[2].Name);
            foreach (InterfaceReference interfaceReference in typeElement.Interfaces)
            {
                Assert.AreEqual(InterfaceReferenceType.Interface, interfaceReference.ReferenceType);
            }
        }

        /// <summary>
        /// Tests parsing an unhandled preprocessor directive.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
          ExpectedMessage = "Cannot arrange files with preprocessor directives")]
        public void ParseUnhandledPreprocessorTest()
        {
            StringReader reader = new StringReader(
                "Public Class Test\r\n" +
                "#Const TEST = 1\r\n" +
                "End Class");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing an unrecognized keyword.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unhandled element text")]
        public void ParseUnkownKeywordTest()
        {
            StringReader reader = new StringReader("Blah");

            VBParser parser = new VBParser();
            parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing an empty using statement.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected a namepace name")]
        public void ParseUsingEmptyNamespaceTest()
        {
            StringReader reader = new StringReader(
                "Imports \r\n" +
                "Imports System \r\n");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing an empty using statement with a redefine.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
          ExpectedMessage = "Expected a type or namepace name")]
        public void ParseUsingEmptyTypeOrNamespaceTest()
        {
            StringReader reader = new StringReader(
                "Imports Test = \r\n");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing an invalid using statement.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
          ExpectedMessage = "Expected = or end of statement")]
        public void ParseUsingInvalidTest()
        {
            StringReader reader = new StringReader(
                "Imports System Text\r\n");

            VBParser parser = new VBParser();
            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
        }

        /// <summary>
        /// Tests parsing a using statement that redefines a class/namespace.
        /// </summary>
        [Test]
        public void ParseUsingRedefineTest()
        {
            string[] usingVariations = new string[]
            {
                "Imports Redefined = System.Text.Encoder",
                "Imports Redefined = System.Text.Encoder\r\n",
                "Imports  Redefined = System.Text.Encoder",
                "Imports _\r\nRedefined = System.Text.Encoder",
                "Imports _\r\n\tRedefined = System.Text.Encoder",
                "Imports _ \t\r\nRedefined = System.Text.Encoder",
                "Imports Redefined _\r\n= System.Text.Encoder",
                "Imports Redefined _ \t\r\n= System.Text.Encoder",
                "Imports Redefined _\r\n= _\r\nSystem.Text.Encoder",
                "Imports Redefined _ \t\r\n= _ \t\r\nSystem.Text.Encoder"
            };

            foreach (string usingStr in usingVariations)
            {
                StringReader reader = new StringReader(usingStr);
                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
                UsingElement usingElement = elements[0] as UsingElement;
                Assert.IsNotNull(usingElement, "Element is not a UsingElement.");
                Assert.AreEqual("Redefined", usingElement.Name);
                Assert.AreEqual("System.Text.Encoder", usingElement.Redefine);
            }
        }

        /// <summary>
        /// Tests parsing a simple using statement.
        /// </summary>
        [Test]
        public void ParseUsingTest()
        {
            string[] usingVariations = new string[]
            {
                "Imports System.Text",
                "Imports System.Text\r\n",
                "Imports  System.Text",
                "Imports _\r\nSystem.Text",
                "Imports _\r\n\tSystem.Text",
                "Imports _ \t\r\n\tSystem.Text"
            };

            foreach (string usingStr in usingVariations)
            {
                StringReader reader = new StringReader(usingStr);

                VBParser parser = new VBParser();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed from '{0}'", usingStr);
                UsingElement usingElement = elements[0] as UsingElement;
                Assert.IsNotNull(usingElement, "Element is not a UsingElement.  '{0}'", usingStr);
                Assert.AreEqual("System.Text", usingElement.Name, "Unexpected name from '{0}'", usingStr);
                Assert.IsFalse(usingElement.IsMovable, "VB should not support moving import directives.");
            }
        }

        /// <summary>
        /// Gets the ClassMembers test class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>A type code element.</returns>
        private static TypeElement GetMembersTestClass(TextReader reader)
        {
            TypeElement classElement;
            VBParser parser = new VBParser();

            ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

            Assert.AreEqual(9, elements.Count, "Unexpected number of top-level elements.");

            NamespaceElement namespaceElement = elements[8] as NamespaceElement;
            Assert.IsNotNull(namespaceElement, "Expected a namespace element.");

            Assert.AreEqual(1, namespaceElement.Children.Count, "Unexpected number of namespace elements.");

            classElement = namespaceElement.Children[0] as TypeElement;
            Assert.IsNotNull(classElement, "Expected a type element.");
            Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class type.");
            Assert.AreEqual("SampleClass", classElement.Name, "Unexpected class name.");

            return classElement;
        }

        #endregion Methods
    }
}