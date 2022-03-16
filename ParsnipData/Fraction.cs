using System;

namespace ParsnipData
{
    public class Fraction
    {
        public int Numerator { get; private set; }
        public int Denominator { get; private set; }
        public decimal AsDecimal { get => (decimal)Numerator / Denominator; }

        public static explicit operator Fraction(string fraction) => new Fraction(fraction);

        public Fraction(string fraction)
        {
            try
            {
                var values = fraction.Split('/');
                Numerator = Convert.ToInt32(values[0]);
                Denominator = Convert.ToInt32(values[1]);
            }
            catch
            {
                throw new InvalidCastException();
            }

            if (Numerator == 0 || Denominator == 0) throw new DivideByZeroException();
        }

        public void DivideBy(int denominator) => Denominator *= denominator;

        public override string ToString() => $"{Numerator}/{Denominator}";
    }
}
