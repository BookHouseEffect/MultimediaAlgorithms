using System.Collections.Generic;
using System.Linq;
using MSLabs.Repository.Calculator;
using MSLabs.Repository.Calculator.Entities;

namespace MSLabs.Service.Models
{
    public class ArithmeticCodingDataModel
    {
        private string data;
        private List<Pair<char, BigDouble>> probabilities;

        public ArithmeticCodingDataModel()
        {
            this.Probabilities = new List<Pair<char, BigDouble>>();
        }

        public string DataToCode
        {
            get
            {
                return this.data;
            }

            set
            {
                this.data = value;
            }
        }

        public List<Pair<char, BigDouble>> Probabilities
        {
            get
            {
                return this.probabilities;
            }

            set
            {
                this.probabilities = value != null ?
                    value.Select(x => new Pair<char, BigDouble>(x.Key, x.Value)).ToList() :
                    new List<Pair<char, BigDouble>>();
            }
        }

        public bool PerformDataValidation()
        {
            var charSet = this.DataToCode.Distinct().OrderBy(x => x).ToArray();
            var propCharSet = this.Probabilities.Select(x => x.Key).Distinct().OrderBy(x => x).ToArray();

            if (charSet.Length != propCharSet.Length || !charSet.SequenceEqual(propCharSet))
            {
                return false;
            }

            BigDouble sum = BigDouble.ZERO;
            foreach (var prob in this.Probabilities)
            {
                if (prob.Value < BigDouble.ZERO || prob.Value > BigDouble.ONE)
                {
                    return false;
                }

                sum += prob.Value;
            }

            return sum == BigDouble.ONE;
        }
    }
}
