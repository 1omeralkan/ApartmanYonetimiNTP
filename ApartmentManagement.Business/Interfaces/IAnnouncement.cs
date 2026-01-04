using ApartmentManagement.DataAccess.Entities;
using System.Collections.Generic;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IAnnouncement
    {
        List<Announcement> GetAll();
        List<Announcement> GetBySiteId(int siteId);
        List<Announcement> GetByApartmentId(int apartmentId);
        List<Announcement> GetActiveAnnouncements(int? siteId = null, int? apartmentId = null);
        Announcement GetById(int id);
        string Add(Announcement announcement);
        string Update(Announcement announcement);
        string Delete(int id);
        string TogglePin(int id);
        string ToggleActive(int id);
    }
}

