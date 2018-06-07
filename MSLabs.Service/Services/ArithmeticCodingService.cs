using System;
using System.Linq;
using MSLabs.Repository.Calculator;
using MSLabs.Service.Models;

namespace MSLabs.Service.Services
{
    public class ArithmeticCodingService
    {
        public static ArithmeticCodingRangeModel PerformArithmeticCoding(ArithmeticCodingDataModel model)
        {
            if (!model.PerformDataValidation())
            {
                throw new FormatException();
            }

            var probs = model.Probabilities.OrderByDescending(x => x.Value).ToArray();
            for (int i = 1, len = probs.Length; i < len; i++)
            {
                probs[i].Value += probs[i - 1].Value;
            }

            BigDouble lowerBound = BigDouble.ZERO;
            BigDouble upperBound = BigDouble.ONE;

            for (int i = 0; i < model.DataToCode.Length; i++)
            {
                var data = model.DataToCode[i];
                for (int j = 0; j < probs.Length; j++)
                {
                    if (probs[j].Key == data)
                    {
                        var range = upperBound - lowerBound;
                        if (j == 0)
                        {
                            upperBound = lowerBound + (range * probs[j].Value);
                        }
                        else if (j == probs.Length - 1)
                        {
                            lowerBound = lowerBound + (range * probs[j - 1].Value);
                        }
                        else
                        {
                            upperBound = lowerBound + (range * probs[j].Value);
                            lowerBound = lowerBound + (range * probs[j - 1].Value);
                        }

                        break;
                    }
                }
            }

            return new ArithmeticCodingRangeModel(lowerBound, upperBound);
        }
    }
}
