using System;
using System.Globalization;
using System.IO;
using Kindruk.lab1;
using MathCommon;
using MathStat.Model;
using Microsoft.Research.DynamicDataDisplay;

namespace MathStat.ViewModel
{
    public class Lab1ViewModel : LabBaseViewModel
    {
        public Func<double, double> DistributionFunc = d => Math.Asin(d) * 2 / Math.PI;
        public string DistributionFuncString = "F(y) = 2/pi*asin(y)";

        #region Properties

        #endregion

        #region Constructors

        public Lab1ViewModel()
        {
            SampleSize = 40;
        }

        #endregion

        #region Commands

        #endregion

        #region Methods

        public void DoActionExecute(ChartPlotter plotter)
        {
            Log.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize));
                var data = Analyzer.GetEmpiricalFuncData(p);
                var empiricalColor = GetRandomColor();
                var funcColor = GetRandomColor();
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                Utility.PlotDistributionFunc(data, plotter, empiricalColor, "Empirical Function");
                Utility.PlotFunc(plotter, funcColor, DistributionFunc, 0, 1, 1e-2, "Theoretical Function");
                plotter.Viewport.FitToView();
                Log.Add(DistributionFuncString);
                Log.Add("y                    F_emp(y)");
                foreach (var tuple in data)
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
                            stream.WriteLine("y,F_emp(y)");
                            foreach (var tuple in data)
                            {
                                stream.WriteLine("{0},{1}",
                                    tuple.Item1.ToString("F6", CultureInfo.InvariantCulture),
                                    tuple.Item2.ToString("F6", CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
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
