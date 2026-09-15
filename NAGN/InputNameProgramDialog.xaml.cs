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

namespace NAGN
{
    /// <summary>
    /// Interaction logic for InputNameProgramDialog.xaml
    /// </summary>
    public partial class InputNameProgramDialog : Window
    {
        public string ProgramName { get; set; }
        public InputNameProgramDialog()
        {
            InitializeComponent();
            txtInputNameModel.Focus();
        }

        private void butHuy_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void butTao_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInputNameModel.Text))
            {
                MessageBox.Show("Không được đặt tên trống.","Cảnh báo", MessageBoxButton.OK);
                return;
            }
            ProgramName = txtInputNameModel.Text.Trim();
            this.DialogResult = true;
        }
    }
}
