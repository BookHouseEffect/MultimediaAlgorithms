using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSLabs.Repository.Calculator.Entities;

namespace MSLabs.Service.Models
{
    public class LZWCompressionResultModel
    {
        private readonly List<int> compressedData;
        private readonly List<Pair<byte[], int>> codeIndexDictionary;

        public LZWCompressionResultModel(byte[] originalData, List<int> compressedData, List<Pair<byte[], int>> dictionary)
        {
            this.compressedData = compressedData;
            this.codeIndexDictionary = dictionary;
            this.PlainTextBitsCount = originalData.LongLength * 8L;
            this.CompressedTextBitsCount = compressedData.LongCount() * this.GetBitCount(compressedData.Max());
            this.CompressionDegree = (compressedData.Count() * (double)this.GetBitCount(compressedData.Max())) / (originalData.Count() * 8.0);
        }

        public long PlainTextBitsCount { get; set; }

        public long CompressedTextBitsCount { get; set; }

        public double CompressionDegree { get; private set; }

        public string GetValuesOfCompressedData()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var x in this.compressedData)
            {
                builder.Append(x).Append(" ");
            }

            return builder.ToString();
        }

        public string GetHexValuesOfCompressedData()
        {
            var max = this.compressedData.Max();
            var bitCount = (int)Math.Ceiling((double)this.GetBitCount(max) / 4.0);

            StringBuilder builder = new StringBuilder();
            foreach (var x in this.compressedData)
            {
                builder.Append(x.ToString("x" + bitCount)).Append(" ");
            }

            return builder.ToString();
        }

        public string GetBinaryValuesOfCompressedData()
        {
            var max = this.compressedData.Max();
            var bitCount = this.GetBitCount(max);

            StringBuilder builder = new StringBuilder();
            foreach (var x in this.compressedData)
            {
                builder.Append(Convert.ToString(x, 2).PadLeft(bitCount, '0')).Append(" ");
            }

            return builder.ToString();
        }

        public List<Pair<string, int>> GetCodeIndexDictionary()
        {
            var result = new List<Pair<string, int>>();
            foreach (var x in this.codeIndexDictionary)
            {
                var item = new Pair<string, int>(Encoding.Default.GetString(x.Key), x.Value);
                result.Add(item);
            }

            return result;
        }

        private int GetBitCount(int value)
        {
            int i = 1;
            while ((int)Math.Pow(2, i) < value)
            {
                i++;
            }

            return i;
        }
    }
}
