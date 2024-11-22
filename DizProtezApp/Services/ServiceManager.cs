using System.Threading.Tasks;
using DizProtezApp.Data;

namespace DizProtezApp.Services
{
    public class ServiceManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PlcService _plcService;

        public ServiceManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _plcService = new PlcService(); // PlcService örneği oluşturuluyor
        }

        public void InitializeServices()
        {
            // PLC işlemini başlatıyoruz
            Task.Run(() =>
            {
                _plcService.Start();
            });

            // SQL işlemini başlatıyoruz
            Task.Run(() =>
            {
                SqlService sqlService = new SqlService(_dbContext, _plcService); // PlcService örneği iletiliyor
                sqlService.Start();
            });
        }
    }
}
