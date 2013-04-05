using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elastic.Routing.Internals;

namespace Elastic.Routing.Tests
{
    [TestClass]
    public class UtilsTests
    {
        static HashSet<char> NoExtraValidChars = new HashSet<char>();
        static HashSet<char> SlashesIsAlsoValid = new HashSet<char>() { '/', '\\' };

        [TestMethod]
        public void Utils_DashedValue_OnlySpaces()
        {
            var str = "this is my test";
            var expected = "this-is-my-test";
            Assert.AreEqual(expected, Utils.DashedValue(str, NoExtraValidChars));
        }

        [TestMethod]
        public void Utils_DashedValue_SpacesAndDashes()
        {
            var str = " this -  my  test  ";
            var expected = "this-my-test";
            Assert.AreEqual(expected, Utils.DashedValue(str, NoExtraValidChars));
        }

        [TestMethod]
        public void Utils_DashedValue_MaxLength()
        {
            var str = "this is my test";
            var expected = "this-is-my";
            Assert.AreEqual(expected, Utils.DashedValue(str, NoExtraValidChars, 10));
        }

        [TestMethod]
        public void Utils_DashedValue_MaxLength_NoWordCut()
        {
            var str = "this is my test";
            var expected = "this-is-my";
            Assert.AreEqual(expected, Utils.DashedValue(str, NoExtraValidChars, 13));
        }

        [TestMethod]
        public void Utils_DashedValue_SlashIsAllowed()
        {
            var str = " First - me / Next - you ";
            var expected = "First-me/Next-you";
            Assert.AreEqual(expected, Utils.DashedValue(str, SlashesIsAlsoValid));
        }

        [TestMethod]
        public void Utils_DashedValue_MultipleExtraChars()
        {
            var str = " First - me / \\ Next - you ";
            var expected = "First-me/Next-you";
            Assert.AreEqual(expected, Utils.DashedValue(str, SlashesIsAlsoValid));
        }
    }
}
