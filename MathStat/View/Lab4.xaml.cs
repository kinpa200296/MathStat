using System.Windows;
using System.Windows.Controls;

namespace MathStat.View
{
    /// <summary>
    /// Interaction logic for Lab4.xaml
    /// </summary>
    public partial class Lab4 : Page
    {
        public Lab4()
        {
            InitializeComponent();
        }

        private void DoAction(object sender, RoutedEventArgs e)
        {
            ViewModel.DoActionExecute(Plotter);
        }
    }
}
