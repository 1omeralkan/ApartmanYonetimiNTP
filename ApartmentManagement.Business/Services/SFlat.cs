using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SFlat : IFlat
    {
        private readonly ApartmentManagementContext _context;

        public SFlat()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Flat> GetAll()
        {
            try
            {
                return _context.Flats
                    .Include(f => f.Apartment)
                    .ThenInclude(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .Include(f => f.FlatResidents)
                    .ToList();
            }
            catch
            {
                return new List<Flat>();
            }
        }

        public List<Flat> GetAllByApartmentId(int apartmentId)
        {
            try
            {
                return _context.Flats
                    .Include(f => f.Apartment)
                    .ThenInclude(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .Include(f => f.FlatResidents)
                    .Where(f => f.ApartmentId == apartmentId)
                    .ToList();
            }
            catch
            {
                return new List<Flat>();
            }
        }

        public List<Flat> GetAllBySiteId(int siteId)
        {
            try
            {
                return _context.Flats
                    .Include(f => f.Apartment)
                    .ThenInclude(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .Include(f => f.FlatResidents)
                    .Where(f => f.Apartment.Block.SiteId == siteId)
                    .ToList();
            }
            catch
            {
                return new List<Flat>();
            }
        }

        public List<Flat> GetAllByBlockId(int blockId)
        {
            try
            {
                return _context.Flats
                    .Include(f => f.Apartment)
                    .ThenInclude(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .Include(f => f.FlatResidents)
                    .Where(f => f.Apartment.BlockId == blockId)
                    .ToList();
            }
            catch
            {
                return new List<Flat>();
            }
        }

        public Flat GetById(int id)
        {
            try
            {
                return _context.Flats
                    .Include(f => f.Apartment)
                    .ThenInclude(a => a.Block)
                    .ThenInclude(b => b.Site)
                    .Include(f => f.FlatResidents)
                    .FirstOrDefault(f => f.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Flat flat)
        {
            try
            {
                _context.Flats.Add(flat);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Flat flat)
        {
            try
            {
                _context.Flats.Update(flat);
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
                var flat = _context.Flats.Find(id);
                if (flat != null)
                {
                    _context.Flats.Remove(flat);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Daire bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateFlatsForApartment(Apartment apartment)
        {
            try
            {
                int doorNumber = 1;
                for (int floor = 1; floor <= apartment.TotalFloors; floor++)
                {
                    for (int flatNum = 1; flatNum <= apartment.FlatsPerFloor; flatNum++)
                    {
                        var flat = new Flat
                        {
                            ApartmentId = apartment.Id,
                            Floor = floor,
                            DoorNumber = doorNumber,
                            Type = apartment.DefaultFlatType ?? "2+1",
                            IsEmpty = true
                        };
                        _context.Flats.Add(flat);
                        doorNumber++;
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
