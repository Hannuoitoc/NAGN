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
    /// Interaction logic for BlurView.xaml
    /// </summary>
    public partial class BlurView : UserControl
    {
        public BlurView()
        {
            InitializeComponent();
        }

        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DataContext is Model.Blur blur)
            {
                blur.UpdateImageAndSend();
            }
        }
        private void KsizeMinSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (DataContext is Model.Blur blur)
            //{
            //    blur.UpdateImageAndSend();
            //}
            Event.IsPreprocessingChanged();
        }
    }
}
