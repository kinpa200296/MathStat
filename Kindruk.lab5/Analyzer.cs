using System;
using System.Collections.Generic;
using System.Linq;
using MathCommon;

namespace Kindruk.lab5
{
    public static partial class Analyzer
    {
        public static double CalcExceptedValueBySample(double[] data)
        {
            var result = data.Sum(x => x)/data.Length;
            return result;
        }

        public static double CalcVarianceValueBySample(double[] data, double expectedValue)
        {
            var result = data.Sum(x => (x - expectedValue)*(x - expectedValue))/(data.Length - 1);
            return result;
        }

        public static ConfidencialInterval[] FindConfidencialIntervalsForExpectedValueUseNormal(double[] data,
            double variance, double left, double right, double step)
        {
            var result = new List<ConfidencialInterval>();
            var expectedValue = CalcExceptedValueBySample(data);
            for (var gamma = left; gamma < right + lab1.Analyzer.Epsilon; gamma += step)
            {
                var alpha = 1 - gamma;
                result.Add(new ConfidencialInterval
                {
                    ConfidencialProbability = gamma,
                    Left =
                        expectedValue -
                        Math.Sqrt(variance)*ArgumentTables.Normal[1 - alpha/2.0]/Math.Sqrt(data.Length),
                    Right =
                        expectedValue +
                        Math.Sqrt(variance)*ArgumentTables.Normal[1 - alpha/2.0]/Math.Sqrt(data.Length)
                });
            }
            return result.ToArray();
        }

        public static ConfidencialInterval[] FindConfidencialIntervalsForExpectedValueUseStudent(double[] data,
            double variance, double left, double right, double step)
        {
            var result = new List<ConfidencialInterval>();
            var expectedValue = CalcExceptedValueBySample(data);
            for (var gamma = left; gamma < right + lab1.Analyzer.Epsilon; gamma += step)
            {
                result.Add(new ConfidencialInterval
                {
                    ConfidencialProbability = gamma,
                    Left =
                        expectedValue -
                        Math.Sqrt(variance)*ArgumentTables.Student[gamma, data.Length - 1]/Math.Sqrt(data.Length),
                    Right =
                        expectedValue +
                        Math.Sqrt(variance)*ArgumentTables.Student[gamma, data.Length - 1]/Math.Sqrt(data.Length)
                });
            }
            return result.ToArray();
        }

        public static ConfidencialInterval[] FindConfidencialIntervalsForVarianceUseNormal(double[] data,
            double expectedValue, double left, double right, double step, bool isExpectedValueTheoretical)
        {
            var result = new List<ConfidencialInterval>();
            var variance = CalcVarianceValueBySample(data, expectedValue);
            for (var gamma = left; gamma < right + lab1.Analyzer.Epsilon; gamma += step)
            {
                result.Add(new ConfidencialInterval
                {
                    ConfidencialProbability = gamma,
                    Left = variance - variance*ArgumentTables.Normal[gamma]*Math.Sqrt(2.0/(data.Length - 1.0)),
                    Right = variance + variance*ArgumentTables.Normal[gamma]*Math.Sqrt(2.0/(data.Length - 1.0))
                });
            }
            return result.ToArray();
        }

        public static ConfidencialInterval[] FindConfidencialIntervalsForVarianceUseXi2(double[] data,
            double expectedValue, double left, double right, double step, bool isExpectedValueTheoretical)
        {
            var result = new List<ConfidencialInterval>();
            var variance = CalcVarianceValueBySample(data, expectedValue);
            for (var gamma = left; gamma < right + lab1.Analyzer.Epsilon; gamma += step)
            {
                var alpha = 1 - gamma;
                var n = data.Length - (isExpectedValueTheoretical ? 0 : 1);
                result.Add(new ConfidencialInterval
                {
                    ConfidencialProbability = gamma,
                    Left = n*variance/ArgumentTables.Xi2[alpha/2.0, n],
                    Right = n*variance/ArgumentTables.Xi2[1.0 - alpha/2.0, n]
                });
            }
            return result.ToArray();
        }
    }
}
