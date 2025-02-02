using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Data;
using PaymentNotificationsAPI.Models;
using System.Collections.Generic;
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

        public PaymentNotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PaymentNotifications
        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _context.PaymentNotifications.ToListAsync();
            return Ok(notifications);
        }

        // GET: api/PaymentNotifications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var notification = await _context.PaymentNotifications.FindAsync(id);
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

                    _context.PaymentNotifications.Add(notification);
                    await _context.SaveChangesAsync();

                    return Ok("Notification received successfully.");
                }
                else
                {
                    return BadRequest("Unhandled event type.");
                }
            }
            catch (StripeException e)
            {
                return BadRequest($"Stripe exception: {e.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
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

                _context.PaymentNotifications.Add(testNotification);
                await _context.SaveChangesAsync();

                // Eliminar el registro de prueba
                _context.PaymentNotifications.Remove(testNotification);
                await _context.SaveChangesAsync();

                return Ok("La conexion a la base de datos es exitosa.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Conexion a base de datos fallida: {ex.Message}");
            }
        }
    }
}
        
