using MSLabs.Repository.Calculator;

namespace MSLabs.Service.Models
{
    public class ArithmeticCodingRangeModel
    {
        public ArithmeticCodingRangeModel()
        {
            this.LowerBound = BigDouble.ZERO;
            this.UpperBound = BigDouble.ONE;
        }

        public ArithmeticCodingRangeModel(BigDouble lower, BigDouble upper)
        {
            this.LowerBound = lower;
            this.UpperBound = upper;
        }

        public BigDouble LowerBound { get; set; }

        public BigDouble UpperBound { get; set; }
    }
}
