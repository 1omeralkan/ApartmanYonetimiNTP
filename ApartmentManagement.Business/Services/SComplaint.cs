using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SComplaint : IComplaint
    {
        private readonly ApartmentManagementContext _context;

        public SComplaint()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Complaint> GetAll()
        {
            try
            {
                return _context.Complaints
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.AssignedToUser)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Complaint>();
            }
        }

        public List<Complaint> GetByUserId(int userId)
        {
            try
            {
                return _context.Complaints
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.AssignedToUser)
                    .Where(c => c.CreatedByUserId == userId)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Complaint>();
            }
        }

        public List<Complaint> GetBySiteId(int siteId)
        {
            try
            {
                return _context.Complaints
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.AssignedToUser)
                    .Where(c => c.SiteId == siteId)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Complaint>();
            }
        }

        public List<Complaint> GetByApartmentId(int apartmentId)
        {
            try
            {
                return _context.Complaints
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.AssignedToUser)
                    .Where(c => c.ApartmentId == apartmentId)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Complaint>();
            }
        }

        public List<Complaint> GetByAssignedToUserId(int assignedToUserId)
        {
            try
            {
                return _context.Complaints
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.AssignedToUser)
                    .Where(c => c.AssignedToUserId == assignedToUserId)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Complaint>();
            }
        }

        public List<Complaint> GetByStatus(string status)
        {
            try
            {
                return _context.Complaints
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.AssignedToUser)
                    .Where(c => c.Status == status)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            catch
            {
                return new List<Complaint>();
            }
        }

        public Complaint GetById(int id)
        {
            try
            {
                return _context.Complaints
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.AssignedToUser)
                    .FirstOrDefault(c => c.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Complaint complaint)
        {
            try
            {
                // Varsayılan değerleri ayarla
                if (string.IsNullOrEmpty(complaint.Status))
                {
                    complaint.Status = "Beklemede";
                }

                // Null string alanları boş stringe çek (DB tarafında NOT NULL olsa bile sorun olmasın)
                complaint.Title = complaint.Title ?? string.Empty;
                complaint.Description = complaint.Description ?? string.Empty;
                complaint.Type = complaint.Type ?? string.Empty;
                complaint.FilePath = complaint.FilePath ?? string.Empty;
                complaint.ManagerComment = complaint.ManagerComment ?? string.Empty;

                complaint.CreatedDate = DateTime.UtcNow;
                
                // Navigation property'leri null yap (sadece ID'leri kullan)
                complaint.CreatedByUser = null;
                complaint.AssignedToUser = null;
                
                _context.Complaints.Add(complaint);
                _context.SaveChanges();

                // Log
                Business.Helpers.Logger.Log("INFO", "COMPLAINT_CREATE", $"Şikayet/Talep oluşturuldu: {complaint.Title}", complaint.CreatedByUserId);
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Inner exception'ı da göster
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage += " | Inner2: " + ex.InnerException.InnerException.Message;
                    }
                }

                Business.Helpers.Logger.Log("ERROR", "COMPLAINT_CREATE_FAIL", errorMessage, complaint?.CreatedByUserId);
                return errorMessage;
            }
        }

        public string Update(Complaint complaint)
        {
            try
            {
                complaint.UpdatedDate = DateTime.UtcNow;
                _context.Complaints.Update(complaint);
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
                var complaint = _context.Complaints.Find(id);
                if (complaint != null)
                {
                    _context.Complaints.Remove(complaint);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Şikayet/Talep bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateStatus(int id, string status, string comment = null)
        {
            try
            {
                var complaint = _context.Complaints.Find(id);
                if (complaint != null)
                {
                    complaint.Status = status;
                    complaint.UpdatedDate = DateTime.UtcNow;
                    if (status == "Çözüldü")
                    {
                        complaint.ResolvedDate = DateTime.UtcNow;
                    }
                    if (!string.IsNullOrEmpty(comment))
                    {
                        complaint.ManagerComment = comment;
                    }
                    _context.Complaints.Update(complaint);
                    _context.SaveChanges();

                    // Log
                    Business.Helpers.Logger.Log("INFO", "COMPLAINT_STATUS", $"Şikayet/Talep durumu '{status}' olarak güncellendi. Id: {complaint.Id}", complaint.AssignedToUserId ?? complaint.CreatedByUserId);
                    return string.Empty;
                }
                return "Şikayet/Talep bulunamadı.";
            }
            catch (Exception ex)
            {
                Business.Helpers.Logger.Log("ERROR", "COMPLAINT_STATUS_FAIL", ex.Message, null);
                return ex.Message;
            }
        }

        public string AssignToUser(int id, int assignedToUserId)
        {
            try
            {
                var complaint = _context.Complaints.Find(id);
                if (complaint != null)
                {
                    complaint.AssignedToUserId = assignedToUserId;
                    complaint.UpdatedDate = DateTime.UtcNow;
                    _context.Complaints.Update(complaint);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Şikayet/Talep bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

