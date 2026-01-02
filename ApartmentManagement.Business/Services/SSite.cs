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
                if (site == null)
                {
                    return "Site bulunamadı.";
                }

                // Check if site has related blocks
                var blocks = _context.Blocks.Where(b => b.SiteId == id).ToList();
                if (blocks.Any())
                {
                    return $"Bu siteye bağlı {blocks.Count} adet blok bulunmaktadır. Önce blokları silmeniz gerekmektedir.";
                }

                _context.Sites.Remove(site);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Site GetById(int id)
        {
            try
            {
                return _context.Sites.Find(id);
            }
            catch
            {
                return null;
            }
        }
    }
}
