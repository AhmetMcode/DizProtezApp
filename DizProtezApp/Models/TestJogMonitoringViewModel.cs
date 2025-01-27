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

        private double _force;
        public double Force1
        {
            get => _force;
            set
            {
                _force = value;
                OnPropertyChanged(nameof(Force1));
                AddDataPoint(); // Yeni veri noktası ekle
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
                    Values = new ObservableCollection<ObservablePoint>(),
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 6 },

                    // Nokta işaretleyicilerini kaldır
                    GeometrySize = 0,

                    // X ekseni (Displacement) için Tooltip formatı
                    XToolTipLabelFormatter = point =>
                        $"Displacement: {point.Coordinate.SecondaryValue:F3} mm",

                    // Y ekseni (Force) için Tooltip formatı
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
        private void AddDataPoint()
        {
            if (ChartSeries[0] is LineSeries<ObservablePoint> lineSeries && lineSeries.Values is ObservableCollection<ObservablePoint> values)
            {
                values.Add(new ObservablePoint(Displacement1, Force1));

                //// Maksimum 500 veri noktasıyla sınırlayın (örnek)
                //if (values.Count > 200)
                //    values.RemoveAt(0);
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
