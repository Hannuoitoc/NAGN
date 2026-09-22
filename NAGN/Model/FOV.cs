using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NAGN.Model
{
    public class FOV:INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public int IdProgram { get; set; }
        public string ImageFilePath { get; set; }
        public ObservableCollection<Algorithms> Algorithmslist { get; set; } = new ObservableCollection<Algorithms>();
        public ObservableCollection<OutputImage> OutputImageslist { get; set; } = new ObservableCollection<OutputImage>(); 
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RemoveOutputImageCommand { get; }
        public FOV()
        {
            // 2. Nối Command với hàm RemoveOutputImage
            RemoveOutputImageCommand = new RelayCommand<OutputImage>(RemoveOutputImage);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void newAlgorithm()
        {
            var algorithm = new Algorithms()
            {
                Id = Algorithmslist.Count,
                Name = "algorithm" + (Algorithmslist.Count+1),
                IdFOV = Id
            };
            Algorithmslist.Add(algorithm);
        }
        public void removeFOV(Model.Program program, Model.FOV fov)
        {
            program.FOVlist.Remove(fov);
        }
        public void newOutputImage()
        {
            var outputImage = new OutputImage()
            {
                Name = "Output Image " + (OutputImageslist.Count + 1),
                IdFOV = Id
            };
            OutputImageslist.Add(outputImage);
        }
        public void RemoveOutputImage(OutputImage outputImage)
        {
            if (outputImage != null && OutputImageslist.Contains(outputImage))
            {
                OutputImageslist.Remove(outputImage);
            }
        }
    }
}
