using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MathCommon;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace MathStat.Model
{
    public static class Utility
    {
        public static PenDescription GetPenDescription(string description, Visibility visibility)
        {
            var penDescription = new PenDescription(description);
            penDescription.LegendItem.Visibility = visibility;
            return penDescription;
        }

        public static Pen GetPen(Color color, double thickness, DashStyle dashStyle)
        {
            return new Pen(new SolidColorBrush(color), thickness) {DashStyle = dashStyle};
        }

        public static void PlotDistributionFunc(Tuple<double, double>[] data, ChartPlotter plotter, Color color,
            string description)
        {
            var x = new EnumerableDataSource<double>(new[] {-0.5, data[0].Item1});
            x.SetXMapping(q => q);
            var y = new EnumerableDataSource<double>(new[] {0.0, data[0].Item2});
            y.SetYMapping(q => q);
            plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Solid),
                GetPenDescription(description, Visibility.Collapsed));
            for (var i = 0; i < data.Length - 1; i++)
            {
                x = new EnumerableDataSource<double>(new[] {data[i].Item1, data[i].Item1});
                x.SetXMapping(q => q);
                y = new EnumerableDataSource<double>(new[] {data[i].Item2, data[i + 1].Item2});
                y.SetYMapping(q => q);
                plotter.AddLineGraph(new CompositeDataSource(x, y),
                    GetPen(color, 1.0, DashStyles.Dash),
                    GetPenDescription(description, Visibility.Collapsed));
                x = new EnumerableDataSource<double>(new[] {data[i].Item1, data[i + 1].Item1});
                x.SetXMapping(q => q);
                y = new EnumerableDataSource<double>(new[] {data[i + 1].Item2, data[i + 1].Item2});
                y.SetYMapping(q => q);
                plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Solid),
                    GetPenDescription(description, Visibility.Collapsed));
            }
            x = new EnumerableDataSource<double>(new[] {data[data.Length - 1].Item1, data[data.Length - 1].Item1});
            x.SetXMapping(q => q);
            y = new EnumerableDataSource<double>(new[] {data[data.Length - 1].Item2, 1.0});
            y.SetYMapping(q => q);
            plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Dash),
                GetPenDescription(description, Visibility.Collapsed));
            x = new EnumerableDataSource<double>(new[] {data[data.Length - 1].Item1 + 0.5, data[data.Length - 1].Item1});
            x.SetXMapping(q => q);
            y = new EnumerableDataSource<double>(new[] {1.0, 1.0});
            y.SetYMapping(q => q);
            plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Solid),
                GetPenDescription(description, Visibility.Visible));
            foreach (var tuple in data)
            {
                x = new EnumerableDataSource<double>(new[] {tuple.Item1});
                x.SetXMapping(q => q);
                y = new EnumerableDataSource<double>(new[] {tuple.Item2});
                y.SetYMapping(q => q);
                plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Solid),
                    new CirclePointMarker
                    {
                        Size = 6.0,
                        Fill = Brushes.White,
                        Pen = new Pen(new SolidColorBrush(color), 1)
                    },
                    GetPenDescription(description, Visibility.Collapsed));
            }
        }

        public static void PlotFunc(ChartPlotter plotter, Color color, Func<double, double> func, double left,
            double right, double step, string description)
        {
            var xList = new List<double>();
            var yList = new List<double>();
            for (var q = left; q < right; q += step)
            {
                xList.Add(q);
                yList.Add(func(q));
            }
            xList.Add(right);
            yList.Add(func(right));
            var x = new EnumerableDataSource<double>(xList);
            x.SetXMapping(q => q);
            var y = new EnumerableDataSource<double>(yList);
            y.SetYMapping(q => q);
            plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Solid),
                GetPenDescription(description, Visibility.Visible));
        }

        public static void PlotPolyLine(Tuple<double, double>[] data, ChartPlotter plotter, Color color,
            string description)
        {
            var x = new EnumerableDataSource<double>(data.Select(q => q.Item1));
            x.SetXMapping(q => q);
            var y = new EnumerableDataSource<double>(data.Select(q => q.Item2));
            y.SetYMapping(q => q);
            plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Solid),
                GetPenDescription(description, Visibility.Visible));
        }

        public static void PlotBarChart(ChartPlotter plotter, Color color, IEnumerable<DataInterval> intervals,
            string description)
        {
            var xList = new List<double>();
            var yList = new List<double>();
            const double epsilon = 1e-6;
            foreach (var interval in intervals)
            {
                xList.Add(interval.Left + epsilon);
                xList.Add(interval.Left + epsilon);
                xList.Add(interval.Right - epsilon);
                xList.Add(interval.Right - epsilon);
                yList.Add(0.0);
                yList.Add(interval.ProbabilityDensity);
                yList.Add(interval.ProbabilityDensity);
                yList.Add(0.0);
            }
            var x = new EnumerableDataSource<double>(xList);
            x.SetXMapping(q => q);
            var y = new EnumerableDataSource<double>(yList);
            y.SetYMapping(q => q);
            plotter.AddLineGraph(new CompositeDataSource(x, y), GetPen(color, 1.0, DashStyles.Solid),
                GetPenDescription(description, Visibility.Visible));
        }
    }
}
