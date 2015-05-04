using System;

namespace MathCommon
{
    public static class SampleBuilder
    {
        public static double[] GetSample(IRandomQuantity x, Func<double, double> phi, int size)
        {
            var res = new double[size];
            for (var i = 0; i < size; i++)
            {
                res[i] = phi(x.Get());
            }
            return res;
        }
    }
}
