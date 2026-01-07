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

                // Check if user is approved (SuperAdmin and Admin are always approved)
                if (!user.IsApproved && user.Role != "SuperAdmin" && user.Role != "Admin")
                {
                    return null; // User not approved
                }

                if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    // Update last login date (real value)
                    try
                    {
                        user.LastLoginDate = DateTime.UtcNow;
                        _context.SaveChanges();
                    }
                    catch
                    {
                        // Tarih güncellenemese de login devam etsin
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
                    Role = "Resident",
                    IsApproved = false, // New users need approval
                    CreatedDate = DateTime.UtcNow
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
                // Eğer henüz Süper Admin yoksa varsayılan Süper Admin oluştur
                if (!_context.Users.Any(u => u.Role == "SuperAdmin"))
                {
                    var superAdmin = new User
                    {
                        FirstName = "Süper",
                        LastName = "Admin",
                        Email = "admin@gmail.com",
                        PasswordHash = PasswordHelper.HashPassword("123"),
                        Role = "SuperAdmin",
                        Phone = "5555555555",
                        TcNo = "11111111111",
                        IsApproved = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Users.Add(superAdmin);
                }

                // Admin
                if (!_context.Users.Any(u => u.Email == "admin1@gmail.com"))
                {
                    _context.Users.Add(new User
                    {
                        FirstName = "Ali",
                        LastName = "Admin",
                        Email = "admin1@gmail.com",
                        PasswordHash = PasswordHelper.HashPassword("123"),
                        Role = "Admin",
                        Phone = "5550000001",
                        TcNo = "22222222222",
                        IsApproved = true,
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Site Manager
                if (!_context.Users.Any(u => u.Email == "sitemanager@gmail.com"))
                {
                    _context.Users.Add(new User
                    {
                        FirstName = "Seda",
                        LastName = "SiteYön",
                        Email = "sitemanager@gmail.com",
                        PasswordHash = PasswordHelper.HashPassword("123"),
                        Role = "SiteManager",
                        Phone = "5550000002",
                        TcNo = "33333333333",
                        IsApproved = true,
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Apartment Manager
                if (!_context.Users.Any(u => u.Email == "apartmentmanager@gmail.com"))
                {
                    _context.Users.Add(new User
                    {
                        FirstName = "Ahmet",
                        LastName = "ApartmanYön",
                        Email = "apartmentmanager@gmail.com",
                        PasswordHash = PasswordHelper.HashPassword("123"),
                        Role = "ApartmentManager",
                        Phone = "5550000003",
                        TcNo = "44444444444",
                        IsApproved = true,
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Resident
                if (!_context.Users.Any(u => u.Email == "resident@gmail.com"))
                {
                    _context.Users.Add(new User
                    {
                        FirstName = "Emir",
                        LastName = "Sakin",
                        Email = "resident@gmail.com",
                        PasswordHash = PasswordHelper.HashPassword("123"),
                        Role = "Resident",
                        Phone = "5550000004",
                        TcNo = "55555555555",
                        IsApproved = true,
                        CreatedDate = DateTime.UtcNow
                    });
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
