using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DizProtezApp
{
    public class TestMonitoringViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ISeries> ChartSeries { get; set; }
        public ObservableCollection<Axis> XAxes { get; set; }
        public ObservableCollection<Axis> YAxes { get; set; }

        private double _force;
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

        public double Force
        {
            get => _force;
            set
            {
                _force = value;
                OnPropertyChanged(nameof(Force));
                AddDataPoint(); // Yeni veri noktası ekle
            }
        }

        private double _displacement;
        public double Displacement
        {
            get => _displacement;
            set
            {
                _displacement = value;
                OnPropertyChanged(nameof(Displacement));
            }
        }

        public TestMonitoringViewModel()
        {
            // Veri serisi
            ChartSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Values = new ObservableCollection<ObservablePoint>(), // Dinamik veri koleksiyonu
                    Fill = null, // Şeffaf çizgi arkaplanı
                    LineSmoothness = 0.5, // Yumuşaklık
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 } // Çizgi rengi
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
                values.Add(new ObservablePoint(Displacement, Force));

                // Maksimum 500 veri noktasıyla sınırlayın (örnek)
                if (values.Count > 500)
                    values.RemoveAt(0);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
