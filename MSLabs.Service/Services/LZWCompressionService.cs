using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSLabs.Repository.Calculator.Entities;
using MSLabs.Service.Models;

namespace MSLabs.Service.Services
{
    public class LZWCompressionService
    {
        public static LZWCompressionResultModel PerformLZWCompression(LZWCompressionDataModel model)
        {
            int indexSeed = 256;
            byte[] dataBytes = Encoding.Default.GetBytes(model.DataToCompress);
            List<Pair<byte[], int>> indexDictionary = new List<Pair<byte[], int>>(
                dataBytes.Distinct().OrderBy(x => x).Select(x => new Pair<byte[], int>(new byte[] { x }, (int)x)));
            List<int> output = new List<int>();

            List<byte> current = new List<byte>();
            byte? next;

            for (int i = 0, len = dataBytes.Length; i < len; i++)
            {
                current.Add(dataBytes[i]);
                next = null;
                next = (i < len - 1) ? dataBytes[i + 1] : next;

                var combined = new List<byte>(current.ToArray());
                if (next.HasValue)
                {
                    combined.Add(next.Value);
                }

                bool isInDictionary = false;
                int currentIndex = -1;
                for (int j = 0, lenj = indexDictionary.Count; j < lenj; j++)
                {
                    if (indexDictionary[j].Key.SequenceEqual(combined))
                    {
                        isInDictionary = true;

                        if (!next.HasValue)
                        {
                            currentIndex = j;
                        }

                        break;
                    }

                    if (indexDictionary[j].Key.SequenceEqual(current))
                    {
                        currentIndex = j;
                    }
                }

                if (!isInDictionary)
                {
                    if (currentIndex != -1)
                    {
                        output.Add(indexDictionary[currentIndex].Value);
                    }

                    indexDictionary.Add(new Pair<byte[], int>(combined.ToArray(), indexSeed++));
                    current.Clear();
                }
                else
                {
                    if (!next.HasValue && currentIndex != -1)
                    {
                        output.Add(indexDictionary[currentIndex].Value);
                    }
                }
            }

            return new LZWCompressionResultModel(dataBytes, output, indexDictionary);
        }
    }
}
