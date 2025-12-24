using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SBlock : IBlock
    {
        private readonly ApartmentManagementContext _context;

        public SBlock()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Block> GetAll()
        {
            try
            {
                return _context.Blocks.Include(b => b.Site).ToList();
            }
            catch
            {
                return new List<Block>();
            }
        }

        public List<Block> GetAllBySiteId(int siteId)
        {
            try
            {
                return _context.Blocks.Include(b => b.Site).Where(b => b.SiteId == siteId).ToList();
            }
            catch
            {
                return new List<Block>();
            }
        }

        public Block GetById(int id)
        {
            try
            {
                return _context.Blocks.Include(b => b.Site).FirstOrDefault(b => b.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Block block)
        {
            try
            {
                block.CreatedDate = DateTime.UtcNow;
                _context.Blocks.Add(block);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Block block)
        {
            try
            {
                _context.Blocks.Update(block);
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
                var block = _context.Blocks.Find(id);
                if (block != null)
                {
                    _context.Blocks.Remove(block);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Blok bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
