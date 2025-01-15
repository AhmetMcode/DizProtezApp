using System;
using System.Collections.Generic;

namespace DizProtezApp.Models
{
    public class TestRecord
    {
        public int Id { get; set; } // Primary Key
        public string TestName { get; set; } // Örnek: ASTM F1223 Testi
        public string TestType { get; set; } // Test Türü
        public string Parameters { get; set; } // Genel test parametreleri (JSON formatında saklanabilir)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Test başlangıç zamanı

        // Specimen ile ilişki
        public List<Specimen> Specimens { get; set; }
    }
}
