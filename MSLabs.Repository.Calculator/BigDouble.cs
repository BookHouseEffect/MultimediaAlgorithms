using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using MSLabs.Repository.Calculator.Entities;

namespace MSLabs.Repository.Calculator
{
    public class BigDouble : IComparable
    {
        public static readonly BigDouble ZERO = new BigDouble(0, 1);
        public static readonly BigDouble ONE = new BigDouble(1, 1);

        private const char CHAR_COMMA = '.';
        private const char CHAR_OPEN_BRACKET = '(';
        private const char CHAR_CLOSE_BRACKET = ')';
        private const char CHAR_ZERO = '0';
        private static readonly BigInteger BIG_INTEGER_TEN = new BigInteger(10);

        public BigDouble()
        {
            this.Numerator = new BigInteger(0);
            this.Denumerator = new BigInteger(1);
        }

        public BigDouble(BigInteger numerator, BigInteger denumerator)
        {
            if (denumerator.IsZero)
            {
                throw new DivideByZeroException();
            }

            this.Numerator = numerator;
            this.Denumerator = denumerator;
            this.Simplify();
        }

        public BigDouble(string value)
        {
            if (!Regex.IsMatch(value, "^[0-9]+\\.[0-9]+$"))
            {
                throw new InvalidCastException();
            }

            var x = value.Split(CHAR_COMMA);
            if (x.Length != 2)
            {
                throw new InvalidCastException();
            }

            if (x[1].Length == x[1].Count(c => c == '0'))
            {
                throw new DivideByZeroException();
            }

            string y = x[0] + x[1];
            this.Numerator = BigInteger.Parse(y);
            this.Denumerator = BigInteger.Pow(BIG_INTEGER_TEN, x[1].Length);
        }

        public BigInteger Numerator { get; private set; }

        public BigInteger Denumerator { get; private set; }

        public static BigDouble operator +(BigDouble left, BigDouble right)
        {
            var result = new BigDouble
            {
                Numerator = (left.Numerator * right.Denumerator) + (right.Numerator * left.Denumerator),
                Denumerator = left.Denumerator * right.Denumerator
            };
            result.Simplify();
            return result;
        }

        public static BigDouble operator -(BigDouble left, BigDouble right)
        {
            var result = new BigDouble
            {
                Numerator = (left.Numerator * right.Denumerator) - (right.Numerator * left.Denumerator),
                Denumerator = left.Denumerator * right.Denumerator
            };
            result.Simplify();
            return result;
        }

        public static BigDouble operator *(BigDouble left, BigDouble right)
        {
            var result = new BigDouble
            {
                Numerator = left.Numerator * right.Numerator,
                Denumerator = left.Denumerator * right.Denumerator
            };
            result.Simplify();
            return result;
        }

        public static BigDouble operator /(BigDouble left, BigDouble right)
        {
            var result = new BigDouble
            {
                Numerator = left.Numerator * right.Denumerator,
                Denumerator = left.Denumerator * right.Numerator
            };
            result.Simplify();
            return result;
        }

        public static bool operator <(BigDouble left, BigDouble right)
        {
            var a = left.Numerator * right.Denumerator;
            var b = right.Numerator * left.Denumerator;
            return a < b;
        }

        public static bool operator >(BigDouble left, BigDouble right)
        {
            var a = left.Numerator * right.Denumerator;
            var b = right.Numerator * left.Denumerator;
            return a > b;
        }

        public static bool operator ==(BigDouble left, BigDouble right)
        {
            var a = left.Numerator * right.Denumerator;
            var b = right.Numerator * left.Denumerator;
            return a == b;
        }

        public static bool operator !=(BigDouble left, BigDouble right)
        {
            var a = left.Numerator * right.Denumerator;
            var b = right.Numerator * left.Denumerator;
            return a != b;
        }

        public override bool Equals(object obj)
        {
            return this == (BigDouble)obj;
        }

        public override string ToString()
        {
            int count = 0, start = -1, length = 0;
            List<Pair<BigInteger, ushort>> divisionHistory = new List<Pair<BigInteger, ushort>>();
            BigInteger wholePart = BigInteger.DivRem(this.Numerator, this.Denumerator, out BigInteger remainder);
            BigInteger decimalPart = BigInteger.Zero;
            BigInteger temp;
            ushort max = 0;
            bool added, done = false;
            bool outOfBound = false;

            while (!remainder.IsZero)
            {
                added = false;
                for (int i = 0; i < divisionHistory.Count; i++)
                {
                    if (divisionHistory[i].Key == remainder)
                    {
                        divisionHistory[i].Value++;
                        if (divisionHistory[i].Value > max)
                        {
                            max = divisionHistory[i].Value;
                        }

                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    divisionHistory.Add(new Pair<BigInteger, ushort>(remainder, 1));
                    if (max < 1)
                    {
                        max = 1;
                    }
                }

                done = false;
                if (max != BigInteger.One && max == divisionHistory[divisionHistory.Count - 1].Value)
                {
                    for (int i = 0; i < divisionHistory.Count; i++)
                    {
                        if (divisionHistory[i].Value != BigInteger.One)
                        {
                            start = i;
                            length = divisionHistory.Count - i;
                            done = true;
                            break;
                        }
                    }
                }

                if (done)
                {
                    break;
                }

                temp = remainder * BIG_INTEGER_TEN;
                decimalPart *= BIG_INTEGER_TEN;
                decimalPart += BigInteger.DivRem(temp, this.Denumerator, out remainder);
                count++;

                if (divisionHistory.LongCount() >= 16383L)
                {
                    outOfBound = true;
                    break;
                }
            }

            var resultWhole = wholePart.ToString();
            var tmpStr = decimalPart.ToString().PadLeft(count, CHAR_ZERO);
            var resultDecimal = (done ? tmpStr.Substring(0, start) + CHAR_OPEN_BRACKET +
                tmpStr.Substring(start, length) + CHAR_CLOSE_BRACKET : tmpStr) +
                (outOfBound ? "..." : string.Empty);
            return resultWhole + CHAR_COMMA + resultDecimal;
        }

        public override int GetHashCode()
        {
            var hashCode = -110269551;
            hashCode = (hashCode * -1521134295) + EqualityComparer<BigInteger>.Default.GetHashCode(this.Numerator);
            hashCode = (hashCode * -1521134295) + EqualityComparer<BigInteger>.Default.GetHashCode(this.Denumerator);
            return hashCode;
        }

        public int CompareTo(object obj)
        {
            var other = obj as BigDouble;
            if (this > other)
            {
                return 1;
            }
            else if (this < other)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private void Simplify()
        {
            var divisor = BigInteger.GreatestCommonDivisor(this.Numerator, this.Denumerator);
            this.Numerator /= divisor;
            this.Denumerator /= divisor;
        }
    }
}
