using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Data;
using PaymentNotificationsAPI.Models;
using System.IO;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Logging;

namespace PaymentNotificationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentNotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentNotificationsController> _logger;

        public PaymentNotificationsController(AppDbContext context, ILogger<PaymentNotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/PaymentNotifications/webhook/payments
        [HttpPost("webhook/payments")]
        public async Task<IActionResult> ReceivePaymentNotification()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogError("Received empty request body");
                return BadRequest("A non-empty request body is required.");
            }

            _logger.LogInformation("Received request body: " + json);

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], "whsec_iVFPSRyLguRBQ2M8gAvnfQJLOVWwTydq");

                _logger.LogInformation("Constructed Stripe event: " + stripeEvent.Type);

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent == null)
                    {
                        _logger.LogError("PaymentIntent is null");
                        return BadRequest("Invalid PaymentIntent");
                    }

                    var notification = new PaymentNotification
                    {
                        FechaHora = DateTime.UtcNow,
                        TransaccionID = paymentIntent.Id,
                        Estado = "Exitoso",
                        Monto = paymentIntent.Amount / 100m, // Convertir a unidades monetarias
                        Banco = "Stripe",
                        MetodoPago = paymentIntent.PaymentMethod.Type,
                        Cliente = paymentIntent.CustomerId,
                        Descripcion = paymentIntent.Description,
                        Moneda = paymentIntent.Currency,
                        EstadoPago = paymentIntent.Status
                    };

                    _context.PaymentNotifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Received payment notification: {notification.TransaccionID}");
                    _logger.LogInformation($"Notification saved to database: {notification.TransaccionID}");
                }
                else
                {
                    _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError($"Stripe exception: {e.Message}");
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception: {e.Message}");
                return StatusCode(500, "Internal server error");
            }
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
    }
}