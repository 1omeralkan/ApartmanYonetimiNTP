using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface ISystemLog
    {
        string Add(SystemLog log);

        /// <summary>
        /// Son X log kaydını getir (varsayılan 200).
        /// </summary>
        List<SystemLog> GetLast(int count = 200);
    }
}


