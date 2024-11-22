using DizProtezApp.Services;
using System.Windows;

namespace DizProtezApp
{
    public partial class MainWindow : Window
    {
        private readonly ServiceManager _serviceManager;

        // Constructor to receive ServiceManager
        public MainWindow(ServiceManager serviceManager)
        {
            InitializeComponent();
            _serviceManager = serviceManager;

            // Initialize services if needed
            _serviceManager.InitializeServices();

            // Load the starting page into the Frame
            MainFrame.Navigate(new TestSelectionPage());
        }
    }
}
