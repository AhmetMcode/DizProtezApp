using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DizProtezApp.Models
{
    public class DynamicTestMonitoringViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ISeries> ChartSeries { get; set; }
        public ObservableCollection<Axis> XAxes { get; set; }
        public ObservableCollection<Axis> YAxes { get; set; }

        private double _time;
        private string _testName;


        public string TestName
        {
            get => _testName;
            set
            {
                _testName = value;
                OnPropertyChanged(nameof(TestName));
            }
        }

        public double Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        private double _force1;
        public double Force1
        {
            get => _force1;
            set
            {
                _force1 = value;
                OnPropertyChanged(nameof(Force1));
            }
        }

        private double _force2;
        public double Force2
        {
            get => _force2;
            set
            {
                _force2 = value;
                OnPropertyChanged(nameof(Force2));
            }
        }

        private double _displacement;
        public double Displacement1
        {
            get => _displacement;
            set
            {
                _displacement = value;
                OnPropertyChanged(nameof(Displacement1));
            }
        }

        public DynamicTestMonitoringViewModel()
        {
            // Veri serisi
            ChartSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    AnimationsSpeed = TimeSpan.Zero,
                    Name = "Axial Load(N)",
                    Values = new ObservableCollection<ObservablePoint>(),
                    DataPadding = new LiveChartsCore.Drawing.LvcPoint(0, 0), // Kenar boşluklarını kaldır
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1 },
                    GeometrySize = 0,
                    ScalesYAt = 0,
                    // X ekseninde artık zaman gösterilecek
                    XToolTipLabelFormatter = point => $"Time: {point.Coordinate.SecondaryValue:F3} s",
                    // Y ekseninde force değeri gösterilecek
                    YToolTipLabelFormatter = point => $"Force: {point.Coordinate.PrimaryValue:F3} N"
                },

                // 2. Çizgi: Femoral Load (Force2)
                new LineSeries<ObservablePoint>
                {
                    AnimationsSpeed = TimeSpan.Zero,
                    Name = "Femoral Load(N)",
                    Values = new ObservableCollection<ObservablePoint>(),
                    DataPadding = new LiveChartsCore.Drawing.LvcPoint(0, 0),
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1 },
                    GeometrySize = 0,
                    ScalesYAt = 0,
                    XToolTipLabelFormatter = point => $"Time: {point.Coordinate.SecondaryValue:F3} s",
                    YToolTipLabelFormatter = point => $"Force: {point.Coordinate.PrimaryValue:F3} N"
                },

                // 3. Çizgi: Displacement (mm)
                new LineSeries<ObservablePoint>
                {
                    AnimationsSpeed = TimeSpan.Zero,
                    Name = "Displacement (mm)", // Seri ismi
                    Values = new ObservableCollection<ObservablePoint>(),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 1 },
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    ScalesYAt = 1, // Sağ Y ekseni
                    XToolTipLabelFormatter = point => $"Time: {point.Coordinate.SecondaryValue:F3} s",
                    YToolTipLabelFormatter = point => $"Displacement: {point.Coordinate.PrimaryValue:F3} mm"
                }
            };

            // X Ekseni
            XAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = "Time (s)",
                    MinStep = 1,
                    Labeler = value => $"{value / 3600:F1} h", // Saniyeyi saate çevirerek etiketler
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray),
                    ShowSeparatorLines = true,
                    MinLimit = 0,         // Sol sınır 0
            MaxLimit = 79200      // Sağ sınır 22 saat = 79200 saniye
                }
            };

            // Y Ekseni
            YAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = "Force (N)",
                    Labeler = value => $"{value:F1} N",
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray),
                    ShowSeparatorLines = true,
                },
                new Axis
                {
                    Name = "Displacement (mm)",
                    Labeler = value => $"{value:F1} mm",
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray),
                    ShowSeparatorLines = false,
                    Position = LiveChartsCore.Measure.AxisPosition.End
                }
            };
        }


        // X ve Y verilerini içeren yeni bir veri noktası ekleme
        public void AddDataPoints()
        {
            // Zamanı x koordinatı olarak kullanıyoruz.
            var t = Math.Round(Time, 3);
            var force1 = Math.Round(Force1, 3);
            var force2 = Math.Round(Force2, 3);
            var displacement = Math.Round(Displacement1, 3);

            // 1. Seri: Axial Load (Force1)
            if (ChartSeries[0] is LineSeries<ObservablePoint> seriesForce1 &&
                seriesForce1.Values is ObservableCollection<ObservablePoint> valuesForce1)
            {
                valuesForce1.Add(new ObservablePoint(t, force1));
            }

            // 2. Seri: Femoral Load (Force2)
            if (ChartSeries[1] is LineSeries<ObservablePoint> seriesForce2 &&
                seriesForce2.Values is ObservableCollection<ObservablePoint> valuesForce2)
            {
                valuesForce2.Add(new ObservablePoint(t, force2));
            }

            // 3. Seri: Displacement (mm)
            if (ChartSeries[2] is LineSeries<ObservablePoint> seriesDisplacement &&
                seriesDisplacement.Values is ObservableCollection<ObservablePoint> valuesDisplacement)
            {
                valuesDisplacement.Add(new ObservablePoint(t, displacement));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
