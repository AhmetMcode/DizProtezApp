using DizProtezApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace DizProtezApp
{
    public partial class ControlPanelPage : Page
    {
        private readonly PlcService _plcService;
        private readonly DispatcherTimer _refreshTimer;
        private bool _isTextBoxFocused = false;

        public ServoViewModel ViewModel { get; set; }

        public ControlPanelPage()
        {
            InitializeComponent();
            // App sınıfından PlcService'i alın
            _plcService = ((App)Application.Current).ServiceProvider.GetRequiredService<PlcService>();
            ViewModel = new ServoViewModel();


            DataContext = ViewModel;

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(40)
            };

            _refreshTimer.Tick += (s, e) =>
            {
                RefreshServoData();
            };

            _refreshTimer.Start();
        }


        private void UpdateButtonState(Button button, bool isActive, string activeContent, string inactiveContent, Color activeColor, Color inactiveColor)
        {
            if (button == null) return;

            button.Content = isActive ? activeContent : inactiveContent;
            button.Background = new SolidColorBrush(isActive ? activeColor : inactiveColor);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Timer'ı durdur
            _refreshTimer.Stop();

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

                    // TextBox'a göre ilgili PLC register ve değeri belirle
                    if (textBox.Name == "ManualSpeedTextBox")
                    {
                        // Manual Speed işlemleri
                        int manualSpeed = (int)(ViewModel.Servo1ManualSpeed * 10); // Ölçekleme
                        _plcService.WriteDWord(PlcRegisters.S1_ManuelHız, manualSpeed);
                    }
                    else if (textBox.Name == "Servo1Acc")
                    {
                        // Acc işlemleri
                        int accValue = (int)(ViewModel.Servo1Acc * 10); // Ölçekleme
                        _plcService.WriteWord(PlcRegisters.S1_Acc_time, accValue);
                    }
                    else if (textBox.Name == "Servo1Dcc")
                    {
                        // Dcc işlemleri
                        int dccValue = (int)(ViewModel.Servo1Dcc * 10); // Ölçekleme
                        _plcService.WriteWord(PlcRegisters.S1_Dec_time, dccValue);
                    }

                    else if (textBox.Name == "Servo1ForwardJogSpeed")
                    {
                        int forwardSpeedValue = (int)(ViewModel.Servo1ForwardJogSpeed * 10); // Ölçeklendirme
                        _plcService.WriteDWord(PlcRegisters.S1_FWD_Speed, forwardSpeedValue);
                    }
                    else if (textBox.Name == "Servo1ReverseJogSpeed")
                    {
                        int reverseSpeedValue = (int)(ViewModel.Servo1ReverseJogSpeed * 10); // Ölçeklendirme
                        _plcService.WriteDWord(PlcRegisters.S1_REV_Speed, reverseSpeedValue);
                    }
                    else if (textBox.Name == "Servo1HomeFirstSpeed")
                    {
                        int HomeFirstSpeed = (int)(ViewModel.Servo1HomeFirstSpeed * 10);
                        _plcService.WriteWord(PlcRegisters.S1_First_Speed, HomeFirstSpeed);
                    }
                    else if (textBox.Name == "Servo1HomeSecondSpeed")
                    {
                        int HomeSecondSpeed = (int)(ViewModel.Servo1HomeSecondSpeed * 10);
                        _plcService.WriteWord(PlcRegisters.S1_Second_Speed, HomeSecondSpeed);
                    }
                    else if (textBox.Name == "deneme")
                    {
                        int ddeneme = (int)(ViewModel.deneme * 1000);
                        _plcService.WriteDWord(PlcRegisters.DENEME, ddeneme);
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
            // Eğer PLC bağlantısı yoksa işlemi durdur
            if (!_plcService.IsConnected)
            {
                Console.WriteLine("PLC bağlantısı yok, veriler güncellenmiyor.");
                return;
            }

            try
            {
                // Eğer TextBox odaklanmışsa, yalnızca kullanıcı girişini korumak için hız veya parametre değerlerini güncellemeyi durdur
                if (!_isTextBoxFocused)
                {
                    // TextBox ile ilgili değerlerin güncellenmesi
                    ViewModel.Servo1ManualSpeed = _plcService.ReadDWord(PlcRegisters.S1_ManuelHız) / 10.0;
                    ViewModel.Servo1Acc = _plcService.ReadWord(PlcRegisters.S1_Acc_time) / 10.0;
                    ViewModel.Servo1Dcc = _plcService.ReadWord(PlcRegisters.S1_Dec_time) / 10.0;
                    ViewModel.Servo1ForwardJogSpeed = _plcService.ReadDWord(PlcRegisters.S1_FWD_Speed) / 10.0;
                    ViewModel.Servo1ReverseJogSpeed = _plcService.ReadDWord(PlcRegisters.S1_REV_Speed) / 10.0;
                    ViewModel.Servo1HomeFirstSpeed = _plcService.ReadWord(PlcRegisters.S1_First_Speed) / 10.0;
                    ViewModel.Servo1HomeSecondSpeed = _plcService.ReadWord(PlcRegisters.S1_Second_Speed) / 10.0;
                    ViewModel.deneme = _plcService.ReadDWord(PlcRegisters.DENEME) / 1000.0;
                }

                // PLC'den pozisyon ve buton durumlarının okunması
                ViewModel.Servo1CurrentPosition = _plcService.ReadDWord(PlcRegisters.S1_Anlık_Poz) / 1000.0;

                // Buton durumlarını güncelle
                bool isOtoManuelActive = _plcService.ReadBool(PlcRegisters.MAN_OTO_SEC_BIT);
                bool isServoOn = _plcService.ReadBool(PlcRegisters.S1_Servo_ON);
                bool isHomeActive = _plcService.ReadBool(PlcRegisters.s1_zrnc_Start);
                bool isInitcActive = _plcService.ReadBool(PlcRegisters.S1_Start_INITC);
                bool isCasdActive = _plcService.ReadBool(PlcRegisters.S1_Start_CASD);

                // Butonları güncelle
                UpdateButtonState(EtkinlestirOtoManuelButton, isOtoManuelActive, "Manuel", "Otomatik", Colors.Green, Colors.Red);
                UpdateButtonState(EtkinlestirServo1Button, isServoOn, "Servo ON", "Servo OFF", Colors.Green, Colors.Red);
                UpdateButtonState(GoHomeServo1Button, isHomeActive, "Home Gidiyor", "Home Gönder", Colors.LightBlue, Colors.Blue);
                UpdateButtonState(ToggleStartInitcButton, isInitcActive, "INITC Aktif", "INITC Pasif", Colors.Green, Colors.Red);
                UpdateButtonState(ToggleStartCasdButton, isCasdActive, "CASD Aktif", "CASD Pasif", Colors.Green, Colors.Red);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PLC verileri okunamadı: {ex.Message}");
            }
        }



        private void ToggleStartInitc_Button(object sender, RoutedEventArgs e)
        {
            if (!_plcService.IsConnected)
            {
                MessageBox.Show("PLC bağlantısı yok. İşlem gerçekleştirilemiyor.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                // Mevcut durumu oku
                bool currentState = _plcService.ReadBool(PlcRegisters.S1_Start_INITC);
                bool newState = !currentState;

                // Yeni durumu PLC'ye yaz
                _plcService.WriteBool(PlcRegisters.S1_Start_INITC, newState);

                // Buton durumunu güncelle
                UpdateButtonState(ToggleStartInitcButton, newState, "INITC Aktif", "INITC Pasif", Colors.Green, Colors.Red);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"INITC işleminde hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ToggleStartCasd_Button(object sender, RoutedEventArgs e)
        {
            if (!_plcService.IsConnected)
            {
                MessageBox.Show("PLC bağlantısı yok. İşlem gerçekleştirilemiyor.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                // PLC'den mevcut durumu oku
                bool currentState = _plcService.ReadBool(PlcRegisters.S1_Start_CASD);
                bool newState = !currentState;

                // Yeni durumu PLC'ye yaz
                _plcService.WriteBool(PlcRegisters.S1_Start_CASD, newState);

                // Buton durumunu güncelle
                UpdateButtonState(ToggleStartCasdButton, newState, "CASD Aktif", "CASD Pasif", Colors.Green, Colors.Red);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CASD işleminde bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void GoPosition_Servo1_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pozisyon değerini PLC'ye yaz
                int servo1TargetPosition = (int)(ViewModel.Servo1TargetPosition * 1000);
                _plcService.WriteDWord(PlcRegisters.S1_Pozisyon, servo1TargetPosition);

                // Gidilecek pozisyon başlatma bitini aktif et
                _plcService.WriteBool(PlcRegisters.S1_DDRVAC_Start, true);

                // Butonun içeriğini ve rengini güncelle
                var button = sender as Button;
                if (button != null)
                {
                    button.Content = "Gidiyor...";
                    button.Background = new SolidColorBrush(Colors.Orange);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo pozisyona gidilemiyor: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Etkinlestir_Servo1_Button(object sender, RoutedEventArgs e)
        {
            if (!_plcService.IsConnected)
            {
                MessageBox.Show("PLC bağlantısı yok. İşlem gerçekleştirilemiyor.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                // PLC'den mevcut durumu oku ve tersini yaz
                bool currentState = _plcService.ReadBool(PlcRegisters.S1_Servo_ON);
                bool newState = !currentState;
                _plcService.WriteBool(PlcRegisters.S1_Servo_ON, newState);

                // Buton durumunu güncelle
                UpdateButtonState(EtkinlestirServo1Button, newState, "Servo ON", "Servo OFF", Colors.Green, Colors.Red);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo 1 işleminde hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void MotorDurdur_Servo1_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                _plcService.WriteBool(PlcRegisters.S1_Servo_DURDUR, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo 1 durdurulamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MotorReset_Servo1_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                _plcService.WriteBool(PlcRegisters.S1_Servo_RESET, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Servo 1 sıfırlanamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                // PLC'den mevcut durumu oku ve tersini yaz
                bool currentState = _plcService.ReadBool(PlcRegisters.s1_zrnc_Start);
                bool newState = !currentState;

                // Yeni durumu PLC'ye yaz
                _plcService.WriteBool(PlcRegisters.s1_zrnc_Start, newState);

                // Buton durumunu güncelle
                UpdateButtonState(GoHomeServo1Button, newState, "Home Gidiyor", "Home Gönder", Colors.LightBlue, Colors.Blue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Home gönderme işleminde hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void GeriJog_MouseDown(object sender, RoutedEventArgs e)
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

        private void GeriJog_MouseUp(object sender, RoutedEventArgs e)
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


        private void İleriJog_MouseDown(object sender, RoutedEventArgs e)
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

        private void İleriJog_MouseUp(object sender, RoutedEventArgs e)
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


        private void OtoManuelButon(object sender, RoutedEventArgs e)
        {
            if (!_plcService.IsConnected)
            {
                MessageBox.Show("PLC bağlantısı yok. İşlem gerçekleştirilemiyor.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                // PLC'den mevcut durumu oku ve tersini yaz
                bool currentState = _plcService.ReadBool(PlcRegisters.MAN_OTO_SEC_BIT);
                bool newState = !currentState;
                _plcService.WriteBool(PlcRegisters.MAN_OTO_SEC_BIT, newState);

                // Buton durumunu güncelle
                UpdateButtonState(EtkinlestirOtoManuelButton, newState, "Manuel", "Otomatik", Colors.Green, Colors.Red);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Oto/Manuel işleminde hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


}
