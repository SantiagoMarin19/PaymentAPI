using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace PaymentNotificationsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PaymentNotification> PaymentNotifications { get; set; }

        public async Task<int> InsertPaymentNotification(PaymentNotification notification)
        {
            var parameters = new[]
            {
                new SqlParameter("@FechaHora", notification.FechaHora),
                new SqlParameter("@TransaccionID", notification.TransaccionID),
                new SqlParameter("@Estado", notification.Estado),
                new SqlParameter("@Monto", notification.Monto),
                new SqlParameter("@Banco", notification.Banco),
                new SqlParameter("@MetodoPago", notification.MetodoPago)
            };

            return await Database.ExecuteSqlRawAsync("EXEC InsertPaymentNotification @FechaHora, @TransaccionID, @Estado, @Monto, @Banco, @MetodoPago", parameters);
        }

        public async Task<PaymentNotification?> GetPaymentNotificationById(int id)
        {
            var parameter = new SqlParameter("@Id", id);
            var notifications = await PaymentNotifications
                .FromSqlRaw("EXEC GetPaymentNotificationById @Id", parameter)
                .ToListAsync();

            return notifications.FirstOrDefault();
        }
    }
}