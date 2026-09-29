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
        private string? _name;
        public int IdFOV { get; set; }
        public string? Name {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public System.Collections.ObjectModel.ObservableCollection<ROI> ROIlist { get; set; } = new System.Collections.ObjectModel.ObservableCollection<ROI>();

        private ROI? _roi;
        public ROI? Roi
        {
            get => _roi;
            set { _roi = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void setROI(double x, double y, double width, double height, string type = "Rectangle")
        {
            if (Roi is RectangleROI rectRoi)
            {
                rectRoi.X = x;
                rectRoi.Y = y;
                rectRoi.Width = width;
                rectRoi.Height = height;
            }
            else
            {
                rectRoi = new RectangleROI(x, y, width, height, "ROI " + (ROIlist.Count + 1))
                {
                    Id = ROIlist.Count + 1
                };
                Roi = rectRoi;
                if (!ROIlist.Contains(rectRoi))
                {
                    ROIlist.Add(rectRoi);
                }
            }
            OnPropertyChanged(nameof(Roi));
        }

        public void setPolygonROI(System.Collections.Generic.IEnumerable<System.Windows.Point> points, string? name = null)
        {
            var polyRoi = new PolygonROI
            {
                Id = ROIlist.Count + 1,
                Name = name ?? ("Polygon ROI " + (ROIlist.Count + 1))
            };
            foreach (var pt in points)
            {
                polyRoi.AddPoint(pt);
            }
            Roi = polyRoi;
            if (!ROIlist.Contains(polyRoi))
            {
                ROIlist.Add(polyRoi);
            }
            OnPropertyChanged(nameof(Roi));
        }

        public void removeAlgorithm(Model.Program? program, Model.Algorithms algorithm)
        {
            if (program == null || algorithm == null) return;

            // Tìm FOV chứa Algorithm này an toàn thay vì truy cập bằng index IdFOV
            var parentFov = program.FOVlist.FirstOrDefault(f => f.Algorithmslist.Contains(algorithm))
                         ?? program.FOVlist.FirstOrDefault(f => f.Id == IdFOV);

            parentFov?.Algorithmslist.Remove(algorithm);
        }
    }
}