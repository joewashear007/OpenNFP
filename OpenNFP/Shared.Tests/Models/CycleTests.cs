using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Tests.Models
{
    [TestClass]
    public class CycleTests
    {
        [TestMethod]
        public void CycleLength_NoEndDate_1()
        {
            Cycle cycle = new Cycle() { StartDate = DateTime.Today};
            Assert.AreEqual(1, cycle.Length);
        }

        [TestMethod]
        public void CycleLength_SameEndDate_1()
        {
            Cycle cycle = new Cycle() { StartDate = DateTime.Today, EndDate = DateTime.Today };
            Assert.AreEqual(1, cycle.Length);
        }
        [TestMethod]
        public void CycleLength_NextEndDate_2()
        {
            Cycle cycle = new Cycle() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1) };
            Assert.AreEqual(2, cycle.Length);
        }

        [TestMethod]
        public void CycleLength_WeekEndDate_7()
        {
            Cycle cycle = new Cycle() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(6) };
            Assert.AreEqual(7, cycle.Length);
        }
    }
}
