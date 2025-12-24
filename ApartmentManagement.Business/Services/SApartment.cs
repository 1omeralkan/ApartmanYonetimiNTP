using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SApartment : IApartment
    {
        private readonly ApartmentManagementContext _context;

        public SApartment()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Apartment> GetAll()
        {
            try
            {
                return _context.Apartments
                    .Include(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .ToList();
            }
            catch
            {
                return new List<Apartment>();
            }
        }

        public List<Apartment> GetAllByBlockId(int blockId)
        {
            try
            {
                return _context.Apartments
                    .Include(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .Where(a => a.BlockId == blockId)
                    .ToList();
            }
            catch
            {
                return new List<Apartment>();
            }
        }

        public List<Apartment> GetAllBySiteId(int siteId)
        {
            try
            {
                return _context.Apartments
                    .Include(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .Where(a => a.Block.SiteId == siteId)
                    .ToList();
            }
            catch
            {
                return new List<Apartment>();
            }
        }

        public Apartment GetById(int id)
        {
            try
            {
                return _context.Apartments
                    .Include(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .FirstOrDefault(a => a.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Apartment apartment)
        {
            try
            {
                apartment.CreatedDate = DateTime.UtcNow;
                _context.Apartments.Add(apartment);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Apartment apartment)
        {
            try
            {
                _context.Apartments.Update(apartment);
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
                var apartment = _context.Apartments.Find(id);
                if (apartment != null)
                {
                    _context.Apartments.Remove(apartment);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Apartman bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
