using DizProtezApp.Models;
using DizProtezApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace DizProtezApp
{
    /// <summary>
    /// Interaction logic for TestJogMonitoringPage.xaml
    /// </summary>
    public partial class TestJogMonitoringPage : Page
    {
        private readonly string _testName;
        private readonly int _testId;
        private readonly PlcService _plcService;
        private DispatcherTimer _dataUpdateTimer;
        private bool _isTextBoxFocused = false;

        public TestJogMonitoringViewModel _viewModel { get; set; }

        public TestJogMonitoringPage(string testName, int testId)
        {
            InitializeComponent();
            _testName = testName;
            _testId = testId;

            // Test adını başlıkta göster
            TestNameTextBlock1.Text = $"Selected Test: {_testName}";

            // App sınıfından PlcService'i alın
            _plcService = ((App)Application.Current).ServiceProvider.GetRequiredService<PlcService>();
            _viewModel = new TestJogMonitoringViewModel();
            DataContext = _viewModel;

            if (!_plcService.IsConnected)
            {
                MessageBox.Show("PLC bağlantısı yok. İşlem gerçekleştirilemiyor.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                _plcService.WriteBool(PlcRegisters.S1_Start_INITC, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _dataUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _dataUpdateTimer.Tick += (s, e) =>
            {
                RefreshServo1PositionAndLoadcell();
                RefreshServoData();
            };
            _dataUpdateTimer.Start();
        }


        private void BackButton_Click1(object sender, RoutedEventArgs e)
        {
            // Timer'ı durdur
            _dataUpdateTimer.Stop();

            // Geri git
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _isTextBoxFocused = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _isTextBoxFocused = false;

            if (sender is TextBox textBox)
            {
                try
                {
                    // Binding'i zorla güncelle
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();

                    if (textBox.Name == "Servo1JogSpeed")
                    {
                        // Mutlak değeri alıp 10 ile çarparak ölçeklendirme
                        int speedValue = (int)(Math.Abs(_viewModel.Servo1JogSpeedBind) * 10);

                        // Reverse için negatif, Forward için pozitif değerler
                        int reverseSpeedValue = -speedValue;
                        int forwardSpeedValue = speedValue;

                        // PLC'ye yazma işlemleri
                        _plcService.WriteDWord(PlcRegisters.S1_REV_Speed, reverseSpeedValue);
                        _plcService.WriteDWord(PlcRegisters.S1_FWD_Speed, forwardSpeedValue);
                    }

                    else if (textBox.Name == "VerticalForceInput1")
                    {
                        // Acc işlemleri
                        int accValue = (int)(_viewModel.VerticalForceInput1 * 10); // Ölçekleme
                        //_plcService.WriteWord(PlcRegisters.S1_Acc_time, accValue);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Değer PLC'ye yazılamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void RefreshServoData()
        {
            // Eğer TextBox odaklanmışsa ya da PLC bağlantısı yoksa güncellemeyi durdur
            if (_isTextBoxFocused || !_plcService.IsConnected)
                return;

            try
            {
                // PLC'den değerleri oku ve mutlak değerlerini al
                // PLC'den ham değerleri oku (negatif/pozitif korunarak)
                double originalForward = (await _plcService.ReadDWord(PlcRegisters.S1_FWD_Speed)) / 10.0;
                double originalReverse = (await _plcService.ReadDWord(PlcRegisters.S1_REV_Speed)) / 10.0;
                // Mutlak değerleri al
                double absForward = Math.Abs(originalForward);
                double absReverse = Math.Abs(originalReverse);
                // Değerleri karşılaştır ve uygun formatı seç
                if (absForward == absReverse)
                {
                    // Değerler eşitse sadece sayıyı göster
                    _viewModel.Servo1JogSpeedBind = absForward;
                }
                else
                {
                    // Hangi yönde daha büyük olduğunu kontrol et
                    string sign = originalForward > -originalReverse ? "+" : "-";
                    _viewModel.Servo1JogSpeedBind = double.Parse($"{sign}{Math.Max(absForward, absReverse)}");
                }

                _viewModel.Servo1CurrentPosition = (await _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz)) / 1000.0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PLC verileri okunamadı: {ex.Message}");
            }
        }

        private async void RefreshServo1PositionAndLoadcell()
        {
            if (!_plcService.IsConnected) return;

            try
            {
                await Task.Run(async () =>
                {
                    // Displacement okuma (örnekte integer olarak kalıyor)
                    int displacement = await _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz);

                    // Force değerini doğru şekilde oku ve Newton'a çevir
                    double forceKg = await _plcService.ReadDWord(PlcRegisters.LOADCELL_2_DWORD);
                    double forceAxialN = forceKg * 9.81; // Newton'a çevir

                    // LOADCELL_TOP_DWORD değeri (Force2) okunup Newton'a çevriliyor
                    double forceTopKg = await _plcService.ReadDWord(PlcRegisters.LOADCELL_TOP_DWORD) / 1000.0;
                    double forceFemoralN = forceTopKg * 9.81;

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
                Console.WriteLine($"Veri okuma hatası: {ex.Message}");
            }
        }

        private void UpdateButtonState(Button button, bool isActive, string activeContent, string inactiveContent, Color activeColor, Color inactiveColor)
        {
            if (button == null) return;

            button.Content = isActive ? activeContent : inactiveContent;
            button.Background = new SolidColorBrush(isActive ? activeColor : inactiveColor);
        }
        private void GoHomeServo1_Button(object sender, RoutedEventArgs e)
        {
            if (!_plcService.IsConnected)
            {
                MessageBox.Show("PLC bağlantısı yok. İşlem gerçekleştirilemiyor.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                _plcService.WriteBool(PlcRegisters.s1_zrnc_Start, true);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Home gönderme işleminde hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GeriJog_MouseDown1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Bit'i 1 yap
                _plcService.WriteBool(PlcRegisters.S1_DPLSVC_REV_Start, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo 1 geri jog başlatılamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GeriJog_MouseUp1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Bit'i 0 yap
                _plcService.WriteBool(PlcRegisters.S1_DPLSVC_REV_Start, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo 1 geri jog durdurulamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void İleriJog_MouseDown1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Bit'i 1 yap
                _plcService.WriteBool(PlcRegisters.S1_DPLSVC_FWD_Start, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo 1 ileri jog başlatılamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void İleriJog_MouseUp1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Bit'i 0 yap
                _plcService.WriteBool(PlcRegisters.S1_DPLSVC_FWD_Start, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo 1 ileri jog durdurulamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdjustParameterButton_Click1(object sender, RoutedEventArgs e)
        {
            string testName = _testName; // Mevcut test adı
            int testId = _testId;
            var sqlService = ((App)Application.Current).ServiceProvider.GetRequiredService<SqlService>();

            // TestNameDialog'a ServiceManager ve test adı parametrelerini geçin
            var testNameDialog = new TestNameDialog(sqlService, testName, _testId, "Specimen1");

            if (testNameDialog.ShowDialog() == true)
            {
                string specimenName = testNameDialog.SpecimenName;
                var parameters = testNameDialog.Parameters;

                // Parametreleri göster veya işle
                foreach (var param in parameters)
                {
                    Console.WriteLine($"{param.Key}: {param.Value}");
                }
            }
        }

    }
}
