using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Kindruk.lab1;
using MathCommon;
using MathStat.Model;
using Microsoft.Research.DynamicDataDisplay;

namespace MathStat.ViewModel
{
    public class Lab3ViewModel : LabBaseViewModel
    {
        public Func<double, double> DistributionFunc = d => Math.Asin(d) * 2 / Math.PI;
        public string DistributionFuncString = "F(y) = 2/pi*asin(y)";
        public Func<double, double> DistributionDensityFunc = d => 2 / Math.PI / Math.Sqrt(1 - d * d);
        public string DistributionDensityFuncString = "f(y) = 2/pi/(1-y^2)^(1/2)";

        #region Properties

        #endregion

        #region Constructors

        public Lab3ViewModel()
        {
            SampleSize = 100;
            IntervalCount = 10;
        }

        #endregion

        #region Commands

        #endregion

        #region Methods

        public void DoActionExecute(ChartPlotter plotter, ChartPlotter plotterBarChart)
        {
            Log.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize));
                var data = Kindruk.lab3.Analyzer.SplitSampleEqualProbability(p, IntervalCount);
                var distributionFuncList = new List<Tuple<double, double>>();
                var distributionPolygonPoints = new List<Tuple<double, double>>();
                var sum = 0.0;
                distributionPolygonPoints.Add(new Tuple<double, double>(0.0, 0.0));
                foreach (var interval in data)
                {
                    distributionFuncList.Add(new Tuple<double, double>(interval.Left, sum));
                    distributionPolygonPoints.Add(new Tuple<double, double>(interval.Right, interval.ProbabilityDensity));
                    sum += interval.Probability;
                }
                Log.Add(DistributionDensityFuncString);
                foreach (var interval in data)
                {
                    Log.Add(string.Format("[{0}; {1}]: f* = {2}",
                        interval.Left.ToString("F6", CultureInfo.InvariantCulture),
                        interval.Right.ToString("F6", CultureInfo.InvariantCulture),
                        interval.ProbabilityDensity.ToString("F6", CultureInfo.InvariantCulture)));
                }
                Log.Add("y                    F_emp(y)");
                foreach (var tuple in distributionFuncList)
                {
                    Log.Add(string.Format("{0}       {1}", tuple.Item1.ToString("F6", CultureInfo.InvariantCulture),
                        tuple.Item2.ToString("F6", CultureInfo.InvariantCulture)));
                }
                if (CheckBoxChecked)
                {
                    using (var file = File.Create(OutputFile))
                    {
                        using (var stream = new StreamWriter(file))
                        {
                            stream.WriteLine("left border,right border,probability density");
                            foreach (var interval in data)
                            {
                                stream.WriteLine("{0},{1},{2}",
                                    interval.Left.ToString("F6", CultureInfo.InvariantCulture),
                                    interval.Right.ToString("F6", CultureInfo.InvariantCulture),
                                    interval.ProbabilityDensity.ToString("F6", CultureInfo.InvariantCulture));
                            }
                            stream.WriteLine("y,F_emp(y)");
                            foreach (var tuple in distributionFuncList)
                            {
                                stream.WriteLine("{0},{1}",
                                    tuple.Item1.ToString("F6", CultureInfo.InvariantCulture),
                                    tuple.Item2.ToString("F6", CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
                var barChartColor = GetRandomColor();
                var polygonColor = GetRandomColor();
                var distributionFuncColor = GetRandomColor();
                var funcColor = GetRandomColor();
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                plotterBarChart.Children.RemoveAll(typeof(LineGraph));
                plotterBarChart.Children.RemoveAll(typeof(MarkerPointsGraph));
                Utility.PlotBarChart(plotterBarChart, barChartColor, data, "Bar Chart");
                Utility.PlotDistributionFunc(distributionFuncList.ToArray(), plotter, distributionFuncColor,
                    "Distribution Fuction");
                Utility.PlotFunc(plotter, funcColor, DistributionFunc, 0, 1, 1e-2, "Theoretical Function");
                Utility.PlotFunc(plotterBarChart, funcColor, DistributionDensityFunc, 0, 0.999, 1e-2,
                    "Theoretical Function");
                distributionFuncList.Add(new Tuple<double, double>(1, DistributionFunc(1)));
                Utility.PlotPolyLine(distributionPolygonPoints.ToArray(), plotterBarChart, polygonColor,
                    "Distribution Polygon");
                plotter.FitToView();
                plotterBarChart.FitToView();
            }
            catch (Exception e)
            {
                Log.Add(e.Message);
                throw;
            }
        }

        #endregion
    }
}
