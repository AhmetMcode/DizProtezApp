using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DizProtezApp.Data;
using DizProtezApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using System.Windows.Threading;

namespace DizProtezApp.Services
{
    public class SqlService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly List<PlcData> _dataBuffer = new List<PlcData>();
        private readonly object _bufferLock = new object();
        private readonly int _bufferLimit = 50; // Örneğin 50 kayıt biriktikten sonra veritabanına yazılacak

        public SqlService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Yeni bir Test kaydı ve buna bağlı Specimen'lar ekler.
        /// </summary>
        public async Task AddTestWithSpecimensAsync(string testName, string testType, object testParameters, string[] specimenNames)
        {
            try
            {
                // Yeni Test kaydı oluştur
                var testRecord = new TestRecord
                {
                    TestName = testName,
                    TestType = testType,
                    Parameters = System.Text.Json.JsonSerializer.Serialize(testParameters),
                    CreatedAt = DateTime.UtcNow,
                    Specimens = specimenNames.Select(name => new Specimen
                    {
                        SpecimenName = name,
                        TestedAt = DateTime.UtcNow
                    }).ToList()
                };

                await _dbContext.TestRecords.AddAsync(testRecord);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("Test ve Specimen'lar başarıyla kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test ve Specimen kaydı sırasında hata: {ex.Message}");
            }
        }

        public List<TestRecord> GetTestRecords()
        {
            try
            {
                // Test kayıtlarını Specimen ilişkileriyle birlikte al ve CreatedAt tarihine göre sırala
                var testRecords = _dbContext.TestRecords
                    .Include(tr => tr.Specimens)
                    .OrderByDescending(tr => tr.CreatedAt)
                    .ToList();

                return testRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanından test kayıtları alınırken hata oluştu: {ex.Message}");
                return new List<TestRecord>();
            }
        }


        public void AddToBuffer(PlcData plcData)
        {
            lock (_bufferLock)
            {
                _dataBuffer.Add(plcData);

                // Eğer buffer limiti aşılırsa toplu yazımı başlat
                if (_dataBuffer.Count >= _bufferLimit)
                {
                    Task.Run(() => FlushDataBufferAsync());
                }
            }
        }

        public async Task FlushDataBufferAsync()
        {
            List<PlcData> dataToSave;

            // Buffer'daki verileri geçici bir listeye taşı
            lock (_bufferLock)
            {
                if (!_dataBuffer.Any()) return;

                dataToSave = new List<PlcData>(_dataBuffer);
                _dataBuffer.Clear();
            }

            // Veritabanına kaydet
            try
            {
                await SavePlcDataBatchAsync(dataToSave);
                Console.WriteLine($"{dataToSave.Count} adet veri veritabanına kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanına veri kaydedilirken hata oluştu: {ex.Message}");
            }
        }


        public async Task SavePlcDataBatchAsync(IEnumerable<PlcData> plcDataList)
        {
            try
            {
                await _dbContext.PlcData.AddRangeAsync(plcDataList);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Toplu veri kaydedilirken hata oluştu: {ex.Message}");
                throw;
            }
        }


    }
}
