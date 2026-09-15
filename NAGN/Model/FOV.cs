using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NAGN.Model
{
    public class FOV:INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public int IdProgram { get; set; }
        public string ImageBit { get; set; }
        public ObservableCollection<Algorithms> Algorithmslist { get; set; } = new ObservableCollection<Algorithms>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void newAlgorithm()
        {
            var algorithm = new Algorithms()
            {
                Id = Algorithmslist.Count,
                Name = "algorithm" + (Algorithmslist.Count+1),
                IdFOV = Id
            };
            Algorithmslist.Add(algorithm);
        }
        public void removeFOV(Model.Program program, Model.FOV fov)
        {
            program.FOVlist.Remove(fov);
        }
    }
}
