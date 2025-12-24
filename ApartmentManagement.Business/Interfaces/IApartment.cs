using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IApartment
    {
        List<Apartment> GetAll();
        List<Apartment> GetAllByBlockId(int blockId);
        List<Apartment> GetAllBySiteId(int siteId);
        Apartment GetById(int id);
        string Add(Apartment apartment);
        string Update(Apartment apartment);
        string Delete(int id);
    }
}
