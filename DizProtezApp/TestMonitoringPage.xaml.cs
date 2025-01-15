using LiveChartsCore.SkiaSharpView;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using DizProtezApp.Services;
using System.Diagnostics;
using DizProtezApp.Models;

namespace DizProtezApp
{
    public partial class TestMonitoringPage : Page
    {
        private TestMonitoringViewModel _viewModel;
        private DispatcherTimer _dataUpdateTimer;
        private readonly PlcService _plcService;
        private readonly SqlService _sqlService;
        private  DispatcherTimer _bufferFlushTimer;
        private readonly string _testName;

        public TestMonitoringPage(SqlService sqlService, PlcService plcService, string testName)
        {
            InitializeComponent();

            _viewModel = new TestMonitoringViewModel
            {
                TestName = testName // Test adını ViewModel'e aktar
            };
            DataContext = _viewModel;

            _sqlService = sqlService;
            _plcService = plcService;
            _testName = testName;


            InitializeBufferFlushTimer();
            // Gerçek zamanlı veri okuma
            _dataUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            _dataUpdateTimer.Tick += async (s, e) =>
            {
                await RefreshServo1PositionAndLoadcellAsync();
            };
            _dataUpdateTimer.Start();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Timer'ı durdur
            _dataUpdateTimer.Stop();

            // Geri git
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private async Task RefreshServo1PositionAndLoadcellAsync()
        {
            try
            {
                if (!_plcService.IsConnected)
                {
                    Console.WriteLine("PLC bağlantısı yok. Veriler güncellenmeyecek.");
                    return;
                }

                // Asenkron olarak PLC'den veri oku
                int displacement = await Task.Run(() => _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz));
                float gramValue = await Task.Run(() => _plcService.ReadReal(PlcRegisters.LOADCELL_1_DWORD));
                double force = (gramValue / 1000.0) * 9.80665; // Gramı Newton'a çevir

                // Yeni bir PlcData nesnesi oluştur
                var plcData = new PlcData
                {
                    Timestamp = DateTime.UtcNow,
                    Displacement = displacement,
                    Force = Math.Round(force, 3),
                    SpecimenId = 1
                };

                _sqlService.AddToBuffer(plcData);

                // ViewModel güncelle
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _viewModel.Displacement = displacement;
                    _viewModel.Force = force;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veriler okunamadı: {ex.Message}");
            }
        }




        private void InitializeBufferFlushTimer()
        {
            _bufferFlushTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10) // Her 10 saniyede bir yazım işlemi yapılır
            };

            _bufferFlushTimer.Tick += async (s, e) => await _sqlService.FlushDataBufferAsync();
            _bufferFlushTimer.Start();
        }


        private void StopTestButton_Click(object sender, RoutedEventArgs e)
        {
            _dataUpdateTimer.Stop();
            MessageBox.Show("Test durduruldu.");
        }
    }
}
