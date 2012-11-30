namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the EventElement class.
    /// </summary>
    [TestFixture]
    public class EventElementTests : AttributedElementTests<EventElement>
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a new instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            EventElement eventElement = new EventElement();

            //
            // Verify default property values
            //
            Assert.AreEqual(ElementType.Event, eventElement.ElementType, "Unexpected element type.");
            Assert.AreEqual(CodeAccess.Public, eventElement.Access, "Unexpected default value for Access.");
            Assert.IsNull(eventElement.Parameters, "Unexpected default value for Params.");
            Assert.IsNotNull(eventElement.Attributes, "Attributes collection should be instantiated.");
            Assert.AreEqual(0, eventElement.Attributes.Count, "Attributes collection should be empty.");
            Assert.IsNull(eventElement.BodyText, "Unexpected default value for BodyText.");
            Assert.IsNotNull(eventElement.Children, "Children collection should be instantiated.");
            Assert.AreEqual(0, eventElement.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(eventElement.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, eventElement.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
            Assert.IsFalse(eventElement.IsAbstract, "Unexpected default value for IsAbstract.");
            Assert.IsFalse(eventElement.IsSealed, "Unexpected default value for IsSealed.");
            Assert.IsFalse(eventElement.IsStatic, "Unexpected default value for IsStatic.");
            Assert.AreEqual(string.Empty, eventElement.Name, "Unexpected default value for Name.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override EventElement DoCreateClonePrototype()
        {
            EventElement prototype = new EventElement();
            prototype.Name = "SomeEvent";
            prototype.Access = CodeAccess.Internal;
            prototype.AddAttribute(new AttributeElement("Obsolete"));
            prototype.Type = "EventHandler";

            prototype.AddHeaderCommentLine("/// <summary>");
            prototype.AddHeaderCommentLine("/// This is an event.");
            prototype.AddHeaderCommentLine("/// </summary>");

            prototype.Parameters = "ByVal ars As EventArgs";
            prototype.BodyText = "add{}remove{}";

            prototype.MemberModifiers = MemberModifiers.Abstract;

            return prototype;
        }

        /// <summary>
        /// Verifies the clone was succesful.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(EventElement original, EventElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name, "Name was not copied correctly.");
            Assert.AreEqual(original.Access, clone.Access, "Access was not copied correctly.");
            Assert.AreEqual(original.Attributes.Count, clone.Attributes.Count, "Attributes were not copied correctly.");
            Assert.AreEqual(original.BodyText, clone.BodyText, "BodyText was not copied correctly.");
            Assert.AreEqual(original.Children.Count, clone.Children.Count, "Children were not copied correctly.");
            Assert.AreEqual(original.HeaderComments.Count, clone.HeaderComments.Count, "HeaderCommentLines were not copied correctly.");
            Assert.AreEqual(original.IsAbstract, clone.IsAbstract, "IsAbstract was not copied correctly.");
            Assert.AreEqual(original.IsSealed, clone.IsSealed, "IsSealed was not copied correctly.");
            Assert.AreEqual(original.IsStatic, clone.IsStatic, "IsStatic was not copied correctly.");
            Assert.AreEqual(original.Type, clone.Type, "Type was not copied correctly.");
            Assert.AreEqual(original.Parameters, clone.Parameters, "Parameters was not copied correctly.");
        }

        #endregion Methods
    }
}