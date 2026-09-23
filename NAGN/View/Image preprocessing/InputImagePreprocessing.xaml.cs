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
using System.Windows.Shapes;

namespace NAGN.View.Image_preprocessing
{
    /// <summary>
    /// Interaction logic for InputImagePreprocessing.xaml
    /// </summary>
    public partial class InputImagePreprocessing : Window
    {
        public int IdInputImagePreprocessing;
        public InputImagePreprocessing()
        {
            InitializeComponent();
        }


        private void Button_Click_Chosse_Image_Preprocessing(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Name)
            {
                case "Threshold":
                    IdInputImagePreprocessing = 0;
                    break;
                case "Blur":
                    IdInputImagePreprocessing = 1;
                    break;
            }
            this.DialogResult = true;
        }
    }
}
