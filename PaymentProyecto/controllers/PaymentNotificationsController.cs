using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Data;
using PaymentNotificationsAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe;
using System.IO;
using System;

namespace PaymentNotificationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentNotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        private readonly string _logFolder = "Logs";

        public PaymentNotificationsController(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;

            // Crear el directorio de logs si no existe
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            }
        }

        // GET: api/PaymentNotifications
        [HttpGet]
        public async Task<IActionResult> GetAllNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string transactionId = null,
            [FromQuery] string status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.PaymentNotifications.AsQueryable();

            if (!string.IsNullOrEmpty(transactionId))
            {
                query = query.Where(n => n.TransaccionID.Contains(transactionId));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(n => n.Estado == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(n => n.FechaHora >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(n => n.FechaHora <= endDate.Value);
            }

            var totalRecords = await query.CountAsync();
            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                Notifications = notifications
            };

            return Ok(result);
        }

        // GET: api/PaymentNotifications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var notification = await _context.GetPaymentNotificationById(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        // POST: api/PaymentNotifications/webhook/payments
        [HttpPost("webhook/payments")]
        public async Task<IActionResult> ReceivePaymentNotification()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(json))
            {
                WriteLog("Error", "Se recibió un cuerpo de solicitud vacío.");
                return BadRequest("Invalid notification.");
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    "whsec_iVFPSRyLguRBQ2M8gAvnfQJLOVWwTydq" 
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent == null)
                    {
                        WriteLog("Error", "PaymentIntent es null.");
                        return BadRequest("Invalid PaymentIntent.");
                    }

                    var notification = new PaymentNotification
                    {
                        FechaHora = DateTime.UtcNow,
                        TransaccionID = paymentIntent.Id,
                        Estado = paymentIntent.Status,
                        Monto = paymentIntent.Amount / 100m, // Convertir a unidades monetarias
                        Banco = "Stripe",
                        MetodoPago = paymentIntent.PaymentMethodId
                    };

                    await _context.InsertPaymentNotification(notification);
                    await _logService.LogTransactionAsync($"Notificación guardada: {notification.TransaccionID} - Monto: {notification.Monto}");

                    return Ok("Notification received successfully.");
                }
                else
                {
                    WriteLog("Info", $"Evento no manejado: {stripeEvent.Type}");
                    return BadRequest("Unhandled event type.");
                }
            }
            catch (StripeException e)
            {
                WriteLog("Error", $"Stripe exception: {e.Message}");
                return BadRequest($"Stripe exception: {e.Message}");
            }
            catch (Exception e)
            {
                WriteLog("Error", $"Exception: {e.Message}");
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // Método para escribir logs en un archivo
        private void WriteLog(string estado, string mensaje)
        {
            try
            {
                string fileName = $"transacciones_{DateTime.UtcNow:yyyyMMdd}.log";
                string filePath = Path.Combine(_logFolder, fileName);
                string logLine = $"{DateTime.UtcNow:HH:mm:ss} [{estado}] - {mensaje}{Environment.NewLine}";
                System.IO.File.AppendAllText(filePath, logLine);
            }
            catch (Exception ex)
            {
                // Manejar errores de escritura de logs
                Console.WriteLine($"No se pudo escribir en el log: {ex.Message}");
            }
        }
         [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var successfulTransactions = await _context.PaymentNotifications
                .CountAsync(n => n.Estado == "succeeded");

            var totalAmount = await _context.PaymentNotifications
                .Where(n => n.Estado == "succeeded")
                .SumAsync(n => n.Monto);

            var mostUsedPaymentMethods = await _context.PaymentNotifications
                .Where(n => n.Estado == "succeeded")
                .GroupBy(n => n.MetodoPago)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(5)
                .ToListAsync();

            var stats = new
            {
                SuccessfulTransactions = successfulTransactions,
                TotalAmount = totalAmount,
                MostUsedPaymentMethods = mostUsedPaymentMethods
            };

            return Ok(stats);
        }

        // GET: api/PaymentNotifications/testconnection
        [HttpGet("testconnection")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                // Crear un registro de prueba
                var testNotification = new PaymentNotification
                {
                    FechaHora = DateTime.UtcNow,
                    TransaccionID = "test_transaction",
                    Estado = "test_status",
                    Monto = 0.0m,
                    Banco = "test_bank",
                    MetodoPago = "test_method"
                };

                await _context.InsertPaymentNotification(testNotification);

                // Eliminar el registro de prueba
                _context.PaymentNotifications.Remove(testNotification);
                await _context.SaveChangesAsync();

                return Ok("Database connection is successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection failed: {ex.Message}");
            }
        }
    }
}