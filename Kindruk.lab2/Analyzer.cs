using System.Collections.Generic;
using System.Linq;
using MathCommon;

namespace Kindruk.lab2
{
    public static partial class Analyzer
    {
        public static DataInterval[] SplitSampleEqualIntervals(double[] sample, int intervalCount)
        {
            var variationalSeries = lab1.Analyzer.GetVariationalSeries(sample);
            var size = variationalSeries.Length;
            //var intervalCount = (int) Math.Truncate(size > 100 ? 4*Math.Log10(size) : Math.Sqrt(size));
            var intervalSize = (variationalSeries[size - 1] - variationalSeries[0])/intervalCount;
            var intervals = new List<DataInterval>();
            for (var i = 0; i < intervalCount; i++)
            {
                var interval = new DataInterval
                {
                    InitialSampleSize = size,
                    Left = i*intervalSize,
                    Right = (i + 1)*intervalSize
                };
                interval.SampleData =
                    variationalSeries.Where(x => x > interval.Left && x - interval.Right < lab1.Analyzer.Epsilon)
                        .ToArray();
                intervals.Add(interval);
            }
            return intervals.ToArray();
        }
    }
}
