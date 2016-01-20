using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Kindruk.lab1;
using MathCommon;
using MathStat.Model;
using Microsoft.Research.DynamicDataDisplay;

namespace MathStat.ViewModel
{
    public class Lab5ViewModel : LabBaseViewModel
    {
        private int _sampleSizeLeftBorder, _sampleSizeRightBorder;
        private bool _displaySample, _displayProbability;
        private double _confidencialProbabilty, _probabilityLeftBorder, _probabilityRightBorder;

        public const double TheoreticalExpectedValue = 0.6366;
        public const double TheoreticalVariance = 0.0947;

        public enum Target
        {
            ExpectedValue, Variance
        }

        public enum Dependency
        {
            Sample, Probability
        }

        #region Properties

        public Target ChoosenTarget { get; set; }
        public Dependency ChoosenDependency { get; set; }

        public double ProbabilityLeftBorder
        {
            get { return _probabilityLeftBorder; }
            set
            {
                _probabilityLeftBorder = value;
                OnPropertyChanged("ProbabilityLeftBorder");
            }
        }

        public double ProbabilityRightBorder
        {
            get { return _probabilityRightBorder; }
            set
            {
                _probabilityRightBorder = value;
                OnPropertyChanged("ProbabilityRightBorder");
            }
        }

        public int SampleSizeLeftBorder
        {
            get { return _sampleSizeLeftBorder; }
            set
            {
                _sampleSizeLeftBorder = value;
                OnPropertyChanged("SampleSizeLeftBorder");
            }
        }

        public int SampleSizeRightBorder
        {
            get { return _sampleSizeRightBorder; }
            set
            {
                _sampleSizeRightBorder = value;
                OnPropertyChanged("SampleSizeRightBorder");
            }
        }

        public bool DisplaySample
        {
            get { return _displaySample; }
            set
            {
                _displaySample = value;
                OnPropertyChanged("DisplaySample");
            }
        }

        public bool DisplayProbability
        {
            get { return _displayProbability; }
            set
            {
                _displayProbability = value;
                OnPropertyChanged("DisplayProbability");
            }
        }

        public double ConfidencialProbabilty
        {
            get { return _confidencialProbabilty; }
            set
            {
                _confidencialProbabilty = value;
                OnPropertyChanged("ConfidencialProbabilty");
            }
        }

        #endregion

        #region Constructors

        public Lab5ViewModel()
        {
            SampleSize = 20;

            ConfidencialProbabilty = 0.95;
            ProbabilityLeftBorder = 0.9;
            ProbabilityRightBorder = 0.999;

            SampleSizeLeftBorder = 30;
            SampleSizeRightBorder = 150;

            ExpectedValueChoosenExecute();
            SampleSizeChoosenExecute();
        }

        #endregion

        #region Commands

        public ICommand ExpectedValueChoosen
        {
            get { return new RelayCommand(ExpectedValueChoosenExecute); }
        }

