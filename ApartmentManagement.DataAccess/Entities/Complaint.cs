using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApartmentManagement.DataAccess.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [MaxLength(2000)]
        public string Description { get; set; }
        
        [MaxLength(20)]
        public string Type { get; set; } // Şikayet, Talep
        
        [MaxLength(20)]
        public string Status { get; set; } = "Beklemede"; // Beklemede, İnceleniyor, Çözüldü, Reddedildi
        
        [MaxLength(500)]
        public string FilePath { get; set; } // Dosya yolu (fotoğraf, belge)
        
        public int CreatedByUserId { get; set; } // Oluşturan kullanıcı
        public User CreatedByUser { get; set; }
        
        public int? AssignedToUserId { get; set; } // Atanan yönetici (SiteManager/ApartmentManager)
        public User AssignedToUser { get; set; }
        
        public int? SiteId { get; set; } // Site bazlı şikayet
        public int? ApartmentId { get; set; } // Apartman bazlı şikayet
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        
        // Yorumlar için ayrı bir entity olabilir ama şimdilik basit tutuyoruz
        [MaxLength(2000)]
        public string ManagerComment { get; set; } // Yönetici yorumu
    }
}

