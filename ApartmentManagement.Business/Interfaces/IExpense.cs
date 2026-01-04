using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IExpense
    {
        List<Expense> GetAll();
        List<Expense> GetBySiteId(int siteId);
        List<Expense> GetByBlockId(int blockId);
        List<Expense> GetByApartmentId(int apartmentId);
        Expense GetById(int id);
        string Add(Expense expense);
        string Update(Expense expense);
        string Delete(int id);
    }
}

