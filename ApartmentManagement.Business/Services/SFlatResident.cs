using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SFlatResident : IFlatResident
    {
        private readonly ApartmentManagementContext _context;

        public SFlatResident()
        {
            _context = new ApartmentManagementContext();
        }

        public string Add(FlatResident flatResident)
        {
            try
            {
                // Bir kullanıcının aynı anda yalnızca bir dairede aktif oturmasına izin ver
                var hasActiveAssignment = _context.FlatResidents
                    .Any(fr => fr.UserId == flatResident.UserId &&
                               (!fr.EndDate.HasValue || fr.EndDate > DateTime.UtcNow));

                if (hasActiveAssignment)
                {
                    return "Bu kullanıcı zaten başka bir daireye atanmış. Önce mevcut atamayı sonlandırın.";
                }

                _context.FlatResidents.Add(flatResident);
                
                // Daire durumunu güncelle: atama yapıldığında daire dolu olsun
                var flat = _context.Flats.FirstOrDefault(f => f.Id == flatResident.FlatId);
                if (flat != null)
                {
                    flat.IsEmpty = false;
                    _context.Flats.Update(flat);
                }

                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(FlatResident flatResident)
        {
            try
            {
                _context.FlatResidents.Update(flatResident);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<FlatResident> GetByFlatId(int flatId)
        {
            try
            {
                return _context.FlatResidents
                    .Include(fr => fr.User)
                    .Where(fr => fr.FlatId == flatId)
                    .OrderByDescending(fr => fr.StartDate)
                    .ToList();
            }
            catch
            {
                return new List<FlatResident>();
            }
        }

        public FlatResident GetActiveByFlatId(int flatId)
        {
            try
            {
                return _context.FlatResidents
                    .Include(fr => fr.User)
                    .FirstOrDefault(fr => fr.FlatId == flatId && (!fr.EndDate.HasValue || fr.EndDate > DateTime.UtcNow));
            }
            catch
            {
                return null;
            }
        }
    }
}


