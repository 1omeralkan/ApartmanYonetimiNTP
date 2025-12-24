using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Flat
    {
        public int Id { get; set; }
        public int DoorNumber { get; set; }
        public int Floor { get; set; }
        [MaxLength(20)]
        public string Type { get; set; } // 2+1, 3+1
        public bool IsEmpty { get; set; } = true;

        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        
        public ICollection<FlatResident> FlatResidents { get; set; }
    }
}
