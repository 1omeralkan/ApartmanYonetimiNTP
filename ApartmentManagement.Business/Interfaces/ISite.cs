using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface ISite
    {
        List<Site> GetAll();
        string Add(Site site);
        string Update(Site site);
        string Delete(int id);
    }
}
