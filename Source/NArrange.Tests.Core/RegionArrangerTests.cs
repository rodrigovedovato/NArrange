namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the RegionArranger class.
    /// </summary>
    [TestFixture]
    public class RegionArrangerTests
    {
        #region Methods

        /// <summary>
        /// Tests the CanArrange method with various inputs.
        /// </summary>
        [Test]
        public void CanArrangeTest()
        {
            RegionConfiguration methodRegionConfiguration = new RegionConfiguration();
            methodRegionConfiguration.Name = "Methods";
            ElementConfiguration methodConfiguration = new ElementConfiguration();
            methodConfiguration.ElementType = ElementType.Method;
            methodRegionConfiguration.Elements.Add(methodConfiguration);

            RegionConfiguration propertyRegionConfiguration = new RegionConfiguration();
            propertyRegionConfiguration.Name = "Properties";
            ElementConfiguration propertyConfiguration = new ElementConfiguration();
            propertyConfiguration.ElementType = ElementType.Property;
            propertyRegionConfiguration.Elements.Add(propertyConfiguration);

            ElementConfiguration parentConfiguration = new ElementConfiguration();
            parentConfiguration.ElementType = ElementType.Type;

            RegionArranger methodRegionArranger = new RegionArranger(
                methodRegionConfiguration, parentConfiguration);

            RegionArranger propertyRegionArranger = new RegionArranger(
                propertyRegionConfiguration, parentConfiguration);

            MethodElement method = new MethodElement();
            method.Name = "DoSomething";

            Assert.IsTrue(
                methodRegionArranger.CanArrange(method),
                "Expected region arranger to be able to arrange the element.");

            Assert.IsFalse(
                propertyRegionArranger.CanArrange(method),
                "Expected region arranger to not be able to arrange the element.");

            Assert.IsFalse(
                methodRegionArranger.CanArrange(null),
                 "Expected region arranger to not be able to arrange a null element.");
        }

        /// <summary>
        /// Tests creating a RegionArranger with a null parent configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNullParentConfigurationTest()
        {
            RegionArranger regionArranger = new RegionArranger(new RegionConfiguration(), null);
        }

        /// <summary>
        /// Tests creating a RegionArranger with a null region configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNullRegionConfigurationTest()
        {
            RegionArranger regionArranger = new RegionArranger(null, new ElementConfiguration());
        }

        #endregion Methods
    }
}