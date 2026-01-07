using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IFlatResident
    {
        string Add(FlatResident flatResident);
        string Update(FlatResident flatResident);
        List<FlatResident> GetByFlatId(int flatId);
        FlatResident GetActiveByFlatId(int flatId);
    }
}


