using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NAGN.Model
{
    public abstract class ROI : INotifyPropertyChanged
    {
        private int _id;
        private string? _name = "ROI";
        private string _type = "ROI";

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string? Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public virtual string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        public virtual double X { get; set; }
        public virtual double Y { get; set; }
        public virtual double Width { get; set; }
        public virtual double Height { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
