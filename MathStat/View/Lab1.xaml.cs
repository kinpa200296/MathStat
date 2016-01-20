using System.Windows;
using System.Windows.Controls;

namespace MathStat.View
{
    /// <summary>
    /// Interaction logic for Lab1.xaml
    /// </summary>
    public partial class Lab1 : Page
    {
        public Lab1()
        {
            InitializeComponent();
        }

        private void DoAction(object sender, RoutedEventArgs e)
        {
            ViewModel.DoActionExecute(Plotter);
        }
    }
}
