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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Dependency Injection için servisleri oluşturun
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // MainWindow'u başlatın
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // DbContext yapılandırması
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=.\\;Database=DizProtezDB;Trusted_Connection=True;TrustServerCertificate=true;"));
            
            // Servisleri ekleyin
            services.AddSingleton<PlcService>();
            services.AddTransient<SqlService>();
            services.AddTransient<ServiceManager>();

            // MainWindow'u servis olarak ekleyin
            services.AddTransient<MainWindow>();
        }
    }
}
