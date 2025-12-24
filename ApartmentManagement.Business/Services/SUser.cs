using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SUser : IUser
    {
        private readonly ApartmentManagementContext _context;

        public SUser()
        {
            _context = new ApartmentManagementContext();
        }

        public List<User> GetAll()
        {
            try
            {
                return _context.Users.OrderByDescending(u => u.Id).ToList();
            }
            catch (Exception)
            {
                // In a real scenario, we might want to log this or return an empty list/null
                return new List<User>();
            }
        }

        public User GetById(int id)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        // Added Add method as it is likely needed
        public string Add(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return string.Empty; // Success
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(User user)
        {
            try
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return string.Empty; // Success
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
                var user = _context.Users.Find(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    return string.Empty; // Success
                }
                return "Kullanıcı bulunamadı.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
