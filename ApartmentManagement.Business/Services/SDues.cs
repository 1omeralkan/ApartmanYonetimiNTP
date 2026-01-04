using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SDues : IDues
    {
        private readonly ApartmentManagementContext _context;

        public SDues()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Dues> GetAll()
        {
            try
            {
                return _context.Dues
                    .Include(d => d.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .OrderByDescending(d => d.Year)
                    .ThenByDescending(d => d.Month)
                    .ToList();
            }
            catch
            {
                return new List<Dues>();
            }
        }

        public List<Dues> GetByFlatId(int flatId)
        {
            try
            {
                return _context.Dues
                    .Include(d => d.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(d => d.FlatId == flatId)
                    .OrderByDescending(d => d.Year)
                    .ThenByDescending(d => d.Month)
                    .ToList();
            }
            catch
            {
                return new List<Dues>();
            }
        }

        public List<Dues> GetByApartmentId(int apartmentId)
        {
            try
            {
                return _context.Dues
                    .Include(d => d.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(d => d.Flat.ApartmentId == apartmentId)
                    .OrderByDescending(d => d.Year)
                    .ThenByDescending(d => d.Month)
                    .ToList();
            }
            catch
            {
                return new List<Dues>();
            }
        }

        public List<Dues> GetBySiteId(int siteId)
        {
            try
            {
                return _context.Dues
                    .Include(d => d.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(d => d.Flat.Apartment.Block.SiteId == siteId)
                    .OrderByDescending(d => d.Year)
                    .ThenByDescending(d => d.Month)
                    .ToList();
            }
            catch
            {
                return new List<Dues>();
            }
        }

        public List<Dues> GetPendingDues(int? siteId = null, int? apartmentId = null)
        {
            try
            {
                var query = _context.Dues
                    .Include(d => d.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .Where(d => !d.IsPaid);

                if (apartmentId.HasValue)
                {
                    query = query.Where(d => d.Flat.ApartmentId == apartmentId.Value);
                }
                else if (siteId.HasValue)
                {
                    query = query.Where(d => d.Flat.Apartment.Block.SiteId == siteId.Value);
                }

                return query
                    .OrderByDescending(d => d.Year)
                    .ThenByDescending(d => d.Month)
                    .ToList();
            }
            catch
            {
                return new List<Dues>();
            }
        }

        public Dues GetById(int id)
        {
            try
            {
                return _context.Dues
                    .Include(d => d.Flat)
                        .ThenInclude(f => f.Apartment)
                            .ThenInclude(a => a.Block)
                                .ThenInclude(b => b.Site)
                    .FirstOrDefault(d => d.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Dues dues)
        {
            try
            {
                // Aynı daire, ay ve yıl için tekrar aidat oluşturulmasını engelle
                var existing = _context.Dues
                    .FirstOrDefault(d => d.FlatId == dues.FlatId && 
                                        d.Month == dues.Month && 
                                        d.Year == dues.Year);
                
                if (existing != null)
                {
                    return "Bu daire için bu ay ve yıl için zaten aidat kaydı mevcut.";
                }

                _context.Dues.Add(dues);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Dues dues)
        {
            try
            {
                _context.Dues.Update(dues);
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
                var dues = _context.Dues.Find(id);
                if (dues != null)
                {
                    _context.Dues.Remove(dues);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Aidat bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string MarkAsPaid(int id)
        {
            try
            {
                var dues = _context.Dues.Find(id);
                if (dues != null)
                {
                    dues.IsPaid = true;
                    _context.Dues.Update(dues);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Aidat bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateBulkDues(int apartmentId, int month, int year, decimal amount)
        {
            try
            {
                var flats = _context.Flats.Where(f => f.ApartmentId == apartmentId).ToList();
                
                foreach (var flat in flats)
                {
                    // Aynı ay ve yıl için zaten aidat varsa atla
                    var existing = _context.Dues
                        .FirstOrDefault(d => d.FlatId == flat.Id && 
                                            d.Month == month && 
                                            d.Year == year);
                    
                    if (existing == null)
                    {
                        var dues = new Dues
                        {
                            FlatId = flat.Id,
                            Month = month,
                            Year = year,
                            Amount = amount,
                            IsPaid = false
                        };
                        _context.Dues.Add(dues);
                    }
                }
                
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

