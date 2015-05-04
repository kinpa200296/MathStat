using System;
using System.Collections.Generic;
using System.Linq;
using MathCommon;

namespace Kindruk.lab3
{
    public static partial class Analyzer
    {
        public static DataInterval[] SplitSampleEqualProbability(double[] sample, int intervalCount)
        {
            if (sample.Length%intervalCount != 0)
            {
                throw new ArgumentException("Sample size should be divisible by interval count");
            }
            var variationalSeries = lab1.Analyzer.GetVariationalSeries(sample);
            var size = variationalSeries.Length;
            var intervalSize = size/intervalCount;
            var intervals = new List<DataInterval>();
            for (var i = 0; i < intervalCount; i++)
            {
                var interval = new DataInterval
                {
                    InitialSampleSize = size,
                    Left = variationalSeries[i*intervalSize],
                    Right = variationalSeries[(i + 1)*intervalSize - ((i + 1)*intervalSize < size ? 0 : 1) ],
                    SampleData =
                        variationalSeries.Where(
                            (x, index) => index >= i*intervalSize && index < (i + 1)*intervalSize)
                            .ToArray()
                };
                intervals.Add(interval);
            }
            return intervals.ToArray();
        }
    }
}
