using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace MathStat.ViewModel
{
    public class LabBaseViewModel : ViewModelBase
    {
        private Random _random;
        private string _inputFile, _outputFile;
        private bool _checkBoxChecked;
        private int _sampleSize, _intervalCount;

        #region Properties

        public ObservableCollection<string> Log { get; set; }

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

        public bool CheckBoxChecked
        {
            get { return _checkBoxChecked; }
            set
            {
                _checkBoxChecked = value;
                OnPropertyChanged("CheckBoxChecked");
            }
        }

        public int SampleSize
        {
            get { return _sampleSize; }
            set
            {
                _sampleSize = value;
                OnPropertyChanged("SampleSize");
            }
        }

        public int IntervalCount
        {
            get { return _intervalCount; }
            set
            {
                _intervalCount = value;
                OnPropertyChanged("IntervalCount2");
            }
        }

        #endregion

        #region Constructors

        public LabBaseViewModel()
        {
            _random =
                new Random(DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Minute + DateTime.Now.Hour);

            OutputFile = Directory.GetCurrentDirectory() + @"\output.csv";
            CheckBoxChecked = false;

            Log = new ObservableCollection<string>();
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
            return Color.FromRgb((byte)(temp[0] % 193), (byte)(temp[1] % 193), (byte)(temp[2] % 193));
        }

        #endregion
    }
}
