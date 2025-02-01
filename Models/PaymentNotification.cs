using System;

namespace PaymentNotificationsAPI.Models
{
    public class PaymentNotification
    {
        public int ID { get; set; } // Clave primaria (autoincremental)
        public DateTime FechaHora { get; set; } // Fecha y hora de la notificación
        public required string TransaccionID { get; set; } // ID de la transacción
        public required string Estado { get; set; } // Estado de la transacción
        public decimal Monto { get; set; } // Monto de la transacción
        public required string Banco { get; set; } // Banco emisor
        public required string MetodoPago { get; set; } // Método de pago
        public string? Cliente { get; set; } // Cliente
        public string? Descripcion { get; set; } // Descripción
        public required string Moneda { get; set; } // Moneda
        public required string EstadoPago { get; set; } // Estado del pago
    }
}