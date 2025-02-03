export default function NotificationDetail({ notification }) {
    return (
      <div>
        <p>ID: {notification.id}</p>
        <p>Fecha y Hora: {new Date(notification.fechaHora).toLocaleString()}</p>
        <p>Transacción ID: {notification.transaccionID}</p>
        <p>Estado: {notification.estado}</p>
        <p>Monto: {notification.monto}</p>
        <p>Banco: {notification.banco}</p>
        <p>Método de Pago: {notification.metodoPago}</p>
      </div>
    );
  }