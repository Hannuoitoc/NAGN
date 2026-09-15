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
        public MainWindow()
        {
            InitializeComponent();
            var Algorithms1 = new Model.Algorithms()
            {
                Name = "Algorithms1"
            };
            var Algorithms2 = new Model.Algorithms()
            {
                Name = "Algorithms2"
            };
            var Algorithms3 = new Model.Algorithms()
            {
                Name = "Algorithms2"
            };
            var fov1 = new Model.FOV()
            {
                Name = "fov1",
                Algorithmslist = new ObservableCollection<Model.Algorithms>
                {
                    Algorithms1,Algorithms2,Algorithms3
                }
            };
            var fov2 = new Model.FOV()
            {
                Name = "fov2",
                Algorithmslist = new ObservableCollection<Model.Algorithms>
                {
                    Algorithms1,Algorithms2,Algorithms3
                }
            };
            var fov3 = new Model.FOV()
            {
                Name = "fov3",
                Algorithmslist = new ObservableCollection<Model.Algorithms>
                {
                    Algorithms1,Algorithms2,Algorithms3
                }
            };
            var program = new Model.Program()
            {
                Name = "Program",
                FOVlist = new ObservableCollection<Model.FOV>
                {
                    fov1, fov2, fov3
                }
            };
            treeViewProgram.updateTreeView(program);
        }
    }
}
