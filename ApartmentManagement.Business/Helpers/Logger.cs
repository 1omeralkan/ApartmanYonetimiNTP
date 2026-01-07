using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.Business.Services;
using System;

namespace ApartmentManagement.Business.Helpers
{
    /// <summary>
    /// Basit, sessiz (fail-safe) log yazıcı.
    /// </summary>
    public static class Logger
    {
        private static readonly Services.SSystemLog _logService = new Services.SSystemLog();

        public static void Log(string level, string category, string message, int? userId = null)
        {
            try
            {
                var log = new SystemLog
                {
                    Level = level?.ToUpperInvariant() ?? "INFO",
                    Category = category ?? string.Empty,
                    Message = message ?? string.Empty,
                    UserId = userId
                };
                _logService.Add(log);
            }
            catch
            {
                // Sessiz yut: log yazılırken hata alınsa bile uygulamayı etkilemesin
            }
        }
    }
}


