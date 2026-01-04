using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SExpense : IExpense
    {
        private readonly ApartmentManagementContext _context;

        public SExpense()
        {
            _context = new ApartmentManagementContext();
        }

        public List<Expense> GetAll()
        {
            try
            {
                return _context.Expenses
                    .OrderByDescending(e => e.Date)
                    .ToList();
            }
            catch
            {
                return new List<Expense>();
            }
        }

        public List<Expense> GetBySiteId(int siteId)
        {
            try
            {
                return _context.Expenses
                    .Where(e => e.SiteId == siteId)
                    .OrderByDescending(e => e.Date)
                    .ToList();
            }
            catch
            {
                return new List<Expense>();
            }
        }

        public List<Expense> GetByBlockId(int blockId)
        {
            try
            {
                return _context.Expenses
                    .Where(e => e.BlockId == blockId)
                    .OrderByDescending(e => e.Date)
                    .ToList();
            }
            catch
            {
                return new List<Expense>();
            }
        }

        public List<Expense> GetByApartmentId(int apartmentId)
        {
            try
            {
                return _context.Expenses
                    .Where(e => e.ApartmentId == apartmentId)
                    .OrderByDescending(e => e.Date)
                    .ToList();
            }
            catch
            {
                return new List<Expense>();
            }
        }

        public Expense GetById(int id)
        {
            try
            {
                return _context.Expenses.FirstOrDefault(e => e.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public string Add(Expense expense)
        {
            try
            {
                _context.Expenses.Add(expense);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(Expense expense)
        {
            try
            {
                _context.Expenses.Update(expense);
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
                var expense = _context.Expenses.Find(id);
                if (expense != null)
                {
                    _context.Expenses.Remove(expense);
                    _context.SaveChanges();
                    return string.Empty;
                }
                return "Gider bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

