using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IPayment
    {
        List<Payment> GetAll();
        List<Payment> GetByFlatId(int flatId);
        List<Payment> GetByApartmentId(int apartmentId);
        List<Payment> GetBySiteId(int siteId);
        List<Payment> GetByUserId(int userId); // Resident için kendi ödemeleri
        Payment GetById(int id);
        string Add(Payment payment);
        string Update(Payment payment);
        string Delete(int id);
    }
}

