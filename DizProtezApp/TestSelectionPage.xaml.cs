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
    public partial class TestSelectionPage : Page
    {
        public TestSelectionPage()
        {
            InitializeComponent();
        }

        // Yalnızca sayısal girişe izin vermek için
        private void NumericInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        // Girişin sayısal olup olmadığını kontrol eden fonksiyon
        private bool IsTextNumeric(string text)
        {
            int output;
            return int.TryParse(text, out output);
        }

        // Alanların visibility durumlarını ayarlamak için fonksiyon
        private void SetVisibility(UIElement element, string value)
        {
            element.Visibility = string.IsNullOrWhiteSpace(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestList.SelectedItem != null)
            {
                var selectedTest = ((ListBoxItem)TestList.SelectedItem).Content.ToString();
                DisplayTestDetails(selectedTest);
            }
        }

        private void DisplayTestDetails(string testName)
        {
            // Varsayılan olarak alanlara değer atayıp visibility'lerini ayarlıyoruz
            switch (testName)
            {
                case "ASTM F1223 Anterior-Posterior (A-P) Test":
                    TestDescription.Text = "This test measures the anterior-posterior displacement limits.";
                    ForceInput.Text = "710"; // Default Force
                    SpeedInput.Text = "1.5"; // Default Speed
                    FlexionInput.Text = "0";  // Default Flexion
                    DisplacementNegative.Text = "-20";
                    DisplacementPositive.Text = "20";
                    break;

                case "ASTM F1223 Medial-Lateral (M-L) Test":
                    TestDescription.Text = "This test measures the medial-lateral displacement limits.";
                    ForceInput.Text = "710"; // Default Force
                    SpeedInput.Text = "1"; // Default Speed
                    FlexionInput.Text = "0";  // Default Flexion
                    DisplacementNegative.Text = "-20";
                    DisplacementPositive.Text = "20";
                    break;

                case "ASTM F1223 Internal-External Rotation Test":
                    TestDescription.Text = "This test measures the internal-external rotation constraints.";
                    ForceInput.Text = "710"; // Default Force
                    SpeedInput.Text = "3"; // Default Speed
                    FlexionInput.Text = "0";
                    TorsionalLoad.Text = "70";
                    MeasuredRotationOrTorqueMin.Text = "-20";
                    MeasuredRotationOrTorqueMax.Text = "20";
                    break;

                case "ASTM F2722 Rotational Stops of Tibial Baseplate":
                    TestDescription.Text = "This test evaluates rotational stops of the tibial baseplate.";
                    ForceInput.Text = "2275"; // Default Force
                    TorsionalLoadUnit.SelectedIndex = 0;
                    TorsionalLoad.Text = "2000";
                    break;

                case "ASTM F2723 Mobile Knee Dislocation Resistance Test":
                    TestDescription.Text = "This test evaluates dynamic separation resistance of the mobile knee tibial baseplate.";
                    ForceInput.Text = "2275"; // Default Axial Force
                    SpeedInput.Text = "450"; // Default Horizontal Force
                    FlexionInput.Text = "";  // Flexion not applicable
                    CycleCountInput.Text = "220000";  // Default Cycle Count
                    break;

                case "ASTM F2777 High-Flexion Durability and Deformation":
                    TestDescription.Text = "This test measures high-flexion durability and deformation.";
                    ForceInput.Text = "2275"; // Default Force
                    SpeedInput.Text = "";  // Speed not applicable
                    FlexionInput.Text = "-20";  // Default Flexion Min
                    FlexionInput.Text = "20"; // Default Flexion Max
                    break;

                case "ASTM F2724 Mobile Knee Dislocation Test":
                    TestDescription.Text = "This test evaluates the dislocation risk in mobile knee prostheses.";
                    ForceInput.Text = "450"; // Default Force
                    SpeedInput.Text = "3"; // Default Speed
                    break;

                default:
                    TestDescription.Text = "Select a test to view its details.";
                    ForceInput.Text = "";
                    SpeedInput.Text = "";
                    FlexionInput.Text = "";
                    CycleCountInput.Text = "";
                    break;
            }

            // Alanların visibility durumlarını belirliyoruz
            SetVisibility(ForceStack, ForceInput.Text);
            SetVisibility(SpeedStack, SpeedInput.Text);
            SetVisibility(FlexionAnglesStack, FlexionInput.Text);
            SetVisibility(TorsionalLoadStack, TorsionalLoad.Text);
            SetVisibility(CycleCountStack, CycleCountInput.Text);
            SetVisibility(DisplacementLimitsStack, DisplacementNegative.Text + DisplacementPositive.Text);
            SetVisibility(RotationOrTorqueLimitStack, MeasuredRotationOrTorqueMin.Text + MeasuredRotationOrTorqueMax.Text);
        }


        // ComboBox seçim olayını yönetmek
        private void MeasuredRotationOrTorqueUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedUnit = ((ComboBoxItem)MeasuredRotationOrTorqueUnit.SelectedItem).Content.ToString();

            // Seçilen birime göre gerekli işlemleri yapalım
            if (selectedUnit == "degree")
            {
                MeasuredRotationOrTorqueMin.Text = "-20";
                MeasuredRotationOrTorqueMax.Text = "20";
            }
            else if (selectedUnit == "Nm")
            {
                MeasuredRotationOrTorqueMin.Text = "-25";
                MeasuredRotationOrTorqueMax.Text = "25";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TestList.SelectedItem != null)
            {
                var selectedTest = ((ListBoxItem)TestList.SelectedItem).Content.ToString();

                // Giriş parametrelerini al
                var forceInput = ForceInput.Text;
                var speedInput = SpeedInput.Text;
                var flexionInput = FlexionInput.Text;
                var torsionalLoad = TorsionalLoad.Text;

                // TestMonitoringPage'e parametreleri aktararak yönlendir
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(new TestMonitoringPage(selectedTest, forceInput, speedInput, flexionInput, torsionalLoad));
            }
            else
            {
                MessageBox.Show("Please select a test before starting.");
            }
        }


        private void OpenPLCControlPanel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new ControlPanelPage()); // Yeni oluşturacağınız sayfaya yönlendirin
        }


    }
}
