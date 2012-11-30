namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the EnumUtilities class.
    /// </summary>
    [TestFixture]
    public class EnumUtilitiesTests
    {
        #region Methods

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            string str;

            str = DayOfWeek.Friday.ToString();
            Assert.AreEqual(str, EnumUtilities.ToString(DayOfWeek.Friday));
            Assert.AreEqual(str, EnumUtilities.ToString(DayOfWeek.Friday));

            str = DayOfWeek.Sunday.ToString();
            Assert.AreEqual(str, EnumUtilities.ToString(DayOfWeek.Sunday));
            Assert.AreEqual(str, EnumUtilities.ToString(DayOfWeek.Sunday));

            MemberModifiers modifiers;
            modifiers = MemberModifiers.Override | MemberModifiers.ReadOnly;
            str = modifiers.ToString();
            Assert.AreEqual(str, EnumUtilities.ToString(modifiers));
            Assert.AreEqual(str, EnumUtilities.ToString(modifiers));

            modifiers = MemberModifiers.Override | MemberModifiers.Partial;
            str = modifiers.ToString();
            Assert.AreEqual(str, EnumUtilities.ToString(modifiers));
            Assert.AreEqual(str, EnumUtilities.ToString(modifiers));
        }

        #endregion Methods
    }
}