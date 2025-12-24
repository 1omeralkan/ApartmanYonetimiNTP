using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Dues
    {
        public int Id { get; set; }
        public int FlatId { get; set; }
        public Flat Flat { get; set; }

        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsPaid { get; set; }
    }
}
