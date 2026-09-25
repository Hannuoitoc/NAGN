using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NAGN.View.Image_preprocessing
{
    /// <summary>
    /// Interaction logic for ListImagePreprocessing.xaml
    /// </summary>
    public partial class ListImagePreprocessing : UserControl
    {
        public ListImagePreprocessing()
        {
            InitializeComponent();
            
        }

        

        private void ImagePreprocessing_OnImagePreprocessSetting(object sender, Model.ImagePreprocessParent e)
        {
            if(e is Model.Threshold threshold)
            {
                Event.SendImagePreprocess(threshold);
            }else if(e is Model.Blur blur)
            {
                Event.SendImagePreprocess(blur);
            }
        }
    }
}
