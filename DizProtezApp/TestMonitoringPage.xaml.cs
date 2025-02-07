using LiveChartsCore.SkiaSharpView;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using DizProtezApp.Services;
using System.Diagnostics;
using DizProtezApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DizProtezApp
{
    public partial class TestMonitoringPage : Page
    {
        private TestMonitoringViewModel _viewModel;
        private DispatcherTimer _dataUpdateTimer;
        private readonly PlcService _plcService;
        private readonly SqlService _sqlService;
        private DispatcherTimer _bufferFlushTimer;

        public TestMonitoringPage(SqlService sqlService, PlcService plcService, string testName)
        {
            InitializeComponent();

            _sqlService = sqlService;

            // App sınıfından PlcService'i alın
            _plcService = ((App)Application.Current).ServiceProvider.GetRequiredService<PlcService>();

            _viewModel = new TestMonitoringViewModel
            {
                TestName = testName // Test adını ViewModel'e aktar
            };
            DataContext = _viewModel;

       
            InitializeBufferFlushTimer();
            // Gerçek zamanlı veri okuma
            _dataUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
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

                await Task.Run(async() =>
                {
                    // Displacement okuma (örnekte integer olarak kalıyor)
                    int displacement = await _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz);

                    // Force değerini doğru şekilde oku ve Newton'a çevir
                    double forceKg = await _plcService.ReadDWord(PlcRegisters.LOADCELL_2_DWORD);
                    double forceAxialN = forceKg * 9.81; // Newton'a çevir

                    // LOADCELL_TOP_DWORD değeri (Force2) okunup Newton'a çevriliyor
                    double forceTopKg = await _plcService.ReadDWord(PlcRegisters.LOADCELL_TOP_DWORD) / 1000.0;
                    double forceFemoralN = forceTopKg * 9.81;

                    // Yeni bir PlcData nesnesi oluştur
                    var plcData = new PlcData
                    {
                        Timestamp = DateTime.UtcNow,
                        Displacement = displacement,
                        Force = Math.Round(forceAxialN, 3),
                        SpecimenId = 1
                    };
                    _sqlService.AddToBuffer(plcData);

                    // UI güncellemeleri için Dispatch
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.Displacement1 = displacement;
                        _viewModel.Force1 = Math.Round(forceAxialN, 3); // 3 ondalık basamak
                        _viewModel.Force2 = Math.Round(forceFemoralN, 3);
                        _viewModel.AddDataPoints();
                    });
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
