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
    public partial class ThresholdView : UserControl
    {
        public ThresholdView()
        {
            InitializeComponent();
        }
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DataContext is Model.Threshold threshold)
            {
                threshold.UpdateImageAndSend();
                //threshold.SendImage();
            }
        }

        private void ThresHoldSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Event.IsPreprocessingChanged();
        }
    }
}
