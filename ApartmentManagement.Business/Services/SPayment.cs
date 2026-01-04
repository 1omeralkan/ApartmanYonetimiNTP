using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SPayment : IPayment
    {
        private readonly ApartmentManagementContext _context;

        public SPayment()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Payment> GetAll()
        {
            try
            {
                return _context.Payments
                    .Include(p => p.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .OrderByDescending(p => p.Date)
                    .ToList();
            }
            catch
            {
                return new List<Payment>();
            }
        }

        public List<Payment> GetByFlatId(int flatId)
        {
            try
            {
                return _context.Payments
                    .Include(p => p.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(p => p.FlatId == flatId)
                    .OrderByDescending(p => p.Date)
                    .ToList();
            }
            catch
            {
                return new List<Payment>();
            }
        }

        public List<Payment> GetByApartmentId(int apartmentId)
        {
            try
            {
                return _context.Payments
                    .Include(p => p.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(p => p.Flat.ApartmentId == apartmentId)
                    .OrderByDescending(p => p.Date)
                    .ToList();
            }
            catch
            {
                return new List<Payment>();
            }
        }

        public List<Payment> GetBySiteId(int siteId)
        {
            try
            {
                return _context.Payments
                    .Include(p => p.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(p => p.Flat.Apartment.Block.SiteId == siteId)
                    .OrderByDescending(p => p.Date)
                    .ToList();
            }
            catch
            {
                return new List<Payment>();
            }
        }

        public List<Payment> GetByUserId(int userId)
        {
            try
            {
                // Kullanıcının dairelerini bul
                var userFlats = _context.FlatResidents
                    .Where(fr => fr.UserId == userId && (fr.EndDate == null || fr.EndDate > DateTime.UtcNow))
                    .Select(fr => fr.FlatId)
                    .ToList();

                return _context.Payments
                    .Include(p => p.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(p => userFlats.Contains(p.FlatId))
                    .OrderByDescending(p => p.Date)
                    .ToList();
            }
            catch
            {
                return new List<Payment>();
            }
        }

        public Payment GetById(int id)
        {
            try
            {
                return _context.Payments
                    .Include(p => p.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .FirstOrDefault(p => p.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Payment payment)
        {
            try
            {
                _context.Payments.Add(payment);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Payment payment)
        {
            try
            {
                _context.Payments.Update(payment);
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
                var payment = _context.Payments.Find(id);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Ödeme bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

