using System;
using System.Globalization;
using System.Windows.Input;
using Kindruk.lab1;
using MathCommon;
using MathStat.Model;
using Microsoft.Research.DynamicDataDisplay;

namespace MathStat.ViewModel
{
    public class Lab4ViewModel : LabBaseViewModel
    {
        public Func<double, double> DistributionFunc = d => Math.Asin(d) * 2 / Math.PI;
        public string DistributionFuncString = "F(y) = 2/pi*asin(y)";
        public Func<double, double> DistributionDensityFunc = d => 2 / Math.PI / Math.Sqrt(1 - d * d);
        public string DistributionDensityFuncString = "f(y) = 2/pi/(1-y^2)^(1/2)";

        private double _tableValue;

        public enum Method
        {
            Pirson, Kolmorogov, Mizes
        }

        #region Properties

        public Method ChoosenMethod { get; set; }

        public double TableValue
        {
            get { return _tableValue; }
            set
            {
                _tableValue = value;
                OnPropertyChanged("TableValue");
            }
        }

        #endregion

        #region Constructors

        public Lab4ViewModel()
        {
            PirsonChoosenExecute();
        }

        #endregion

        #region Commands

        public ICommand PirsonChoosen
        {
            get { return new RelayCommand(PirsonChoosenExecute); }
        }

        public ICommand KolmogorovChoosen
        {
            get { return new RelayCommand(KolmogorovChoosenExecute); }
        }

        public ICommand MizesChoosen
        {
            get { return new RelayCommand(MizesChoosenExecute); }
        }

        #endregion

        #region Methods

        public void PirsonChoosenExecute()
        {
            ChoosenMethod = Method.Pirson;
            SampleSize = 200;
            IntervalCount = 10;
            TableValue = 14.07;
        }

        public void KolmogorovChoosenExecute()
        {
            ChoosenMethod = Method.Kolmorogov;
            SampleSize = 30;
            IntervalCount = 0;
            TableValue = 1.36;
        }

        public void MizesChoosenExecute()
        {
            ChoosenMethod = Method.Mizes;
            SampleSize = 50;
            IntervalCount = 0;
            TableValue = 0.347;
        }

        public void DoActionExecute(ChartPlotter plotter)
        {
            Log.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize));
                var res = 0.0;
                var distributionFunc = DistributionFunc;
                var distributionDensityFunc = DistributionDensityFunc;
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                switch (ChoosenMethod)
                {
                    case Method.Pirson:
                        res = Kindruk.lab4.Analyzer.Pirson(p, distributionFunc, IntervalCount);
                        var intervals = Kindruk.lab3.Analyzer.SplitSampleEqualProbability(p, IntervalCount);
                        var barChartColor = GetRandomColor();
                        var densityFuncColor = GetRandomColor();
                        Utility.PlotBarChart(plotter, barChartColor, intervals, "Bar Chart");
                        Utility.PlotFunc(plotter, densityFuncColor, distributionDensityFunc, 0, 0.999, 1e-2,
                            "Theoretical Function");
                        break;
                    case Method.Kolmorogov:
                        res = Kindruk.lab4.Analyzer.Kolmogorov(p, distributionFunc);
                        break;
                    case Method.Mizes:
                        res = Kindruk.lab4.Analyzer.Mizes(p, distributionFunc);
                        break;
                }
                if (ChoosenMethod != Method.Pirson)
                {
                    var data = Analyzer.GetEmpiricalFuncData(p);
                    var empiricalColor = GetRandomColor();
                    var funcColor = GetRandomColor();
                    Utility.PlotDistributionFunc(data, plotter, empiricalColor, "Empirical Function");
                    Utility.PlotFunc(plotter, funcColor, distributionFunc, 0, 1, 1e-2, "Theoretical Function");
                }
                plotter.Viewport.FitToView();
                Log.Add(string.Format("{0} {2} {1}", res.ToString("F6", CultureInfo.InvariantCulture),
                    TableValue.ToString("F6", CultureInfo.InvariantCulture), res <= TableValue ? "<=" : ">"));
                Log.Add(res <= TableValue
                    ? "There is no reason to reject the hypothesis H0"
                    : "There is no reason to accept the hypothesis H0");
                Log.Add("Initial sample:");
                foreach (var d in p)
                {
                    Log.Add(d.ToString("F6", CultureInfo.InvariantCulture));
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
