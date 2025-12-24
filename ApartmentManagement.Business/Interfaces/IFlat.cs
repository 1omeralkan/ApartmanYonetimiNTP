using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IFlat
    {
        List<Flat> GetAll();
        List<Flat> GetAllByApartmentId(int apartmentId);
        List<Flat> GetAllBySiteId(int siteId);
        List<Flat> GetAllByBlockId(int blockId);
        Flat GetById(int id);
        string Add(Flat flat);
        string Update(Flat flat);
        string Delete(int id);
        string CreateFlatsForApartment(Apartment apartment);
    }
}
