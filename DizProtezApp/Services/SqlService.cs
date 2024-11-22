using System;
using System.Threading.Tasks;
using DizProtezApp.Data;
using DizProtezApp.Models;

namespace DizProtezApp.Services
{
    public class SqlService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PlcService _plcService;

        public SqlService(ApplicationDbContext dbContext, PlcService plcService)
        {
            _dbContext = dbContext;
            _plcService = plcService;
        }

        // PLC'den veri almak için GetPlcData metodu
        private string GetPlcData(string key)
        {
            //// PlcService'den veri okuyun
            //ushort[] registers = _plcService.ReadData(0, 1); // Örnek olarak register 0'dan 1 değer alınıyor
            //if (registers != null && registers.Length > 0)
            //{
            //    return registers[0].ToString(); // İlk veriyi string olarak döndürün
            //}
            return null;
        }

        // Veriyi SQL Server'a kaydetme işlemi
        private async Task SaveDataToSqlAsync(string key)
        {
            try
            {
                string data = GetPlcData(key); // PLC'den veri al

                if (!string.IsNullOrEmpty(data))
                {
                    var plcData = new PlcData
                    {
                        DataKey = key,
                        DataValue = data,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _dbContext.PlcData.AddAsync(plcData);
                    await _dbContext.SaveChangesAsync();

                    Console.WriteLine($"SQL Server'a veri kaydedildi: {key} - {data}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SQL'e veri kaydetme hatası: {ex.Message}");
            }
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    string plcKey = "plcData";
                    await SaveDataToSqlAsync(plcKey);
                    await Task.Delay(60000);
                }
            });
        }
    }
}
