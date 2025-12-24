using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IUser
    {
        List<User> GetAll();
        User GetById(int id);
        string Update(User user);
        string Delete(int id);
        string Add(User user);
    }
}
