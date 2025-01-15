using System;

namespace DizProtezApp.Models
{
    public class PlcData
    {
        public int Id { get; set; } // Primary Key
        public DateTime Timestamp { get; set; } // Veri okuma zamanı
        public int Displacement { get; set; } // X ekseni (Servo pozisyonu)
        public double Force { get; set; } // Y ekseni (Kuvvet)

        // Specimen ile ilişki
        public int SpecimenId { get; set; } // Foreign Key
        public Specimen Specimen { get; set; } // Specimen ile navigation property
    }
}
