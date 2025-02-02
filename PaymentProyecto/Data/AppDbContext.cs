using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Models;

namespace PaymentNotificationsAPI.Data
{
    public class AppDbContext : DbContext
    {
        // Representa la tabla PaymentNotifications en la base de datos
        public DbSet<PaymentNotification> PaymentNotifications { get; set; }

        // Constructor que recibe las opciones de configuración
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentNotification>(entity =>
            {
                entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
            });
        }
    }
}