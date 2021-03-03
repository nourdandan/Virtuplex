using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Virtuplex;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        private Helper helper = new Helper();

        [TestMethod]
        public async Task Evaluate_Okay()
        {
            var line1 = "1 * 3 + 1 * 4";
            var line2 = "2 + -3 * 2";
            var line3 = "1 + 2 * 3 * 2 / 2";
            var res1 = await helper.Evaluate(line1);
            var res2 = await helper.Evaluate(line2);
            var res3 = await helper.Evaluate(line3);

            Assert.AreEqual("7", res1);
            Assert.AreEqual("-4", res2);
            Assert.AreEqual("7", res3);

        }


        
        [TestMethod]
        public async Task Evaluate_Error()
        {
            var line1 = "2.1/2";
            var line2 = "2,1/2";
            var line3 = "a/2";
            var line4 = ";/2";

            var res1= await helper.Evaluate(line1);
            var res2 = await helper.Evaluate(line2);
            var res3 = await helper.Evaluate(line3);
            var res4 = await helper.Evaluate(line4);

            Assert.AreEqual("Error - Invalid character: .", res1);
            Assert.AreEqual("Error - Invalid character: ,", res2);
            Assert.AreEqual("Error - Invalid character: a", res3);
            Assert.AreEqual("Error - Invalid character: ;", res4);

        }


    }
}
