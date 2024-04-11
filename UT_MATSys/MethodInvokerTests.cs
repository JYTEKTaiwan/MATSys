using NUnit.Framework;
using MATSys.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Utilities.Tests
{
    [TestFixture()]
    public class MethodInvokerTests
    {
        internal class TestClass
        {
            public static string WhoAreYou() => nameof(TestClass);
            public int GetID(int id) => id;
            public void ReturnOne(ref int num) => num = 1;
        }

        [Test()]
        [Category("Invoker")]
        public void StaticInvokerTest()
        {
            var mi = typeof(TestClass).GetMethod("WhoAreYou");
            var invoker = MethodInvoker.Create(mi);
            Assert.That((string)(invoker.Invoke()) == "TestClass");
        }
        [Test()]
        [Category("Invoker")]
        public void NonStaticInvokerTest()
        {
            var instance=new TestClass();
            var mi = instance.GetType().GetMethod("GetID");
            var invoker=MethodInvoker.Create(instance, mi);
            Assert.That((int)(invoker.Invoke(1))==1);
        }

        [Test()]
        [Category("Invoker")]
        public void MarkAsRefInvokerTest()
        {
            Assert.Catch(typeof(InvalidOperationException), () => 
            {
                var instance = new TestClass();
                var mi = instance.GetType().GetMethod("ReturnOne");
                var invoker = MethodInvoker.Create(instance, mi);
            });
        }
    }
}