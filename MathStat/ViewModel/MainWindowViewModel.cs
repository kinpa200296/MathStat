﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using Kindruk.lab1;
using MathCommon;
using MathStat.Model;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Win32;

namespace MathStat.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Random _random;
        private string _inputFile, _outputFile;
        private int _sampleSize1, _sampleSize2, _sampleSize3, _sampleSize4, _sampleSize5, _intervalCount2,
            _intervalCount3, _intervalCount4;
        private bool _checkBox1Checked, _checkBox2Checked, _checkBox3Checked;
        private double _tableValue4;

        public Func<double, double> DistributionFunc = d => Math.Asin(d)*2/Math.PI;
        public string DistributionFuncString = "F(y) = 2/pi*asin(y)";
        public Func<double, double> DistributionDensityFunc = d => 2/Math.PI/Math.Sqrt(1 - d*d);
        public string DistributionDensityFuncString = "f(y) = 2/pi/(1-y^2)^(1/2)";

        public enum Method4
        {
            Pirson, Kolmorogov, Mizes
        }

        public enum Target5
        {
            ExpectedValue, Variance
        }

        #region Properties

        public ObservableCollection<string> Log1 { get; set; }
        public ObservableCollection<string> Log2 { get; set; }
        public ObservableCollection<string> Log3 { get; set; }
        public ObservableCollection<string> Log4 { get; set; }
        public ObservableCollection<string> Log5 { get; set; }
        public Method4 ChoosenMethod4 { get; set; }
        public Target5 ChoosenTarget5 { get; set; }

        public string InputFile
        {
            get { return _inputFile; }
            set
            {
                _inputFile = value;
                OnPropertyChanged("InputFile");
            }
        }

        public string OutputFile
        {
            get { return _outputFile; }
            set
            {
                _outputFile = value;
                OnPropertyChanged("OutputFile");
            }
        }

        public int SampleSize1
        {
            get { return _sampleSize1; }
            set
            {
                _sampleSize1 = value;
                OnPropertyChanged("SampleSize1");
            }
        }

        public int SampleSize2
        {
            get { return _sampleSize2; }
            set
            {
                _sampleSize2 = value;
                OnPropertyChanged("SampleSize2");
            }
        }

        public int SampleSize3
        {
            get { return _sampleSize3; }
            set
            {
                _sampleSize3 = value;
                OnPropertyChanged("SampleSize3");
            }
        }

        public int SampleSize4
        {
            get { return _sampleSize4; }
            set
            {
                _sampleSize4 = value;
                OnPropertyChanged("SampleSize4");
            }
        }

        public int SampleSize5
        {
            get { return _sampleSize5; }
            set
            {
                _sampleSize5 = value;
                OnPropertyChanged("SampleSize5");
            }
        }

        public int IntervalCount2
        {
            get { return _intervalCount2; }
            set
            {
                _intervalCount2 = value;
                OnPropertyChanged("IntervalCount2");
            }
        }

        public int IntervalCount3
        {
            get { return _intervalCount3; }
            set
            {
                _intervalCount3 = value;
                OnPropertyChanged("IntervalCount3");
            }
        }

        public int IntervalCount4
        {
            get { return _intervalCount4; }
            set
            {
                _intervalCount4 = value;
                OnPropertyChanged("IntervalCount4");
            }
        }

        public bool CheckBox1Checked
        {
            get { return _checkBox1Checked; }
            set
            {
                _checkBox1Checked = value;
                OnPropertyChanged("CheckBox1Checked");
            }
        }

        public bool CheckBox2Checked
        {
            get { return _checkBox2Checked; }
            set
            {
                _checkBox2Checked = value;
                OnPropertyChanged("CheckBox2Checked");
            }
        }

        public bool CheckBox3Checked
        {
            get { return _checkBox3Checked; }
            set
            {
                _checkBox3Checked = value;
                OnPropertyChanged("CheckBox3Checked");
            }
        }

        public double TableValue4
        {
            get { return _tableValue4; }
            set
            {
                _tableValue4 = value;
                OnPropertyChanged("TableValue4");
            }
        }

        #endregion

        #region Consructors

        public MainWindowViewModel()
        {
            _random =
                new Random(DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Minute + DateTime.Now.Hour);

            Log1 = new ObservableCollection<string>();
            Log2 = new ObservableCollection<string>();
            Log3 = new ObservableCollection<string>();
            Log4 = new ObservableCollection<string>();
            Log5 = new ObservableCollection<string>();

            SampleSize1 = 40;
            SampleSize2 = 100;
            SampleSize3 = 100;
            SampleSize5 = 20;

            _intervalCount2 = 10;
            _intervalCount3 = 10;

            OutputFile = Directory.GetCurrentDirectory() + @"\output.csv";

            CheckBox1Checked = false;
            CheckBox2Checked = false;
            CheckBox3Checked = false;

            PirsonChoosenExecute();
            ExpectedValueChoosenExecute();
        }

        #endregion

        #region Commands

        public ICommand OpenFileDialog
        {
            get { return new RelayCommand(OpenFileDialogExecute); }
        }

        public ICommand SaveFileDialog
        {
            get { return new RelayCommand(SaveFileDialogExecute); }
        }

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

        public ICommand ExpectedValueChoosen
        {
            get { return new RelayCommand(ExpectedValueChoosenExecute); }
        }

        public ICommand VarianceChoosen 
        {
            get { return new RelayCommand(VarianceChoosenExecute);}
        }

        #endregion

        #region Methods

        public void OpenFileDialogExecute()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open File",
                InitialDirectory = Directory.GetCurrentDirectory(),
                RestoreDirectory = false,
                DefaultExt = "in",
                Filter = "CSV files(*.csv)|*.csv|Input files (*.in)|*.in"
            };
            dialog.ShowDialog();
            InputFile = dialog.FileName;
        }

        public void SaveFileDialogExecute()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save File As",
                InitialDirectory = Directory.GetCurrentDirectory(),
                RestoreDirectory = false,
                DefaultExt = "out",
                Filter = "CSV files(*.csv)|*.csv|Output files (*.out)|*.out"
            };
            dialog.ShowDialog();
            OutputFile = dialog.FileName;
        }

        public Color GetRandomColor()
        {
            var temp = new byte[3];
            _random.NextBytes(temp);
            return Color.FromRgb((byte) (temp[0]%193), (byte) (temp[1]%193), (byte) (temp[2]%193));
        }

        public void DoAction1Execute(ChartPlotter plotter)
        {
            Log1.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize1));
                var data = Analyzer.GetEmpiricalFuncData(p);
                var empiricalColor = GetRandomColor();
                var funcColor = GetRandomColor();
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                Utility.PlotDistributionFunc(data, plotter, empiricalColor, "Empirical Function");
                Utility.PlotFunc(plotter, funcColor, DistributionFunc, 0, 1, 1e-2, "Theoretical Function");
                plotter.Viewport.FitToView();
                Log1.Add(DistributionFuncString);
                Log1.Add("y                    F_emp(y)");
                foreach (var tuple in data)
                {
                    Log1.Add(string.Format("{0}       {1}", tuple.Item1.ToString("F6", CultureInfo.InvariantCulture),
                        tuple.Item2.ToString("F6", CultureInfo.InvariantCulture)));
                }
                if (CheckBox1Checked)
                {
                    using (var file = File.Create(OutputFile))
                    {
                        using (var stream = new StreamWriter(file))
                        {
                            stream.WriteLine("y;F_emp(y)");
                            foreach (var tuple in data)
                            {
                                stream.WriteLine("{0};{1}",
                                    tuple.Item1.ToString("F6", CultureInfo.InvariantCulture),
                                    tuple.Item2.ToString("F6", CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log1.Add(e.Message);
            }
        }

        public void DoAction2Execute(ChartPlotter plotter, ChartPlotter plotterBarChart)
        {
            Log2.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize2));
                var data = Kindruk.lab2.Analyzer.SplitSampleEqualIntervals(p, IntervalCount2);
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
                Log2.Add(DistributionDensityFuncString);
                foreach (var interval in data)
                {
                    Log2.Add(string.Format("[{0}; {1}]: f* = {2}",
                        interval.Left.ToString("F6", CultureInfo.InvariantCulture),
                        interval.Right.ToString("F6", CultureInfo.InvariantCulture),
                        interval.ProbabilityDensity.ToString("F6", CultureInfo.InvariantCulture)));
                }
                Log2.Add("y                    F_emp(y)");
                foreach (var tuple in distributionFuncList)
                {
                    Log2.Add(string.Format("{0}       {1}", tuple.Item1.ToString("F6", CultureInfo.InvariantCulture),
                        tuple.Item2.ToString("F6", CultureInfo.InvariantCulture)));
                }
                if (CheckBox2Checked)
                {
                    using (var file = File.Create(OutputFile))
                    {
                        using (var stream = new StreamWriter(file))
                        {
                            stream.WriteLine("left border;right border;probability density");
                            foreach (var interval in data)
                            {
                                stream.WriteLine("{0};{1};{2}",
                                    interval.Left.ToString("F6", CultureInfo.InvariantCulture),
                                    interval.Right.ToString("F6", CultureInfo.InvariantCulture),
                                    interval.ProbabilityDensity.ToString("F6", CultureInfo.InvariantCulture));
                            }
                            stream.WriteLine("y;F_emp(y)");
                            foreach (var tuple in distributionFuncList)
                            {
                                stream.WriteLine("{0};{1}",
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
                Utility.PlotPolyLine(distributionPolygonPoints.ToArray(), plotterBarChart, polygonColor,
                    "Distribution Polygon");
                plotter.FitToView();
                plotterBarChart.FitToView();
            }
            catch (Exception e)
            {
                Log2.Add(e.Message);
            }
        }

        public void DoAction3Execute(ChartPlotter plotter, ChartPlotter plotterBarChart)
        {
            Log3.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize3));
                var data = Kindruk.lab3.Analyzer.SplitSampleEqualProbability(p, IntervalCount3);
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
                Log3.Add(DistributionDensityFuncString);
                foreach (var interval in data)
                {
                    Log3.Add(string.Format("[{0}; {1}]: f* = {2}",
                        interval.Left.ToString("F6", CultureInfo.InvariantCulture),
                        interval.Right.ToString("F6", CultureInfo.InvariantCulture),
                        interval.ProbabilityDensity.ToString("F6", CultureInfo.InvariantCulture)));
                }
                Log3.Add("y                    F_emp(y)");
                foreach (var tuple in distributionFuncList)
                {
                    Log3.Add(string.Format("{0}       {1}", tuple.Item1.ToString("F6", CultureInfo.InvariantCulture),
                        tuple.Item2.ToString("F6", CultureInfo.InvariantCulture)));
                }
                if (CheckBox3Checked)
                {
                    using (var file = File.Create(OutputFile))
                    {
                        using (var stream = new StreamWriter(file))
                        {
                            stream.WriteLine("left border;right border;probability density");
                            foreach (var interval in data)
                            {
                                stream.WriteLine("{0};{1};{2}",
                                    interval.Left.ToString("F6", CultureInfo.InvariantCulture),
                                    interval.Right.ToString("F6", CultureInfo.InvariantCulture),
                                    interval.ProbabilityDensity.ToString("F6", CultureInfo.InvariantCulture));
                            }
                            stream.WriteLine("y;F_emp(y)");
                            foreach (var tuple in distributionFuncList)
                            {
                                stream.WriteLine("{0};{1}",
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
                Log3.Add(e.Message);
            }
        }

        public void PirsonChoosenExecute()
        {
            ChoosenMethod4 = Method4.Pirson;
            SampleSize4 = 200;
            IntervalCount4 = 10;
            TableValue4 = 14.07;
        }

        public void KolmogorovChoosenExecute()
        {
            ChoosenMethod4 = Method4.Kolmorogov;
            SampleSize4 = 30;
            IntervalCount4 = 0;
            TableValue4 = 1.36;
        }

        public void MizesChoosenExecute()
        {
            ChoosenMethod4 = Method4.Mizes;
            SampleSize4 = 50;
            IntervalCount4 = 0;
            TableValue4 = 0.347;
        }

        public void DoAction4Execute(ChartPlotter plotter)
        {
            Log4.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize4));
                var res = 0.0;
                var distributionFunc = DistributionFunc;
                var distributionDensityFunc = DistributionDensityFunc;
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                switch (ChoosenMethod4)
                {
                    case Method4.Pirson:
                        res = Kindruk.lab4.Analyzer.Pirson(p, distributionFunc, IntervalCount4);
                        var intervals = Kindruk.lab3.Analyzer.SplitSampleEqualProbability(p, IntervalCount4);
                        var barChartColor = GetRandomColor();
                        var densityFuncColor = GetRandomColor();
                        Utility.PlotBarChart(plotter, barChartColor, intervals, "Bar Chart");
                        Utility.PlotFunc(plotter, densityFuncColor, distributionDensityFunc, 0, 0.999, 1e-2,
                            "Theoretical Function");
                        break;
                    case Method4.Kolmorogov:
                        res = Kindruk.lab4.Analyzer.Kolmogorov(p, distributionFunc);
                        break;
                    case Method4.Mizes:
                        res = Kindruk.lab4.Analyzer.Mizes(p, distributionFunc);
                        break;
                }
                if (ChoosenMethod4 != Method4.Pirson)
                {
                    var data = Analyzer.GetEmpiricalFuncData(p);
                    var empiricalColor = GetRandomColor();
                    var funcColor = GetRandomColor();
                    Utility.PlotDistributionFunc(data, plotter, empiricalColor, "Empirical Function");
                    Utility.PlotFunc(plotter, funcColor, distributionFunc, 0, 1, 1e-2, "Theoretical Function");
                }
                plotter.Viewport.FitToView();
                Log4.Add(string.Format("{0} {2} {1}", res.ToString("F6", CultureInfo.InvariantCulture),
                    TableValue4.ToString("F6", CultureInfo.InvariantCulture), res <= TableValue4 ? "<=" : ">"));
                Log4.Add(res <= TableValue4
                    ? "There is no reason to reject the hypothesis H0"
                    : "There is no reason to accept the hypothesis H0");
                Log4.Add("Initial sample:");
                foreach (var d in p)
                {
                    Log4.Add(d.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                Log4.Add(e.Message);
            }
        }

        public void ExpectedValueChoosenExecute()
        {
            ChoosenTarget5 = Target5.ExpectedValue;
        }

        public void VarianceChoosenExecute()
        {
            ChoosenTarget5 = Target5.Variance;
        }

        public void DoAction5Execute(ChartPlotter plotter)
        {
            Log5.Clear();
            try
            {
                var p =
                    Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                        Math.Sin,
                        SampleSize5));
                Log5.Add("Initial sample:");
                foreach (var d in p)
                {
                    Log5.Add(d.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                Log5.Add(e.Message);
            }
        }

        #endregion
    }
}
