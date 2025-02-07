using DizProtezApp.Models;
using DizProtezApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;

using System.Collections.Concurrent;
using System.Timers;

namespace DizProtezApp
{
    public partial class DynamicTestMonitoringPage : Page
    {
        // PLC okunan verileri depolayacağımız thread-safe kuyruk
        private ConcurrentQueue<PlcData> _plcDataQueue = new ConcurrentQueue<PlcData>();

        // PLC okuma timer (arka plan)
        private System.Timers.Timer _plcReadingTimer;

        private readonly DynamicTestMonitoringViewModel _viewModel;
        private readonly DispatcherTimer _dataUpdateTimer;
        private double _time;
        private readonly PlcService _plcService;
        private readonly SqlService _sqlService;
        // UI güncelleme timer
        private DispatcherTimer _uiUpdateTimer;


        public DynamicTestMonitoringPage(SqlService sqlService, PlcService plcService, string testName)
        {
            InitializeComponent();

            _sqlService = sqlService;

            // App sınıfından PlcService'i alın
            _plcService = ((App)Application.Current).ServiceProvider.GetRequiredService<PlcService>();

            _viewModel = new DynamicTestMonitoringViewModel { TestName = testName };
            DataContext = _viewModel;

            // 1) PLC OKUMA TIMER -> 100 ms aralıklarla veri oku, queue'ya at
            _plcReadingTimer = new System.Timers.Timer(50); // 100 ms
            _plcReadingTimer.Elapsed += PlcReadingTimer_Elapsed;
            _plcReadingTimer.AutoReset = true; // her aralıkta tekrar çalışsın
            _plcReadingTimer.Start();

            // 2) UI GÜNCELLEME TIMER -> 500 ms'de bir, kuyruktan verileri al ve grafiğe ekle
            _uiUpdateTimer = new DispatcherTimer();
            _uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;
            _uiUpdateTimer.Start();
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

        // =====================
        //  PLC'den veri oku
        // =====================
        private async void PlcReadingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Timer'ın kendi threadinde çalışır (UI thread'de değil)
            // Burada UI erişimi YAPMAYIN, sadece PLC'den değerleri okuyun ve queue'ya ekleyin

            if (_plcService.IsConnected)
            {
                try
                {
                    int displacement = await _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz);
                    double forceKg = await _plcService.ReadDWord(PlcRegisters.LOADCELL_2_DWORD);
                    double forceAxialN = forceKg * 9.81;

                    double forceTopKg = (await _plcService.ReadDWord(PlcRegisters.LOADCELL_TOP_DWORD)) / 1000.0;
                    double forceFemoralN = forceTopKg * 9.81;

                    // PLC verisini oluştur
                    var plcData = new PlcData
                    {
                        Timestamp = DateTime.UtcNow,
                        Displacement = displacement,
                        ForceAxialN = Math.Round(forceAxialN, 3),
                        ForceFemoralN = Math.Round(forceFemoralN, 3)
                    };

                    // Kuyruğa ekle
                    _plcDataQueue.Enqueue(plcData);
                }
                catch (Exception ex)
                {
                    // Hata yönetimi
                    Console.WriteLine($"PLC okunamadı: {ex.Message}");
                }
            }
        }

        // =====================
        //  UI'YI GÜNCELLE
        // =====================
        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Bu timer WPF'in UI thread'inde çalışır
            // Kuyrukta bekleyen tüm verileri tek seferde çekip grafiği güncelleyelim
            while (_plcDataQueue.TryDequeue(out PlcData data))
            {
                // ViewModel içindeki grafiğe veri ekleme
                // (Ham veriyi 100 ms'de topladık,
                //  UI'ya 500 ms'de bir ekleniyor)
                _viewModel.Time += 0.1; // örnek
                _viewModel.Force1 = data.ForceAxialN;
                _viewModel.Force2 = data.ForceFemoralN;
                _viewModel.Displacement1 = data.Displacement;

                // Grafiğe veri noktası ekle
                _viewModel.AddDataPoints();
            }
        }

        // Sayfa kapanırken Timer'ları durdurmayı unutmayın
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _plcReadingTimer?.Stop();
            _plcReadingTimer?.Dispose();

            _uiUpdateTimer?.Stop();
        }

        private void ResetZoomButton_Click(object sender, RoutedEventArgs e)
        {
            // Otomatik ölçekleme (auto-scale) moduna geçmek için x ekseni limitlerini kaldırıyoruz.
            _viewModel.XAxes[0].MinLimit = null;
            _viewModel.XAxes[0].MaxLimit = null;
        }



        private void StopTestButton_Click(object sender, RoutedEventArgs e)
        {
            _dataUpdateTimer.Stop();
            MessageBox.Show("Test durduruldu.");
        }
    }
}
