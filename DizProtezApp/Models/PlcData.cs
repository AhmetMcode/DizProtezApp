using System;

namespace DizProtezApp.Models
{
    public class PlcData
    {
        public int Id { get; set; } // Primary Key
        public DateTime Timestamp { get; set; } // Veri okuma zamanı
        public double ForceAxialN { get; set; }
        public double ForceFemoralN { get; set; }
        public int Displacement { get; set; }

        // Specimen ile ilişki
        public int SpecimenId { get; set; } // Foreign Key
        public Specimen Specimen { get; set; } // Specimen ile navigation property
    }
}
