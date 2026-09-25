using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NAGN.Model
{

    public class Threshold : ImagePreprocessParent
    {
        private double _min { get; set; }
        private double _max { get; set; }

        public double Min
        {
            get => _min;
            set
            {
                _min = value;
                OnPropertyChanged();
            }
        }
        public double Max
        {
            get => _max;
            set
            {
                _max = value;
                OnPropertyChanged();
            }
        }
        public override void UpdateImageAndSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Threshold(bitmap, Min, Max);
            ImageIntermediate = bitmapSource;
            Event.SendImage(bitmapSource);
        }
        public override void UpdateImageNotSend()
        {
            if (string.IsNullOrEmpty(this.ImageOld)) return;
            BitmapImage bitmap = NAGN_CV.nagnCV.StringToBitmap(this.ImageOld);
            BitmapSource bitmapSource = NAGN_CV.nagnCV.Threshold(bitmap, Min, Max);
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
