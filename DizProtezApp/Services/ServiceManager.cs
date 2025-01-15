using System;
using System.Threading.Tasks;
using DizProtezApp.Data;

namespace DizProtezApp.Services
{
    public class ServiceManager
    {
        private readonly PlcService _plcService;
        public SqlService SqlService { get; }

        public ServiceManager(PlcService plcService, ApplicationDbContext dbContext)
        {
            _plcService = plcService;
            SqlService = new SqlService(dbContext);
        }

        public async Task InitializeServices()
        {
            try
            {
                // PLC bağlantısını başlat
                bool isPlcConnected = await _plcService.Connect();
                if (!isPlcConnected)
                {
                    Console.WriteLine("PLC bağlantısı başarısız oldu. Devam ediliyor...");
                }
                else
                {
                    _plcService.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hizmetleri başlatırken hata: {ex.Message}");
            }
        }

    }
}
