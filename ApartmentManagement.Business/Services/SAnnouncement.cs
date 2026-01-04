using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SAnnouncement : IAnnouncement
    {
        private readonly ApartmentManagementContext _context;

        public SAnnouncement()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Announcement> GetAll()
        {
            try
            {
                return _context.Announcements
                    .Include(a => a.CreatedByUser)
                    .OrderByDescending(a => a.IsPinned)
                    .ThenByDescending(a => a.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Announcement>();
            }
        }

        public List<Announcement> GetBySiteId(int siteId)
        {
            try
            {
                return _context.Announcements
                    .Include(a => a.CreatedByUser)
                    .Where(a => a.SiteId == siteId)
                    .OrderByDescending(a => a.IsPinned)
                    .ThenByDescending(a => a.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Announcement>();
            }
        }

        public List<Announcement> GetByApartmentId(int apartmentId)
        {
            try
            {
                return _context.Announcements
                    .Include(a => a.CreatedByUser)
                    .Where(a => a.ApartmentId == apartmentId)
                    .OrderByDescending(a => a.IsPinned)
                    .ThenByDescending(a => a.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Announcement>();
            }
        }

        public List<Announcement> GetActiveAnnouncements(int? siteId = null, int? apartmentId = null)
        {
            try
            {
                var query = _context.Announcements
                    .Include(a => a.CreatedByUser)
                    .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate >= DateTime.UtcNow));

                if (apartmentId.HasValue)
                {
                    query = query.Where(a => a.ApartmentId == apartmentId.Value);
                }
                else if (siteId.HasValue)
                {
                    query = query.Where(a => a.SiteId == siteId.Value);
                }

                return query
                    .OrderByDescending(a => a.IsPinned)
                    .ThenByDescending(a => a.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Announcement>();
            }
        }

        public Announcement GetById(int id)
        {
            try
            {
                return _context.Announcements
                    .Include(a => a.CreatedByUser)
                    .FirstOrDefault(a => a.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Announcement announcement)
        {
            try
            {
                announcement.CreatedDate = DateTime.UtcNow;
                _context.Announcements.Add(announcement);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Announcement announcement)
        {
            try
            {
                _context.Announcements.Update(announcement);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Delete(int id)
        {
            try
            {
                var announcement = _context.Announcements.Find(id);
                if (announcement != null)
                {
                    _context.Announcements.Remove(announcement);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Duyuru bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string TogglePin(int id)
        {
            try
            {
                var announcement = _context.Announcements.Find(id);
                if (announcement != null)
                {
                    announcement.IsPinned = !announcement.IsPinned;
                    _context.Announcements.Update(announcement);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Duyuru bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ToggleActive(int id)
        {
            try
            {
                var announcement = _context.Announcements.Find(id);
                if (announcement != null)
                {
                    announcement.IsActive = !announcement.IsActive;
                    _context.Announcements.Update(announcement);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Duyuru bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

