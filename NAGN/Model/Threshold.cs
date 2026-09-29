using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NAGN.Model
{
    public class Threshold : ImagePreprocessParent
    {
        private double _min;
        private double _max = 255;

        public double Min
        {
            get => _min;
            set
            {
                _min = Math.Clamp(value, 0, 255);
                OnPropertyChanged();
            }
        }
        public double Max
        {
            get => _max;
            set
            {
                _max = Math.Clamp(value, 0, 255);
                OnPropertyChanged();
            }
        }
        public override void UpdateImageAndSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage? bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            if (bitmap == null) return;
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Threshold(bitmap, Min, Max);
            ImageIntermediate = bitmapSource;
            Event.SendImage(bitmapSource);
        }
        public override void UpdateImageNotSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage? bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            if (bitmap == null) return;
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Threshold(bitmap, Min, Max);
            ImageIntermediate = bitmapSource;
        }
        public override void UpdateImage()
        {
            if (string.IsNullOrEmpty(this.ImageOld))
            {
                return;
            }
            if (ImageIntermediate != null)
            {
                this.ImageNew = NAGN_CV.nagnCV.BitmapToString(ImageIntermediate);
            }
        }
    }
}
