using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Apartment
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Aktif";
        
        public int BlockId { get; set; }
        public Block Block { get; set; }
        
        public int FlatsPerFloor { get; set; } = 2;
        public int TotalFloors { get; set; } = 8;
        public int TotalFlats { get; set; } = 16;
        
        [MaxLength(20)]
        public string DefaultFlatType { get; set; } = "2+1";
        
        public decimal DefaultMonthlyDue { get; set; } = 0;
        
        public bool HasBalcony { get; set; } = false;
        public bool HasElevator { get; set; } = false;
        public bool HasParking { get; set; } = false;
        
        public decimal DefaultGrossArea { get; set; } = 0;
        public decimal DefaultNetArea { get; set; } = 0;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public ICollection<Flat> Flats { get; set; }
    }
}
