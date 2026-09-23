using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NAGN.Model
{
    public class OutputImage:INotifyPropertyChanged
    {
        private string _name { get; set; }
        public string Name
        {
            get => _name;
            set { _name = value;  OnPropertyChanged(); }
        }
        public ObservableCollection<Threshold> ImagePreprocessingList { get; set; } = new ObservableCollection<Threshold>();
        public int IdFOV { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand AddImagePreprocessingCommand { get; }
        public OutputImage()
        {
            AddImagePreprocessingCommand = new RelayCommand(AddImagePreprocessing);
        }
        public void AddImagePreprocessing()
        {
            View.Image_preprocessing.InputImagePreprocessing inputImagePreprocessing = new View.Image_preprocessing.InputImagePreprocessing() { Owner = System.Windows.Application.Current.MainWindow };
            if (inputImagePreprocessing.ShowDialog() == true)
            {
                switch (inputImagePreprocessing.IdInputImagePreprocessing)
                {
                    case 0:
                        Threshold threshold = new Threshold()
                        {
                            Name = "Threshold"
                        };
                        ImagePreprocessingList.Add(threshold);
                        break;
                    case 1:
                        MessageBox.Show("Blur");
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }
            }
        }
    }
}
