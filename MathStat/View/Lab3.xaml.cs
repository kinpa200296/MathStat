using System.Windows;
using System.Windows.Controls;

namespace MathStat.View
{
    /// <summary>
    /// Interaction logic for Lab3.xaml
    /// </summary>
    public partial class Lab3 : Page
    {
        public Lab3()
        {
            InitializeComponent();
        }

        private void DoAction(object sender, RoutedEventArgs e)
        {
            ViewModel.DoActionExecute(Plotter, PlotterBarChart);
        }
    }
}
