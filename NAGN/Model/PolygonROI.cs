using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace NAGN.Model
{
    public class PolygonROI : ROI
    {
        private ObservableCollection<Point> _points = new ObservableCollection<Point>();

        public override string Type
        {
            get => "Polygon";
            set { }
        }

        public ObservableCollection<Point> Points
        {
            get => _points;
            set
            {
                if (_points != null)
                {
                    _points.CollectionChanged -= Points_CollectionChanged;
                }
                _points = value;
                if (_points != null)
                {
                    _points.CollectionChanged += Points_CollectionChanged;
                }
                OnPropertyChanged();
                UpdateBoundingBox();
            }
        }

        private double _x;
        private double _y;
        private double _width;
        private double _height;

        public override double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        public override double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }

        public override double Width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(); }
        }

        public override double Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(); }
        }

        public PolygonROI()
        {
            Name = "Polygon ROI";
            Points.CollectionChanged += Points_CollectionChanged;
        }

        private void Points_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateBoundingBox();
        }

        public void AddPoint(Point point)
        {
            Points.Add(point);
        }

        public void AddPoint(double x, double y)
        {
            Points.Add(new Point(x, y));
        }

        public void ClearPoints()
        {
            Points.Clear();
        }

        public void UpdateBoundingBox()
        {
            if (Points == null || Points.Count == 0)
            {
                X = 0;
                Y = 0;
                Width = 0;
                Height = 0;
                return;
            }

            double minX = Points.Min(p => p.X);
            double minY = Points.Min(p => p.Y);
            double maxX = Points.Max(p => p.X);
            double maxY = Points.Max(p => p.Y);

            X = Math.Round(minX, 1);
            Y = Math.Round(minY, 1);
            Width = Math.Round(maxX - minX, 1);
            Height = Math.Round(maxY - minY, 1);
        }
    }

    // Alias hỗ trợ cách gọi tắt: Poly, PolyROI, PolygonRoi
    public class PolyROI : PolygonROI { }
    public class PolygonRoi : PolygonROI { }
    public class PolyRoi : PolygonROI { }
}
