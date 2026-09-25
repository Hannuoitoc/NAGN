using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NAGN.Model
{
    public abstract class ImagePreprocessParent : INotifyPropertyChanged
    {
        private string _name { get; set; }
        private string _imageOld { get; set; }
        private string _imageNew { get; set; }
        [JsonIgnore]
        public BitmapSource ImageIntermediate { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string ImageOld
        {
            get => _imageOld;
            set
            {
                _imageOld = value;
                OnPropertyChanged();
            }
        }
        public string ImageNew
        {
            get => _imageNew;
            set
            {
                _imageNew = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //public void SendImage()
        //{
        //    if (string.IsNullOrEmpty(this.ImageNew)) return;
        //    BitmapImage bitmapImage = NAGN_CV.nagnCV.StringToBitmap(this.ImageNew);
        //    Event.SendImage(bitmapImage);
        //}
        public abstract void UpdateImageAndSend();
        public abstract void UpdateImageNotSend();
        public abstract void UpdateImage();
        public ICommand OutputImageCommand { get; }
        public ImagePreprocessParent()
        {
            OutputImageCommand = new RelayCommand(UpdateImage);
        }
        
    }
}
