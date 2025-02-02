using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Data;
using PaymentNotificationsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IActionResult> ReceivePaymentNotification([FromBody] PaymentNotification notification)
        {
            if (notification == null)
            {
                return BadRequest("Invalid notification.");
            }

            _context.PaymentNotifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok("Notification received successfully.");
        }
    }
}