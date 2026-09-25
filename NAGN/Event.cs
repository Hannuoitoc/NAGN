using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NAGN
{
    public static class Event
    {
        public static event Action OnPreprecessingChanged;
        public static event Action<Model.ImagePreprocessParent> OnImagePreprocessSetting;
        public static event Action<BitmapSource> OnImageProcessed;
        public static void SendImage(BitmapSource image)
        {
            OnImageProcessed?.Invoke(image);
        }
        public static void SendImagePreprocess(Model.ImagePreprocessParent imagePreprocess)
        {
            OnImagePreprocessSetting?.Invoke(imagePreprocess);
        }
        public static void IsPreprocessingChanged( )
        {
            OnPreprecessingChanged?.Invoke();
        }
    }
}
