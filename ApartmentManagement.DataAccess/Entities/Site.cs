using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Site
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string Code { get; set; }
        
        [MaxLength(500)]
        public string Address { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; } = "Aktif"; // Aktif, Pasif
        
        public int TotalBlocks { get; set; } = 0;
        public int ApartmentsPerBlock { get; set; } = 0;
        public int FloorsPerApartment { get; set; } = 0;
        public int FlatsPerFloor { get; set; } = 0;
        
        public ICollection<Block> Blocks { get; set; }
    }
}
