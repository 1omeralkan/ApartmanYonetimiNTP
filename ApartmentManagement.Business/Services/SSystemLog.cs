using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using ApartmentManagement.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagement.Business.Services
{
    public class SSystemLog : ISystemLog
    {
        private readonly ApartmentManagementContext _context;

        public SSystemLog()
        {
            _context = new ApartmentManagementContext();
        }

        public string Add(SystemLog log)
        {
            try
            {
                if (log == null)
                    return "Log nesnesi null olamaz.";

                if (string.IsNullOrEmpty(log.Level))
                    log.Level = "INFO";

                log.CreatedDate = DateTime.UtcNow;

                _context.SystemLogs.Add(log);
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<SystemLog> GetLast(int count = 200)
        {
            try
            {
                return _context.SystemLogs
                    .OrderByDescending(l => l.CreatedDate)
                    .Take(count)
                    .ToList();
            }
            catch
            {
                return new List<SystemLog>();
            }
        }
    }
}


