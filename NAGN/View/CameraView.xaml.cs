using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        public Model.FOV FOV { get; set; }
        public CameraView()
        {
            InitializeComponent();
            Event.OnImageProcessed += Event_OnImageProcessed;
        }

        private void Event_OnImageProcessed(BitmapSource image)
        {
            ImageScreen.Source = image;
        }

        private void Click_Open_Image(object sender, RoutedEventArgs e)
        {
            if (FOV != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    InitialDirectory = @"C:\Users\AT\Documents\HanNuoiToc\NAGN\NAGN\ImageTrain\",
                    Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files (*.*)|*.*",
                    Title = "Image open"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    byte[] imageBytes = File.ReadAllBytes(filePath);
                    string base64String = Convert.ToBase64String(imageBytes);
                    BitmapImage bitmap = new BitmapImage();

                    using (MemoryStream stream = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    ImageScreen.Source = bitmap;
                    if(FOV.ImageFilePath != null)
                    {

                        FOV.updateOutputImageslist(filePath);
                    }
                        
                    FOV.ImageFilePath = filePath;
                }
            }
            else
            {
                MessageBox.Show("Xin vui lòng chọn FOV!","Thông báo!");
            }
            
        }
        public void SelectedImageFromFOV(Model.FOV fov=null)
        {
            if (fov == null)
            {
                ImageScreen.Source= null;
                return;
            }
            if (fov.ImageFilePath != null)
            {
                byte[] imageBytes = File.ReadAllBytes(fov.ImageFilePath);
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                ImageScreen.Source = bitmap;
            }
            else
            {
                ImageScreen.Source=null;
            }
        }
    }
}
