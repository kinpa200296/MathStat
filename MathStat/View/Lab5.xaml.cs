using System.Windows;
using System.Windows.Controls;

namespace MathStat.View
{
    /// <summary>
    /// Interaction logic for Lab5.xaml
    /// </summary>
    public partial class Lab5 : Page
    {
        public Lab5()
        {
            InitializeComponent();
        }

        private void DoAction(object sender, RoutedEventArgs e)
        {
            ViewModel.DoActionExecute(Plotter, PlotterSizes);
        }
    }
}
