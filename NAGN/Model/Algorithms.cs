using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NAGN.Model
{
    public class Algorithms : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name { get; set; }
        public int IdFOV { get; set; }
        public string Name {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void removeAlgorithm(Model.Program program, Model.Algorithms algorithm)
        {
            program.FOVlist[IdFOV].Algorithmslist.Remove(algorithm);
        }
    }
}