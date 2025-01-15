using DizProtezApp.Data;
using DizProtezApp.Models;
using DizProtezApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DizProtezApp
{
    /// <summary>
    /// Interaction logic for TestSelectionPage.xaml
    /// </summary>
    /// 

    public partial class TestSelectionPage : Page
    {

        private readonly SqlService _sqlService;
        private List<TestRecord> _allTestRecords;

        public TestSelectionPage(SqlService sqlService)
        {
            InitializeComponent();
            _sqlService = sqlService;

            // Test kayıtlarını yükle
            LoadTestRecords();
        }

        // Test kayıtlarını yükle ve listeyi bağla
        private void LoadTestRecords()
        {
            // Örnek olarak SQL servisinden test kayıtlarını alıyoruz
            _allTestRecords = _sqlService.GetTestRecords().OrderByDescending(tr => tr.CreatedAt).ToList();
            TestRecordsList.ItemsSource = _allTestRecords;
        }

        // Arama kutusu metni değiştiğinde çağrılır
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Arama kutusundaki metni al
            string searchText = SearchBox.Text.ToLower();

            // Placeholder görünürlüğünü kontrol et
            PlaceholderLabel.Visibility = string.IsNullOrEmpty(searchText)
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Arama metnine göre test kayıtlarını filtrele
            var filteredRecords = _allTestRecords
                .Where(tr => tr.TestName.ToLower().Contains(searchText))
                .ToList();

            // Filtrelenmiş kayıtları ListView'e bağla
            TestRecordsList.ItemsSource = filteredRecords;
        }

        private void TestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestList.SelectedItem != null)
            {
                // Seçilen testi ve sırasını al
                var selectedItem = (ListBoxItem)TestList.SelectedItem;
                var testName = selectedItem.Content.ToString();
                var testId = int.Parse(selectedItem.Tag.ToString());

                // Test detaylarını göster
                DisplayTestDetails(testName);

                // Konsola yazdır (isteğe bağlı)
                Console.WriteLine($"Seçilen Test: {testName}, Sıra: {testId}");
            }
        }

        private void DisplayTestDetails(string testName)
        {

            switch (testName)
            {
                case "ASTM F1223 Anterior-Posterior (A-P) Test":
                    TestDescription.Text = "This test measures the anterior-posterior displacement limits.";
                    break;

                case "ASTM F1223 Medial-Lateral (M-L) Test":
                    TestDescription.Text = "This test measures the medial-lateral displacement limits.";
                    break;

                case "ASTM F1223 Internal-External Rotation Test":
                    TestDescription.Text = "This test measures the internal-external rotation constraints.";
                    break;

                case "ASTM F2722 Rotational Stops of Tibial Baseplate":
                    TestDescription.Text = "This test evaluates rotational stops of the tibial baseplate.";
                    break;

                case "ASTM F2723 Mobile Knee Dislocation Resistance Test":
                    TestDescription.Text = "This test evaluates dynamic separation resistance of the mobile knee tibial baseplate.";
                    break;

                case "ASTM F2777 High-Flexion Durability and Deformation":
                    TestDescription.Text = "This test measures high-flexion durability and deformation.";
                    break;

                case "ASTM F2724 Mobile Knee Dislocation Test":
                    TestDescription.Text = "This test evaluates the dislocation risk in mobile knee prostheses.";
                    break;

                default:
                    TestDescription.Text = "Select a test to view its details.";
                    break;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TestList.SelectedItem != null)
            {
                // Seçilen testi ve sırasını al
                var selectedItem = (ListBoxItem)TestList.SelectedItem;
                var testName = selectedItem.Content.ToString();
                var testId = int.Parse(selectedItem.Tag.ToString());

                // Konsola yazdır (isteğe bağlı)
                Console.WriteLine($"Seçilen Test: {testName}, Sıra: {testId}");

                // TestJogMonitoringPage'e geçiş yap
                var jogMonitoringPage = new TestJogMonitoringPage(testName, testId);
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(jogMonitoringPage);
            }
            else
            {
                MessageBox.Show("Please select a test before continuing.");
            }
        }




        private void OpenPLCControlPanel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new ControlPanelPage()); // Yeni oluşturacağınız sayfaya yönlendirin
        }


    }
}
