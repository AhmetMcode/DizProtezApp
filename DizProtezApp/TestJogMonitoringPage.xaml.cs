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

            _plcService = new PlcService();
            _viewModel = new TestJogMonitoringViewModel();
            DataContext = _viewModel;


            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _dataUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
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

                    if (textBox.Name == "Servo1BackJogSpeed1")
                    {
                        int reverseSpeedValue = (int)(_viewModel.Servo1ReverseJogSpeed * 10); // Ölçeklendirme
                        _plcService.WriteDWord(PlcRegisters.S1_REV_Speed, reverseSpeedValue);
                    }
                    else if (textBox.Name == "Servo1ForwardJogSpeed1")
                    {
                        int forwardSpeedValue = (int)(_viewModel.Servo1ForwardJogSpeed * 10); // Ölçeklendirme
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

        private void RefreshServoData()
        {
            // Eğer TextBox odaklanmışsa ya da PLC bağlantısı yoksa güncellemeyi durdur
            if (_isTextBoxFocused || !_plcService.IsConnected)
                return;

            try
            {
                _viewModel.Servo1ForwardJogSpeed = _plcService.ReadDWord(PlcRegisters.S1_FWD_Speed) / 10.0;
                _viewModel.Servo1ReverseJogSpeed = _plcService.ReadDWord(PlcRegisters.S1_REV_Speed) / 10.0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PLC verileri okunamadı: {ex.Message}");
            }
        }

        private async void RefreshServo1PositionAndLoadcell()
        {
            // PLC bağlantısı yoksa çalışmayı durdur
            if (!_plcService.IsConnected)
                return;

            try
            {
                await Task.Run(() =>
                {
                    int displacement = _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz);
                    float gramValue = _plcService.ReadReal(PlcRegisters.LOADCELL_1_DWORD);
                    double force = (gramValue / 1000.0) * 9.80665;

                    // ViewModel değerlerini güncelle
                    _viewModel.Displacement1 = displacement;
                    _viewModel.Force1 = Math.Round(force, 3);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veriler okunamadı: {ex.Message}");
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
