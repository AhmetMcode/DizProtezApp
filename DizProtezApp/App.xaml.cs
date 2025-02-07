using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DizProtezApp.Data;
using DizProtezApp.Services;

namespace DizProtezApp
{
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Dependency Injection için servisleri oluşturun
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // ServiceManager'i başlat
            var serviceManager = ServiceProvider.GetRequiredService<ServiceManager>();
            await serviceManager.InitializeServices();

            // MainWindow'u başlatın
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }


        private void ConfigureServices(IServiceCollection services)
        {
            // DbContext yapılandırması
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=104.247.167.18\\MSSQLSERVER2019;Database=gurumeng_dizprotez;user id=gurumeng_dizprotez;password=Wert789.;TrustServerCertificate=true;"));

            // Servisleri ekleyin
            services.AddSingleton<PlcService>();
            services.AddSingleton<SqlService>();
            services.AddSingleton<ServiceManager>();

            // MainWindow'u servis olarak ekleyin
            services.AddTransient<MainWindow>();
        }
    }
}
