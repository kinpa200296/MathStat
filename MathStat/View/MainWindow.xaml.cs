using System.Windows;

namespace MathStat.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Action1_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.DoAction1Execute(Plotter1);
        }

        private void Action2_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.DoAction2Execute(Plotter2, Plotter2BarChart);
        }

        private void Action3_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.DoAction3Execute(Plotter3, Plotter3BarChart);
        }
    }
}
