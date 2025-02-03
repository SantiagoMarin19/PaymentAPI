import Link from 'next/link';

export default function NotificationList({ notifications }) {
  if (!Array.isArray(notifications)) {
    return <p>No hay notificaciones disponibles.</p>;
  }

  return (
    <ul>
      {notifications.map(notification => (
        <li key={notification.id}>
          <Link href={`/client/notification/${notification.id}`}>
            {notification.transaccionID} - {notification.estado}
          </Link>
        </li>
      ))}
    </ul>
  );
}