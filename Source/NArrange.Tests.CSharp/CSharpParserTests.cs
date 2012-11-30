namespace NArrange.Tests.CSharp
{
	using System;
	using System.Collections.ObjectModel;
	using System.IO;

	using NArrange.Core;
	using NArrange.Core.CodeElements;
	using NArrange.CSharp;

	using NUnit.Framework;

	/// <summary>
	/// Test fixture for the CSharpParser class.
	/// </summary>
	[SetUpFixture]
	public class CSharpParserTests
	{
		[SetUp]
		public void StartTesting()
		{
			System.Diagnostics.Debugger.Launch();
		}

		#region Fields

		/// <summary>
		/// Constructor region index in ClassMembers.cs.
		/// </summary>
		private const int ConstructorRegionIndex = 1;

		/// <summary>
		/// Delegate region index in ClassMembers.cs.
		/// </summary>
		private const int DelegateRegionIndex = 4;

		/// <summary>
		/// Event region index in ClassMembers.cs.
		/// </summary>
		private const int EventRegionIndex = 5;

		/// <summary>
		/// Field region index in ClassMembers.cs.
		/// </summary>
		private const int FieldRegionIndex = 0;

		/// <summary>
		/// Method region index in ClassMembers.cs.
		/// </summary>
		private const int MethodRegionIndex = 3;

		/// <summary>
		/// Nested type region index in ClassMembers.cs.
		/// </summary>
		private const int NestedTypeRegionIndex = 6;

		/// <summary>
		/// Property region index in ClassMembers.cs.
		/// </summary>
		private const int PropertyRegionIndex = 2;

		#endregion Fields

		#region Methods

		/// <summary>
		/// Tests that when the end of a block is expected, the appropriate
		/// error is thrown.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Expected }: Line 2, Column 2")]
		public void ExpectedBlockCloseTest()
		{
			StringReader reader = new StringReader(
				"namespace SampleNamespace\r\n{");

			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}

		/// <summary>
		/// Tests that when an initial value is expected, the appropriate
		/// error is thrown.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Unexpected end of file. Expected ;: Line 7, Column 2")]
		public void ExpectedFieldEndOfStatementTest()
		{
			StringReader reader = new StringReader(
				"namespace SampleNamespace\r\n" +
				"{\r\n" +
				"    public class SampleClass\r\n" +
				"    {\r\n" +
				"        private string test =\r\n" +
				"    }\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}

		/// <summary>
		/// Tests that when an initial value is expected, the appropriate
		/// error is thrown.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Expected an initial value: Line 5, Column 31")]
		public void ExpectedFieldInitialValueTest()
		{
			StringReader reader = new StringReader(
				"namespace SampleNamespace\r\n" +
				"{\r\n" +
				"    public class SampleClass\r\n" +
				"    {\r\n" +
				"        private string test =;\r\n" +
				"    }\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a property with a multidimension array return type.
		/// </summary>
		[Test]
		public void ParseArrayMultidimensionReturnTypeTest()
		{
			string[] variations =
                {
                    "public string[][] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [][] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [] [] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string[ ][ ] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [ ] [ ] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [ ] [ ]SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",
                };

			foreach (string variation in variations)
			{
				StringReader reader = new StringReader(variation);
				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
				PropertyElement propertyElement = elements[0] as PropertyElement;
				Assert.IsNotNull(propertyElement, "Element is not a PropertyElement.");
				Assert.AreEqual("SomeStrings", propertyElement.Name, "Unexpected name.");
				Assert.IsNull(propertyElement.IndexParameter, "Unexpected index parameter.");
				Assert.AreEqual(CodeAccess.Public, propertyElement.Access, "Unexpected code access.");
				Assert.AreEqual("string[][]", propertyElement.Type, "Unexpected member type.");
				Assert.IsTrue(propertyElement.BodyText.Contains("get{return null;}"), "Unexpected body text.");
			}
		}

		/// <summary>
		/// Tests parsing a property with an array return type.
		/// </summary>
		[Test]
		public void ParseArrayReturnTypeTest()
		{
			string[] variations =
                {
                    "public string[] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string[ ] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [ ] SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [ ]SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",

                    "public string [\r\n" +
                    "]SomeStrings\r\n" +
                    "{\r\n" +
                    "\tget{return null;}\r\n" +
                    "}",
                };

			foreach (string variation in variations)
			{
				StringReader reader = new StringReader(variation);
				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
				PropertyElement propertyElement = elements[0] as PropertyElement;
				Assert.IsNotNull(propertyElement, "Element is not a PropertyElement.");
				Assert.AreEqual("SomeStrings", propertyElement.Name, "Unexpected name.");
				Assert.IsNull(propertyElement.IndexParameter, "Unexpected index parameter.");
				Assert.AreEqual(CodeAccess.Public, propertyElement.Access, "Unexpected code access.");
				Assert.AreEqual("string[]", propertyElement.Type, "Unexpected member type.");
				Assert.IsTrue(propertyElement.BodyText.Contains("get{return null;}"), "Unexpected body text.");
			}
		}

		/// <summary>
		/// Tests the parsing of assembly attributes.
		/// </summary>
		[Test]
		public void ParseAssemblyAttributesTest()
		{
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetAssemblyAttributesFile();
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
				Assert.AreEqual("false", attributeElement.BodyText, "Unexpected attribute text.");
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
		   MatchType = MessageMatch.Contains, ExpectedMessage = "Expected ]")]
		public void ParseAttributeCloseExpectedTest()
		{
			StringReader reader = new StringReader(
				"[assembly: AssemblyDescription(\"SampleAssembly\")");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing an attribute with intermixed comments.
		/// </summary>
		[Test]
		public void ParseAttributeCommentedTest()
		{
			StringReader reader = new StringReader(
				"[assembly: ThemeInfo(\r\n" +
				"\tResourceDictionaryLocation.None,\r\n" +
				"\t//where theme specific resource dictionaries are located\r\n" +
				"\t//(used if a resource is not found in the page,\r\n" +
				"\t// or application resource dictionaries)" +
				"\tResourceDictionaryLocation.SourceAssembly\r\n" +
				"\t//where the generic resource dictionary is located\r\n" +
				"\t//(used if a resource is not found in the page,\r\n" +
				"\t// app, or any theme specific resource dictionaries)\r\n" +
				"\t)]");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			AttributeElement attributeElement = elements[0] as AttributeElement;
			Assert.IsNotNull(attributeElement, "Element is not a AttributeElement.");
			Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
			Assert.AreEqual("ThemeInfo", attributeElement.Name, "Unexpected attribute name.");
			Assert.IsTrue(attributeElement.BodyText.StartsWith("\r\n\tResourceDictionaryLocation.None,"), "Unexpected attribute text.");
			Assert.IsTrue(attributeElement.BodyText.EndsWith("specific resource dictionaries)\r\n\t"), "Unexpected attribute text.");
		}

		/// <summary>
		/// Tests parsing an attribute list.
		/// </summary>
		[Test]
		public void ParseAttributeListTest()
		{
			StringReader reader = new StringReader(
				"[assembly: AssemblyDescription(\"SampleAssembly\"), ComVisible(false),\tAssemblyConfiguration(\"\"), TestAttribute]");

			CSharpParser parser = new CSharpParser();
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
				"[assembly: AssemblyDescription(\"SampleAssembly\")]");

			CSharpParser parser = new CSharpParser();
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
				"[assembly: AssemblyDescription(\"SampleAssembly]\")]");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			AttributeElement attributeElement = elements[0] as AttributeElement;
			Assert.IsNotNull(attributeElement, "Element is not a AttributeElement.");
			Assert.AreEqual("assembly", attributeElement.Target, "Unexpected attribute target.");
			Assert.AreEqual("AssemblyDescription", attributeElement.Name, "Unexpected attribute name.");
			Assert.AreEqual("\"SampleAssembly]\"", attributeElement.BodyText, "Unexpected attribute text.");
		}

		/// <summary>
		/// Tests parsing a member body that has complex escaped strings.
		/// </summary>
		[Test]
		public void ParseBodyEscapedStringTest()
		{
			StringReader reader = new StringReader(
				"public void DoSomething()\r\n" +
				"{\r\n" +
				"\tstring v = string.Empty;\r\n" +
				"\tv = v.Replace(\"\\\\\\\"\", \"\\\"\"\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "Unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement);

			Assert.IsTrue(
				methodElement.BodyText.Contains("v = v.Replace(\"\\\\\\\"\", \"\\\"\""),
				"Escaped string line was not found in the member body.");
		}

		/// <summary>
		/// Tests the parsing of a single namespace with a single class
		/// definition.
		/// </summary>
		[Test]
		public void ParseClassDefinitionTest()
		{
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetClassDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.IsNotNull(elements, "Code element collection should not be null.");
				Assert.AreEqual(8, elements.Count, "An unexpected number of elements were parsed.");

				CommentElement commentElement = elements[0] as CommentElement;
				Assert.IsNotNull(commentElement, "Expected a CommentElement.");

				CommentElement commentElement1 = elements[1] as CommentElement;
				Assert.IsNotNull(commentElement1, "Expected a CommentElement.");
				Assert.AreEqual(" This is comment line 1", commentElement1.Text, "Unexpected comment text.");

				CommentElement commentElement2 = elements[2] as CommentElement;
				Assert.IsNotNull(commentElement2, "Expected a CommentElement.");
				Assert.AreEqual(" This is comment line 2", commentElement2.Text, "Unexpected comment text.");

				CommentElement commentElement3 = elements[3] as CommentElement;
				Assert.IsNotNull(commentElement3, "Expected a CommentElement.");
				Assert.AreEqual(" This is comment line 3", commentElement3.Text, "Unexpected comment text.");

				UsingElement using1 = elements[4] as UsingElement;
				Assert.IsNotNull(using1, "Expected a UsingElement.");
				Assert.AreEqual("System", using1.Name, "Unexpected using name.");

				UsingElement using2 = elements[5] as UsingElement;
				Assert.IsNotNull(using2, "Expected a UsingElement.");
				Assert.AreEqual("System.Collections.Generic", using2.Name, "Unexpected using name.");

				UsingElement using3 = elements[6] as UsingElement;
				Assert.IsNotNull(using3, "Expected a UsingElement.");
				Assert.AreEqual("System.Text", using3.Name, "Unexpected using name.");

				NamespaceElement namespaceElement = elements[7] as NamespaceElement;
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
		/// Tests parsing a class with an empty parameter constraint list.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		   ExpectedMessage = "Expected a class or interface name")]
		public void ParseClassEmptyParameterConstraintListTest()
		{
			StringReader reader = new StringReader(
				"public class Test<T> where T : {}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a class where a type implementation specification is expected.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		  ExpectedMessage = "Expected :")]
		public void ParseClassExpectedTypeImplementsTest()
		{
			StringReader reader = new StringReader(
				"public class Test<T> where T {}");

			CSharpParser parser = new CSharpParser();
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
				"public class Test<T> when T : {}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a class that implements a generic interface.
		/// </summary>
		[Test]
		public void ParseClassImplementsGenericTest()
		{
			string[] variations = new string[]
            {
                "public class Test : IEnumerable<string>{}",
                "public class Test : IEnumerable <string>{}"
            };

			foreach (string variation in variations)
			{
				StringReader reader = new StringReader(variation);

				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
				TypeElement typeElement = elements[0] as TypeElement;
				Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
				Assert.AreEqual("Test", typeElement.Name, "Unexpected name.");
				Assert.AreEqual(CodeAccess.Public, typeElement.Access, "Unexpected code access.");
				Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected type element type.");
				Assert.AreEqual(1, typeElement.Interfaces.Count, "Unexpected number of interface implementations.");
				Assert.AreEqual("IEnumerable<string>", typeElement.Interfaces[0].Name, "Unexpected interface implementation name.");
			}
		}

		/// <summary>
		/// Tests parsing a class with an invalid New type parameter constraint.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Invalid new constraint, use new()")]
		public void ParseClassInvalidNewConstraintTest()
		{
			StringReader reader = new StringReader(
				"public class Test<T> where T : IDisposable, new {}");

			CSharpParser parser = new CSharpParser();
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
				"public class Test\r\n" +
				"{\r\n" +
				"\t#region Fields\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a class with an empty region name (regions do not have to have a name)
		/// </summary>
		[Test]
		public void ParseClassMissingRegionNameTest()
		{
			StringReader reader = new StringReader(
				"public class Test\r\n" +
				"{\r\n" +
				"\t#region\r\n" +
				"\t#endregion\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			Assert.AreEqual(1, elements[0].Children.Count, "An unexpected number of child elements were parsed.");
			RegionElement regionElement = elements[0].Children[0] as RegionElement;
			Assert.IsNotNull(regionElement, "Expected a region element.");
			Assert.AreEqual(string.Empty, regionElement.Name, "Unexpected region name.");
		}

		/// <summary>
		/// Tests parsing a class where the new() constraint is not the last 
		/// type parameter constraint.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		 ExpectedMessage = "must be the last declared type parameter constraint")]
		public void ParseClassNewConstraintOrderTest()
		{
			StringReader reader = new StringReader(
				"public class Test<T> where T : new(), IDisposable {}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a class with unspecified access.
		/// </summary>
		[Test]
		public void ParseClassPartialUnspecifiedAccessTest()
		{
			StringReader reader = new StringReader(
				"partial class Test{}");

			CSharpParser parser = new CSharpParser();
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
			string[] variations =
                {
                    "public class Test{}",
                    "public class Test{};",
                    "public class Test\r\n{\r\n}\r\n"
                };

			foreach (string variation in variations)
			{
				StringReader reader = new StringReader(variation);

				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
				TypeElement typeElement = elements[0] as TypeElement;
				Assert.IsNotNull(typeElement, "Element is not a TypeElement.");
				Assert.AreEqual("Test", typeElement.Name, "Unexpected name.");
				Assert.AreEqual(CodeAccess.Public, typeElement.Access, "Unexpected code access.");
				Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected type element type.");
			}
		}

		/// <summary>
		/// Tests parsing a class with an unclosed type parameter constraint.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		   ExpectedMessage = "Expected >")]
		public void ParseClassUnclosedTypeParameterConstraintTest()
		{
			StringReader reader = new StringReader(
				"public class Test<T> where T : IComparable<T {}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a class with an unclosed type parameter.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		   ExpectedMessage = "Expected >")]
		public void ParseClassUnclosedTypeParameterTest()
		{
			StringReader reader = new StringReader(
				"public class Test<T {}");

			CSharpParser parser = new CSharpParser();
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
				"public class Test{test}");
			CSharpParser parser = new CSharpParser();
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
				"public class Test{\r\ntest\r\npublic string Test;\r\n}");
			CSharpParser parser = new CSharpParser();
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
				"public class Test{\r\n{\r\npublic string Test;\r\n}");
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a class with an unknown type parameter constraint.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		  ExpectedMessage = "Unknown type parameter")]
		public void ParseClassUnknownTypeParameterTest()
		{
			StringReader reader = new StringReader(
				"public class Test<T> where S : new() {}");

			CSharpParser parser = new CSharpParser();
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
				"{\r\n" +
				"\tclass Nested{}\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
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
		/// Tests parsing a comment block.
		/// </summary>
		[Test]
		public void ParseCommentBlockTest()
		{
			StringReader reader = new StringReader(
				"/*\r\n" +
				" * Block comment here\r\n" +
				" */\r\n");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			CommentElement commentBlockElement = elements[0] as CommentElement;
			Assert.AreEqual(CommentType.Block, commentBlockElement.Type, "Element is not a CommentBlockElement.");

			string[] lines = commentBlockElement.Text.Split(
				new string[] { Environment.NewLine }, StringSplitOptions.None);
			Assert.AreEqual(3, lines.Length, "An unexpected number of comment lines were parsed.");
			Assert.AreEqual(string.Empty, lines[0], "Unexpected comment line at index 0.");
			Assert.AreEqual(" * Block comment here", lines[1], "Unexpected comment line at index 1.");
			Assert.AreEqual(" ", lines[2], "Unexpected comment line at index 2.");
		}

		/// <summary>
		/// Tests parsing a commented member.
		/// </summary>
		[Test]
		public void ParseCommentedMemberTest()
		{
			StringReader reader = new StringReader(
				"//private void DoSomething()\r\n" +
				"//{\r\n" +
				"//}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(3, elements.Count, "An unexpected number of elements were parsed.");
			Assert.AreEqual("private void DoSomething()", ((ICommentElement)elements[0]).Text, "Unexpected comment text.");
			Assert.AreEqual("{", ((ICommentElement)elements[1]).Text, "Unexpected comment text.");
			Assert.AreEqual("}", ((ICommentElement)elements[2]).Text, "Unexpected comment text.");
		}

		/// <summary>
		/// Tests parsing a single comment line.
		/// </summary>
		[Test]
		public void ParseCommentLineTest()
		{
			StringReader reader = new StringReader(
				"//using System.Text;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			CommentElement commentElement = elements[0] as CommentElement;
			Assert.IsNotNull(commentElement, "Element is not a CommentElement.");
			Assert.AreEqual(CommentType.Line, commentElement.Type, "Unexpected comment type.");
			Assert.AreEqual("using System.Text;", commentElement.Text, "Unexpected comment text.");
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
				"#if DEBUG\r\n" +
				"[Obsolete()]\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a condition directive with an else if condition.
		/// </summary>
		[Test]
		public void ParseConditionDirectiveElseIfTest()
		{
			StringReader reader = new StringReader(
				"#if DEBUG\r\n" +
				"public TestClass(){}\r\n" +
				"#elif TEST\r\n" +
				"protected TestClass(){}\r\n" +
				"#else\r\n" +
				"private TestClass(){}\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
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
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

			Assert.IsNotNull(conditionElement.ElseCondition, "Expected an Else condition.");
			Assert.AreEqual("TEST", conditionElement.ElseCondition.ConditionExpression, "Unexpected condition expression.");
			Assert.AreEqual(1, conditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

			ConstructorElement elseIfConstructorElement =
				conditionElement.ElseCondition.Children[0] as ConstructorElement;
			Assert.IsNotNull(elseIfConstructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", elseIfConstructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Protected, elseIfConstructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, elseIfConstructorElement.Parameters, "Unexpected parameter string.");

			Assert.IsNotNull(conditionElement.ElseCondition.ElseCondition, "Expected an Else condition.");
			Assert.AreEqual(1, conditionElement.ElseCondition.ElseCondition.Children.Count, "Unexpected number of child elements.");

			ConstructorElement elseConstructorElement =
				conditionElement.ElseCondition.ElseCondition.Children[0] as ConstructorElement;
			Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", elseConstructorElement.Name, "Unexpected name.");
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
				"#if DEBUG\r\n" +
				"public TestClass(){}\r\n" +
				"#else\r\n" +
				"private TestClass(){}\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

			ConditionDirectiveElement conditionElement =
				elements[0] as ConditionDirectiveElement;
			Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
			Assert.AreEqual(1, conditionElement.Children.Count, "Unexpected number of child elements.");

			ConstructorElement constructorElement =
				conditionElement.Children[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

			Assert.IsNotNull(conditionElement.ElseCondition, "Expected an Else condition.");
			Assert.AreEqual(1, conditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

			ConstructorElement elseConstructorElement =
				conditionElement.ElseCondition.Children[0] as ConstructorElement;
			Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", elseConstructorElement.Name, "Unexpected name.");
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
				"#if \r\n" +
				"public TestClass(){}\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing an invalid condition directive.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Expected #endif")]
		public void ParseConditionDirectiveNestedMissingEndTest()
		{
			StringReader reader = new StringReader(
				"#if DEBUG\r\n" +
				"#if TEST\r\n" +
				"public TestClass(){}\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a condition directive with a nested region.
		/// </summary>
		[Test]
		public void ParseConditionDirectiveNestedRegionTest()
		{
			StringReader reader = new StringReader(
				"#if DEBUG\r\n" +
				"#region Constructors\r\n" +
				"public TestClass(){}\r\n" +
				"#endregion\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
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
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
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
				"#if DEBUG\r\n" +
				"#if TEST\r\n" +
				"public TestClass(){}\r\n" +
				"#else\r\n" +
				"private TestClass(){}\r\n" +
				"#endif\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
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
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

			Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
			Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

			Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
			Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

			ConstructorElement elseConstructorElement =
				nestedConditionElement.ElseCondition.Children[0] as ConstructorElement;
			Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", elseConstructorElement.Name, "Unexpected name.");
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
				"#if DEBUG\r\n" +
				"public TestClass(){}\r\n" +
				"#if TEST\r\n" +
				"protected TestClass(){}\r\n" +
				"#else\r\n" +
				"private TestClass(){}\r\n" +
				"#endif\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

			ConditionDirectiveElement conditionElement = elements[0] as ConditionDirectiveElement;
			Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
			Assert.AreEqual(2, conditionElement.Children.Count, "Unexpected number of child elements.");
			Assert.IsNull(conditionElement.ElseCondition);

			ConstructorElement constructorElement =
				conditionElement.Children[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

			ConditionDirectiveElement nestedConditionElement = conditionElement.Children[1] as ConditionDirectiveElement;
			Assert.IsNotNull(nestedConditionElement, "Element is not a ConditionDirectiveElement");
			Assert.AreEqual(1, nestedConditionElement.Children.Count, "Unexpected number of child elements.");

			constructorElement =
				nestedConditionElement.Children[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Protected, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

			Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
			Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

			Assert.IsNotNull(nestedConditionElement.ElseCondition, "Expected an Else condition.");
			Assert.AreEqual(1, nestedConditionElement.ElseCondition.Children.Count, "Unexpected number of child elements.");

			ConstructorElement elseConstructorElement =
				nestedConditionElement.ElseCondition.Children[0] as ConstructorElement;
			Assert.IsNotNull(elseConstructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", elseConstructorElement.Name, "Unexpected name.");
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
				"#if DEBUG\r\n" +
				"public TestClass(){}\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");

			ConditionDirectiveElement conditionElement = elements[0] as ConditionDirectiveElement;
			Assert.IsNotNull(conditionElement, "Element is not a ConditionDirectiveElement");
			Assert.AreEqual("DEBUG", conditionElement.ConditionExpression, "Unexpected condition expression.");
			Assert.AreEqual(1, conditionElement.Children.Count, "Unexpected number of child elements.");

			ConstructorElement constructorElement = conditionElement.Children[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");

			Assert.IsNull(conditionElement.ElseCondition);
		}

		/// <summary>
		/// Tests parsing an invalid condition directive.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Unhandled preprocessor")]
		public void ParseConditionDirectiveUnhandledTest()
		{
			StringReader reader = new StringReader(
				"#ifdef \r\n" +
				"public TestClass(){}\r\n" +
				"#endif");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a constructor with constructor reference.
		/// </summary>
		[Test]
		public void ParseConstructorBaseReferenceTest()
		{
			StringReader reader = new StringReader(
				"public TestClass(int value, int max) : base(value){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual("int value, int max", constructorElement.Parameters, "Unexpected parameter string.");
			Assert.AreEqual("base(value)", constructorElement.Reference, "Unexpected constructor reference.");
		}

		/// <summary>
		/// Tests parsing a constructor with parameters.
		/// </summary>
		[Test]
		public void ParseConstructorParametersTest()
		{
			StringReader reader = new StringReader(
				"public TestClass(int value, int max){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual("int value, int max", constructorElement.Parameters, "Unexpected parameter string.");
		}

		/// <summary>
		/// Tests parsing a constructor with constructor reference.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Expected (",
			MatchType = MessageMatch.Contains)]
		public void ParseConstructorReferenceExpectedParamsTest()
		{
			StringReader reader = new StringReader(
				"public TestClass(int value, int max) : base");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a constructor with constructor reference.
		/// </summary>
		[Test]
		public void ParseConstructorReferenceTest()
		{
			StringReader reader = new StringReader(
				"public TestClass(int value, int max) \r\n" +
				"\t// Comment 1\r\n" +
				"\t// Comment 2r\n" +
				"\t: this(value){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual("int value, int max", constructorElement.Parameters, "Unexpected parameter string.");
			Assert.AreEqual("this(value)", constructorElement.Reference, "Unexpected constructor reference.");
			Assert.AreEqual(2, constructorElement.HeaderComments.Count, "Unexpected number of header comments.");
		}

		/// <summary>
		/// Verifies the parsing of constructor members from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseConstructorsTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
				TypeElement classElement = GetMembersTestClass(reader);

				ConstructorElement constructor;

				RegionElement regionElement = classElement.Children[ConstructorRegionIndex] as RegionElement;
				Assert.IsNotNull(regionElement, "Expected a region element.");

				constructor = regionElement.Children[0] as ConstructorElement;
				Assert.IsNotNull(constructor, "Expected a constructor.");
				Assert.AreEqual("SampleClass", constructor.Name, "Unexpected constructor name.");
				Assert.AreEqual(CodeAccess.Public, constructor.Access, "Unexpected access level.");
				Assert.AreEqual(3, constructor.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(constructor.IsStatic, "Constructor should not be static.");
				Assert.IsEmpty(constructor.Parameters, "Parameter string should be empty.");

				constructor = regionElement.Children[1] as ConstructorElement;
				Assert.IsNotNull(constructor, "Expected a constructor.");
				Assert.AreEqual("SampleClass", constructor.Name, "Unexpected constructor name.");
				Assert.AreEqual(CodeAccess.Internal, constructor.Access, "Unexpected access level.");
				Assert.AreEqual(4, constructor.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(constructor.IsStatic, "Constructor should not be static.");
				Assert.AreEqual("string[] arrayParam", constructor.Parameters, "Unexpected parameters string.");

				constructor = regionElement.Children[2] as ConstructorElement;
				Assert.IsNotNull(constructor, "Expected a constructor.");
				Assert.AreEqual("SampleClass", constructor.Name, "Unexpected constructor name.");
				Assert.AreEqual(CodeAccess.None, constructor.Access, "Unexpected access level.");
				Assert.AreEqual(1, constructor.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(constructor.IsStatic, "Constructor should be static.");
				Assert.IsEmpty(constructor.Parameters, "Parameter string should be empty.");

				constructor = regionElement.Children[3] as ConstructorElement;
				Assert.IsNotNull(constructor, "Expected a constructor.");
				Assert.AreEqual("~SampleClass", constructor.Name, "Unexpected constructor name.");
				Assert.AreEqual(CodeAccess.None, constructor.Access, "Unexpected access level.");
				Assert.AreEqual(4, constructor.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(constructor.IsStatic, "Constructor should not be static.");
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
				"public TestClass(){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");
		}

		/// <summary>
		/// Tests parsing a generic delegate.
		/// </summary>
		[Test]
		public void ParseDelegateGenericTest()
		{
			string[] variations =
                {
                    "public delegate void Action<T>(T value);",
                    "public delegate void Action<T> (T value);",
                    "public delegate void Action <T> (T value);",
                    "public delegate void Action\t<T>\t(T value);",
                    "public delegate void Action\r\n<T>(T value);"
                };

			foreach (string variation in variations)
			{
				try
				{
					StringReader reader = new StringReader(variation);

					CSharpParser parser = new CSharpParser();
					ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

					Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
					DelegateElement delegateElement = elements[0] as DelegateElement;
					Assert.IsNotNull(delegateElement, "Element is not a DelegateElement.");
					Assert.AreEqual("Action", delegateElement.Name, "Unexpected name: {0}", variation);
					Assert.AreEqual(1, delegateElement.TypeParameters.Count, "Unexpected number of type parameters: {0}", variation);
					Assert.AreEqual("T", delegateElement.TypeParameters[0].Name, "Unexpected type parameter name: {0}", variation);
					Assert.AreEqual(CodeAccess.Public, delegateElement.Access, "Unexpected code access: {0}", variation);
					Assert.AreEqual("void", delegateElement.Type, "Unexpected delegate return type: {0}", variation);
					Assert.AreEqual("T value", delegateElement.Parameters, "Unexpected parameters: {0}", variation);
				}
				catch (ParseException ex)
				{
					Assert.Fail(ex.Message + ": " + variation);
				}
			}
		}

		/// <summary>
		/// Verifies the parsing of delegates from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseDelegatesTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
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
				Assert.AreEqual("void", delegateElement.Type, "Unexpected return type.");
				Assert.IsFalse(delegateElement.IsAbstract, "Delegate should not be abstract.");
				Assert.IsFalse(delegateElement.IsOverride, "Delegate should not be an override.");
				Assert.IsFalse(delegateElement.IsSealed, "Delegate should not be sealed.");
				Assert.IsFalse(delegateElement.IsVirtual, "Delegate should not be virtual.");
				Assert.IsFalse(delegateElement.IsNew, "Delegate should not be new.");
				Assert.AreEqual(0, delegateElement.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual("object sender, bool boolParam", delegateElement.Parameters, "Unexpected parameter string.");

				delegateElement = regionElement.Children[1] as DelegateElement;
				Assert.IsNotNull(delegateElement, "Expected a delegate.");
				Assert.AreEqual("Compare", delegateElement.Name, "Unexpected delegate name.");
				Assert.AreEqual(CodeAccess.Private, delegateElement.Access, "Unexpected access level.");
				Assert.AreEqual(7, delegateElement.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(delegateElement.IsStatic, "Delegate should not be static.");
				Assert.AreEqual("int", delegateElement.Type, "Unexpected return type.");
				Assert.IsFalse(delegateElement.IsAbstract, "Delegate should not be abstract.");
				Assert.IsFalse(delegateElement.IsOverride, "Delegate should not be an override.");
				Assert.IsFalse(delegateElement.IsSealed, "Delegate should not be sealed.");
				Assert.IsFalse(delegateElement.IsVirtual, "Delegate should not be virtual.");
				Assert.IsFalse(delegateElement.IsNew, "Delegate should not be new.");
				Assert.AreEqual(0, delegateElement.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual("T t1, T t2", delegateElement.Parameters, "Unexpected parameter string.");
				Assert.AreEqual(1, delegateElement.TypeParameters.Count, "Unexpected number of type parameters");
				Assert.AreEqual("T", delegateElement.TypeParameters[0].Name, "Unexpected type parameter name.");
				Assert.AreEqual(1, delegateElement.TypeParameters[0].Constraints.Count, "Unexpected number of type parameter constraints.");
				Assert.AreEqual("class", delegateElement.TypeParameters[0].Constraints[0], "Unexpected type parameter constraint.");
			}
		}

		/// <summary>
		/// Tests parsing an enum.
		/// </summary>
		[Test]
		public void ParseEnumTest()
		{
			StringReader reader = new StringReader(
				"public enum TestEnum\r\n" +
				"{\r\n" +
				"\tOff = 0,\r\n" +
				"\tOn = 1\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
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
				"public enum TestEnum : short\r\n" +
				"{\r\n" +
				"\tOff = 0,\r\n" +
				"\tOn = 1\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
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
			Assert.AreEqual(1, typeElement.Interfaces.Count);
			Assert.AreEqual("short", typeElement.Interfaces[0].Name);
		}

		/// <summary>
		/// Verifies the parsing of events with generic type signatures.
		/// </summary>
		[Test]
		public void ParseEventGenericTest()
		{
			StringReader reader = new StringReader(
			   "public event EventHandler<TEventArgs<T1, T2>> EventName;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			EventElement eventElement = elements[0] as EventElement;
			Assert.IsNotNull(eventElement, "Element is not a EventElement.");
			Assert.AreEqual("EventName", eventElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, eventElement.Access, "Unexpected code access.");
			Assert.AreEqual("EventHandler<TEventArgs<T1, T2>>", eventElement.Type, "Unexpected member type.");
		}

		/// <summary>
		/// Verifies the parsing of events from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseEventsTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
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
				Assert.AreEqual("EventHandler<EventArgs>", eventElement.Type, "Unexpected return type.");
				Assert.IsFalse(eventElement.IsAbstract, "Event should not be abstract.");
				Assert.IsFalse(eventElement.IsOverride, "Event should not be an override.");
				Assert.IsFalse(eventElement.IsSealed, "Event should not be sealed.");
				Assert.IsFalse(eventElement.IsVirtual, "Event should not be virtual.");
				Assert.IsFalse(eventElement.IsNew, "Event should not be new.");
				Assert.AreEqual(0, eventElement.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsNull(eventElement.BodyText, "Unexpected body text.");

				eventElement = regionElement.Children[2] as EventElement;
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
				Assert.IsTrue(eventElement.BodyText.Contains("add"), "Unexpected body text.");
				Assert.IsTrue(eventElement.BodyText.Contains("remove"), "Unexpected body text.");
			}
		}

		/// <summary>
		/// Tests parsing a fixed field.
		/// </summary>
		[Test]
		public void ParseFieldFixedTest()
		{
			StringReader reader = new StringReader(
				"public fixed char pathName[128];");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("pathName[128]", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("char", fieldElement.Type, "Unexpected member type.");
			Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
			Assert.AreEqual(true, fieldElement[CSharpExtendedProperties.Fixed], "Unexpected value for extended property Fixed.");
		}

		/// <summary>
		/// Tests parsing a generic field.
		/// </summary>
		[Test]
		public void ParseFieldGenericTest()
		{
			string[] variations = new string[]
            {
                "private static Dictionary<string, int> _dictionary = new Dictionary<string, int>();",
                "private static Dictionary< string, int> _dictionary = new Dictionary<string, int>();",
                "private static Dictionary <string, int> _dictionary = new Dictionary<string, int>();",
                "private static Dictionary < string, int > _dictionary = new Dictionary<string, int>();"
            };

			foreach (string variation in variations)
			{
				try
				{
					StringReader reader = new StringReader(variation);

					CSharpParser parser = new CSharpParser();
					ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

					Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
					FieldElement fieldElement = elements[0] as FieldElement;
					Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
					Assert.AreEqual("_dictionary", fieldElement.Name, "Unexpected name.");
					Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
					Assert.IsTrue(fieldElement.IsStatic, "Unexpected value for IsStatic.");
					Assert.AreEqual("Dictionary<string, int>", fieldElement.Type, "Unexpected member type.");
					Assert.AreEqual("new Dictionary<string, int>()", fieldElement.InitialValue, "Unexpected initial value.");
				}
				catch (ParseException ex)
				{
					Assert.Fail(ex.Message + ": " + variation);
				}
			}
		}

		/// <summary>
		/// Tests parsing a simple field that instantiates an object.
		/// </summary>
		[Test]
		public void ParseFieldInstanceTest()
		{
			StringReader reader = new StringReader(
				"private Object obj = new Object();");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("obj", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("Object", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("new Object()", fieldElement.InitialValue, "Unexpected initial value.");
		}

		/// <summary>
		/// Tests parsing multiple fields with an initial value from a single statement.
		/// </summary>
		[Test]
		public void ParseFieldMultiInitialValueTest()
		{
			StringReader reader = new StringReader(
				"private int val1, val2 = 1;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val1", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("1", fieldElement.InitialValue, "Unexpected initial value.");

			fieldElement = elements[1] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val2", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("1", fieldElement.InitialValue, "Unexpected initial value.");
		}

		/// <summary>
		/// Tests parsing multiple fields from a single statement.
		/// </summary>
		[Test]
		public void ParseFieldMultiTest()
		{
			string[] fieldDefinitions = new string[]
            {
                "private int val1, val2;",
                "private int val1 , val2;",
                "private int val1 ,val2;"
            };

			foreach (string fieldDefinition in fieldDefinitions)
			{
				StringReader reader = new StringReader(fieldDefinition);

				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.AreEqual(2, elements.Count, "An unexpected number of elements were parsed.");
				FieldElement fieldElement1 = elements[0] as FieldElement;
				Assert.IsNotNull(fieldElement1, "Element is not a FieldElement.");
				Assert.AreEqual("val1", fieldElement1.Name, "Unexpected name.");
				Assert.AreEqual(CodeAccess.Private, fieldElement1.Access, "Unexpected code access.");
				Assert.AreEqual("int", fieldElement1.Type, "Unexpected member type.");
				Assert.IsNull(fieldElement1.InitialValue, "Unexpected initial value.");

				FieldElement fieldElement2 = elements[1] as FieldElement;
				Assert.IsNotNull(fieldElement2, "Element is not a FieldElement.");
				Assert.AreEqual("val2", fieldElement2.Name, "Unexpected name.");
				Assert.AreEqual(CodeAccess.Private, fieldElement2.Access, "Unexpected code access.");
				Assert.AreEqual("int", fieldElement2.Type, "Unexpected member type.");
				Assert.IsNull(fieldElement2.InitialValue, "Unexpected initial value.");
			}
		}

		/// <summary>
		/// Tests parsing a simple field.
		/// </summary>
		[Test]
		public void ParseFieldSimpleTest()
		{
			StringReader reader = new StringReader(
				"private int val;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type, "Unexpected member type.");
			Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
		}

		/// <summary>
		/// Verifies the parsing of field members from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseFieldsTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
				TypeElement classElement = GetMembersTestClass(reader);

				FieldElement field;

				RegionElement regionElement = classElement.Children[FieldRegionIndex] as RegionElement;
				Assert.IsNotNull(regionElement, "Expected a region element.");

				field = regionElement.Children[0] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_simpleField", field.Name, "Unexpected field name.");
				Assert.AreEqual("bool", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.None, field.Access, "Unexpected field access level.");
				Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(1, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be readonly.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");

				field = regionElement.Children[1] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_fieldWithInitialVal", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.AreEqual("1", field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");

				field = regionElement.Children[2] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("StaticStr", field.Name, "Unexpected field name.");
				Assert.AreEqual("string", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Protected, field.Access, "Unexpected field access level.");
				Assert.AreEqual("\"static; string;\"", field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(3, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(field.IsStatic, "Field should be static.");
				Assert.IsTrue(field.IsReadOnly, "Field should be readonly.");

				field = regionElement.Children[3] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_genericField", field.Name, "Unexpected field name.");
				Assert.AreEqual("Nullable<int>", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");

				field = regionElement.Children[4] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_arrayField", field.Name, "Unexpected field name.");
				Assert.AreEqual("string[]", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Protected | CodeAccess.Internal, field.Access, "Unexpected field access level.");
				Assert.AreEqual("{ }", field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");

				field = regionElement.Children[5] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("@internal", field.Name, "Unexpected field name.");
				Assert.AreEqual("bool", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Internal, field.Access, "Unexpected field access level.");
				Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");

				field = regionElement.Children[6] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_globalNamespaceTypeField", field.Name, "Unexpected field name.");
				Assert.AreEqual("global::System.Boolean", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");

				field = regionElement.Children[7] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_attributedField", field.Name, "Unexpected field name.");
				Assert.AreEqual("string", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.IsTrue(field.InitialValue.Contains("null"), "Unexpected field initial value.");
				Assert.AreEqual(3, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(field.IsStatic, "Field should be static.");
				Assert.AreEqual(1, field.Attributes.Count, "Unexpected number of attributes.");

				field = regionElement.Children[8] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("ConstantStr", field.Name, "Unexpected field name.");
				Assert.AreEqual("string", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Public, field.Access, "Unexpected field access level.");
				Assert.IsTrue(field.InitialValue.Contains("\"constant string\""), "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsTrue(field.IsConstant, "Field should be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be readonly.");

				field = regionElement.Children[9] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_volatileField", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(field.IsVolatile, "Field should be volatile.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");

				field = regionElement.Children[10] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_val1", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");

				field = regionElement.Children[11] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_val2", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.IsNull(field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");

				field = regionElement.Children[12] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_val3", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.AreEqual("10", field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");

				field = regionElement.Children[13] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_val4", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.AreEqual("10", field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");

				field = regionElement.Children[14] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_val5", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.AreEqual("10", field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");

				field = regionElement.Children[15] as FieldElement;
				Assert.IsNotNull(field, "Expected a field.");
				Assert.AreEqual("_val6", field.Name, "Unexpected field name.");
				Assert.AreEqual("int", field.Type, "Unexpected field type.");
				Assert.AreEqual(CodeAccess.Private, field.Access, "Unexpected field access level.");
				Assert.AreEqual("10", field.InitialValue, "Unexpected field initial value.");
				Assert.AreEqual(0, field.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(field.IsVolatile, "Field should not be volatile.");
				Assert.IsFalse(field.IsStatic, "Field should not be static.");
				Assert.AreEqual(0, field.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsFalse(field.IsConstant, "Field should not be a constant.");
				Assert.IsFalse(field.IsReadOnly, "Field should not be a readonly.");
			}
		}

		/// <summary>
		/// Tests parsing a field with a trailing block comment.
		/// </summary>
		[Test]
		public void ParseFieldTrailingBlockCommentTest()
		{
			StringReader reader = new StringReader(
				"private int _commented;  /*Comment line 1\r\n" +
				"\tComment line 2*/");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("_commented", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type, "Unexpected member type.");
			Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
			Assert.IsNotNull(fieldElement.TrailingComment, "Expected a trailing comment.");
			Assert.AreEqual("Comment line 1\r\n\tComment line 2", fieldElement.TrailingComment.Text);
			Assert.AreEqual(CommentType.Block, fieldElement.TrailingComment.Type);
		}

		/// <summary>
		/// Tests parsing a field with a trailing comment.
		/// </summary>
		[Test]
		public void ParseFieldTrailingCommentTest()
		{
			StringReader reader = new StringReader(
				"private int _commented;  //This is a comment");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("_commented", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type, "Unexpected member type.");
			Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
			Assert.IsNotNull(fieldElement.TrailingComment, "Expected a trailing comment.");
			Assert.AreEqual("This is a comment", fieldElement.TrailingComment.Text);
		}

		/// <summary>
		/// Tests parsing a volatile field.
		/// </summary>
		[Test]
		public void ParseFieldVolatileTest()
		{
			StringReader reader = new StringReader(
				"private volatile int val;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type, "Unexpected member type.");
			Assert.IsNull(fieldElement.InitialValue, "Unexpected initial value.");
			Assert.IsTrue(fieldElement.IsVolatile, "Unexpected value for IsVolatile.");
		}

		/// <summary>
		/// Tests parsing a field with an initial value that contains
		/// character symbols.
		/// </summary>
		[Test]
		public void ParseFieldWithCharInitialValueTest()
		{
			StringReader reader = new StringReader(
				"private char val = 'X';");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("char", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("'X'", fieldElement.InitialValue, "Unexpected initial value.");
		}

		/// <summary>
		/// Tests parsing a field with an initial value.
		/// </summary>
		[Test]
		public void ParseFieldWithInitialValueTest()
		{
			StringReader reader = new StringReader(
				"private int val = 17;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("17", fieldElement.InitialValue, "Unexpected initial value.");
		}

		/// <summary>
		/// Tests parsing a field with an initial value that contains
		/// character symbols.
		/// </summary>
		[Test]
		public void ParseFieldWithNestedCharInitialValueTest()
		{
			string fieldText = "private char val = '\\'';";
			StringReader reader = new StringReader(fieldText);

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("char", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("'\\''", fieldElement.InitialValue, "Unexpected initial value.");

			fieldText = "private char val = '\\\\';";
			reader = new StringReader(fieldText);

			parser = new CSharpParser();
			elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("char", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("'\\\\'", fieldElement.InitialValue, "Unexpected initial value.");

			fieldText = "private string val = \"\\\\\\\\Server\\\\share\\\\\";";
			reader = new StringReader(fieldText);

			parser = new CSharpParser();
			elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("string", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("\"\\\\\\\\Server\\\\share\\\\\"", fieldElement.InitialValue, "Unexpected initial value.");

			fieldText = "private string val = @\"\\\\Server\\share\\\";";
			reader = new StringReader(fieldText);

			parser = new CSharpParser();
			elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
			Assert.AreEqual("string", fieldElement.Type, "Unexpected member type.");
			Assert.AreEqual("@\"\\\\Server\\share\\\"", fieldElement.InitialValue, "Unexpected initial value.");
		}

		/// <summary>
		/// Tests parsing header comments.
		/// </summary>
		[Test]
		public void ParseHeaderCommentsBlockTest()
		{
			StringReader reader = new StringReader(
				"/*Comment1\r\nComment2*/\r\npublic TestClass(){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access, "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Parameters, "Unexpected parameter string.");
			Assert.AreEqual(1, constructorElement.HeaderComments.Count, "Unexpected number of header comment lines.");
			Assert.AreEqual(CommentType.Block, constructorElement.HeaderComments[0].Type, "Expected a block comment.");
			Assert.AreEqual("Comment1\r\nComment2", constructorElement.HeaderComments[0].Text);
		}

		/// <summary>
		/// Tests parsing header comments.
		/// </summary>
		[Test]
		public void ParseHeaderCommentsTest()
		{
			StringReader reader = new StringReader(
				"//Comment1\r\n//Comment2\r\npublic TestClass(){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
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
				"///<summary>Comment1\r\n///Comment2</summary>\r\npublic TestClass(){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement, "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name, "Unexpected name.");
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
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetInterfaceDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.IsNotNull(elements, "Code element collection should not be null.");
				Assert.AreEqual(3, elements.Count, "An unexpected number of elements were parsed.");

				Assert.IsTrue(elements[0] is CommentElement);

				UsingElement using1 = elements[1] as UsingElement;
				Assert.IsNotNull(using1, "Expected a UsingElement.");
				Assert.AreEqual("System", using1.Name, "Unexpected using name.");

				NamespaceElement namespaceElement = elements[2] as NamespaceElement;
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
		/// Tests parsing an event from an interface.
		/// </summary>
		[Test]
		public void ParseInterfaceEventTest()
		{
			StringReader reader = new StringReader(
				"public interface ISettings{\r\nevent SettingsEventHandler Changed;\r\n}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			TypeElement typeElement = elements[0] as TypeElement;
			Assert.IsNotNull(typeElement, "Element is not a type element.");
			Assert.AreEqual(TypeElementType.Interface, typeElement.Type, "Expected an interface.");
			Assert.AreEqual("ISettings", typeElement.Name, "Unexpected type name.");
			Assert.AreEqual(CodeAccess.Public, typeElement.Access, "Unexpected type access.");

			Assert.AreEqual(1, typeElement.Children.Count, "An unexpected number of child elements were parsed.");
			EventElement eventElement = typeElement.Children[0] as EventElement;
			Assert.IsNotNull(eventElement, "Expected an event element.");
			Assert.AreEqual(CodeAccess.None, eventElement.Access);
			Assert.AreEqual("SettingsEventHandler", eventElement.Type);
			Assert.AreEqual("Changed", eventElement.Name);
			Assert.IsNull(eventElement.BodyText);
		}

		/// <summary>
		/// Tests parsing an invalid type definition.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		   ExpectedMessage = "Expected {")]
		public void ParseInvalidTypeDefinitionTest()
		{
			StringReader reader = new StringReader(
				"public class struct Test{}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Verifies that the correct number of members are parsed from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseMembersCountTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
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
		/// Tests parsing a method without a closing brace.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Unexpected end of file")]
		public void ParseMethodBodyUnexpectedEndOfFileTest()
		{
			StringReader reader = new StringReader(
				"private void DoSomething(){");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a method with a generic return type.
		/// </summary>
		[Test]
		public void ParseMethodGenericReturnTypeTest()
		{
			string[] variations = new string[]
            {
                "IEnumerator<T> IEnumerable<T>.GetEnumerator()\r\n" +
                "{\r\n" +
                "\treturn null;\r\n" +
                "}",

                "IEnumerator<T> IEnumerable <T>.GetEnumerator()\r\n" +
                "{\r\n" +
                "\treturn null;\r\n" +
                "}"
            };

			foreach (string variation in variations)
			{
				try
				{
					StringReader reader = new StringReader(variation);

					CSharpParser parser = new CSharpParser();
					ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

					Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
					MethodElement methodElement = elements[0] as MethodElement;
					Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
					Assert.AreEqual("IEnumerable<T>.GetEnumerator", methodElement.Name, "Unexpected name.");
					Assert.AreEqual(CodeAccess.None, methodElement.Access, "Unexpected code access.");
					Assert.AreEqual("IEnumerator<T>", methodElement.Type, "Unexpected member type.");
				}
				catch (ParseException ex)
				{
					Assert.Fail(ex.Message + ": " + variation);
				}
			}
		}

		/// <summary>
		/// Tests parsing a method with a nullable return type.
		/// </summary>
		[Test]
		public void ParseMethodNullableReturnTypeTest()
		{
			string[] variations = new string[]
            {
                "int? GetValue()\r\n" +
                "{\r\n" +
                "\treturn null;\r\n" +
                "}",

                "int ? GetValue()\r\n" +
                "{\r\n" +
                "\treturn null;\r\n" +
                "}"
            };

			foreach (string variation in variations)
			{
				try
				{
					StringReader reader = new StringReader(variation);

					CSharpParser parser = new CSharpParser();
					ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

					Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
					MethodElement methodElement = elements[0] as MethodElement;
					Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
					Assert.AreEqual("GetValue", methodElement.Name, "Unexpected name.");
					Assert.AreEqual(CodeAccess.None, methodElement.Access, "Unexpected code access.");
					Assert.AreEqual("int?", methodElement.Type, "Unexpected member type.");
				}
				catch (ParseException ex)
				{
					Assert.Fail(ex.Message + ": " + variation);
				}
			}
		}

		/// <summary>
		/// Tests parsing a partial method declaration.
		/// </summary>
		[Test]
		public void ParseMethodPartialDeclarationTest()
		{
			StringReader reader = new StringReader(
				"partial void DoSomething();");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.None, methodElement.Access, "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type, "Unexpected member type.");
			Assert.IsTrue(methodElement.IsPartial, "Expected a partial method.");
			Assert.IsNull(methodElement.BodyText, "Unexpected body text.");
		}

		/// <summary>
		/// Tests parsing a partial method implementation.
		/// </summary>
		[Test]
		public void ParseMethodPartialImplementationTest()
		{
			StringReader reader = new StringReader(
				"partial void DoSomething()\r\n" +
				"{\r\n" +
				"\t//Do something here\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.None, methodElement.Access, "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type, "Unexpected member type.");
			Assert.IsTrue(methodElement.IsPartial, "Expected a partial method.");
			Assert.IsTrue(methodElement.BodyText.Contains("//Do something here"), "Unexpected body text.");
		}

		/// <summary>
		/// Verifies the parsing of methods from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseMethodsTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
				TypeElement classElement = GetMembersTestClass(reader);

				MethodElement method;

				RegionElement regionElement = classElement.Children[MethodRegionIndex] as RegionElement;
				Assert.IsNotNull(regionElement, "Expected a region element.");

				method = regionElement.Children[0] as MethodElement;
				Assert.IsNotNull(method, "Expected a method.");
				Assert.AreEqual("DoSomething", method.Name, "Unexpected method name.");
				Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
				Assert.AreEqual(2, method.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(method.IsStatic, "Method should not be static.");
				Assert.AreEqual("void", method.Type, "Unexpected return type.");
				Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
				Assert.IsFalse(method.IsOverride, "Method should not be an override.");
				Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
				Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
				Assert.IsFalse(method.IsNew, "Method should not be new.");
				Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual(string.Empty, method.Parameters, "Unexpected parameter string.");

				method = regionElement.Children[1] as MethodElement;
				Assert.IsNotNull(method, "Expected a method.");
				Assert.AreEqual("ToString", method.Name, "Unexpected method name.");
				Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
				Assert.AreEqual(1, method.HeaderComments.Count, "Unexpecte number of comments.");
				Assert.AreEqual(CommentType.Block, method.HeaderComments[0].Type, "Expected a block comment.");
				Assert.AreEqual(" \r\n         * Block comment here\r\n         ", method.HeaderComments[0].Text, "Unexpected header comment text.");
				Assert.IsFalse(method.IsStatic, "Method should not be static.");
				Assert.AreEqual("string", method.Type, "Unexpected return type.");
				Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
				Assert.IsTrue(method.IsOverride, "Method should be an override.");
				Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
				Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
				Assert.IsFalse(method.IsNew, "Method should not be new.");
				Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual(string.Empty, method.Parameters, "Unexpected parameter string.");

				method = regionElement.Children[2] as MethodElement;
				Assert.IsNotNull(method, "Expected a method.");
				Assert.AreEqual("GetBoolValue", method.Name, "Unexpected method name.");
				Assert.AreEqual(CodeAccess.Private, method.Access, "Unexpected access level.");
				Assert.AreEqual(7, method.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(method.IsStatic, "Method should not be static.");
				Assert.AreEqual("bool", method.Type, "Unexpected return type.");
				Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
				Assert.IsFalse(method.IsOverride, "Method should not be an override.");
				Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
				Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
				Assert.IsFalse(method.IsNew, "Method should not be new.");
				Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual("int intParam, string stringParam", method.Parameters, "Unexpected parameter string.");
				Assert.IsTrue(method.BodyText.Contains("return true;"), "Unexpected body text.");

				method = regionElement.Children[3] as MethodElement;
				Assert.IsNotNull(method, "Expected a method.");
				Assert.AreEqual("GetWithParamAttributes", method.Name, "Unexpected method name.");
				Assert.AreEqual(CodeAccess.Internal, method.Access, "Unexpected access level.");
				Assert.AreEqual(5, method.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(method.IsStatic, "Method should be static.");
				Assert.AreEqual("int?", method.Type, "Unexpected return type.");
				Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
				Assert.IsFalse(method.IsOverride, "Method should not be an override.");
				Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
				Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
				Assert.IsFalse(method.IsNew, "Method should not be new.");
				Assert.AreEqual(1, method.Attributes.Count, "Unexpected number of attributes.");
				Assert.IsTrue(
					method.Parameters.Contains(
					"[Description(\"Int parameter\")] int intParam"),
					"Unexpected params string.");
				Assert.IsTrue(
					method.Parameters.Contains(
					"[Description(\"String parameter\")] string stringParam"),
					"Unexpected params string.");

				method = regionElement.Children[4] as MethodElement;
				Assert.IsNotNull(method, "Expected a method.");
				Assert.AreEqual("GetWithTypeParameters", method.Name, "Unexpected method name.");
				Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
				Assert.AreEqual(4, method.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(method.IsStatic, "Method should not be static.");
				Assert.AreEqual("bool", method.Type, "Unexpected return type.");
				Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
				Assert.IsFalse(method.IsOverride, "Method should not be an override.");
				Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
				Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
				Assert.IsFalse(method.IsNew, "Method should not be new.");
				Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual(
					"Action<T1> typeParam1, Action<T2> typeParam2",
					method.Parameters.Trim(),
					"Unexpected params string.");
				Assert.AreEqual(2, method.TypeParameters.Count, "Unexpected number of type parameters.");
				Assert.AreEqual("T1", method.TypeParameters[0].Name, "Unexpected type parameter name.");
				Assert.AreEqual(2, method.TypeParameters[0].Constraints.Count, "Unexpected type parameter constraints.");
				Assert.AreEqual("T2", method.TypeParameters[1].Name, "Unexpected type parameter name.");
				Assert.AreEqual(2, method.TypeParameters[1].Constraints.Count, "Unexpected type parameter constraints.");

				//
				// External method
				//
				method = regionElement.Children[5] as MethodElement;
				Assert.IsNotNull(method, "Expected a method.");
				Assert.AreEqual("MessageBox", method.Name, "Unexpected method name.");
				Assert.AreEqual(CodeAccess.Public, method.Access, "Unexpected access level.");
				Assert.AreEqual(0, method.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(method.IsStatic, "Method should be static.");
				Assert.AreEqual("int", method.Type, "Unexpected return type.");
				Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
				Assert.IsTrue(method.IsExternal, "Method should be external.");
				Assert.IsFalse(method.IsOverride, "Method should not be an override.");
				Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
				Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
				Assert.IsFalse(method.IsNew, "Method should not be new.");
				Assert.AreEqual(1, method.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual(
					"int h, string m, string c, int type",
					method.Parameters.Trim(),
					"Unexpected params string.");
				Assert.AreEqual(0, method.TypeParameters.Count, "Unexpected number of type parameters.");

				//
				// Unsafe method
				//
				method = regionElement.Children[6] as MethodElement;
				Assert.IsNotNull(method, "Expected a method.");
				Assert.AreEqual("UnsafeSqrPtrParam", method.Name, "Unexpected method name.");
				Assert.AreEqual(CodeAccess.None, method.Access, "Unexpected access level.");
				Assert.AreEqual(4, method.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(method.IsStatic, "Method should be static.");
				Assert.AreEqual("void", method.Type, "Unexpected return type.");
				Assert.IsFalse(method.IsAbstract, "Method should not be abstract.");
				Assert.IsTrue(method.IsUnsafe, "Method should be unsafe.");
				Assert.IsFalse(method.IsExternal, "Method should not be external.");
				Assert.IsFalse(method.IsOverride, "Method should not be an override.");
				Assert.IsFalse(method.IsSealed, "Method should not be sealed.");
				Assert.IsFalse(method.IsVirtual, "Method should not be virtual.");
				Assert.IsFalse(method.IsNew, "Method should not be new.");
				Assert.AreEqual(0, method.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual("int* p", method.Parameters.Trim(), "Unexpected params string.");
				Assert.AreEqual(0, method.TypeParameters.Count, "Unexpected number of type parameters.");
			}
		}

		/// <summary>
		/// Tests parsing a method.
		/// </summary>
		[Test]
		public void ParseMethodTest()
		{
			StringReader reader = new StringReader(
				"private void DoSomething(){}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type, "Unexpected member type.");
		}

		/// <summary>
		/// Tests parsing a method with a block character in a comment of the 
		/// body text.
		/// </summary>
		[Test]
		public void ParseMethodWithBlockCharBlockCommentTest()
		{
			StringReader reader = new StringReader(
				"private void DoSomething(){/*Test }*/}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type, "Unexpected member type.");
			Assert.AreEqual("/*Test }*/", methodElement.BodyText, "Unexpected body text.");
		}

		/// <summary>
		/// Tests parsing a method with a block character in a comment of the 
		/// body text.
		/// </summary>
		[Test]
		public void ParseMethodWithBlockCharLineCommentTest()
		{
			StringReader reader = new StringReader(
				"private void DoSomething(){\r\n//Test }\r\n}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type, "Unexpected member type.");
			Assert.AreEqual("//Test }", methodElement.BodyText, "Unexpected body text.");
		}

		/// <summary>
		/// Tests parsing a method with a block character in the body text.
		/// </summary>
		[Test]
		public void ParseMethodWithBlockCharTest()
		{
			StringReader reader = new StringReader(
				"private void DoSomething(){Console.WriteLine(\"}\";Console.WriteLine();}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement, "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name, "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access, "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type, "Unexpected member type.");
			Assert.AreEqual("Console.WriteLine(\"}\";Console.WriteLine();", methodElement.BodyText, "Unexpected body text.");
		}

		/// <summary>
		/// Tests the parsing of a single namespace with a multiple class
		/// definitions.
		/// </summary>
		[Test]
		public void ParseMultiClassDefinitionTest()
		{
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetMultiClassDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.IsNotNull(elements, "Code element collection should not be null.");
				Assert.AreEqual(7, elements.Count, "An unexpected number of elements were parsed.");

				CommentElement comment = elements[0] as CommentElement;
				Assert.IsNotNull(comment, "Expected a CommentElement.");

				UsingElement using1 = elements[1] as UsingElement;
				Assert.IsNotNull(using1, "Expected a UsingElement.");
				Assert.AreEqual("System", using1.Name, "Unexpected using name.");

				UsingElement using2 = elements[2] as UsingElement;
				Assert.IsNotNull(using2, "Expected a UsingElement.");
				Assert.AreEqual("System.Collections.Generic", using2.Name, "Unexpected using name.");

				UsingElement using3 = elements[3] as UsingElement;
				Assert.IsNotNull(using3, "Expected a UsingElement.");
				Assert.AreEqual("System.Text", using3.Name, "Unexpected using name.");

				UsingElement using4 = elements[4] as UsingElement;
				Assert.IsNotNull(using4, "Expected a UsingElement.");
				Assert.AreEqual("System.ComponentModel", using4.Name, "Unexpected using name.");

				NamespaceElement namespaceElement = elements[5] as NamespaceElement;
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
				Assert.AreEqual("List<int>", classElement3.Interfaces[0].Name, "Unexpected interface name.");
				Assert.AreEqual("IDisposable", classElement3.Interfaces[1].Name, "Unexpected interface name.");
				Assert.AreEqual("IComparable", classElement3.Interfaces[2].Name, "Unexpected interface name.");
				Assert.IsFalse(classElement3.IsStatic, "Class should not be static.");
				Assert.IsFalse(classElement3.IsSealed, "Class should not be sealed.");

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
				Assert.AreEqual("new()", parameter1.Constraints[2], "Unexpected type parameter contraint.");

				TypeParameter parameter2 = classElement4.TypeParameters[1];
				Assert.AreEqual("T2", parameter2.Name, "Unexpected type parameter name.");

				// where T2 : class, IComparable, IConvertible, new()
				Assert.AreEqual(4, parameter2.Constraints.Count, "Unexpected number of type parameter constraints.");
				Assert.AreEqual("class", parameter2.Constraints[0], "Unexpected type parameter contraint.");
				Assert.AreEqual("IComparable<T2>", parameter2.Constraints[1], "Unexpected type parameter contraint.");
				Assert.AreEqual("global::System.IConvertible", parameter2.Constraints[2], "Unexpected type parameter contraint.");
				Assert.AreEqual("new()", parameter2.Constraints[3], "Unexpected type parameter contraint.");

				//
				// Sample class 5
				//
				TypeElement classElement5 = namespaceElement.Children[4] as TypeElement;
				Assert.IsNotNull(classElement5, "Expected a TypeElement.");
				Assert.AreEqual("SampleClass5", classElement5.Name, "Unexpected class name.");
				Assert.AreEqual(CodeAccess.Internal, classElement5.Access, "Unexpected class code access level.");
				Assert.IsTrue(classElement5.IsStatic, "Class should be static.");
				Assert.IsFalse(classElement5.IsSealed, "Class should not be sealed.");

				//
				// Sample class 6
				//
				TypeElement classElement6 = namespaceElement.Children[5] as TypeElement;
				Assert.IsNotNull(classElement6, "Expected a TypeElement.");
				Assert.AreEqual("SampleClass6", classElement6.Name, "Unexpected class name.");
				Assert.AreEqual(CodeAccess.Public, classElement6.Access, "Unexpected class code access level.");
				Assert.IsTrue(classElement6.IsStatic, "Class should be static.");
				Assert.IsFalse(classElement6.IsSealed, "Class should not be sealed.");

				//
				// Sample class 7
				//
				TypeElement classElement7 = namespaceElement.Children[6] as TypeElement;
				Assert.IsNotNull(classElement7, "Expected a TypeElement.");
				Assert.AreEqual("SampleClass7", classElement7.Name, "Unexpected class name.");
				Assert.AreEqual(CodeAccess.Public, classElement7.Access, "Unexpected class code access level.");
				Assert.IsFalse(classElement7.IsStatic, "Class should be static.");
				Assert.IsTrue(classElement7.IsSealed, "Class should be sealed.");
				Assert.AreEqual(2, classElement7.Interfaces.Count, "Unexpected number of interfaces.");
				Assert.AreEqual("global::System.IDisposable", classElement7.Interfaces[0].Name, "Unexpected interface name.");
				Assert.AreEqual("IComparable<int>", classElement7.Interfaces[1].Name, "Unexpected interface name.");

				//
				// Sample class 8
				//
				TypeElement classElement8 = namespaceElement.Children[7] as TypeElement;
				Assert.IsNotNull(classElement8, "Expected a TypeElement.");
				Assert.AreEqual("SampleClass8", classElement8.Name, "Unexpected class name.");
				Assert.AreEqual(CodeAccess.Public, classElement8.Access, "Unexpected class code access level.");
				Assert.IsFalse(classElement8.IsStatic, "Class should be static.");
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
				Assert.AreEqual(2, classElement4.TypeParameters.Count, "Unexpected number of type parameters.");

				//
				// Global class
				//
				TypeElement globalClassElement = elements[6] as TypeElement;
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
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetMultipleNamespaceFile();
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
		/// Tests parsing of a namepsace where a closing 
		/// brace is expected.";
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Expected }")]
		public void ParseNamespaceExpectedBlockEnd()
		{
			StringReader reader = new StringReader(
				"namespace TestNamespace{");

			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing of a namepsace where an opening 
		/// brace is expected.";
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Expected {")]
		public void ParseNamespaceExpectedBlockStart()
		{
			StringReader reader = new StringReader(
				"namespace TestNamespace");

			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}

		/// <summary>
		/// Verifies the parsing of nested types from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseNestedTypesTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
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
				Assert.IsTrue(type.IsStatic, "Type should be static.");
				Assert.IsFalse(type.IsAbstract, "Type should not be abstract.");
				Assert.IsFalse(type.IsSealed, "Type should not be sealed.");
				Assert.AreEqual(0, type.Attributes.Count, "Unexpected number of attributes.");
				Assert.AreEqual(1, type.Children.Count, "Unexpected number of child elements.");
			}
		}

		/// <summary>
		/// Tests parsing a null stream.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseNullStreamTest()
		{
			CSharpParser parser = new CSharpParser();
			parser.Parse(null);
		}

		/// <summary>
		/// Verifies the parsing of operators.
		/// </summary>
		[Test]
		public void ParseOperatorsTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetOperatorsFile();
			using (TextReader reader = testFile.GetReader())
			{
				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.AreEqual(3, elements.Count, "Unexpected number of top-level elements.");

				NamespaceElement namespaceElement = elements[2] as NamespaceElement;
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

				FieldElement fieldElement1 = classElement.Children[0] as FieldElement;
				Assert.IsNotNull(fieldElement1, "Expected a field element.");
				Assert.AreEqual("num", fieldElement1.Name, "Unexpected field name.");
				Assert.AreEqual("int", fieldElement1.Type, "Unexpected field type.");

				FieldElement fieldElement2 = classElement.Children[1] as FieldElement;
				Assert.IsNotNull(fieldElement2, "Expected a field element.");
				Assert.AreEqual("den", fieldElement2.Name, "Unexpected field name.");
				Assert.AreEqual("int", fieldElement2.Type, "Unexpected field type.");

				ConstructorElement constructorElement = classElement.Children[2] as ConstructorElement;
				Assert.IsNotNull(constructorElement, "Expected a constructor element.");
				Assert.AreEqual("Fraction", constructorElement.Name, "Unexpected constructor name.");
				Assert.AreEqual("int num, int den", constructorElement.Parameters, "Unexpected constructor parameters.");

				MethodElement operatorElement = classElement.Children[3] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.AreEqual("+", operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("Fraction", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction a, Fraction b", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("return"), "Unexpected operator body text.");

				operatorElement = classElement.Children[4] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.AreEqual("*", operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("Fraction", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction a, Fraction b", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("return"), "Unexpected operator body text.");

				operatorElement = classElement.Children[5] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.AreEqual("/", operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("Fraction", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction a, Fraction b", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("return"), "Unexpected operator body text.");

				operatorElement = classElement.Children[6] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.AreEqual("==", operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("bool", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction a, Fraction b", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("return"), "Unexpected operator body text.");

				operatorElement = classElement.Children[7] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.AreEqual("!=", operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("bool", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction a, Fraction b", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("return"), "Unexpected operator body text.");

				operatorElement = classElement.Children[8] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.AreEqual("<=", operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("bool", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction a, Fraction b", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("throw"), "Unexpected operator body text.");

				operatorElement = classElement.Children[9] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.AreEqual(">=", operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.None, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("bool", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction a, Fraction b", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("throw"), "Unexpected operator body text.");

				operatorElement = classElement.Children[10] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.IsNull(operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.Implicit, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("double", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction f", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("return"), "Unexpected operator body text.");

				operatorElement = classElement.Children[11] as MethodElement;
				Assert.IsNotNull(operatorElement, "Expected a method element.");
				Assert.IsTrue(operatorElement.IsOperator, "Expected method to be an operator.");
				Assert.IsNull(operatorElement.Name, "Unexpected operator name.");
				Assert.AreEqual(OperatorType.Explicit, operatorElement.OperatorType, "Unexpected operator attributes.");
				Assert.AreEqual(CodeAccess.Public, operatorElement.Access, "Unexpected operator access.");
				Assert.IsTrue(operatorElement.IsStatic, "Expected operator to be static.");
				Assert.AreEqual("decimal", operatorElement.Type, "Unexpected operator return type.");
				Assert.AreEqual("Fraction f", operatorElement.Parameters, "Unexpected operator parameters.");
				Assert.IsTrue(operatorElement.BodyText.Contains("return"), "Unexpected operator body text.");
			}
		}

		/// <summary>
		/// Verifies the parsing of properties members from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParsePropertiesTest()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
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
				Assert.AreEqual("bool", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsTrue(
					property.BodyText.Contains("set"), "Unexpeced body text.");

				property = regionElement.Children[1] as PropertyElement;
				Assert.IsNotNull(property, "Expected a property.");
				Assert.AreEqual("ProtectedProperty", property.Name, "Unexpected property name.");
				Assert.AreEqual(CodeAccess.Protected, property.Access, "Unexpected access level.");
				Assert.AreEqual(3, property.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(property.IsStatic, "Property should not be static.");
				Assert.AreEqual("int", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsTrue(property.IsVirtual, "Property should be virtual.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsFalse(
					property.BodyText.Contains("set"), "Unexpeced body text.");

				property = regionElement.Children[2] as PropertyElement;
				Assert.IsNotNull(property, "Expected a property.");
				Assert.AreEqual("StaticProperty", property.Name, "Unexpected property name.");
				Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
				Assert.AreEqual(5, property.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(property.IsStatic, "Property should be static.");
				Assert.AreEqual("string", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsFalse(
					property.BodyText.Contains("set"), "Unexpeced body text.");
				Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");

				property = regionElement.Children[3] as PropertyElement;
				Assert.IsNotNull(property, "Expected a property.");
				Assert.AreEqual("AttributedProperty", property.Name, "Unexpected property name.");
				Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
				Assert.AreEqual(2, property.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsTrue(property.IsStatic, "Property should be static.");
				Assert.AreEqual("string", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsFalse(
					property.BodyText.Contains("set"), "Unexpeced body text.");
				Assert.AreEqual(2, property.Attributes.Count, "Unexpected number of attributes.");

				property = regionElement.Children[4] as PropertyElement;
				Assert.IsNotNull(property, "Expected a property.");
				Assert.AreEqual("GenericProperty", property.Name, "Unexpected property name.");
				Assert.AreEqual(CodeAccess.Internal, property.Access, "Unexpected access level.");
				Assert.AreEqual(6, property.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(property.IsStatic, "Property should not be static.");
				Assert.AreEqual("Nullable<int>", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
				Assert.IsFalse(property.IsNew, "Property should not be new.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsTrue(
					property.BodyText.Contains("set"), "Unexpeced body text.");
				Assert.AreEqual(1, property.Attributes.Count, "Unexpected number of attributes.");
				IAttributeElement attributeElement = property.Attributes[0];
				Assert.IsNull(attributeElement.Target, "Unexpected attribute target.");
				Assert.AreEqual("Obsolete", property.Attributes[0].Name);
				Assert.IsNull(attributeElement.BodyText, "Unexpected attribute body text.");
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
				Assert.AreEqual("string[]", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
				Assert.IsTrue(property.IsNew, "Property should be new.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsFalse(
					property.BodyText.Contains("set"), "Unexpeced body text.");
				Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");

				//
				// Indexer property
				//
				property = regionElement.Children[6] as PropertyElement;
				Assert.IsNotNull(property, "Expected a property.");
				Assert.AreEqual("this", property.Name, "Unexpected property name.");
				Assert.AreEqual("int index", property.IndexParameter, "Unexpected index parameter.");
				Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
				Assert.AreEqual(5, property.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(property.IsStatic, "Property should not be static.");
				Assert.AreEqual("string", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
				Assert.IsFalse(property.IsNew, "Property should not be new.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsTrue(
					property.BodyText.Contains("set"), "Unexpeced body text.");
				Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");

				property = regionElement.Children[7] as PropertyElement;
				Assert.IsNotNull(property, "Expected a property.");
				Assert.AreEqual("this", property.Name, "Unexpected property name.");
				Assert.AreEqual("string string1, string string2", property.IndexParameter, "Unexpected index parameter.");
				Assert.AreEqual(CodeAccess.Public, property.Access, "Unexpected access level.");
				Assert.AreEqual(0, property.HeaderComments.Count, "Unexpected number of header comment lines.");
				Assert.IsFalse(property.IsStatic, "Property should not be static.");
				Assert.AreEqual("int", property.Type, "Unexpected property type.");
				Assert.IsFalse(property.IsAbstract, "Property should not be abstract.");
				Assert.IsFalse(property.IsOverride, "Property should not be an override.");
				Assert.IsFalse(property.IsSealed, "Property should not be sealed.");
				Assert.IsFalse(property.IsVirtual, "Property should not be virtual.");
				Assert.IsFalse(property.IsNew, "Property should not be new.");
				Assert.IsTrue(
					property.BodyText.Contains("get"), "Unexpected body text.");
				Assert.IsTrue(
					property.BodyText.Contains("set"), "Unexpeced body text.");
				Assert.AreEqual(0, property.Attributes.Count, "Unexpected number of attributes.");
			}
		}

		/// <summary>
		/// Tests parsing an indexer property.
		/// </summary>
		[Test]
		public void ParsePropertyIndexerTest()
		{
			string[] variations =
                {
                    "public bool this[int index]\r\n" +
                    "{\r\n" +
                    "\treturn true;\r\n" +
                    "}",

                    "public bool this [int index]\r\n" +
                    "{\r\n" +
                    "\treturn true;\r\n" +
                    "}",

                    "public bool this [ int index ]\r\n" +
                    "{\r\n" +
                    "\treturn true;\r\n" +
                    "}",

                    "public bool this [\r\n" +
                    "int index ]\r\n" +
                    "{\r\n" +
                    "\treturn true;\r\n" +
                    "}",

                    "public bool this [ int index\r\n" +
                    "]\r\n" +
                    "{\r\n" +
                    "\treturn true;\r\n" +
                    "}"
                };

			foreach (string variation in variations)
			{
				StringReader reader = new StringReader(variation);
				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
				PropertyElement propertyElement = elements[0] as PropertyElement;
				Assert.IsNotNull(propertyElement, "Element is not a PropertyElement.");
				Assert.AreEqual("this", propertyElement.Name, "Unexpected name.");
				Assert.AreEqual("int index", propertyElement.IndexParameter, "Unexpected index parameter.");
				Assert.AreEqual(CodeAccess.Public, propertyElement.Access, "Unexpected code access.");
				Assert.AreEqual("bool", propertyElement.Type, "Unexpected member type.");
				Assert.IsTrue(propertyElement.BodyText.Contains("return true;"), "Unexpected body text.");
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
				"public class Test\r\n" +
				"{\r\n" +
				"\t// $(Begin) Fields\r\n" +
				"\t//$( Begin )  Private\r\n" +
				"\tprivate bool _test = false;\r\n" +
				"\t// $(End) Private\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing nested regions that use comment directives.  
		/// </summary>
		[Test]
		public void ParseRegionCommentDirectiveTest()
		{
			StringReader reader = new StringReader(
				"public class Test\r\n" +
				"{\r\n" +
				"\t// $(Begin) Fields\r\n" +
				"\t//$( Begin )  Private\r\n" +
				"\tprivate bool _test = false;\r\n" +
				"\t// $(End) Private\r\n" +
				"\t//$(End)\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
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
				"//Leading\r\n" +
				"#region Comments\r\n" +
				"//Enclosed\r\n" +
				"#endregion");

			CSharpParser parser = new CSharpParser();
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
				"{\r\n" +
				"\t#region Fields\r\n" +
				"\t#region Private\r\n" +
				"\tprivate bool _test = false;\r\n" +
				"\t#endregion\r\n" +
				"\t#endregion\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
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
		/// Tests an unmatched end region directive.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Unmatched end region directive")]
		public void ParseRegionUnmatchedEndDirectiveTest()
		{
			StringReader reader = new StringReader("\t#endregion Fields");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests the parsing of a single namespace.
		/// </summary>
		[Test]
		public void ParseSingleNamespaceTest()
		{
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetSingleNamespaceFile();
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
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetStructDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.IsNotNull(elements, "Code element collection should not be null.");
				Assert.AreEqual(3, elements.Count, "An unexpected number of elements were parsed.");

				Assert.IsTrue(elements[0] is CommentElement);

				UsingElement using1 = elements[1] as UsingElement;
				Assert.IsNotNull(using1, "Expected a UsingElement.");
				Assert.AreEqual("System", using1.Name, "Unexpected using name.");

				NamespaceElement namespaceElement = elements[2] as NamespaceElement;
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
		/// Tests parsing a non-region preprocessor directive.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
		  ExpectedMessage = "Cannot arrange files with preprocessor directives")]
		public void ParseUnhandledPreprocessorTest()
		{
			StringReader reader = new StringReader(
				"public class Test\r\n" +
				"{\r\n" +
				"#pragma warning disable 414, 3021\r\n" +
				"}");

			CSharpParser parser = new CSharpParser();
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
			StringReader reader = new StringReader("blah");

			CSharpParser parser = new CSharpParser();
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
				"using ;");

			CSharpParser parser = new CSharpParser();
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
				"using Test = ;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a using statement where an end 
		/// of statement is expected.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
			MatchType = MessageMatch.Contains,
			ExpectedMessage = "Expected = or ;")]
		public void ParseUsingExpectedStatementEnd()
		{
			StringReader reader = new StringReader(
				"using System.Text");

			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}

		/// <summary>
		/// Tests parsing a using statement that redefines a class to a generic type.
		/// </summary>
		[Test]
		public void ParseUsingRedefineGenericTest()
		{
			StringReader reader = new StringReader(
				"using Redefined = System.Collections.Generic.Dictionary<int,object>;");
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			UsingElement usingElement = elements[0] as UsingElement;
			Assert.IsNotNull(usingElement, "Element is not a UsingElement.");
			Assert.AreEqual("Redefined", usingElement.Name);
			Assert.AreEqual("System.Collections.Generic.Dictionary<int,object>", usingElement.Redefine);
		}

		/// <summary>
		/// Tests parsing a using statement that redefines a class/namespace.
		/// </summary>
		[Test]
		public void ParseUsingRedefineTest()
		{
			StringReader reader = new StringReader(
				"using Redefined = System.Text.Encoder;");
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			UsingElement usingElement = elements[0] as UsingElement;
			Assert.IsNotNull(usingElement, "Element is not a UsingElement.");
			Assert.AreEqual("Redefined", usingElement.Name);
			Assert.AreEqual("System.Text.Encoder", usingElement.Redefine);
		}

		/// <summary>
		/// Tests parsing a simple using statement.
		/// </summary>
		[Test]
		public void ParseUsingTest()
		{
			StringReader reader = new StringReader(
				"using System.Text;");

			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(1, elements.Count, "An unexpected number of elements were parsed.");
			UsingElement usingElement = elements[0] as UsingElement;
			Assert.IsNotNull(usingElement, "Element is not a UsingElement.");
			Assert.AreEqual("System.Text", usingElement.Name, "Unexpected name.");
			Assert.IsTrue(usingElement.IsMovable, "C# should support moving using directives.");
		}

		/// <summary>
		/// Tests parsing a file with alternative encoding.
		/// </summary>
		[Test]
		public void ParseUTF8Test()
		{
			CSharpParser parser = new CSharpParser();

			CSharpTestFile testFile = CSharpTestUtilities.GetUTF8File();
			using (TextReader reader = testFile.GetReader())
			{
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

				Assert.IsNotNull(elements, "Code element collection should not be null.");

				TypeElement classElement = elements[0] as TypeElement;
				Assert.IsNotNull(classElement, "Expected a class element.");
				Assert.AreEqual("UnicodeClass", classElement.Name, "Unexpected class name.");

				Assert.AreEqual(1, classElement.Children.Count, "Unexpected number of child elements.");
				RegionElement regionElement = classElement.Children[0] as RegionElement;
				Assert.IsNotNull(regionElement, "Element is not a RegionElement.");

				Assert.AreEqual(1, regionElement.Children.Count, "Unexpected number of child elements.");
				FieldElement fieldElement = regionElement.Children[0] as FieldElement;
				Assert.IsNotNull(fieldElement, "Element is not a FieldElement.");
				Assert.AreEqual("val", fieldElement.Name, "Unexpected name.");
				Assert.AreEqual(CodeAccess.Private, fieldElement.Access, "Unexpected code access.");
				Assert.AreEqual("string", fieldElement.Type, "Unexpected member type.");
				Assert.AreEqual(1, fieldElement.HeaderComments.Count, "Unexpected number of header comments.");
			}
		}

		/// <summary>
		/// Gets the ClassMembers test class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns>Type code element.</returns>
		private static TypeElement GetMembersTestClass(TextReader reader)
		{
			TypeElement classElement;
			CSharpParser parser = new CSharpParser();

			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

			Assert.AreEqual(9, elements.Count, "Unexpected number of top-level elements.");

			NamespaceElement namespaceElement = elements[8] as NamespaceElement;
			Assert.IsNotNull(namespaceElement, "Expected a namespace element.");

			Assert.AreEqual(2, namespaceElement.Children.Count, "Unexpected number of namespace elements.");

			UsingElement usingElement = namespaceElement.Children[0] as UsingElement;
			Assert.IsNotNull(usingElement, "Expected a using element.");
			Assert.AreEqual("System.ComponentModel", usingElement.Name, "Unexpected using element name.");

			classElement = namespaceElement.Children[1] as TypeElement;
			Assert.IsNotNull(classElement, "Expected a type element.");
			Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class type.");
			Assert.AreEqual("SampleClass", classElement.Name, "Unexpected class name.");
			Assert.AreEqual(4, classElement.HeaderComments.Count, "Unexpected number of header comments.");

			return classElement;
		}

		#endregion Methods
	}
}