using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IDues
    {
        List<Dues> GetAll();
        List<Dues> GetByFlatId(int flatId);
        List<Dues> GetByApartmentId(int apartmentId);
        List<Dues> GetBySiteId(int siteId);
        List<Dues> GetPendingDues(int? siteId = null, int? apartmentId = null);
        Dues GetById(int id);
        string Add(Dues dues);
        string Update(Dues dues);
        string Delete(int id);
        string MarkAsPaid(int id);
        string CreateBulkDues(int apartmentId, int month, int year, decimal amount);
    }
}

