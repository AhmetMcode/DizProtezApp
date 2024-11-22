using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Windows;

namespace DizProtezApp
{
    public partial class TestMonitoringPage : Page
    {
        private readonly DispatcherTimer _timer;
        private readonly Random _random = new Random();

        public ObservableCollection<ISeries> Series { get; private set; }

        public TestMonitoringPage(string testName, string forceInput, string speedInput, string flexionInput, string torsionalLoad)
        {
            InitializeComponent();

            // Initialize Series with a LineSeries for Force data with some initial dummy data
            var initialValues = new ObservableCollection<double> { 600, 620, 640, 610, 630 };
            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = initialValues,
                    Name = "Force (N)",
                    Fill = null, // Optional: No fill below the line
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue)
                }
            };

            // Set DataContext to this page to bind the Series
            DataContext = this;

            // Display test parameters
            DisplayTestParameters(testName, forceInput, speedInput, flexionInput, torsionalLoad);

            // Setup timer for adding data
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Generate random data for testing purposes
            double force = _random.Next(500, 800);

            // Update UI with random values
            ForceValue.Text = force.ToString();
            DisplacementValue.Text = _random.Next(0, 10).ToString(); // Dummy displacement value

            // Update the chart data
            if (Series[0] is LineSeries<double> lineSeries && lineSeries.Values is ObservableCollection<double> values)
            {
                values.Add(force);

                // Optionally limit data points to avoid performance issues
                if (values.Count > 50)
                {
                    values.RemoveAt(0); // Keep only the latest 50 data points
                }
            }
        }

        private void DisplayTestParameters(string testName, string forceInput, string speedInput, string flexionInput, string torsionalLoad)
        {
            TestDescription.Text = $"Test: {testName}";
            ForceInputValue.Text = forceInput;
            SpeedInputValue.Text = speedInput;
            FlexionInputValue.Text = flexionInput;
            TorsionalLoadValue.Text = torsionalLoad;
        }

        private void StopTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _timer.Stop();
            System.Windows.MessageBox.Show("Test Stopped");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer != null && _timer.IsEnabled)
            {
                // Kullanıcıya testin durdurulması gerektiğini sor
                var result = MessageBox.Show(
                    "Test şu anda çalışıyor. Testi durdurup geri dönmek istiyor musunuz?",
                    "Testi Durdur",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Testi durdur ve geri dön
                    _timer.Stop();
                    if (NavigationService != null && NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
                // Kullanıcı 'Hayır' derse hiçbir şey yapma
            }
            else
            {
                // Test zaten durdurulmuşsa doğrudan geri git
                if (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Geriye gidilecek bir sayfa yok!");
                }
            }
        }

    }
}
