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
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace NAGN.Model
{
    public class OutputImage : INotifyPropertyChanged
    {
        private string? _name;
        public string? Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        private string? _filePathImage;
        public string? FilePathImage
        {
            get => _filePathImage;
            set { _filePathImage = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ImagePreprocessParent> ImagePreprocessingList { get; set; } = new ObservableCollection<ImagePreprocessParent>();
        public int IdFOV { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [JsonIgnore]
        public ICommand AddImagePreprocessingCommand { get; }

        [JsonIgnore]
        public ICommand SendImageOutputCommand { get; }

        [JsonIgnore]
        public ICommand RemoveImagePreprocessingCommand { get; }

        public OutputImage()
        {
            AddImagePreprocessingCommand = new RelayCommand(AddImagePreprocessing);
            SendImageOutputCommand = new RelayCommand<OutputImage>(SendImageOutput);
            RemoveImagePreprocessingCommand = new RelayCommand<ImagePreprocessParent>(RemoveImagePreprocessing);
        }

        private void RemoveImagePreprocessing(ImagePreprocessParent? imagePreprocessParent)
        {
            if (imagePreprocessParent != null && ImagePreprocessingList.Contains(imagePreprocessParent))
            {
                ImagePreprocessingList.Remove(imagePreprocessParent);
            }
            updateImagePreprocessingList();

            if (ImagePreprocessingList.Count > 0)
            {
                var last = ImagePreprocessingList[ImagePreprocessingList.Count - 1];
                if (!string.IsNullOrEmpty(last.ImageNew))
                {
                    var bmp = NAGN_CV.nagnCV.StringToBitmap(last.ImageNew);
                    if (bmp != null) Event.SendImage(bmp);
                    return;
                }
            }

            string base64 = ImageToString(FilePathImage);
            if (!string.IsNullOrEmpty(base64))
            {
                var bmp = NAGN_CV.nagnCV.StringToBitmap(base64);
                if (bmp != null) Event.SendImage(bmp);
            }
        }

        private void SendImageOutput(OutputImage? outputimage)
        {
            if (outputimage == null) return;
            BitmapSource? imageSource = null;
            if (outputimage.ImagePreprocessingList.Count > 0)
            {
                var last = outputimage.ImagePreprocessingList[outputimage.ImagePreprocessingList.Count - 1];
                if (!string.IsNullOrEmpty(last.ImageNew))
                {
                    imageSource = NAGN_CV.nagnCV.StringToBitmap(last.ImageNew);
                }
            }
            if (imageSource == null && !string.IsNullOrEmpty(outputimage.FilePathImage))
            {
                string base64 = ImageToString(outputimage.FilePathImage);
                if (!string.IsNullOrEmpty(base64))
                {
                    imageSource = NAGN_CV.nagnCV.StringToBitmap(base64);
                }
            }
            if (imageSource != null)
            {
                Event.SendImage(imageSource);
            }
        }

        public void AddImagePreprocessing()
        {
            View.Image_preprocessing.InputImagePreprocessing inputImagePreprocessing = new View.Image_preprocessing.InputImagePreprocessing() 
            { 
                Owner = System.Windows.Application.Current?.MainWindow 
            };
            if (inputImagePreprocessing.ShowDialog() == true)
            {
                string? image;
                if (ImagePreprocessingList.Count != 0)
                {
                    image = ImagePreprocessingList[ImagePreprocessingList.Count - 1].ImageNew;
                }
                else
                {
                    image = ImageToString(FilePathImage);
                }

                if (string.IsNullOrEmpty(image))
                {
                    MessageBox.Show("Chưa có ảnh đầu vào hợp lệ hoặc không tìm thấy file ảnh!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                switch (inputImagePreprocessing.IdInputImagePreprocessing)
                {
                    case 0:
                        Threshold threshold = new Threshold()
                        {
                            Name = "Threshold " + (ImagePreprocessingList.Count + 1),
                            ImageOld = image,
                            ImageNew = image,
                            Min = 0,
                            Max = 255
                        };
                        threshold.UpdateImageNotSend();
                        threshold.UpdateImage();
                        ImagePreprocessingList.Add(threshold);
                        threshold.UpdateImageAndSend();
                        break;

                    case 1:
                        Blur blur = new Blur()
                        {
                            Name = "Blur " + (ImagePreprocessingList.Count + 1),
                            ImageOld = image,
                            ImageNew = image,
                            Ksize = 3 // Mặc định ksize = 3 an toàn với OpenCV
                        };
                        blur.UpdateImageNotSend();
                        blur.UpdateImage();
                        ImagePreprocessingList.Add(blur);
                        blur.UpdateImageAndSend();
                        break;
                }
            }
        }

        public string ImageToString(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return string.Empty;

            try
            {
                byte[] imageBytes = File.ReadAllBytes(filePath);
                return Convert.ToBase64String(imageBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        public void updateImagePreprocessingList()
        {
            if (string.IsNullOrEmpty(FilePathImage) || !File.Exists(FilePathImage))
                return;

            string base64 = ImageToString(FilePathImage);
            if (string.IsNullOrEmpty(base64)) return;

            if (ImagePreprocessingList.Count != 0)
            {
                ImagePreprocessingList[0].ImageOld = base64;
            }

            string? currentImage = base64;
            foreach (var imagePreprocessing in ImagePreprocessingList)
            {
                imagePreprocessing.ImageOld = currentImage;
                imagePreprocessing.UpdateImageNotSend();
                imagePreprocessing.UpdateImage();
                currentImage = imagePreprocessing.ImageNew;
            }
        }
    }
}
