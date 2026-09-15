using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAGN.Model
{
    public class FOV
    {
        private string _name;
        public string Name { get => _name; set { _name = value; } }
        public string ImageBit { get; set; }
        public ObservableCollection<Algorithms> Algorithmslist { get; set; } = new ObservableCollection<Algorithms>();
    }
}
