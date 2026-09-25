using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NAGN.NAGN_CV
{
    public static class nagnCV
    {
        public static BitmapSource Threshold(BitmapImage image, double min, double max)
        {
            using (Mat imageMat = image.ToMat())
            using (Mat grayImage = new Mat())
            using (Mat thresholdImage = new Mat())
            {
                if (imageMat.Channels() == 1)
                imageMat.CopyTo(grayImage);
                else
                {
                    ColorConversionCodes colorConversionCodes = (imageMat.Channels() == 3) ? ColorConversionCodes.RGB2GRAY : ColorConversionCodes.RGBA2GRAY;
                    Cv2.CvtColor(imageMat, grayImage, colorConversionCodes);
                }
                Cv2.InRange(grayImage, new Scalar(min), new Scalar(max), thresholdImage);
                BitmapSource bs = thresholdImage.ToBitmapSource();
                bs.Freeze();
                return bs;
            }
        }
        public static BitmapSource Blur(BitmapImage image,int ksize)
        {
            using (Mat imageMat = image.ToMat())
            using (Mat blurImage = new Mat())
            {
                Cv2.MedianBlur(imageMat, blurImage, ksize);
                BitmapSource bs = blurImage.ToBitmapSource(); ;
                bs.Freeze();
                return bs;
            }
        }
        public static BitmapImage StringToBitmap(string image)
        {
            byte[] imageBytes = Convert.FromBase64String(image);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }
        public static string BitmapToString(BitmapSource image)
        {
            if (image == null) return string.Empty;

            byte[] byteArray;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                byteArray = ms.ToArray();
            }
            return Convert.ToBase64String(byteArray);
        }
    }
}
