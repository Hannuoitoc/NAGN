using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
        private string _filePathImage { get; set; }
        public string FilePathImage
        {
            get => _filePathImage;
            set { _filePathImage = value; OnPropertyChanged();}
        }
        public ObservableCollection<ImagePreprocessParent> ImagePreprocessingList { get; set; } = new ObservableCollection<ImagePreprocessParent>();
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
                string image;
                switch (inputImagePreprocessing.IdInputImagePreprocessing)
                {
                    case 0:
                        
                        if(ImagePreprocessingList.Count != 0)
                        {
                            image = ImagePreprocessingList[ImagePreprocessingList.Count-1 ].ImageNew;
                            MessageBox.Show(image);
                        }
                        else
                        {
                            image = ImageToString(FilePathImage);
                        }
                        Threshold threshold = new Threshold()
                        {
                            Name = "Threshold",
                            ImageOld = image,
                            ImageNew = image,
                        };
                        ImagePreprocessingList.Add(threshold);
                        break;
                    case 1:
                        if (ImagePreprocessingList.Count != 0)
                        {
                            image = ImagePreprocessingList[ImagePreprocessingList.Count - 1].ImageNew;
                            MessageBox.Show(image);
                        }
                        else
                        {
                            image = ImageToString(FilePathImage);
                        }
                        Blur blur = new Blur()
                        {
                            Name = "Blur",
                            ImageOld = image,
                            ImageNew = image,
                            Ksize = 1
                        };
                        ImagePreprocessingList.Add(blur);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }
            }
        }
        public string ImageToString(string filePath)
        {
            if(filePath == null)
                return string.Empty;
            byte[] imageBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        public void updateImagePreprocessingList()
        {
            if(ImagePreprocessingList.Count !=0)
                ImagePreprocessingList[0].ImageOld = ImageToString(FilePathImage);
            string image=null;
            foreach (var imagePreprocessing in ImagePreprocessingList)
            {
                if (imagePreprocessing != ImagePreprocessingList[0])
                {
                    imagePreprocessing.ImageOld = image;
                } 
                imagePreprocessing.UpdateImageNotSend();
                imagePreprocessing.UpdateImage();
                image = imagePreprocessing.ImageNew;
            };
        }
    }
}
