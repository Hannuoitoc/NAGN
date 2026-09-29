using System;

namespace NAGN.Model
{
    public class RectangleROI : ROI
    {
        private double _x;
        private double _y;
        private double _width;
        private double _height;

        public override string Type
        {
            get => "Rectangle";
            set { }
        }

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

        public RectangleROI()
        {
            Name = "Rectangle ROI";
        }

        public RectangleROI(double x, double y, double width, double height, string? name = null)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            Name = name ?? "Rectangle ROI";
        }
    }

    public class RetangleROI : RectangleROI { }
    public class RectangleRoi : RectangleROI { }
}
