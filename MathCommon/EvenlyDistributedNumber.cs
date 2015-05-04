using System;

namespace MathCommon
{
    public class EvenlyDistributedNumber : IRandomQuantity
    {
        private readonly Random _random;
        private readonly double _a, _b;

        public EvenlyDistributedNumber(double a, double b)
        {
            _random = new Random((int)DateTime.Now.Ticks);
            _a = a;
            _b = b;
        }

        public double Get()
        {
            return _a + _random.NextDouble()*(_b - _a);
        }
    }
}
