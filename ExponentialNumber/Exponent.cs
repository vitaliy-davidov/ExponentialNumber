using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExponentialNumber
{
    public class Exponent
    {
        private readonly static Regex _exponentRegex = new Regex(@"^([+-]?(0?|([1-9]\d*)))\.?\d*[eE]?(0?|([+-]?[1-9]\d*))$", RegexOptions.Compiled);
        private readonly static Regex _numberRegex = new Regex(@"^([+-]?(0?|([1-9]\d*)))\.?\d*$", RegexOptions.Compiled);

        private byte[] _mantissa;
        private int _power;
        private char _sign;
        private int _dotIdx;

        private Exponent(char? sign, int power, IEnumerable<byte> mantissa, int dotIdx)
        {
            if (dotIdx > mantissa.Count())
                throw new ArgumentException("Dot index out of range");
            _dotIdx = dotIdx;
            _power = power;
            _mantissa = mantissa.ToArray();
            if (sign == null)
                sign = '+';
            _sign = sign.Value;
        }
        private Exponent(char? sign, int power, byte[] mantissa) : this(sign, power, mantissa, 1) { }
        public Exponent() : this('+', 0, new byte[] { 0 }) { }

        public Exponent(Exponent other) :this(other._sign, other._power, other._mantissa, other._dotIdx) { }

        private static void RemoveZeros(Exponent exponent)
        {
            int newMantisaSize = exponent._mantissa.Length;
            while(exponent._mantissa[newMantisaSize - 1] == 0)
            {
                --newMantisaSize;
            }
            if (newMantisaSize < exponent._mantissa.Length)
            {
                byte[] newMantissa = new byte[newMantisaSize];
                for (int i = 0; i < newMantissa.Length; i++)
                {
                    newMantissa[i] = exponent._mantissa[i];
                }
                exponent._mantissa = newMantissa;
            }
        }

        public static Exponent Parse(string exponent)
        {
            if (_exponentRegex.IsMatch(exponent))
            {
                if (_numberRegex.IsMatch(exponent))
                {
                    return ParseFromNumber(exponent);
                }

                return ParseFromExponent(exponent);
            }

            throw new ArgumentException("Exception11!!");
        }

        private static Exponent ParseFromNumber(string number)
        {
            byte[] mantissa;
            int power = 0;
            char sign;
            if (_numberRegex.IsMatch(number))
            {
                sign = number.FirstOrDefault(x => x == '+' || x == '-');
                power += FindPowerFromMantissa(number);
                mantissa = ConvertFromString(number);

                return new Exponent(sign, power, mantissa, 1);
            }
            throw new ArgumentException("Exception1!!1");
        }

        private static Exponent ParseFromExponent(string exponent)
        {
            byte[] mantissa;
            int power;
            char sign;
            if (_exponentRegex.IsMatch(exponent))
            {
                string[] parts = exponent.Split(new char[] { 'e', 'E' });
                int.TryParse(parts[1], out power);
                sign = parts[0].FirstOrDefault(x => x == '+' || x == '-');

                power += FindPowerFromMantissa(parts[0]);
                mantissa = ConvertFromString(parts[0]);

                return new Exponent(sign, power, mantissa, 1);
            }
            throw new ArgumentException("Exception!!11");
        }

        private static bool IsNormallized(string mantissa)
        {
            int idx = mantissa.IndexOf('.');

            return !( idx > 1 || idx == -1 || mantissa[0] == '0');
        }

        private static int FindPowerFromMantissa(string mantissa)
        {
            int power = 0;

            if(!IsNormallized(mantissa))
            {
                if (mantissa.Contains('.'))
                    power = mantissa.IndexOf('.') - 1;
                else power = mantissa.Length - 1;
            }

            return power;
        }

        private static byte[] ConvertFromString(string mantissa)
        {
            byte[] _mantissa = new byte[mantissa.Count(x => x != '.')];
            int counter = 0;

            foreach (char digit in mantissa.Where(x => x != '.' && x != '+' && x != '-'))
            {
                _mantissa[counter++] = Convert.ToByte(digit.ToString());
            }

            return _mantissa;
        }

        private static Exponent Sum(Exponent first, Exponent second)
        {
            int maxPower = first._power;
            byte isFirstMax = 1;
            Exponent[] exponents = new Exponent[] { first, second };
            Exponent result;

            if(maxPower < second._power)
            {
                maxPower = second._power;
                isFirstMax = 0;
            }

            result = ChangeMantissaToPower(exponents[isFirstMax], maxPower);
            for (int i = 0; i < exponents[(isFirstMax + 1) % 2]._mantissa.Length; i++)
            {
                if (exponents[(isFirstMax + 1) % 2]._mantissa.Length > i)
                    result._mantissa[i] += exponents[(isFirstMax + 1) % 2]._mantissa[i];
            }

            NormalizeMantissa(result);
            RemoveZeros(result);

            return result;
        }

        public static Exponent operator+(Exponent first, Exponent second)
        {
            return Sum(first, second);
        }
        private static void NormalizeMantissa(Exponent exponent)
        { 

            for (int i = exponent._mantissa.Length - 1; i > 0; i--)
            {
                if(exponent._mantissa[i] > 9)
                {
                    exponent._mantissa[i] %= 10;
                    ++exponent._mantissa[i - 1];
                }
            }
            if (exponent._mantissa[0] > 9)
            {
                byte[] newMantissa = new byte[exponent._mantissa.Length + 1];
                exponent._mantissa[0] %= 10;
                exponent._mantissa.CopyTo(newMantissa, 1);
                ++newMantissa[0];
                exponent._mantissa = newMantissa;
                ++exponent._power;
            }
        }

        private static Exponent ChangeMantissaToPower(Exponent exponent, int power)
        {
            int differ = Math.Abs(power - exponent._power);
            byte[] newMantissa;
            if(exponent._power > power)
            {
                int sizeDiffer = differ - (exponent._mantissa.Length - 1);
                newMantissa = new byte[exponent._mantissa.Length + sizeDiffer];
                for (int i = 0; i < exponent._mantissa.Length; i++)
                {
                    newMantissa[i] = exponent._mantissa[i];
                }
                return new Exponent(exponent._sign, power, newMantissa);
            }
            if(exponent._power < power)
            {
                newMantissa = new byte[differ + exponent._mantissa.Length];
                for (int i = differ; i < newMantissa.Length; i++)
                {
                    newMantissa[i] = exponent._mantissa[i-differ];
                }
                return new Exponent(exponent._sign, power, newMantissa);
            }
            return new Exponent (exponent);
        }

        public override string ToString()
        {
            StringBuilder outString = new StringBuilder();
            if(_sign == '-')
            {
                outString.Append(_sign);
            }
            for (int i = 0; i < _mantissa.Length; i++)
            {
                if (i == _dotIdx) outString.Append('.');
                outString.Append(_mantissa[i]);
            }

            return $"{outString}E{_power}";
        }
    }
}
