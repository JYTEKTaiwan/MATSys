using NUnit.Framework;
using MATSys.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MATSys.Plugins;

namespace MATSys.Utilities.Tests
{
    [TestFixture()]
    public class TypeParserTests
    {
        public class Test : ModuleBase
        {
            public override object Configuration { get; set; }
        }
        [Test()]
        public void SearchLoadedTypeTest()
        {
            var t=TypeParser.SearchType("MATSys.ModuleBase", "");
            Assert.That(t.Name == "ModuleBase");
        }

        [Test()]
        public void SearchExternalTypeTest()
        {
            var t = TypeParser.SearchType("MATSys.Plugins.CSVRecorder", @".\MATSys.Plugins.CSVRecorder.dll");
            Assert.That(t.Name == "CSVRecorder");
        }
    }
}