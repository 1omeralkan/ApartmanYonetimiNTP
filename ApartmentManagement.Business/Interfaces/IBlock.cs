using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IBlock
    {
        List<Block> GetAll();
        List<Block> GetAllBySiteId(int siteId);
        Block GetById(int id);
        string Add(Block block);
        string Update(Block block);
        string Delete(int id);
    }
}
