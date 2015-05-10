using System;
using System.Linq;

namespace Kindruk.lab4
{
    public static partial class Analyzer
    {
        public static double Pirson(double[] variationalSeries, Func<double, double> distributionFunc, int intervalCount)
        {
            var intervals = lab3.Analyzer.SplitSampleEqualProbability(variationalSeries, intervalCount);
            var res =
                intervals.Sum(
                    interval =>
                        (interval.Probability - (distributionFunc(interval.Right) - distributionFunc(interval.Left)))*
                        (interval.Probability - (distributionFunc(interval.Right) - distributionFunc(interval.Left)))/
                        (distributionFunc(interval.Right) - distributionFunc(interval.Left)));
            res *= variationalSeries.Length;
            return res;
        }

        public static double Kolmogorov(double[] variationalSeries, Func<double, double> distributionFunc)
        {
            double n = variationalSeries.Length;
            var res = Math.Sqrt(n)*
                      Math.Max(
                          variationalSeries.Select((d, index) => Math.Abs((index + 1)/n - distributionFunc(d))).Max(),
                          variationalSeries.Select((d, index) => Math.Abs(distributionFunc(d) - index/n)).Max());
            return res;
        }

        public static double Mizes(double[] variationalSeries, Func<double, double> distributionFunc)
        {
            double n = variationalSeries.Length;
            var res = 1.0/12/n;
            res +=
                variationalSeries.Select(
                    (d, index) => (distributionFunc(d) - (index - 0.5)/n)*(distributionFunc(d) - (index - 0.5)/n)).Sum();
            return res;
        }
    }
}
