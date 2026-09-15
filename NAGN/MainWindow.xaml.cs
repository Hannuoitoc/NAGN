using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace NAGN
{
    
    public partial class MainWindow : Window
    {
        public Model.Program program_new { get; set; }
        public Model.Program program_old { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click_Add_Node(object sender, RoutedEventArgs e)
        {
            treeViewProgram.NewNode();
            
        }
        private void Button_Click_Remove_Node(object sender, RoutedEventArgs e)
        {
            treeViewProgram.RemoveNode(program_new);
        }

        private void Button_Click_New_Program(object sender, RoutedEventArgs e)
        {
            InputNameProgramDialog inputNameProgramDialog = new InputNameProgramDialog() { Owner = this };
            if(inputNameProgramDialog.ShowDialog() == true)
            {
                program_old = null;
                program_new = Model.Program.newProgram(inputNameProgramDialog.ProgramName);
                treeViewProgram.updateTreeView(program_new);
            }
        }

        private void Button_Click_Open_Program(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = @"C:\Downloads",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Mở model"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                program_new = Services.JsonService.LoadFromJson(openFileDialog.FileName);
                string jsonCopy = System.Text.Json.JsonSerializer.Serialize(program_new);
                program_old = System.Text.Json.JsonSerializer.Deserialize<Model.Program>(jsonCopy);
                treeViewProgram.updateTreeView(program_new);
            }
        }

        private void Button_Click_Save_Program(object sender, RoutedEventArgs e)
        {
            if(program_old == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    InitialDirectory = @"C:\Downloads",
                    FileName = $"{program_new.Name.Trim()}.json",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    Title = "Lưu model"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    program_new.FilePath = saveFileDialog.FileName;
                    Services.JsonService.SaveToJson(program_new, program_new.FilePath); 
                }
            }
            else
            {
                Services.JsonService.SaveToJson(program_new, program_new.FilePath);;
            }
            program_new = Services.JsonService.LoadFromJson(program_new.FilePath);
            string jsonCopy = System.Text.Json.JsonSerializer.Serialize(program_new);
            program_old = System.Text.Json.JsonSerializer.Deserialize<Model.Program>(jsonCopy);
        }
    }
}
