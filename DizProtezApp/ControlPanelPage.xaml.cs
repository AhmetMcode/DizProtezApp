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
    /// Interaction logic for ControlPanelPage.xaml
    /// </summary>
    public partial class ControlPanelPage : Page
    {
        public ControlPanelPage()
        {
            InitializeComponent();
        }

        private void Etkinlestir_Servo1_Button(object sender, RoutedEventArgs e)
        {

        }

        private void MotorDurdur_Servo1_Button(object sender, RoutedEventArgs e)
        {

        }

        private void MotorReset_Servo1_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GoPosition_Servo1_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GoHome_Servo1_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GeriJog_Servo1_Button(object sender, RoutedEventArgs e)
        {

        }

        private void İleriJog_Servo1_Button(object sender, RoutedEventArgs e)
        {

        }

        private void MotorDurdur_Servo2_Button(object sender, RoutedEventArgs e)
        {

        }

        private void Etkinlestir_Servo2_Button(object sender, RoutedEventArgs e)
        {

        }

        private void MotorReset_Servo2_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GoPosition_Servo2_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GoHome_Servo2_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GeriJog_Servo2_Button(object sender, RoutedEventArgs e)
        {

        }

        private void İleriJog_Servo2_Button(object sender, RoutedEventArgs e)
        {

        }

        private void Etkinlestir_Servo3_Button(object sender, RoutedEventArgs e)
        {

        }

        private void MotorDurdur_Servo3_Button(object sender, RoutedEventArgs e)
        {

        }

        private void MotorReset_Servo3_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GoPosition_Servo3_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GoHome_Servo3_Button(object sender, RoutedEventArgs e)
        {

        }

        private void GeriJog_Servo3_Button(object sender, RoutedEventArgs e)
        {

        }

        private void İleriJog_Servo3_Button(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
