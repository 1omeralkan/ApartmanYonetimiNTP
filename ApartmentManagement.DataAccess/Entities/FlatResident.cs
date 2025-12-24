using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class FlatResident
    {
        public int Id { get; set; }
        public int FlatId { get; set; }
        public Flat Flat { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        
        public bool IsOwner { get; set; } // true: Ev Sahibi, false: Kiracı
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
    }
}
