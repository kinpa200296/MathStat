using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
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
            _intervalCount3, _intervalCount4, _left5SampleSize, _right5SampleSize;
        private bool _checkBox1Checked, _checkBox2Checked, _checkBox3Checked, _display5Sample, _display5Probability;
        private double _tableValue4, _confidencialProbabilty5, _left5Probability, _right5Probability;

        public Func<double, double> DistributionFunc = d => Math.Asin(d)*2/Math.PI;
        public string DistributionFuncString = "F(y) = 2/pi*asin(y)";
        public Func<double, double> DistributionDensityFunc = d => 2/Math.PI/Math.Sqrt(1 - d*d);
        public string DistributionDensityFuncString = "f(y) = 2/pi/(1-y^2)^(1/2)";
        public const double TheoreticalExpectedValue = 0.6366;
        public const double TheoreticalVariance = 0.0947;

        public enum Method4
        {
            Pirson, Kolmorogov, Mizes
        }

        public enum Target5
        {
            ExpectedValue, Variance
        }

        public enum Dependency5
        {
            Sample, Probability
        }

        #region Properties

        public ObservableCollection<string> Log1 { get; set; }
        public ObservableCollection<string> Log2 { get; set; }
        public ObservableCollection<string> Log3 { get; set; }
        public ObservableCollection<string> Log4 { get; set; }
        public ObservableCollection<string> Log5 { get; set; }
        public Method4 ChoosenMethod4 { get; set; }
        public Target5 ChoosenTarget5 { get; set; }
        public Dependency5 ChoosenDependency5 { get; set; }

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

        public double Left5Probability
        {
            get { return _left5Probability; }
            set
            {
                _left5Probability = value;
                OnPropertyChanged("Left5Probability");
            }
        }

        public double Right5Probability
        {
            get { return _right5Probability; }
            set
            {
                _right5Probability = value;
                OnPropertyChanged("Right5Probability");
            }
        }

        public int Left5SampleSize
        {
            get { return _left5SampleSize; }
            set
            {
                _left5SampleSize = value;
                OnPropertyChanged("Left5SampleSize");
            }
        }

        public int Right5SampleSize
        {
            get { return _right5SampleSize; }
            set
            {
                _right5SampleSize = value;
                OnPropertyChanged("Right5SampleSize");
            }
        }

        public bool Display5Sample
        {
            get { return _display5Sample; }
            set
            {
                _display5Sample = value;
                OnPropertyChanged("Display5Sample");
            }
        }

        public bool Display5Probability
        {
            get { return _display5Probability; }
            set
            {
                _display5Probability = value;
                OnPropertyChanged("Display5Probability");
            }
        }

        public double ConfidencialProbabilty5
        {
            get { return _confidencialProbabilty5; }
            set
            {
                _confidencialProbabilty5 = value;
                OnPropertyChanged("ConfidencialProbabilty5");
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

            ConfidencialProbabilty5 = 0.95;
            Left5Probability = 0.9;
            Right5Probability = 0.999;

            Left5SampleSize = 30;
            Right5SampleSize = 150;

            PirsonChoosenExecute();
            ExpectedValueChoosenExecute();
            SampleSizeChoosenExecute();
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

        public ICommand SampleSizeChoosen
        {
            get { return new RelayCommand(SampleSizeChoosenExecute); }
        }

        public ICommand ConfidencialProbabiltyChoosen
        {
            get { return new RelayCommand(ConfidencialProbabiltyChoosenExecute); }
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

        public void SampleSizeChoosenExecute()
        {
            Display5Sample = true;
            Display5Probability = false;
            ChoosenDependency5 = Dependency5.Probability;
        }

        public void ConfidencialProbabiltyChoosenExecute()
        {
            Display5Sample = false;
            Display5Probability = true;
            ChoosenDependency5 = Dependency5.Sample;
        }

        private void Action5FixedSampleSize(ChartPlotter plotter, ChartPlotter plotterSizes)
        {
            var p =
                Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                    Math.Sin, SampleSize5));
            var expectedValue = Kindruk.lab5.Analyzer.CalcExceptedValueBySample(p);
            var variance = Kindruk.lab5.Analyzer.CalcVarianceValueBySample(p, expectedValue);
            Log5.Add(string.Format("Calculated expected value: {0}",
                expectedValue.ToString("F6", CultureInfo.InvariantCulture)));
            Log5.Add(string.Format("Calculated variance: {0}",
                variance.ToString("F6", CultureInfo.InvariantCulture)));
            switch (ChoosenTarget5)
            {
                case Target5.ExpectedValue:
                    var intervalsNormal = Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p,
                        variance, Left5Probability, Right5Probability, (Right5Probability - Left5Probability)/100.0);
                    var intervalSizesNormal = intervalsNormal.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var intervalsNormalTheoretical =
                        Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p, TheoreticalVariance,
                            Left5Probability, Right5Probability, (Right5Probability - Left5Probability)/100.0);
                    var intervalSizesNormalTheoretical = intervalsNormalTheoretical.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    Utility.PlotConfidencialIntervals(plotter, Colors.Green, intervalsNormal, "Normal/Calc");
                    Utility.PlotPolyLine(intervalSizesNormal, plotterSizes, Colors.Green, "Normal/Calc");
                    Utility.PlotConfidencialIntervals(plotter, Colors.Yellow, intervalsNormalTheoretical,
                        "Normal/Theory");
                    Utility.PlotPolyLine(intervalSizesNormalTheoretical, plotterSizes, Colors.Yellow, "Normal/Theory");
                    if (p.Length < ArgumentTables.Student.MaxNumberOfFreedoms)
                    {
                        var intervalsStudent =
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseStudent(p,
                                variance, Left5Probability, Right5Probability,
                                (Right5Probability - Left5Probability)/100.0);
                        var intervalSizesStudent = intervalsStudent.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        var intervalsStudentTheoretical =
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseStudent(p,
                                TheoreticalVariance, Left5Probability, Right5Probability,
                                (Right5Probability - Left5Probability)/100.0);
                        var intervalSizesStudentTheoretical = intervalsStudentTheoretical.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        Utility.PlotConfidencialIntervals(plotter, Colors.Blue, intervalsStudent, "Student/Calc");
                        Utility.PlotPolyLine(intervalSizesStudent, plotterSizes, Colors.Blue, "Student/Calc");
                        Utility.PlotConfidencialIntervals(plotter, Colors.DarkViolet, intervalsStudentTheoretical,
                            "Student/Theory");
                        Utility.PlotPolyLine(intervalSizesStudentTheoretical, plotterSizes, Colors.DarkViolet,
                            "Student/Theory");
                    }
                    Utility.PlotLine(plotter, Colors.Red, Left5Probability, Right5Probability, TheoreticalExpectedValue,
                        "ExpectedValue");
                    break;
                case Target5.Variance:
                    var varianceIntervalsNormal = Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(
                        p, expectedValue, Left5Probability, Right5Probability,
                        (Right5Probability - Left5Probability)/100.0, false);
                    var varianceIntervalSizesNormal = varianceIntervalsNormal.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var varianceIntervalsNormalTheoretical =
                        Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(p, TheoreticalExpectedValue,
                            Left5Probability, Right5Probability, (Right5Probability - Left5Probability)/100.0, true);
                    var varianceIntervalSizesNormalTheoretical = varianceIntervalsNormalTheoretical.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    Utility.PlotConfidencialIntervals(plotter, Colors.Green, varianceIntervalsNormal, "Normal/Calc");
                    Utility.PlotPolyLine(varianceIntervalSizesNormal, plotterSizes, Colors.Green, "Normal/Calc");
                    Utility.PlotConfidencialIntervals(plotter, Colors.Yellow, varianceIntervalsNormalTheoretical,
                        "Normal/Theory");
                    Utility.PlotPolyLine(varianceIntervalSizesNormalTheoretical, plotterSizes, Colors.Yellow,
                        "Normal/Theory");
                    if (p.Length < ArgumentTables.Xi2.MaxNumberOfFreedoms)
                    {
                        var varianceIntervalsXi2 =
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseXi2(p, expectedValue,
                                Left5Probability, Right5Probability, (Right5Probability - Left5Probability)/100.0, false);
                        var varianceIntervalSizesXi2 = varianceIntervalsXi2.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        var varianceIntervalsXi2Theoretical =
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseXi2(p, TheoreticalExpectedValue,
                                Left5Probability, Right5Probability, (Right5Probability - Left5Probability)/100.0, true);
                        var varianceIntervalSizesXi2Theoretical = varianceIntervalsXi2Theoretical.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        Utility.PlotConfidencialIntervals(plotter, Colors.Blue, varianceIntervalsXi2, "Xi2/Calc");
                        Utility.PlotPolyLine(varianceIntervalSizesXi2, plotterSizes, Colors.Blue, "Xi2/Calc");
                        Utility.PlotConfidencialIntervals(plotter, Colors.DarkViolet, varianceIntervalsXi2Theoretical,
                            "Xi2/Theory");
                        Utility.PlotPolyLine(varianceIntervalSizesXi2Theoretical, plotterSizes, Colors.DarkViolet,
                            "Xi2/Theory");
                    }
                    Utility.PlotLine(plotter, Colors.Red, Left5Probability, Right5Probability, TheoreticalVariance,
                        "Variance");
                    break;
            }
            Log5.Add("Initial sample:");
            foreach (var d in p)
            {
                Log5.Add(d.ToString("F6", CultureInfo.InvariantCulture));
            }
        }

        private void Action5FixedConfidencialProbability(ChartPlotter plotter, ChartPlotter plotterSizes)
        {
            Log5.Add("Sampe size | Expected Value | Variance");
            switch (ChoosenTarget5)
            {
                case Target5.ExpectedValue:
                    var intervalsNormal = new List<ConfidencialInterval>();
                    var intervalsNormalTheoretical = new List<ConfidencialInterval>();
                    var intervalsStudent = new List<ConfidencialInterval>();
                    var intervalsStudentTheoretical = new List<ConfidencialInterval>();
                    for (var size = Left5SampleSize; size <= Right5SampleSize; size++)
                    {
                        var p =
                            Analyzer.GetVariationalSeries(
                                SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI), Math.Sin, size));
                        var expectedValue = Kindruk.lab5.Analyzer.CalcExceptedValueBySample(p);
                        var variance = Kindruk.lab5.Analyzer.CalcVarianceValueBySample(p, expectedValue);
                        Log5.Add(string.Format("    {0}     |    {1}    |    {2}", size,
                            expectedValue.ToString("F6", CultureInfo.InvariantCulture),
                            variance.ToString("F6", CultureInfo.InvariantCulture)));
                        intervalsNormal.Add(Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p,
                            variance, ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        intervalsNormalTheoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p,
                                TheoreticalVariance, ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        if (p.Length >= ArgumentTables.Student.MaxNumberOfFreedoms) continue;
                        intervalsStudent.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseStudent(p, variance,
                                ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        intervalsStudentTheoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseStudent(p,
                                TheoreticalVariance, ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                    }
                    var intervalSizesNormal = intervalsNormal.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var intervalSizesNormalTheoretical = intervalsNormalTheoretical.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var intervalSizesStudent = intervalsStudent.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var intervalSizesStudentTheoretical = intervalsStudentTheoretical.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    Utility.PlotConfidencialIntervals(plotter, Colors.Green, intervalsNormal.ToArray(), "Normal/Calc");
                    Utility.PlotPolyLine(intervalSizesNormal, plotterSizes, Colors.Green, "Normal/Calc");
                    Utility.PlotConfidencialIntervals(plotter, Colors.Yellow, intervalsNormalTheoretical.ToArray(),
                        "Normal/Theory");
                    Utility.PlotPolyLine(intervalSizesNormalTheoretical, plotterSizes, Colors.Yellow, "Normal/Theory");
                    Utility.PlotConfidencialIntervals(plotter, Colors.Blue, intervalsStudent.ToArray(), "Student/Calc");
                    Utility.PlotPolyLine(intervalSizesStudent, plotterSizes, Colors.Blue, "Student/Calc");
                    Utility.PlotConfidencialIntervals(plotter, Colors.DarkViolet, intervalsStudentTheoretical.ToArray(),
                        "Student/Theory");
                    Utility.PlotPolyLine(intervalSizesStudentTheoretical, plotterSizes, Colors.DarkViolet,
                        "Student/Theory");
                    Utility.PlotLine(plotter, Colors.Red, Left5SampleSize, Right5SampleSize, TheoreticalExpectedValue,
                        "ExpectedValue");
                    break;
                case Target5.Variance:
                    var varianceIntervalsNormal = new List<ConfidencialInterval>();
                    var varianceIntervalsNormalTheoretical = new List<ConfidencialInterval>();
                    var varianceIntervalsXi2 = new List<ConfidencialInterval>();
                    var varianceIntervalsXi2Theoretical = new List<ConfidencialInterval>();
                    for (var size = Left5SampleSize; size <= Right5SampleSize; size++)
                    {
                        var p =
                            Analyzer.GetVariationalSeries(
                                SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI), Math.Sin, size));
                        var expectedValue = Kindruk.lab5.Analyzer.CalcExceptedValueBySample(p);
                        var variance = Kindruk.lab5.Analyzer.CalcVarianceValueBySample(p, expectedValue);
                        Log5.Add(string.Format("    {0}     |    {1}    |    {2}", size,
                            expectedValue.ToString("F6", CultureInfo.InvariantCulture),
                            variance.ToString("F6", CultureInfo.InvariantCulture)));
                        varianceIntervalsNormal.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(p, expectedValue,
                                ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0, false)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        varianceIntervalsNormalTheoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(p, TheoreticalExpectedValue,
                                ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0, true)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        if (p.Length >= ArgumentTables.Student.MaxNumberOfFreedoms) continue;
                        varianceIntervalsXi2.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseXi2(p, expectedValue,
                                ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0, false)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        varianceIntervalsXi2Theoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseXi2(p, TheoreticalExpectedValue,
                                ConfidencialProbabilty5, ConfidencialProbabilty5, 1.0, true)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                    }
                    var varianceIntervalSizesNormal = varianceIntervalsNormal.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var varianceIntervalSizesNormalTheoretical = varianceIntervalsNormalTheoretical.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var varianceIntervalSizesXi2 = varianceIntervalsXi2.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var varianceIntervalSizesXi2Theoretical = varianceIntervalsXi2Theoretical.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    Utility.PlotConfidencialIntervals(plotter, Colors.Green, varianceIntervalsNormal.ToArray(),
                        "Normal/Calc");
                    Utility.PlotPolyLine(varianceIntervalSizesNormal, plotterSizes, Colors.Green, "Normal/Calc");
                    Utility.PlotConfidencialIntervals(plotter, Colors.Yellow, varianceIntervalsNormalTheoretical.ToArray(),
                        "Normal/Theory");
                    Utility.PlotPolyLine(varianceIntervalSizesNormalTheoretical, plotterSizes, Colors.Yellow,
                        "Normal/Theory");
                    Utility.PlotConfidencialIntervals(plotter, Colors.Blue, varianceIntervalsXi2.ToArray(), "Xi2/Calc");
                    Utility.PlotPolyLine(varianceIntervalSizesXi2, plotterSizes, Colors.Blue, "Xi2/Calc");
                    Utility.PlotConfidencialIntervals(plotter, Colors.DarkViolet, varianceIntervalsXi2Theoretical.ToArray(),
                        "Xi2/Theory");
                    Utility.PlotPolyLine(varianceIntervalSizesXi2Theoretical, plotterSizes, Colors.DarkViolet,
                        "Xi2/Theory");
                    Utility.PlotLine(plotter, Colors.Red, Left5SampleSize, Right5SampleSize, TheoreticalVariance,
                        "Variance");
                    break;
            }
        }

        public void DoAction5Execute(ChartPlotter plotter, ChartPlotter plotterSizes)
        {
            Log5.Clear();
            try
            {
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                plotterSizes.Children.RemoveAll(typeof(LineGraph));
                plotterSizes.Children.RemoveAll(typeof(MarkerPointsGraph));
                Log5.Add(string.Format("Theoretical expected value: {0}",
                    TheoreticalExpectedValue.ToString("F6", CultureInfo.InvariantCulture)));
                Log5.Add(string.Format("Theoretical variance: {0}",
                    TheoreticalVariance.ToString("F6", CultureInfo.InvariantCulture)));
                switch (ChoosenDependency5)
                {
                    case Dependency5.Sample:
                        Action5FixedConfidencialProbability(plotter, plotterSizes);
                        break;
                    case Dependency5.Probability:
                        Action5FixedSampleSize(plotter, plotterSizes);
                        break;
                }
                plotter.FitToView();
                plotterSizes.FitToView();
            }
            catch (Exception e)
            {
                Log5.Add(e.Message);
            }
        }

        #endregion
    }
}
