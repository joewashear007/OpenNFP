using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Tests.Models
{
    [TestClass]
    public class DayRecordTests
    {

        [TestMethod]
        public void IsEmpty_NewObject()
        {
            DayRecord record = new();
            Assert.IsTrue(record.IsEmpty());
        }

        [TestMethod]
        public void IsEmpty_ModifiedObject()
        {
            DayRecord record = new()
            {
                Temperature = 98.6M
            };
            Assert.IsFalse(record.IsEmpty());
        }

        [TestMethod]
        public void IsEmpty_CheckEachProperty()
        {
            DayRecord record = new();
            foreach (var p in typeof(DayRecord).GetProperties().Where(q => q.CanWrite))
            {
                object? orgValue = p.GetValue(record);
                if (p.PropertyType == typeof(string))
                {
                    p.SetValue(record, "Hello World");
                    Assert.IsFalse(record.IsEmpty(), $"Checking {p.Name} is not empty");
                }
                if (p.PropertyType.IsEnum)
                {
                    p.SetValue(record, 1);
                    Assert.IsFalse(record.IsEmpty(), $"Checking {p.Name} is not empty");
                }
                if (p.PropertyType == typeof(decimal?))
                {
                    p.SetValue(record, 98.7M);
                    Assert.IsFalse(record.IsEmpty(), $"Checking {p.Name} is not empty");
                }
                p.SetValue(record, orgValue);
            }

            Console.WriteLine(record.ToString());
            Assert.IsTrue(record.IsEmpty(), "All Properties unset");
        }
    }
}
