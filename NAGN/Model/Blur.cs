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
        private int _ksize = 3;
        public int Ksize
        {
            get => _ksize;
            set
            {
                int val = value < 3 ? 3 : value;
                if (val % 2 == 0) val += 1;
                _ksize = val;
                OnPropertyChanged();
            }
        }
        
        public override void UpdateImageAndSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage? bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            if (bitmap == null) return;
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Blur(bitmap, Ksize);
            ImageIntermediate = bitmapSource;
            Event.SendImage(bitmapSource);
        }
        public override void UpdateImageNotSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage? bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            if (bitmap == null) return;
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Blur(bitmap, Ksize);
            ImageIntermediate = bitmapSource;
        }
        public override void UpdateImage()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            if (ImageIntermediate != null)
            {
                this.ImageNew = NAGN_CV.nagnCV.BitmapToString(ImageIntermediate);
                ImageIntermediate = null;
            }
        }
    }
}
