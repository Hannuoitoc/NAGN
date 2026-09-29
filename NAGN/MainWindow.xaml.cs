using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
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

namespace NAGN
{
    
    public partial class MainWindow : Window
    {
        public Model.Program? program_new { get; set; }
        public Model.Program? program_old { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Event.OnImagePreprocessSetting += Event_OnImagePreprocessSetting;
        }

        private void Event_OnImagePreprocessSetting(Model.ImagePreprocessParent imagePreprocess)
        {
            ContentControlSettingAlgorithms.Content = imagePreprocess;
        }

        private void Button_Click_Add_Node(object sender, RoutedEventArgs e)
        {
            treeViewProgram.NewNode();
        }

        private void Button_Click_Remove_Node(object sender, RoutedEventArgs e)
        {
            if (program_new != null)
            {
                treeViewProgram.RemoveNode(program_new);
            }
        }

        private void Button_Click_New_Program(object sender, RoutedEventArgs e)
        {
            IsSave();
            InputNameProgramDialog inputNameProgramDialog = new InputNameProgramDialog() { Owner = this };
            if (inputNameProgramDialog.ShowDialog() == true)
            {
                program_old = null;
                program_new = Model.Program.newProgram(inputNameProgramDialog.ProgramName);
                treeViewProgram.updateTreeView(program_new);
            }
        }

        private void Button_Click_Open_Program(object sender, RoutedEventArgs e)
        {
            IsSave();
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Mở model"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // 1. Load chương trình mới
                var loaded = Services.JsonService.LoadFromJson(openFileDialog.FileName);
                if (loaded != null)
                {
                    program_new = loaded;
                    program_new.FilePath = openFileDialog.FileName;

                    // 2. Clone sang program_old bằng Newtonsoft.Json
                    var settings = new Newtonsoft.Json.JsonSerializerSettings
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
                    };

                    string jsonCopy = Newtonsoft.Json.JsonConvert.SerializeObject(program_new, settings);
                    program_old = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Program>(jsonCopy, settings);

                    // 3. Cập nhật TreeView
                    treeViewProgram.updateTreeView(program_new);
                }
            }
        }

        private void Button_Click_Save_Program(object sender, RoutedEventArgs e)
        {
            if (program_new != null)
            {
                if (string.IsNullOrEmpty(program_new.FilePath) || program_old == null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        FileName = $"{program_new.Name?.Trim() ?? "Program"}.json",
                        Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                        Title = "Lưu model"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        program_new.FilePath = saveFileDialog.FileName;
                    }
                    else
                    {
                        return; // Hủy lưu nếu người dùng bấm Cancel
                    }
                }

                Services.JsonService.SaveToJson(program_new, program_new.FilePath!);

                // Reload lại chương trình
                program_new = Services.JsonService.LoadFromJson(program_new.FilePath!);

                if (program_new != null)
                {
                    var settings = new Newtonsoft.Json.JsonSerializerSettings
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
                    };

                    string jsonCopy = Newtonsoft.Json.JsonConvert.SerializeObject(program_new, settings);
                    program_old = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Program>(jsonCopy, settings);
                }
            }
        }

        private void treeViewProgram_OnSelected_FOV(object sender, Model.FOV? selectedFOV)
        {
            ContentControlProperties.Content = selectedFOV;
            CameraView.FOV = selectedFOV;
            CameraView.Algorithm = null;
            CameraView.DataContext = selectedFOV;
            CameraView.SelectedImageFromFOV(selectedFOV);
        }

        private void treeViewProgram_OnSelected_Algorithm(object sender, Model.Algorithms? algorithms)
        {
            ContentControlProperties.Content = algorithms;
            CameraView.Algorithm = algorithms;

            Model.FOV? parentFov = null;
            if (program_new != null && algorithms != null)
            {
                parentFov = program_new.FOVlist.FirstOrDefault(f => f.Algorithmslist.Contains(algorithms))
                         ?? program_new.FOVlist.FirstOrDefault(f => f.Id == algorithms.IdFOV);
            }

            if (parentFov != null)
            {
                CameraView.FOV = parentFov;
                CameraView.SelectedImageFromFOV(parentFov);
            }
            else
            {
                CameraView.RedrawRoi();
            }
        }

        private void IsSave()
        {
            if (program_new != null)
            {
                string newJson = Newtonsoft.Json.JsonConvert.SerializeObject(program_new, Newtonsoft.Json.Formatting.None);
                string oldJson = program_old != null ? Newtonsoft.Json.JsonConvert.SerializeObject(program_old, Newtonsoft.Json.Formatting.None) : string.Empty;

                if (newJson != oldJson)
                {
                    if (MessageBox.Show("Bạn chưa lưu model hiện tại, bạn có muốn lưu không?", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Button_Click_Save_Program(this, new RoutedEventArgs());
                    }
                    else
                    {
                        program_old = null;
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsSave();
        }

        private void Click_Close_Open(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Name == "btn_Close_Open_Program")
                {
                    bool isSildeBar = Grid_Program.Width.Value > 70;
                    Grid_Program.Width = new GridLength(isSildeBar ? 25 : 200);
                    btn_Close_Open_Program.Content = (Grid_Program.Width.Value > 70) ? "◀" : "▶";
                }
                else if (button.Name == "btn_Close_Open_Properties")
                {
                    bool isSildeBar = Grid_Properties.Width.Value > 70;
                    Grid_Properties.Width = new GridLength(isSildeBar ? 25 : 200);
                    btn_Close_Open_Properties.Content = (Grid_Properties.Width.Value > 70) ? "◀" : "▶";
                }
                else
                {
                    bool isSildeBar = Grid_Setting_Algorithms.Width.Value > 70;
                    Grid_Setting_Algorithms.Width = new GridLength(isSildeBar ? 25 : 200);
                    btn_Close_Open_Setting_Algorithms.Content = (Grid_Setting_Algorithms.Width.Value > 70) ? "◀" : "▶";
                }
            }
        }

        private void Grid_Size_Changed_Program(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 70)
            {
                tbProgram.Visibility=Visibility.Collapsed;
                toolBar.Visibility=Visibility.Collapsed;
                treeViewProgram.Visibility=Visibility.Collapsed;
            }
            else
            {
                tbProgram.Visibility = Visibility.Visible;
                toolBar.Visibility = Visibility.Visible;
                treeViewProgram.Visibility = Visibility.Visible;
            }
        }

        private void Grid_Size_Changed_Properties(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 70)
            {
                tbProperties.Visibility = Visibility.Collapsed;
                stackPanel_PropertiesFOV_PropertiesAlgorithm.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbProperties.Visibility = Visibility.Visible;
                stackPanel_PropertiesFOV_PropertiesAlgorithm.Visibility = Visibility.Visible;
            }
        }
        private void Grid_Size_Changed_Setting_Algorithms(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 70)
            {
                tbSetting_Algorithms.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbSetting_Algorithms.Visibility = Visibility.Visible;
            }
        }
    }
}
