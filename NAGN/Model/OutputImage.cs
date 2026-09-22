using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NAGN.Model
{
    public class OutputImage:INotifyPropertyChanged
    {
        private string _name { get; set; }
        public string Name
        {
            get => _name;
            set { _name = value;  OnPropertyChanged(); }
        }
        public int IdFOV { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}
