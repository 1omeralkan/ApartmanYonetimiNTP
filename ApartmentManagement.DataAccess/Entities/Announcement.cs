using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Announcement
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [MaxLength(2000)]
        public string Content { get; set; }
        
        [MaxLength(50)]
        public string Category { get; set; } // Genel, Bakım, Toplantı, Acil, Diğer
        
        public bool IsPinned { get; set; } = false; // Önemli duyuru (pin)
        public bool IsActive { get; set; } = true; // Aktif/Pasif
        
        public int? SiteId { get; set; } // SiteManager için
        public int? ApartmentId { get; set; } // ApartmentManager için
        
        public int CreatedByUserId { get; set; } // Oluşturan kullanıcı
        public User CreatedByUser { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; } // Son geçerlilik tarihi (opsiyonel)
    }
}

