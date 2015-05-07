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
            var cnt = 0;
            for (var i = 1; i < size; i++)
            {
                if (Math.Abs(variationalSeries[i] - variationalSeries[i - 1]) > Epsilon)
                {
                    res.Add(new Tuple<double, double>(variationalSeries[i - 1], ((double) i - 1 - cnt)/size));
                    cnt = 0;
                }
                else
                {
                    cnt++;
                }
            }
            res.Add(new Tuple<double, double>(variationalSeries[size - 1], ((double) (size - 1 - cnt)/size)));
            return res.ToArray();
        }
    }
}
