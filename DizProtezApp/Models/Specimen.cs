using System;
using System.Collections.Generic;

namespace DizProtezApp.Models
{
    public class Specimen
    {
        public int Id { get; set; } // Primary Key
        public string SpecimenName { get; set; } // Örnek: Specimen 1, Specimen 2
        public DateTime TestedAt { get; set; } = DateTime.UtcNow; // Test zamanı
        public int TestRecordId { get; set; } // Foreign Key
        public TestRecord TestRecord { get; set; } // TestRecord ile ilişki

        // Specimen'e ait PLC verileri
        public List<PlcData> PlcData { get; set; }
    }
}
