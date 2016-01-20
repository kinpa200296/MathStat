using System.Windows;
using System.Windows.Controls;

namespace MathStat.View
{
    /// <summary>
    /// Interaction logic for Lab2.xaml
    /// </summary>
    public partial class Lab2 : Page
    {
        public Lab2()
        {
            InitializeComponent();
        }

        private void DoAction(object sender, RoutedEventArgs e)
        {
            ViewModel.DoActionExecute(Plotter, PlotterBarChart);
        }
    }
}
