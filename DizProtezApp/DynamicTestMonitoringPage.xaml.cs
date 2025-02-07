using DizProtezApp.Models;
using DizProtezApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;


namespace DizProtezApp
{
    public partial class DynamicTestMonitoringPage : Page
    {
        private readonly DynamicTestMonitoringViewModel _viewModel;
        private readonly DispatcherTimer _dataUpdateTimer;
        private double _time;
        private readonly PlcService _plcService;
        private readonly SqlService _sqlService;
        private DispatcherTimer _bufferFlushTimer;
        

        public DynamicTestMonitoringPage(SqlService sqlService, PlcService plcService, string testName)
        {
            InitializeComponent();

            _sqlService = sqlService;

            // App sınıfından PlcService'i alın
            _plcService = ((App)Application.Current).ServiceProvider.GetRequiredService<PlcService>();

            _viewModel = new DynamicTestMonitoringViewModel { TestName = testName };
            DataContext = _viewModel;

            InitializeBufferFlushTimer();
            // Zamanlı veri güncellemeleri için timer
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

        private bool _isReading = false;

        private async Task RefreshServo1PositionAndLoadcellAsync()
        {
            if (_isReading)
                return; // Eğer önceki okuma henüz bitmemişse, yeni okuma başlatmayın.
            _isReading = true;

            // Stopwatch başlatılıyor.
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                if (!_plcService.IsConnected)
                {
                    Console.WriteLine("PLC bağlantısı yok. Veriler güncellenmeyecek.");
                    return;
                }

                await Task.Run(async () =>
                {
                    // Displacement okuma (örnekte integer olarak kalıyor)
                    int displacement = await _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz);

                    // Force değerini doğru şekilde oku ve Newton'a çevir
                    double forceKg = await _plcService.ReadDWord(PlcRegisters.LOADCELL_2_DWORD);
                    double forceAxialN = forceKg * 9.81; // Newton'a çevir

                    // LOADCELL_TOP_DWORD değeri (Force2) okunup Newton'a çevriliyor
                    double forceTopKg = (await _plcService.ReadDWord(PlcRegisters.LOADCELL_TOP_DWORD)) / 1000.0;
                    double forceFemoralN = forceTopKg * 9.81;

                    //// Yeni bir PlcData nesnesi oluştur YAVAŞLATIYOR
                    //var plcData = new PlcData
                    //{
                    //    Timestamp = DateTime.UtcNow,
                    //    Displacement = displacement,
                    //    Force = Math.Round(forceAxialN, 3),
                    //    SpecimenId = 1
                    //};
                    //_sqlService.AddToBuffer(plcData);

                    // UI güncellemeleri için Dispatch
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Zamanı her güncellemede 0,1 s artırıyoruz.
                        _viewModel.Time += 0.1;

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
            finally
            {
                _isReading = false;
                sw.Stop();
                Console.WriteLine("RefreshServo1PositionAndLoadcellAsync Süresi: " + sw.ElapsedMilliseconds + " ms");
            }
        }

        private void ResetZoomButton_Click(object sender, RoutedEventArgs e)
        {
            // Otomatik ölçekleme (auto-scale) moduna geçmek için x ekseni limitlerini kaldırıyoruz.
            _viewModel.XAxes[0].MinLimit = null;
            _viewModel.XAxes[0].MaxLimit = 79200;
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
