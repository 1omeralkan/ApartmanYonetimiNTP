using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int FlatId { get; set; } // Ödemeyi yapan daire
        public Flat Flat { get; set; }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // Aidat, Demirbaş, Yakıt, vb.
    }
}
