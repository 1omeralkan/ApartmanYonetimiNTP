using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IComplaint
    {
        List<Complaint> GetAll();
        List<Complaint> GetByUserId(int userId);
        List<Complaint> GetBySiteId(int siteId);
        List<Complaint> GetByApartmentId(int apartmentId);
        List<Complaint> GetByAssignedToUserId(int assignedToUserId);
        List<Complaint> GetByStatus(string status);
        Complaint GetById(int id);
        string Add(Complaint complaint);
        string Update(Complaint complaint);
        string Delete(int id);
        string UpdateStatus(int id, string status, string comment = null);
        string AssignToUser(int id, int assignedToUserId);
    }
}

