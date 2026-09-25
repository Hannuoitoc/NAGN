using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NAGN.Model
{
    public class Blur : ImagePreprocessParent
    {
        private int _ksize { get; set; }
        public int Ksize { get=>_ksize; set
            {
                _ksize = value;
                OnPropertyChanged();
            } }
        
        public override void UpdateImageAndSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Blur(bitmap, Ksize);
            ImageIntermediate = bitmapSource;
            Event.SendImage(bitmapSource);
        }
        public override void UpdateImageNotSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Blur(bitmap, Ksize);
            ImageIntermediate = bitmapSource;
        }
        public override void UpdateImage()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            this.ImageNew = NAGN_CV.nagnCV.BitmapToString(ImageIntermediate);
            ImageIntermediate = null;
        }
    }
}
