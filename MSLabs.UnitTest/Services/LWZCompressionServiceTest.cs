using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSLabs.Repository.Calculator.Entities;
using MSLabs.Service.Models;
using MSLabs.Service.Services;

namespace MSLabs.UnitTest.Services
{
    [TestClass]
    public class LWZCompressionServiceTest
    {
        [TestMethod]
        public void BasicTest()
        {
            string textToCompress = "thisisthe";

            LZWCompressionResultModel actual = LZWCompressionService.PerformLZWCompression(
                new LZWCompressionDataModel { DataToCompress = textToCompress });

            var expectedDictionary = new List<Pair<byte[], int>>
            {
                new Pair<byte[], int>(new byte[] { (byte)'e' }, 'e'),
                new Pair<byte[], int>(new byte[] { (byte)'h' }, 'h'),
                new Pair<byte[], int>(new byte[] { (byte)'i' }, 'i'),
                new Pair<byte[], int>(new byte[] { (byte)'s' }, 's'),
                new Pair<byte[], int>(new byte[] { (byte)'t' }, 't'),
                new Pair<byte[], int>(new byte[] { (byte)'t', (byte)'h' }, 256),
                new Pair<byte[], int>(new byte[] { (byte)'h', (byte)'i' }, 257),
                new Pair<byte[], int>(new byte[] { (byte)'i', (byte)'s' }, 258),
                new Pair<byte[], int>(new byte[] { (byte)'s', (byte)'i' }, 259),
                new Pair<byte[], int>(new byte[] { (byte)'i', (byte)'s', (byte)'t' }, 260),
                new Pair<byte[], int>(new byte[] { (byte)'t', (byte)'h', (byte)'e' }, 261),
            };

            LZWCompressionResultModel expected = new LZWCompressionResultModel(
                Encoding.Default.GetBytes(textToCompress),
                new List<int> { 116, 104, 105, 115, 258, 256, 101 },
                expectedDictionary);

            Assert.AreEqual(expected.GetValuesOfCompressedData(), actual.GetValuesOfCompressedData());
            Assert.AreEqual(expected.GetHexValuesOfCompressedData(), actual.GetHexValuesOfCompressedData());
            Assert.AreEqual(expected.GetBinaryValuesOfCompressedData(), actual.GetBinaryValuesOfCompressedData());
            Assert.AreEqual(expected.CompressionDegree, actual.CompressionDegree);

            var x = expected.GetCodeIndexDictionary();
            var y = actual.GetCodeIndexDictionary();

            Assert.AreEqual(x.Count(), y.Count);

            for (int i = 0; i < x.Count() && i < y.Count; i++)
            {
                Assert.AreEqual(x[i].Key, y[i].Key);
                Assert.AreEqual(x[i].Value, y[i].Value);
            }
        }
    }
}
