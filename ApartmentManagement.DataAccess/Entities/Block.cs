using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Block
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; } = "Aktif";
        
        public int SiteId { get; set; }
        public Site Site { get; set; }
        
        // Calculated/Configured fields
        public int TotalApartments { get; set; } = 0;
        public int TotalFloors { get; set; } = 0;
        public int FlatsPerFloor { get; set; } = 0;
        public int TotalFlats { get; set; } = 0;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public ICollection<Apartment> Apartments { get; set; }
    }
}
