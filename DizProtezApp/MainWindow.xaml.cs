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

            // SqlService'i TestSelectionPage'e geçir
            MainFrame.Navigate(new TestSelectionPage(_serviceManager.SqlService));
        }
    }
}
