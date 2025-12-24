using ApartmentManagement.DataAccess.Entities;
using System;

namespace ApartmentManagement.Business.Interfaces
{
    public interface IAuth
    {
        User Login(string email, string password);
        bool EmailExists(string email);
        User Register(string firstName, string lastName, string email, string phone, string password);
        User RegisterFull(string firstName, string lastName, string tcNo, string gender, DateTime? birthDate, string email, string phone, string address, string emergencyContact, string emergencyPhone, string password);
        string SeedAdminUser();
    }
}
