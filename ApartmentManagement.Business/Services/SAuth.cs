using ApartmentManagement.Business.Helpers;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SAuth : IAuth
    {
        private readonly ApartmentManagementContext _context;

        // Available roles
        public static readonly string[] Roles = new[] 
        { 
            "SuperAdmin", 
            "Admin", 
            "SiteManager", 
            "ApartmentManager", 
            "Resident" 
        };

        public SAuth()
        {
            _context = new ApartmentManagementContext();
        }

        public User Login(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null) return null;

                if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    // Update last login date
                    try
                    {
                        user.LastLoginDate = DateTime.Now;
                        _context.Entry(user).Property(x => x.LastLoginDate).IsModified = true;
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // If save fails, still return user (login is successful)
                        // LastLoginDate update is not critical for login
                        // Log error if needed: System.Diagnostics.Debug.WriteLine($"LastLoginDate update failed: {ex.Message}");
                    }
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log error if needed: System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        public bool EmailExists(string email)
        {
            try
            {
                return _context.Users.Any(u => u.Email.ToLower() == email.ToLower());
            }
            catch
            {
                return false;
            }
        }

        public User Register(string firstName, string lastName, string email, string phone, string password)
        {
            return RegisterFull(firstName, lastName, null, null, null, email, phone, null, null, null, password);
        }

        public User RegisterFull(
            string firstName, 
            string lastName, 
            string tcNo,
            string gender,
            DateTime? birthDate,
            string email, 
            string phone,
            string address,
            string emergencyContact,
            string emergencyPhone,
            string password)
        {
            // Check if email already exists
            if (EmailExists(email))
            {
                throw new Exception("Bu e-posta adresi zaten kayıtlı!");
            }

            try
            {
                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    TcNo = tcNo ?? "",
                    Gender = gender,
                    BirthDate = birthDate.HasValue ? DateTime.SpecifyKind(birthDate.Value, DateTimeKind.Utc) : null,
                    Email = email,
                    Phone = phone ?? "",
                    Address = address,
                    EmergencyContact = emergencyContact,
                    EmergencyPhone = emergencyPhone,
                    PasswordHash = PasswordHelper.HashPassword(password),
                    Role = "Resident"
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Kayıt hatası: {innerMessage}");
            }
        }

        public string SeedAdminUser()
        {
            try
            {
                if (!_context.Users.Any())
                {
                    var admin = new User
                    {
                        FirstName = "Süper",
                        LastName = "Admin",
                        Email = "admin@gmail.com",
                        PasswordHash = PasswordHelper.HashPassword("123"),
                        Role = "SuperAdmin",
                        Phone = "5555555555",
                        TcNo = "11111111111"
                    };

                    _context.Users.Add(admin);
                    _context.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
