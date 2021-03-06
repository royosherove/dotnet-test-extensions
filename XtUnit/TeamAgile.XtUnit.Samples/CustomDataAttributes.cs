using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TeamAgile.ApplicationBlocks.Interception;
using TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions;

namespace XtUnit.Framework
{
    [TestFixture]
    public class CustomDataAttributes:ExtensibleFixture
    {
        [Test]
        [Data("sql","Select * from categories")]
        public void useCustomDataAttributes()
        {
            string actual = DataAttribute.items["sql"];
            string expected = "Select * from categories";
            Assert.AreEqual(expected,actual);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    class DataAttribute:BaseProcessingAttribute
    {
        private readonly string data = string.Empty;
        private readonly string name = string.Empty;


        public string Name
        {
            get { return name; }
        }

        public DataAttribute(string name, string data)
        {
            this.data = data;
            this.name = name;
        }

        public string Data
        {
            get { return data; }
        }

        public static Dictionary<string, string> Items
        {
            get { return items; }
        }

        public static readonly Dictionary<string, string> items = new Dictionary<string, string>();

        protected override void OnPreProcess(object sender, PreProcessEventArgs args)
        {
            items[name] = data;
        }

        protected override void OnPostProcess(object sender, PostProcessEventArgs args)
        {
        }
    }
}
