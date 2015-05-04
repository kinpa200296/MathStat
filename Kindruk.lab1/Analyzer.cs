using System;
using System.Collections.Generic;

namespace Kindruk.lab1
{
    public static partial class Analyzer
    {
        public const double Epsilon = 1e-9;

        public static double[] GetVariationalSeries(double[] sample)
        {
            var res = new double[sample.Length];
            sample.CopyTo(res, 0);
            Array.Sort(res);
            return res;
        }

        public static Tuple<double, double>[] GetEmpiricalFuncData(double[] variationalSeries)
        {
            var res = new List<Tuple<double, double>>();
            var size = variationalSeries.Length;
            for (var i = 1; i < size; i++)
            {
                if (Math.Abs(variationalSeries[i] - variationalSeries[i - 1]) > Epsilon)
                {
                    res.Add(new Tuple<double, double>(variationalSeries[i - 1], ((double) i - 1)/size));
                }
            }
            res.Add(new Tuple<double, double>(variationalSeries[size - 1], ((double) (size - 1)/size)));
            return res.ToArray();
        }
    }
}
