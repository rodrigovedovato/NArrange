namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture base for the CodeElement class.
    /// </summary>
    /// <typeparam name="TCodeElement">Code element type.</typeparam>
    public abstract class CodeElementTests<TCodeElement>
        where TCodeElement : CodeElement, new()
    {
        #region Methods

        /// <summary>
        /// Tests the AddChild method.
        /// </summary>
        [Test]
        public void AddChildTest()
        {
            TCodeElement codeElement = new TCodeElement();

            GroupElement child1 = new GroupElement("Test");
            GroupElement child2 = new GroupElement("Ignore");
            codeElement.AddChild(child1);
            codeElement.AddChild(child2);

            Assert.AreEqual(2, codeElement.Children.Count, "Children were not added correctly.");

            Assert.AreSame(codeElement, child1.Parent, "Attribute parent was not set correctly.");
            Assert.AreSame(codeElement, child2.Parent, "Attribute parent was not set correctly.");

            codeElement.AddChild(child2);

            Assert.AreEqual(2, codeElement.Children.Count, "Attribute should not have been added again.");

            Assert.IsTrue(codeElement.Children.Contains(child1));
            Assert.IsTrue(codeElement.Children.Contains(child2));
        }

        /// <summary>
        /// Tests the ClearChildren method.
        /// </summary>
        [Test]
        public void ClearChildrenTest()
        {
            TCodeElement codeElement = new TCodeElement();

            GroupElement child1 = new GroupElement("Test");
            GroupElement child2 = new GroupElement("Ignore");
            codeElement.AddChild(child1);
            codeElement.AddChild(child2);

            Assert.AreEqual(2, codeElement.Children.Count, "Children were not added correctly.");

            codeElement.ClearChildren();

            Assert.AreEqual(0, codeElement.Children.Count, "Children were not cleared correctly.");

            Assert.IsNull(child1.Parent, "Attribute parent should have been cleared.");
            Assert.IsNull(child2.Parent, "Attribute parent should have been cleared.");
        }

        /// <summary>
        /// Tests the clone method.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            const string Key1 = "Test1";
            const string Key2 = "Test2";

            TCodeElement original = DoCreateClonePrototype();
            original[Key1] = "SomeValue";
            original[Key2] = false;

            TCodeElement clone = original.Clone() as TCodeElement;
            Assert.IsNotNull(
                clone,
                "Clone did not create an instance of type {0}.",
                typeof(TCodeElement).Name);
            Assert.AreNotSame(original, clone, "Clone should be a different instance.");

            Assert.AreEqual(
                original[Key1],
                clone[Key1],
                "Extended properties were not cloned correctly.");
            Assert.AreEqual(
                original[Key2],
                clone[Key2],
                "Extended properties were not cloned correctly.");

            DoVerifyClone(original, clone);
        }

        /// <summary>
        /// Tests the InsertChild method.
        /// </summary>
        [Test]
        public void InsertChildTest()
        {
            TCodeElement codeElement = new TCodeElement();

            GroupElement child1 = new GroupElement("Test");
            GroupElement child2 = new GroupElement("Ignore");
            codeElement.AddChild(child1);
            codeElement.InsertChild(0, child2);

            Assert.AreEqual(2, codeElement.Children.Count, "Children were not added correctly.");

            Assert.AreEqual(0, codeElement.Children.IndexOf(child2));
            Assert.AreEqual(1, codeElement.Children.IndexOf(child1));

            Assert.AreSame(codeElement, child1.Parent, "Attribute parent was not set correctly.");
            Assert.AreSame(codeElement, child2.Parent, "Attribute parent was not set correctly.");

            codeElement.InsertChild(1, child2);

            Assert.AreEqual(2, codeElement.Children.Count, "Attribute should not have been added again.");

            Assert.AreEqual(0, codeElement.Children.IndexOf(child1));
            Assert.AreEqual(1, codeElement.Children.IndexOf(child2));
        }

        /// <summary>
        /// Tests getting and setting the parent property.
        /// </summary>
        [Test]
        public virtual void ParentTest()
        {
            TCodeElement parentElement = new TCodeElement();
            TCodeElement childElement = new TCodeElement();
            Assert.IsNull(childElement.Parent, "Parent should not be set.");

            childElement.Parent = parentElement;
            Assert.AreSame(
                parentElement,
                childElement.Parent,
                "Parent was not set correctly.");

            Assert.IsTrue(
                parentElement.Children.Contains(childElement),
                "Parent Children collection does not contain the child element.");

            childElement.Parent = null;
            Assert.IsNull(childElement.Parent, "Parent should not be set.");

            Assert.IsFalse(
                parentElement.Children.Contains(childElement),
                "Parent Children collection should not contain the child element.");

            parentElement.AddChild(childElement);
            Assert.AreSame(
                parentElement,
                childElement.Parent,
                 "Parent was not set correctly.");

            parentElement.RemoveChild(childElement);
            Assert.IsNull(childElement.Parent, "Parent should not be set.");
        }

        /// <summary>
        /// Tests the RemoveChild method.
        /// </summary>
        [Test]
        public void RemoveChildTest()
        {
            TCodeElement codeElement1 = new TCodeElement();

            GroupElement child1 = new GroupElement("Test");
            GroupElement child2 = new GroupElement("Ignore");
            codeElement1.AddChild(child1);
            codeElement1.AddChild(child2);

            Assert.AreEqual(2, codeElement1.Children.Count, "Children were not added correctly.");

            //
            // Remove the attribute using the method
            //
            codeElement1.RemoveChild(child2);

            Assert.AreEqual(1, codeElement1.Children.Count, "Attribute should have been removed.");

            Assert.IsTrue(codeElement1.Children.Contains(child1));
            Assert.IsFalse(codeElement1.Children.Contains(child2));

            //
            // Remove the attribute by assigning a different parent
            //
            TCodeElement codeElement2 = new TCodeElement();
            child1.Parent = codeElement2;

            Assert.AreEqual(
                0,
                codeElement1.Children.Count,
                "Attribute should have been removed from the original element.");
            Assert.AreEqual(
                1,
                codeElement2.Children.Count,
                "Attribute should have been added to the new element.");

            Assert.IsFalse(codeElement1.Children.Contains(child1));
            Assert.IsTrue(codeElement2.Children.Contains(child1));
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public virtual void ToStringTest()
        {
            DoToStringTest();
        }

        /// <summary>
        /// Creates an instance to be cloned.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected abstract TCodeElement DoCreateClonePrototype();

        /// <summary>
        /// Performs the ToString test.
        /// </summary>
        protected virtual void DoToStringTest()
        {
            TCodeElement codeElement = new TCodeElement();
            codeElement.Name = "Element";
            string str = codeElement.ToString();
            Assert.AreEqual("Element", str, "Unexpected string representation.");
        }

        /// <summary>
        /// Verifies that a clone has the same state as the original.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected abstract void DoVerifyClone(TCodeElement original, TCodeElement clone);

        #endregion Methods
    }
}