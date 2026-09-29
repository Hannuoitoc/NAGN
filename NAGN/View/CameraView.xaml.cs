using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NAGN.Model;

namespace NAGN.View
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// Hỗ trợ Zoom, Pan, Resize đồng bộ 100% giữa Ảnh và ROI
    /// </summary>
    public partial class CameraView : UserControl
    {
        public Model.FOV? FOV { get; set; }
        public Model.Algorithms? Algorithm { get; set; }

        private string? _currentLoadedFilePath;

        // Trạng thái vẽ ROI
        private bool _isDrawingRoiMode = false;
        private bool _isDrawingPolyMode = false;
        private bool _isDragging = false;
        private Point _startPoint;
        private Rectangle? _activeRectShape;
        private Border? _activeLabelBorder;
        private TextBlock? _activeLabelText;
        private readonly List<Point> _polygonPoints = new List<Point>();

        // Trạng thái Zoom & Pan
        private bool _isFitMode = true;
        private double _currentZoom = 1.0;
        private bool _isPanning = false;
        private Point _panStartMousePoint;
        private Point _panStartTranslate;

        public CameraView()
        {
            InitializeComponent();
            Event.OnImageProcessed += Event_OnImageProcessed;
        }

        private void Event_OnImageProcessed(BitmapSource image)
        {
            SetBitmapSource(image);
        }

        #region Nạp và hiển thị Ảnh

        private void Click_Open_Image(object sender, RoutedEventArgs e)
        {
            if (FOV != null)
            {
                string candidateDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageTrain");
                if (!Directory.Exists(candidateDir))
                {
                    candidateDir = AppDomain.CurrentDomain.BaseDirectory;
                }

                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    InitialDirectory = candidateDir,
                    Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files (*.*)|*.*",
                    Title = "Chọn ảnh đầu vào"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    if (!File.Exists(filePath)) return;

                    try
                    {
                        byte[] imageBytes = File.ReadAllBytes(filePath);
                        BitmapImage bitmap = new BitmapImage();

                        using (MemoryStream stream = new MemoryStream(imageBytes))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }

                        FOV.ImageFilePath = filePath;
                        FOV.updateOutputImageslist(filePath);
                        _currentLoadedFilePath = filePath;
                        SetBitmapSource(bitmap);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể mở file ảnh: {ex.Message}", "Lỗi đọc ảnh", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Xin vui lòng chọn FOV trước khi nạp ảnh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void SelectedImageFromFOV(Model.FOV? fov = null)
        {
            if (fov == null)
            {
                SetBitmapSource(null);
                return;
            }

            // Nếu ảnh này đã được nạp sẵn, chỉ cần vẽ lại ROI (giữ nguyên vị trí zoom/pan người dùng đang thao tác)
            if (!string.IsNullOrEmpty(fov.ImageFilePath)
                && string.Equals(_currentLoadedFilePath, fov.ImageFilePath, StringComparison.OrdinalIgnoreCase)
                && ImageScreen.Source != null)
            {
                RedrawRoi();
                return;
            }

            if (!string.IsNullOrEmpty(fov.ImageFilePath) && File.Exists(fov.ImageFilePath))
            {
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(fov.ImageFilePath);
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    _currentLoadedFilePath = fov.ImageFilePath;
                    SetBitmapSource(bitmap);
                }
                catch
                {
                    SetBitmapSource(null);
                }
            }
            else
            {
                SetBitmapSource(null);
            }
        }

        private void SetBitmapSource(BitmapSource? bitmap)
        {
            if (bitmap == null)
            {
                ImageScreen.Source = null;
                ImageScreen.Width = 0;
                ImageScreen.Height = 0;
                CanvasRoi.Width = 0;
                CanvasRoi.Height = 0;
                ViewportCanvas.Width = 0;
                ViewportCanvas.Height = 0;
                CanvasRoi.Children.Clear();
                _currentZoom = 1.0;
                UpdateZoomText();
                return;
            }

            ImageScreen.Source = bitmap;
            ImageScreen.Width = bitmap.PixelWidth;
            ImageScreen.Height = bitmap.PixelHeight;
            CanvasRoi.Width = bitmap.PixelWidth;
            CanvasRoi.Height = bitmap.PixelHeight;
            ViewportCanvas.Width = bitmap.PixelWidth;
            ViewportCanvas.Height = bitmap.PixelHeight;

            if (GridImageContainer.ActualWidth > 0 && GridImageContainer.ActualHeight > 0)
            {
                FitImageToView();
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    FitImageToView();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        #endregion

        #region Cơ chế Zoom và Pan (Phóng to / Thu nhỏ & Di chuyển)

        public void FitImageToView()
        {
            if (ImageScreen.Source is not BitmapSource bitmap) return;

            double containerW = GridImageContainer.ActualWidth;
            double containerH = GridImageContainer.ActualHeight;

            if (containerW <= 0 || containerH <= 0) return;
            if (bitmap.PixelWidth <= 0 || bitmap.PixelHeight <= 0) return;

            // Tính tỉ lệ fit sao cho toàn bộ ảnh nằm trọn trong khung nhìn
            double scaleX = (containerW - 8) / bitmap.PixelWidth;
            double scaleY = (containerH - 8) / bitmap.PixelHeight;
            double fitScale = Math.Min(scaleX, scaleY);
            if (fitScale <= 0) fitScale = 0.1;

            _currentZoom = fitScale;
            _isFitMode = true;

            ViewportScale.ScaleX = fitScale;
            ViewportScale.ScaleY = fitScale;

            // Căn giữa ảnh trong container
            ViewportTranslate.X = (containerW - (bitmap.PixelWidth * fitScale)) / 2.0;
            ViewportTranslate.Y = (containerH - (bitmap.PixelHeight * fitScale)) / 2.0;

            UpdateZoomText();
            RedrawRoi();
        }

        private void ZoomAtPoint(Point centerPoint, double zoomFactor)
        {
            if (ImageScreen.Source is not BitmapSource bitmap) return;

            double newZoom = _currentZoom * zoomFactor;
            // Giới hạn zoom từ 5% đến 5000% (50x)
            newZoom = Math.Clamp(newZoom, 0.05, 50.0);

            double effectiveFactor = newZoom / _currentZoom;
            _currentZoom = newZoom;
            _isFitMode = false;

            // Thuật toán Zoom chuẩn: giữ nguyên tọa độ pixel dưới vị trí con trỏ chuột
            ViewportTranslate.X = centerPoint.X - ((centerPoint.X - ViewportTranslate.X) * effectiveFactor);
            ViewportTranslate.Y = centerPoint.Y - ((centerPoint.Y - ViewportTranslate.Y) * effectiveFactor);

            ViewportScale.ScaleX = _currentZoom;
            ViewportScale.ScaleY = _currentZoom;

            UpdateZoomText();
            RedrawRoi();
        }

        private void UpdateZoomText()
        {
            if (TxtZoomLevel != null)
            {
                TxtZoomLevel.Text = $"{(int)Math.Round(_currentZoom * 100)}%";
            }
        }

        private void GridImageContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Khi kích thước CameraView thay đổi (kéo GridSplitter, phóng to thu nhỏ cửa sổ MainWindow):
            if (_isFitMode)
            {
                FitImageToView();
            }
            else
            {
                RedrawRoi();
            }
        }

        private void GridImageContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ImageScreen.Source == null) return;

            Point mousePos = e.GetPosition(GridImageContainer);
            double zoomFactor = e.Delta > 0 ? 1.2 : 0.833333333333; // 1.2x hoặc 1/1.2x
            ZoomAtPoint(mousePos, zoomFactor);
            e.Handled = true;
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomAtPoint(new Point(GridImageContainer.ActualWidth / 2.0, GridImageContainer.ActualHeight / 2.0), 1.25);
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomAtPoint(new Point(GridImageContainer.ActualWidth / 2.0, GridImageContainer.ActualHeight / 2.0), 0.8);
        }

        private void BtnZoomActual_Click(object sender, RoutedEventArgs e)
        {
            if (ImageScreen.Source is not BitmapSource bitmap) return;
            _currentZoom = 1.0;
            _isFitMode = false;
            ViewportScale.ScaleX = 1.0;
            ViewportScale.ScaleY = 1.0;
            ViewportTranslate.X = (GridImageContainer.ActualWidth - bitmap.PixelWidth) / 2.0;
            ViewportTranslate.Y = (GridImageContainer.ActualHeight - bitmap.PixelHeight) / 2.0;
            UpdateZoomText();
            RedrawRoi();
        }

        private void BtnZoomFit_Click(object sender, RoutedEventArgs e)
        {
            FitImageToView();
        }

        private void StartPanning(MouseButtonEventArgs e)
        {
            _isPanning = true;
            _panStartMousePoint = e.GetPosition(GridImageContainer);
            _panStartTranslate = new Point(ViewportTranslate.X, ViewportTranslate.Y);
            GridImageContainer.CaptureMouse();
            Cursor = Cursors.SizeAll;
        }

        private void ContinuePanning(MouseEventArgs e)
        {
            Point currentMousePoint = e.GetPosition(GridImageContainer);
            Vector delta = currentMousePoint - _panStartMousePoint;
            ViewportTranslate.X = _panStartTranslate.X + delta.X;
            ViewportTranslate.Y = _panStartTranslate.Y + delta.Y;
        }

        private void EndPanning()
        {
            _isPanning = false;
            GridImageContainer.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
            UpdateButtonState();
        }

        private void GridImageContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageScreen.Source == null) return;

            // Cho phép Pan khi bấm chuột giữa hoặc chuột trái (nếu không ở chế độ vẽ ROI)
            if (!_isDrawingRoiMode && !_isDrawingPolyMode)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
                {
                    StartPanning(e);
                }
            }
        }

        private void GridImageContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                ContinuePanning(e);
            }
        }

        private void GridImageContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                EndPanning();
            }
        }

        #endregion

        #region Kích hoạt chế độ vẽ ROI

        private bool EnsureTargetAlgorithmAvailable()
        {
            if (FOV == null && Algorithm == null)
            {
                MessageBox.Show("Xin vui lòng chọn FOV hoặc Algorithm trước khi vẽ ROI!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (Algorithm == null)
            {
                if (FOV != null && FOV.Algorithmslist.Count > 0)
                {
                    Algorithm = FOV.Algorithmslist[0];
                }
                else
                {
                    MessageBox.Show("FOV này chưa có Algorithm nào. Vui lòng tạo Algorithm trước khi vẽ ROI!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            if (ImageScreen.Source == null)
            {
                MessageBox.Show("Chưa có ảnh để vẽ ROI. Vui lòng mở ảnh trước!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            return true;
        }

        private void Click_RectangleRoi(object sender, RoutedEventArgs e)
        {
            if (!EnsureTargetAlgorithmAvailable()) return;

            _isDrawingRoiMode = !_isDrawingRoiMode;
            if (_isDrawingRoiMode)
            {
                _isDrawingPolyMode = false;
                _polygonPoints.Clear();
            }

            UpdateButtonState();
            if (!_isDrawingRoiMode) RedrawRoi();
        }

        private void Click_PolygonRoi(object sender, RoutedEventArgs e)
        {
            if (!EnsureTargetAlgorithmAvailable()) return;

            _isDrawingPolyMode = !_isDrawingPolyMode;
            if (_isDrawingPolyMode)
            {
                _isDrawingRoiMode = false;
                _polygonPoints.Clear();
            }

            UpdateButtonState();
            if (!_isDrawingPolyMode) RedrawRoi();
        }

        private void UpdateButtonState()
        {
            RectangleRoi.Background = _isDrawingRoiMode ? new SolidColorBrush(Color.FromRgb(0, 122, 204)) : Brushes.White;
            PolygonRoi.Background = _isDrawingPolyMode ? new SolidColorBrush(Color.FromRgb(0, 122, 204)) : Brushes.White;
            CanvasRoi.Cursor = (_isDrawingRoiMode || _isDrawingPolyMode) ? Cursors.Cross : Cursors.Arrow;
        }

        #endregion

        #region Xử lý vẽ ROI trực tiếp trên tọa độ Pixel ảnh

        private void CanvasRoi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageScreen.Source is not BitmapSource bitmap)
                return;

            // Nếu không vẽ ROI, nhấp chuột để Pan ảnh
            if (!_isDrawingRoiMode && !_isDrawingPolyMode)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
                {
                    StartPanning(e);
                    return;
                }
            }

            // Tọa độ p trả về ĐÚNG pixel của ảnh nhờ CanvasRoi nằm trong ViewportCanvas
            Point p = e.GetPosition(CanvasRoi);
            double clampedX = Math.Clamp(p.X, 0, bitmap.PixelWidth);
            double clampedY = Math.Clamp(p.Y, 0, bitmap.PixelHeight);

            // Xử lý vẽ Polygon ROI
            if (_isDrawingPolyMode)
            {
                if (e.RightButton == MouseButtonState.Pressed || e.ClickCount >= 2)
                {
                    FinishPolygonDrawing();
                    return;
                }

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _polygonPoints.Add(new Point(clampedX, clampedY));
                    DrawPolygonPreview(new Point(clampedX, clampedY));
                    return;
                }
            }

            // Xử lý vẽ Rectangle ROI
            if (_isDrawingRoiMode && e.LeftButton == MouseButtonState.Pressed)
            {
                _startPoint = new Point(clampedX, clampedY);
                _isDragging = true;
                CanvasRoi.CaptureMouse();

                CanvasRoi.Children.Clear();

                double currentScale = ViewportScale.ScaleX > 0 ? ViewportScale.ScaleX : 1.0;
                double labelScale = 1.0 / currentScale;

                _activeRectShape = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = Math.Max(1.0, 2.0 / currentScale),
                    StrokeDashArray = new DoubleCollection { 4, 2 },
                    Fill = new SolidColorBrush(Color.FromArgb(40, 255, 0, 0)),
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(_activeRectShape, _startPoint.X);
                Canvas.SetTop(_activeRectShape, _startPoint.Y);
                CanvasRoi.Children.Add(_activeRectShape);

                _activeLabelText = new TextBlock
                {
                    Foreground = Brushes.White,
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    Text = "ROI: 0 x 0 px"
                };
                _activeLabelBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(4, 2, 4, 2),
                    Child = _activeLabelText,
                    RenderTransform = new ScaleTransform(labelScale, labelScale),
                    RenderTransformOrigin = new Point(0, 1)
                };
                Canvas.SetLeft(_activeLabelBorder, _startPoint.X);
                Canvas.SetTop(_activeLabelBorder, Math.Max(0, _startPoint.Y - (22 * labelScale)));
                CanvasRoi.Children.Add(_activeLabelBorder);
            }
        }

        private void CanvasRoi_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                ContinuePanning(e);
                return;
            }

            if (ImageScreen.Source is not BitmapSource bitmap) return;

            Point curPoint = e.GetPosition(CanvasRoi);
            double clampedX = Math.Clamp(curPoint.X, 0, bitmap.PixelWidth);
            double clampedY = Math.Clamp(curPoint.Y, 0, bitmap.PixelHeight);

            // Cập nhật đường nối và xem trước Polygon
            if (_isDrawingPolyMode && _polygonPoints.Count > 0)
            {
                DrawPolygonPreview(new Point(clampedX, clampedY));
                return;
            }

            // Cập nhật hình chữ nhật khi kéo chuột
            if (!_isDragging || _activeRectShape == null || _activeLabelBorder == null || _activeLabelText == null)
                return;

            double x = Math.Min(_startPoint.X, clampedX);
            double y = Math.Min(_startPoint.Y, clampedY);
            double width = Math.Abs(clampedX - _startPoint.X);
            double height = Math.Abs(clampedY - _startPoint.Y);

            Canvas.SetLeft(_activeRectShape, x);
            Canvas.SetTop(_activeRectShape, y);
            _activeRectShape.Width = width;
            _activeRectShape.Height = height;

            _activeLabelText.Text = $"ROI: {width:F0} x {height:F0} px";

            double currentScale = ViewportScale.ScaleX > 0 ? ViewportScale.ScaleX : 1.0;
            double labelScale = 1.0 / currentScale;
            Canvas.SetLeft(_activeLabelBorder, x);
            Canvas.SetTop(_activeLabelBorder, Math.Max(0, y - (22 * labelScale)));
        }

        private void CanvasRoi_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                EndPanning();
                return;
            }

            if (!_isDragging)
                return;

            _isDragging = false;
            CanvasRoi.ReleaseMouseCapture();

            if (_activeRectShape == null || Algorithm == null || ImageScreen.Source is not BitmapSource bitmap)
                return;

            double pixelX = Canvas.GetLeft(_activeRectShape);
            double pixelY = Canvas.GetTop(_activeRectShape);
            double pixelW = _activeRectShape.Width;
            double pixelH = _activeRectShape.Height;

            double currentScale = ViewportScale.ScaleX > 0 ? ViewportScale.ScaleX : 1.0;
            if (pixelW < (5 / currentScale) || pixelH < (5 / currentScale))
            {
                CanvasRoi.Children.Clear();
                _activeRectShape = null;
                _activeLabelBorder = null;
                _activeLabelText = null;
                RedrawRoi();
                return;
            }

            pixelX = Math.Clamp(pixelX, 0, bitmap.PixelWidth);
            pixelY = Math.Clamp(pixelY, 0, bitmap.PixelHeight);
            pixelW = Math.Clamp(pixelW, 0, bitmap.PixelWidth - pixelX);
            pixelH = Math.Clamp(pixelH, 0, bitmap.PixelHeight - pixelY);

            // Lưu trực tiếp tọa độ Pixel thực vào Algorithm
            Algorithm.setROI(
                Math.Round(pixelX, 1),
                Math.Round(pixelY, 1),
                Math.Round(pixelW, 1),
                Math.Round(pixelH, 1),
                "Rectangle"
            );

            _isDrawingRoiMode = false;
            UpdateButtonState();
            RedrawRoi();
        }

        private void DrawPolygonPreview(Point currentCursor)
        {
            CanvasRoi.Children.Clear();

            if (_polygonPoints.Count == 0) return;

            double currentScale = ViewportScale.ScaleX > 0 ? ViewportScale.ScaleX : 1.0;
            double labelScale = 1.0 / currentScale;

            var pointCollection = new PointCollection(_polygonPoints) { currentCursor };

            var polyline = new Polyline
            {
                Stroke = Brushes.Red,
                StrokeThickness = Math.Max(1.0, 2.0 / currentScale),
                StrokeDashArray = new DoubleCollection { 4, 2 },
                Points = pointCollection
            };
            CanvasRoi.Children.Add(polyline);

            // Vẽ các đỉnh tròn
            double dotSize = Math.Max(3.0, 6.0 / currentScale);
            foreach (var pt in _polygonPoints)
            {
                var dot = new Ellipse
                {
                    Width = dotSize,
                    Height = dotSize,
                    Fill = Brushes.Yellow,
                    Stroke = Brushes.Red,
                    StrokeThickness = Math.Max(1.0, 1.5 / currentScale)
                };
                Canvas.SetLeft(dot, pt.X - (dotSize / 2.0));
                Canvas.SetTop(dot, pt.Y - (dotSize / 2.0));
                CanvasRoi.Children.Add(dot);
            }

            // Hiển thị nhãn hướng dẫn
            var tipText = new TextBlock
            {
                Text = $"Polygon: {_polygonPoints.Count} điểm. Chuột phải / Đúp chuột để xong",
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.Bold
            };
            var tipBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5, 2, 5, 2),
                Child = tipText,
                RenderTransform = new ScaleTransform(labelScale, labelScale),
                RenderTransformOrigin = new Point(0, 1)
            };
            Canvas.SetLeft(tipBorder, _polygonPoints[0].X);
            Canvas.SetTop(tipBorder, Math.Max(0, _polygonPoints[0].Y - (24 * labelScale)));
            CanvasRoi.Children.Add(tipBorder);
        }

        private void FinishPolygonDrawing()
        {
            if (_polygonPoints.Count >= 3 && Algorithm != null)
            {
                var pixelPoints = _polygonPoints.Select(p => new Point(
                    Math.Round(p.X, 1),
                    Math.Round(p.Y, 1)
                )).ToList();

                Algorithm.setPolygonROI(pixelPoints);
            }

            _isDrawingPolyMode = false;
            _polygonPoints.Clear();
            UpdateButtonState();
            RedrawRoi();
        }

        #endregion

        #region Vẽ lại ROI trên Canvas

        public void RedrawRoiFromFOV()
        {
            RedrawRoi();
        }

        public void RedrawRoi()
        {
            CanvasRoi.Children.Clear();
            _activeRectShape = null;
            _activeLabelBorder = null;
            _activeLabelText = null;

            if (ImageScreen.Source is not BitmapSource bitmap)
                return;

            double currentScale = ViewportScale.ScaleX > 0 ? ViewportScale.ScaleX : 1.0;

            // 1. Vẽ ROI của các Algorithm khác trong cùng FOV (màu xanh cyan nét đứt)
            if (FOV != null)
            {
                foreach (var algo in FOV.Algorithmslist)
                {
                    if (algo == Algorithm || algo.Roi == null) continue;
                    if (algo.Roi.Width <= 0 || algo.Roi.Height <= 0) continue;

                    DrawSingleRoi(
                        algo.Roi,
                        currentScale,
                        new SolidColorBrush(Color.FromRgb(0, 180, 255)),
                        new SolidColorBrush(Color.FromArgb(25, 0, 180, 255)),
                        $"{algo.Name ?? "Algorithm"}: {algo.Roi.Name ?? "ROI"}",
                        new DoubleCollection { 4, 2 },
                        1.5
                    );
                }
            }

            // 2. Vẽ ROI của Algorithm hiện tại đang chọn (màu đỏ nổi bật)
            var activeRoi = Algorithm?.Roi;
            if (activeRoi != null && activeRoi.Width > 0 && activeRoi.Height > 0)
            {
                DrawSingleRoi(
                    activeRoi,
                    currentScale,
                    Brushes.Red,
                    new SolidColorBrush(Color.FromArgb(40, 255, 0, 0)),
                    $"{Algorithm?.Name ?? "Algorithm"} - {activeRoi.Name ?? "ROI"} ({activeRoi.Type}: X:{activeRoi.X:F0}, Y:{activeRoi.Y:F0}, W:{activeRoi.Width:F0}, H:{activeRoi.Height:F0})",
                    null,
                    2.0
                );
            }
        }

        private void DrawSingleRoi(
            ROI roi,
            double currentScale,
            Brush strokeBrush,
            Brush fillBrush,
            string labelText,
            DoubleCollection? dashArray,
            double strokeThickness)
        {
            double adjustedThickness = Math.Max(1.0, strokeThickness / currentScale);
            double labelScale = 1.0 / currentScale;

            if (roi is PolygonROI polyRoi && polyRoi.Points.Count > 1)
            {
                var pointCollection = new PointCollection(polyRoi.Points);
                var polyShape = new Polygon
                {
                    Stroke = strokeBrush,
                    StrokeThickness = adjustedThickness,
                    StrokeDashArray = dashArray,
                    Fill = fillBrush,
                    Points = pointCollection
                };
                CanvasRoi.Children.Add(polyShape);
            }
            else
            {
                var rectShape = new Rectangle
                {
                    Stroke = strokeBrush,
                    StrokeThickness = adjustedThickness,
                    StrokeDashArray = dashArray,
                    Fill = fillBrush,
                    Width = Math.Max(0, roi.Width),
                    Height = Math.Max(0, roi.Height)
                };
                Canvas.SetLeft(rectShape, roi.X);
                Canvas.SetTop(rectShape, roi.Y);
                CanvasRoi.Children.Add(rectShape);
            }

            // Nhãn ROI: Giữ kích thước chữ 11pt cố định không bị mờ hay phóng đại quá lớn khi zoom
            var labelTb = new TextBlock
            {
                Text = labelText,
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.Bold
            };
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(4, 2, 4, 2),
                Child = labelTb,
                RenderTransform = new ScaleTransform(labelScale, labelScale),
                RenderTransformOrigin = new Point(0, 1)
            };
            Canvas.SetLeft(border, roi.X);
            Canvas.SetTop(border, Math.Max(0, roi.Y - (22 * labelScale)));
            CanvasRoi.Children.Add(border);
        }

        #endregion
    }
}
