using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSLabs.Repository.Calculator;
using MSLabs.Repository.Calculator.Entities;
using MSLabs.Service.Models;
using MSLabs.Service.Services;

namespace MSLabs.UnitTest.Services
{
    [TestClass]
    public class ArithmeticCodingServiceTest
    {
        [TestMethod]
        public void BasicTest()
        {
            var model = new ArithmeticCodingDataModel
            {
                DataToCode = "Multi",
                Probabilities = new List<Pair<char, BigDouble>>
                {
                    new Pair<char, BigDouble>('M', new BigDouble(1, 10)),
                    new Pair<char, BigDouble>('u', new BigDouble(3, 10)),
                    new Pair<char, BigDouble>('l', new BigDouble(3, 10)),
                    new Pair<char, BigDouble>('t', new BigDouble(2, 10)),
                    new Pair<char, BigDouble>('i', new BigDouble(1, 10))
                }
            };

            var result = ArithmeticCodingService.PerformArithmeticCoding(model);

            Assert.AreEqual("0.81602", result.LowerBound.ToString());
            Assert.AreEqual("0.8162", result.UpperBound.ToString());
        }
    }
}
