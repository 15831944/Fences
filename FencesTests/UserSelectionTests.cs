using Fences;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fences.Tests
{
    [TestFixture()]
    public class UserSelectionTests
    {
        private bool a = true;
        readonly UserSelection _userSelection = new UserSelection();
        [Test()]
        public void LengthOfAllSegmentsMustBeDivisibleByTen()
        {
            int[] segments = _userSelection.Divide(5000, 0, 3);
            Assert.AreEqual(8, segments.Length);
            Assert.AreEqual(100, segments[0]);
            Assert.AreEqual(800, segments[1]);
            Assert.AreEqual(790, segments[2]);
            Assert.AreEqual(790, segments[3]);
            Assert.AreEqual(790, segments[4]);
            Assert.AreEqual(790, segments[5]);
            Assert.AreEqual(790, segments[6]);
            Assert.AreEqual(150, segments[7]);
        }

        [Test()]
        public void Test()
        {
            int[] segments = _userSelection.Divide(5000, 0, 1);
            Assert.AreEqual(8, segments.Length);
            Assert.AreEqual(100, segments[0]);
            Assert.AreEqual(800, segments[1]);
            Assert.AreEqual(800, segments[2]);
            Assert.AreEqual(800, segments[3]);
            Assert.AreEqual(800, segments[4]);
            Assert.AreEqual(800, segments[5]);
            Assert.AreEqual(800, segments[6]);
            Assert.AreEqual(100, segments[7]);
        }

        [Test()]
        public void DivisionWithoutRest()
        {
            int[] segments = _userSelection.Divide(2100, 2, 5);
            Assert.AreEqual(4, segments.Length);
            Assert.AreEqual(150, segments[0]);
            Assert.AreEqual(900, segments[1]);
            Assert.AreEqual(900, segments[2]);
            Assert.AreEqual(150, segments[3]);
        }

        [Test()]
        public void IfLineIsTooSmallThereMustBeOneBar()
        {
            int[] segments = _userSelection.Divide(200, 0, 3);
            Assert.AreEqual(2, segments.Length);
            Assert.AreEqual(100, segments[0]);
            Assert.AreEqual(100, segments[1]);
        }

        [Test()]
        public void IfLastOneIsTooSmallThereMustBeOneBar()
        {
            int[] segments = _userSelection.Divide(230, 2, 3);
            Assert.AreEqual(2, segments.Length);
            Assert.AreEqual(130, segments[0]);
            Assert.AreEqual(100, segments[1]);
        }
    }
}