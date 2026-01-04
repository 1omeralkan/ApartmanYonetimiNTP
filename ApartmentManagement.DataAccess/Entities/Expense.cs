using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(50)]
        public string Category { get; set; } // Bakım, Temizlik, Elektrik, Su, Güvenlik, vb.
        
        public int? SiteId { get; set; }
        public int? BlockId { get; set; }
        public int? ApartmentId { get; set; }
    }
}
