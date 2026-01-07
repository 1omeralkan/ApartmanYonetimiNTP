using System;

namespace ApartmentManagement.DataAccess.Entities
{
    public class SystemLog
    {
        public int Id { get; set; }

        // Örn: INFO, WARNING, ERROR, SECURITY
        public string Level { get; set; }

        // Kısa başlık / kategori (Login, UserUpdate, PaymentCreated vs.)
        public string Category { get; set; }

        // Detaylı mesaj
        public string Message { get; set; }

        // İlgili kullanıcı (opsiyonel)
        public int? UserId { get; set; }

        // Oluşturulma tarihi (UTC)
        public DateTime CreatedDate { get; set; }
    }
}


