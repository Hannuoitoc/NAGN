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

namespace NAGN.UsersControllers
{
    public partial class Program : UserControl
    {
        public Program()
        {
            InitializeComponent();
        }
        public void updateTreeView(Model.Program program)
        {
            TreeViewProgram.Items.Clear();
            if(program != null)
            {

            }
            var nodeProgram = new TreeViewItem
            {
                Header = program.Name,
                Tag = program,
                IsExpanded = true,
            };
            foreach(var fov in program.FOVlist)
            {
                var nodeFov = new TreeViewItem
                {
                    Header = fov.Name,
                    Tag = fov,
                    IsExpanded = true,
                };
                foreach(var algorithms in fov.Algorithmslist)
                {
                    var nodeAlgorithms = new TreeViewItem
                    {
                        Header = algorithms.Name,
                        Tag = algorithms,
                        IsExpanded = true
                    };
                    nodeFov.Items.Add(nodeAlgorithms);
                }
                nodeProgram.Items.Add(nodeFov);
            }
            TreeViewProgram.Items.Add(nodeProgram);
        }
        
    }
}
