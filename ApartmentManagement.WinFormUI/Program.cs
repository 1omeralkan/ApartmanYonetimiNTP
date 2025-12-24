using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.WinFormUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // DevExpress Setup
            // DevExpress.UserSkins.BonusSkins.Register(); // Optional skins, removing to fix build
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("WXI"); // Windows 11 Premium Theme

            // Initialize Database - Apply migrations and create tables
            try
            {
                using (var context = new ApartmentManagementContext())
                {
                    // Apply pending migrations (this creates tables if they don't exist)
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı migration uygulanırken hata oluştu: " + ex.Message, "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Seed Database
            try
            {
                var authService = new SAuth();
                authService.SeedAdminUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı başlatılırken hata oluştu: " + ex.Message);
            }

            Application.Run(new FrmLogin());
        }
    }
}