        public ICommand VarianceChoosen
        {
            get { return new RelayCommand(VarianceChoosenExecute); }
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

        public void ExpectedValueChoosenExecute()
        {
            ChoosenTarget = Target.ExpectedValue;
        }

        public void VarianceChoosenExecute()
        {
            ChoosenTarget = Target.Variance;
        }

        public void SampleSizeChoosenExecute()
        {
            DisplaySample = true;
            DisplayProbability = false;
            ChoosenDependency = Dependency.Probability;
        }

        public void ConfidencialProbabiltyChoosenExecute()
        {
            DisplaySample = false;
            DisplayProbability = true;
            ChoosenDependency = Dependency.Sample;
        }

        private void DoWithFixedSampleSize(ChartPlotter plotter, ChartPlotter plotterSizes)
        {
            var p =
                Analyzer.GetVariationalSeries(SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI),
                    Math.Sin, SampleSize));
            var expectedValue = Kindruk.lab5.Analyzer.CalcExceptedValueBySample(p);
            var variance = Kindruk.lab5.Analyzer.CalcVarianceValueBySample(p, expectedValue);
            Log.Add(string.Format("Calculated expected value: {0}",
                expectedValue.ToString("F6", CultureInfo.InvariantCulture)));
            Log.Add(string.Format("Calculated variance: {0}",
                variance.ToString("F6", CultureInfo.InvariantCulture)));
            switch (ChoosenTarget)
            {
                case Target.ExpectedValue:
                    var intervalsNormal = Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p,
                        variance, ProbabilityLeftBorder, ProbabilityRightBorder, (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0);
                    var intervalSizesNormal = intervalsNormal.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var intervalsNormalTheoretical =
                        Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p, TheoreticalVariance,
                            ProbabilityLeftBorder, ProbabilityRightBorder, (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0);
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
                                variance, ProbabilityLeftBorder, ProbabilityRightBorder,
                                (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0);
                        var intervalSizesStudent = intervalsStudent.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        var intervalsStudentTheoretical =
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseStudent(p,
                                TheoreticalVariance, ProbabilityLeftBorder, ProbabilityRightBorder,
                                (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0);
                        var intervalSizesStudentTheoretical = intervalsStudentTheoretical.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        Utility.PlotConfidencialIntervals(plotter, Colors.Blue, intervalsStudent, "Student/Calc");
                        Utility.PlotPolyLine(intervalSizesStudent, plotterSizes, Colors.Blue, "Student/Calc");
                        Utility.PlotConfidencialIntervals(plotter, Colors.DarkViolet, intervalsStudentTheoretical,
                            "Student/Theory");
                        Utility.PlotPolyLine(intervalSizesStudentTheoretical, plotterSizes, Colors.DarkViolet,
                            "Student/Theory");
                    }
                    Utility.PlotLine(plotter, Colors.Red, ProbabilityLeftBorder, ProbabilityRightBorder, TheoreticalExpectedValue,
                        "ExpectedValue");
                    break;
                case Target.Variance:
                    var varianceIntervalsNormal = Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(
                        p, expectedValue, ProbabilityLeftBorder, ProbabilityRightBorder,
                        (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0, false);
                    var varianceIntervalSizesNormal = varianceIntervalsNormal.Select(
                        x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                    var varianceIntervalsNormalTheoretical =
                        Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(p, TheoreticalExpectedValue,
                            ProbabilityLeftBorder, ProbabilityRightBorder, (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0, true);
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
                                ProbabilityLeftBorder, ProbabilityRightBorder, (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0, false);
                        var varianceIntervalSizesXi2 = varianceIntervalsXi2.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        var varianceIntervalsXi2Theoretical =
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseXi2(p, TheoreticalExpectedValue,
                                ProbabilityLeftBorder, ProbabilityRightBorder, (ProbabilityRightBorder - ProbabilityLeftBorder) / 100.0, true);
                        var varianceIntervalSizesXi2Theoretical = varianceIntervalsXi2Theoretical.Select(
                            x => new Tuple<double, double>(x.ConfidencialProbability, x.Right - x.Left)).ToArray();
                        Utility.PlotConfidencialIntervals(plotter, Colors.Blue, varianceIntervalsXi2, "Xi2/Calc");
                        Utility.PlotPolyLine(varianceIntervalSizesXi2, plotterSizes, Colors.Blue, "Xi2/Calc");
                        Utility.PlotConfidencialIntervals(plotter, Colors.DarkViolet, varianceIntervalsXi2Theoretical,
                            "Xi2/Theory");
                        Utility.PlotPolyLine(varianceIntervalSizesXi2Theoretical, plotterSizes, Colors.DarkViolet,
                            "Xi2/Theory");
                    }
                    Utility.PlotLine(plotter, Colors.Red, ProbabilityLeftBorder, ProbabilityRightBorder, TheoreticalVariance,
                        "Variance");
                    break;
            }
            Log.Add("Initial sample:");
            foreach (var d in p)
            {
                Log.Add(d.ToString("F6", CultureInfo.InvariantCulture));
            }
        }

        private void DoWithFixedConfidencialProbability(ChartPlotter plotter, ChartPlotter plotterSizes)
        {
            Log.Add("Sampe size | Expected Value | Variance");
            switch (ChoosenTarget)
            {
                case Target.ExpectedValue:
                    var intervalsNormal = new List<ConfidencialInterval>();
                    var intervalsNormalTheoretical = new List<ConfidencialInterval>();
                    var intervalsStudent = new List<ConfidencialInterval>();
                    var intervalsStudentTheoretical = new List<ConfidencialInterval>();
                    for (var size = SampleSizeLeftBorder; size <= SampleSizeRightBorder; size++)
                    {
                        var p =
                            Analyzer.GetVariationalSeries(
                                SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI), Math.Sin, size));
                        var expectedValue = Kindruk.lab5.Analyzer.CalcExceptedValueBySample(p);
                        var variance = Kindruk.lab5.Analyzer.CalcVarianceValueBySample(p, expectedValue);
                        Log.Add(string.Format("    {0}     |    {1}    |    {2}", size,
                            expectedValue.ToString("F6", CultureInfo.InvariantCulture),
                            variance.ToString("F6", CultureInfo.InvariantCulture)));
                        intervalsNormal.Add(Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p,
                            variance, ConfidencialProbabilty, ConfidencialProbabilty, 1.0)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        intervalsNormalTheoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseNormal(p,
                                TheoreticalVariance, ConfidencialProbabilty, ConfidencialProbabilty, 1.0)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        if (p.Length >= ArgumentTables.Student.MaxNumberOfFreedoms) continue;
                        intervalsStudent.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseStudent(p, variance,
                                ConfidencialProbabilty, ConfidencialProbabilty, 1.0)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        intervalsStudentTheoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForExpectedValueUseStudent(p,
                                TheoreticalVariance, ConfidencialProbabilty, ConfidencialProbabilty, 1.0)
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
                    Utility.PlotLine(plotter, Colors.Red, SampleSizeLeftBorder, SampleSizeRightBorder, TheoreticalExpectedValue,
                        "ExpectedValue");
                    break;
                case Target.Variance:
                    var varianceIntervalsNormal = new List<ConfidencialInterval>();
                    var varianceIntervalsNormalTheoretical = new List<ConfidencialInterval>();
                    var varianceIntervalsXi2 = new List<ConfidencialInterval>();
                    var varianceIntervalsXi2Theoretical = new List<ConfidencialInterval>();
                    for (var size = SampleSizeLeftBorder; size <= SampleSizeRightBorder; size++)
                    {
                        var p =
                            Analyzer.GetVariationalSeries(
                                SampleBuilder.GetSample(new EvenlyDistributedNumber(0, Math.PI), Math.Sin, size));
                        var expectedValue = Kindruk.lab5.Analyzer.CalcExceptedValueBySample(p);
                        var variance = Kindruk.lab5.Analyzer.CalcVarianceValueBySample(p, expectedValue);
                        Log.Add(string.Format("    {0}     |    {1}    |    {2}", size,
                            expectedValue.ToString("F6", CultureInfo.InvariantCulture),
                            variance.ToString("F6", CultureInfo.InvariantCulture)));
                        varianceIntervalsNormal.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(p, expectedValue,
                                ConfidencialProbabilty, ConfidencialProbabilty, 1.0, false)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        varianceIntervalsNormalTheoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseNormal(p, TheoreticalExpectedValue,
                                ConfidencialProbabilty, ConfidencialProbabilty, 1.0, true)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        if (p.Length >= ArgumentTables.Student.MaxNumberOfFreedoms) continue;
                        varianceIntervalsXi2.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseXi2(p, expectedValue,
                                ConfidencialProbabilty, ConfidencialProbabilty, 1.0, false)
                            .Select(x => new ConfidencialInterval
                            {
                                Left = x.Left,
                                Right = x.Right,
                                ConfidencialProbability = size
                            }).First());
                        varianceIntervalsXi2Theoretical.Add(
                            Kindruk.lab5.Analyzer.FindConfidencialIntervalsForVarianceUseXi2(p, TheoreticalExpectedValue,
                                ConfidencialProbabilty, ConfidencialProbabilty, 1.0, true)
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
                    Utility.PlotLine(plotter, Colors.Red, SampleSizeLeftBorder, SampleSizeRightBorder, TheoreticalVariance,
                        "Variance");
                    break;
            }
        }

        public void DoActionExecute(ChartPlotter plotter, ChartPlotter plotterSizes)
        {
            Log.Clear();
            try
            {
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                plotterSizes.Children.RemoveAll(typeof(LineGraph));
                plotterSizes.Children.RemoveAll(typeof(MarkerPointsGraph));
                Log.Add(string.Format("Theoretical expected value: {0}",
                    TheoreticalExpectedValue.ToString("F6", CultureInfo.InvariantCulture)));
                Log.Add(string.Format("Theoretical variance: {0}",
                    TheoreticalVariance.ToString("F6", CultureInfo.InvariantCulture)));
                switch (ChoosenDependency)
                {
                    case Dependency.Sample:
                        DoWithFixedConfidencialProbability(plotter, plotterSizes);
                        break;
                    case Dependency.Probability:
                        DoWithFixedSampleSize(plotter, plotterSizes);
                        break;
                }
                plotter.FitToView();
                plotterSizes.FitToView();
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
