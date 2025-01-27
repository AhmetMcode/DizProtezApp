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
        private double _force;
        private double _displacement;
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

        public double Force
        {
            get => _force;
            set
            {
                _force = value;
                OnPropertyChanged(nameof(Force));
                AddDataPoint();
            }
        }

        public double Displacement
        {
            get => _displacement;
            set
            {
                _displacement = value;
                OnPropertyChanged(nameof(Displacement));
                AddDataPoint();
            }
        }

        public DynamicTestMonitoringViewModel()
        {
            // Veri serisi
            ChartSeries = new ObservableCollection<ISeries>
            {
                // Force için seri
                new LineSeries<ObservablePoint>
                {
                    Name = "Force (N)", // Seri ismi
                    Values = new ObservableCollection<ObservablePoint>(),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1 },
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    ScalesYAt = 0 // Sol Y ekseni
                },
                // Displacement için seri
                new LineSeries<ObservablePoint>
                {
                    Name = "Displacement (mm)", // Seri ismi
                    Values = new ObservableCollection<ObservablePoint>(),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1 },
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    ScalesYAt = 1 // Sağ Y ekseni
                }
            };

            // X Ekseni
            XAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = "Time (s)",
                    MinStep = 1,
                    Labeler = value => $"{value:F1} s",
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray)
                }
            };

            // Y Ekseni
            YAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = "Force (N)",
                    Labeler = value => $"{value:F1} N",
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray)
                },
                new Axis
                {
                    Name = "Displacement (mm)",
                    Labeler = value => $"{value:F1} mm",
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray),
                    Position = LiveChartsCore.Measure.AxisPosition.End
                }
            };
        }

        private void AddDataPoint()
        {
            if (ChartSeries[0] is LineSeries<ObservablePoint> forceSeries && forceSeries.Values is ObservableCollection<ObservablePoint> forceValues)
            {
                forceValues.Add(new ObservablePoint(Time, Force));
            }

            if (ChartSeries[1] is LineSeries<ObservablePoint> displacementSeries && displacementSeries.Values is ObservableCollection<ObservablePoint> displacementValues)
            {
                displacementValues.Add(new ObservablePoint(Time, Displacement));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
