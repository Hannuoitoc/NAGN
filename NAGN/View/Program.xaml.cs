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

namespace NAGN.UsersControllers
{
    public partial class Program : UserControl
    {
        public event EventHandler<Model.FOV> OnFOVSelected;
        public Program()
        {
            InitializeComponent();
        }
        public void updateTreeView(Model.Program program)
        {
            if (program == null) return;

            // Chỉ cần gán Nguồn dữ liệu, WPF sẽ tự tạo toàn bộ Cây giao diện!
            TreeViewProgram.ItemsSource = new ObservableCollection<Model.Program> { program };
        }

        public void NewNode()
        {
            if(TreeViewProgram.SelectedItem == null)
            {
                MessageBox.Show("Chưa có đối tượng nào được chọn để thêm!");
                return;
            }
            if(TreeViewProgram.SelectedItem is Model.Program program)
            {
                program.newFOV();
            }
            if(TreeViewProgram.SelectedItem is Model.FOV fov)
            {
                fov.newAlgorithm();
            }
        }
        public void RemoveNode(Model.Program program)
        {
            if (TreeViewProgram.SelectedItem == null)
            {
                MessageBox.Show("Chưa có đối tượng nào được chọn!");
                return;
            }
            if (TreeViewProgram.SelectedItem is Model.FOV fov)
            {
                fov.removeFOV(program, fov);
            }
            if (TreeViewProgram.SelectedItem is Model.Algorithms algorithm)
            {
                algorithm.removeAlgorithm(program,algorithm);
            }
        }

        private void TreeViewProgram_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(TreeViewProgram.SelectedItem is Model.FOV selectedFOV)
            {
                OnFOVSelected?.Invoke(this, selectedFOV);
            }
        }
    }
}
