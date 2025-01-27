using DizProtezApp.Data;
using DizProtezApp.Models;
using DizProtezApp.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DizProtezApp
{
    public partial class TestNameDialog : Window
    {
        private readonly SqlService _sqlService;
        private readonly PlcService _plcService;

        private string _defaultTestName;
        private readonly int _testId;
        public string TestName { get; private set; } // Kullanıcının girdiği test adı
        public string SpecimenName { get; private set; } // Kullanıcının girdiği specimen adı
        public Dictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();

        public TestNameDialog(SqlService sqlService, string defaultTestName, int testId, string defaultSpecimenName = "Specimen1")
        {
            InitializeComponent();
            _sqlService = sqlService;
            _plcService = new PlcService();
            _defaultTestName = defaultTestName;
            _testId = testId;
            TestNameInput.Text = defaultTestName; // Varsayılan test adını göster
            SpecimenNameInput.Text = defaultSpecimenName; // Varsayılan specimen adını göster

            // Test türüne göre parametre alanlarını oluştur
            CreateDynamicParameterFields(defaultTestName);
        }

        private void CreateDynamicParameterFields(string testName)
        {
            DynamicParametersPanel.Children.Clear();

            var parameters = GetParametersForTest(testName);

            foreach (var param in parameters)
            {
                var label = new TextBlock
                {
                    Text = param.Key,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var textBox = new TextBox
                {
                    Text = param.Value,
                    Tag = param.Key,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                DynamicParametersPanel.Children.Add(label);
                DynamicParametersPanel.Children.Add(textBox);
            }
        }

        private Dictionary<string, string> GetParametersForTest(string testName)
        {
            switch (testName)
            {
                case "ASTM F1223 (A-P) (static) Anterior-Posterior":
                    return new Dictionary<string, string>
            {
                { "Flexion (°)", "0" },
                { "Force (N)", "710" },
                { "Speed (mm/s)", "1.5" },
                { "Displacement Negative", "-20" },
                { "Displacement Positive", "20" }
            };
                case "ASTM F2723 (A-P) (dynamic) Anterior-Posterior":
                    return new Dictionary<string, string>
            {
                { "Flexion (°)", "0" },
                { "Force (N)", "710" },
                { "Speed (mm/s)", "1.5" },
                { "Displacement Negative", "-20" },
                { "Displacement Positive", "20" }
            };
                case "ASTM F1223 (M-L) (static) Medial-Lateral":
                    return new Dictionary<string, string>
            {
                { "Flexion (°)", "0" },
                { "Force (N)", "710" },
                { "Speed (mm/s)", "1.5" },
                { "Displacement Negative", "-20" },
                { "Displacement Positive", "20" }
            };
                case "ASTM F1223 Internal-External Rotation Test":
                    return new Dictionary<string, string>
            {
                { "Flexion (°)", "0" },
                { "Force (N)", "710" },
                { "Speed (°)", "3" },
                { "Displacement Negative", "-20" },
                { "Displacement Positive", "20" }
            };
                case "ASTM F2722 Rotational Stops of Tibial Baseplate":
                    return new Dictionary<string, string>
            {
                { "Flexion (°)", "0" },
                { "Force (N)", "710" },
                { "Speed (mm/s)", "1.5" },
                { "Displacement Negative", "-20" },
                { "Displacement Positive", "20" },
                { "Cycle", "220000" }
            };


                case "ASTM F2777 High-Flexion Durability and Deformation":
                    return new Dictionary<string, string>
            {
                { "Force (N)", "2275" },
                { "Torsional Load (Nm)", "2000" }
            };

                case "ASTM F2724 Mobile Knee Dislocation Test":
                    return new Dictionary<string, string>
            {
                { "Force (N)", "2275" },
                { "Flexion Min (°)", "-20" },
                { "Flexion Max (°)", "20" }
            };

                default:
                    return new Dictionary<string, string>();
            }
        }

        private void ActivateTestBit(int testId)
        {
            switch (testId)
            {
                case 1:
                    _plcService.WriteBool(PlcRegisters.TEST1_BASLA_DURDUR, true);
                    break;
                case 2:
                    _plcService.WriteBool(PlcRegisters.TEST2_BASLA_DURDUR, true);
                    break;
                case 3:
                    _plcService.WriteBool(PlcRegisters.TEST3_BASLA_DURDUR, true);
                    break;
                case 4:
                    _plcService.WriteBool(PlcRegisters.TEST4_BASLA_DURDUR, true);
                    break;
                case 5:
                    _plcService.WriteBool(PlcRegisters.TEST5_BASLA_DURDUR, true);
                    break;
                default:
                    throw new Exception("Geçersiz testId.");
            }
        }

        private bool IsDynamicTestId(int testId)
        {
            var dynamicTestIds = new List<int> { 2, 4 }; // Dinamik testlerin testId değerleri
            return dynamicTestIds.Contains(testId);
        }


        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TestName = TestNameInput.Text; // Test adı
            SpecimenName = string.IsNullOrWhiteSpace(SpecimenNameInput.Text)
                ? "Specimen1"
                : SpecimenNameInput.Text;

            // Dinamik parametreleri topla
            foreach (var child in DynamicParametersPanel.Children)
            {
                if (child is TextBox textBox && textBox.Tag is string key)
                {
                    Parameters[key] = textBox.Text;
                }
            }


            try
            {
                // Test parametrelerini PLC'ye aktar
                WriteParametersToPlc();

                // SQL'e kaydet
                await SaveTestToDatabase();

                // TestMonitoringPage'e yönlendir
                // testId'ye göre yönlendirme
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (IsDynamicTestId(_testId))
                {
                    mainWindow.MainFrame.Navigate(new DynamicTestMonitoringPage(_sqlService, _plcService, TestName));
                }
                else
                {
                    mainWindow.MainFrame.Navigate(new TestMonitoringPage(_sqlService, _plcService, TestName));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test başlatılamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DialogResult = true;
            Close();
        }

        private void WriteParametersToPlc()
        {
            try
            {
                // Displacement Positive ve Negative için ilgili pozisyonları PLC'ye yaz
                if (Parameters.TryGetValue("Displacement Positive", out var forwardPos))
                {
                    _plcService.WriteWord(PlcRegisters.S1_TEST_ILERI_POZ, int.Parse(forwardPos));
                }
                if (Parameters.TryGetValue("Displacement Negative", out var reversePos))
                {
                    _plcService.WriteWord(PlcRegisters.S1_TEST_GERI_POZ, int.Parse(reversePos));
                }

                // Speed (mm/s) için ileri ve geri hızları PLC'ye yaz
                if (Parameters.TryGetValue("Speed (mm/s)", out var speed))
                {
                    // 1.5 değeri parse edilebileceği için double dönüştürüp scale edebilirsiniz
                    int scaledSpeed = (int)(double.Parse(speed) * 10); // Örnek ölçekleme
                    _plcService.WriteWord(PlcRegisters.S1_TEST_ILERI_HIZ, scaledSpeed);
                    _plcService.WriteWord(PlcRegisters.S1_TEST_GERI_HIZ, scaledSpeed);
                }

                // MANUEL-OTOMATİK seçimi için MAN_OTO_SEC_BIT bitini true yap
                _plcService.WriteBool(PlcRegisters.MAN_OTO_SEC_BIT, true);

                // Seçilen testId'ye göre ilgili TESTX_BASLA_DURDUR bitini true yap
                ActivateTestBit(_testId);

                Console.WriteLine("Test parametreleri başarıyla PLC'ye yazıldı.");
            }
            catch (Exception ex)
            {
                throw new Exception($"PLC'ye parametre yazılamadı: {ex.Message}");
            }
        }



        private async Task SaveTestToDatabase()
        {
            try
            {
                string[] specimenNames = { SpecimenName };

                await _sqlService.AddTestWithSpecimensAsync(
                    testName: TestName,
                    testType: $"{_testId}:{_defaultTestName}", // Test türünü belirleyebilirsiniz
                    testParameters: Parameters,
                    specimenNames: specimenNames
                );

                MessageBox.Show("Test ve Specimen'lar başarıyla kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanına kaydetme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // İşlemi iptal et
            Close();
        }
    }
}
