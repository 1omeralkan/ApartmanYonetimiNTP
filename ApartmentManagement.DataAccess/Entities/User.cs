using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(20)]
        public string TcNo { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; } // Erkek, Kadın, Diğer
        public DateTime? BirthDate { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        [MaxLength(100)]
        public string EmergencyContact { get; set; }
        [MaxLength(20)]
        public string EmergencyPhone { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        
        // Roles: SuperAdmin, Admin, SiteManager, ApartmentManager, Resident
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } 
    }
}
