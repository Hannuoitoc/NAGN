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
    public class Program : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name { get; set; }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string FilePath { get; set; }
        public ObservableCollection<FOV> FOVlist { get; set; } = new ObservableCollection<FOV>();
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public static Model.Program newProgram(string name)
        {
            var model = new Model.Program()
            {
                Name = name,
            };
            return model;
        }
        public void newFOV()
        {
            var fov = new FOV()
            {
                Id = FOVlist.Count,
                Name = "FOV " + (FOVlist.Count + 1),
                IdProgram = Id
            };
            FOVlist.Add(fov);
        }
        
    }
}
