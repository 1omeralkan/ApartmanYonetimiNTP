using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SSite : ISite
    {
        private readonly ApartmentManagementContext _context;

        public SSite()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Site> GetAll()
        {
            try
            {
                return _context.Sites.ToList();
            }
            catch (Exception)
            {
                return new List<Site>();
            }
        }

        public string Add(Site site)
        {
            try
            {
                _context.Sites.Add(site);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Site site)
        {
            try
            {
                _context.Sites.Update(site);
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
                var site = _context.Sites.Find(id);
                if (site != null)
                {
                    _context.Sites.Remove(site);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Site bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
