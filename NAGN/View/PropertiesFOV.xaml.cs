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

namespace NAGN.View
{
    /// <summary>
    /// Interaction logic for PropertiesFOV.xaml
    /// </summary>
    public partial class PropertiesFOV : UserControl
    {
        public PropertiesFOV()
        {
            InitializeComponent();
        }
        private void Button_Click_Add_Output_Image(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is Model.FOV fov)
            {
                fov.newOutputImage();
            }
        }
    }
}
