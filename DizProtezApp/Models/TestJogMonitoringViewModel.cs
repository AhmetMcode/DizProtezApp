using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DizProtezApp
{
    public class TestJogMonitoringViewModel : INotifyPropertyChanged
    {
        private double _servo1JogSpeed;
        private double _verticalForceInput1;
        private double _servo1CurrentPosition;

        public ObservableCollection<ISeries> ChartSeries { get; set; }
        public ObservableCollection<Axis> XAxes { get; set; }
        public ObservableCollection<Axis> YAxes { get; set; }

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

        public TestJogMonitoringViewModel()
        {
            // Veri serisi
            ChartSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Axial Load(N)",
                    Values = new ObservableCollection<ObservablePoint>(),
                    DataPadding = new LiveChartsCore.Drawing.LvcPoint(0, 0), // Kenar boşluklarını kaldır
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1 },

                    // Nokta işaretleyicilerini kaldır
                    GeometrySize = 0,

                    // X ekseni (Displacement) için Tooltip formatı
                    XToolTipLabelFormatter = point =>
                        $"Displacement: {point.Coordinate.SecondaryValue:F3} mm",

                    // Y ekseni (Force) için Tooltip formatı
                    YToolTipLabelFormatter = point =>
                        $"Force: {point.Coordinate.PrimaryValue:F3} N"
                },
                // 2. Çizgi: LOADCELL_TOP_DWORD (Force2)
                new LineSeries<ObservablePoint>
                {
                    Name = "Femoral Load(N)",
                    Values = new ObservableCollection<ObservablePoint>(),
                    DataPadding = new LiveChartsCore.Drawing.LvcPoint(0, 0),
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1 },
                    GeometrySize = 0,
                    XToolTipLabelFormatter = point =>
                        $"Displacement: {point.Coordinate.SecondaryValue:F3} mm",
                    YToolTipLabelFormatter = point =>
                        $"Force: {point.Coordinate.PrimaryValue:F3} N"
                }
            };


            // X Ekseni
            XAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = "Displacement (mm)",
                    ShowSeparatorLines = true, // Izgara çizgileri
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray) // Izgara çizgi rengi
                }
            };

            // Y Ekseni
            YAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = "Force (N)",
                    ShowSeparatorLines = true, // Izgara çizgileri
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray) // Izgara çizgi rengi
                }
            };
        }

        // X ve Y verilerini içeren yeni bir veri noktası ekleme
        public void AddDataPoints()
        {
            // İki çizginin de Values koleksiyonu varsa
            if (ChartSeries[0] is LineSeries<ObservablePoint> lineSeries1 &&
                ChartSeries[1] is LineSeries<ObservablePoint> lineSeries2)
            {
                // Değerleri yuvarlayarak alıyoruz
                var x = Math.Round(Displacement1, 3);
                var y1 = Math.Round(Force1, 3);
                var y2 = Math.Round(Force2, 3);

                // İlk çizgiye (Force1) veri noktası ekleniyor
                if (lineSeries1.Values is ObservableCollection<ObservablePoint> values1)
                {
                    values1.Add(new ObservablePoint(x, y1));
                    if (values1.Count > 500) values1.RemoveAt(0); // Opsiyonel: Eski verileri temizle
                }

                // İkinci çizgiye (Force2) veri noktası ekleniyor
                if (lineSeries2.Values is ObservableCollection<ObservablePoint> values2)
                {
                    values2.Add(new ObservablePoint(x, y2));
                    if (values2.Count > 500) values2.RemoveAt(0); // Opsiyonel: Eski verileri temizle
                }
            }
        }


        public double Servo1JogSpeedBind
        {
            get => _servo1JogSpeed;
            set
            {
                _servo1JogSpeed = value;
                OnPropertyChanged();
            }
        }

        public double VerticalForceInput1
        {
            get => _verticalForceInput1;
            set
            {
                _verticalForceInput1 = value;
                OnPropertyChanged();
            }
        }

        public double Servo1CurrentPosition
        {
            get => _servo1CurrentPosition;
            set
            {
                _servo1CurrentPosition = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
