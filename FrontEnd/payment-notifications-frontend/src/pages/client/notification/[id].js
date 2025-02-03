import axios from 'axios';
import { useRouter } from 'next/router';
import { useEffect, useState } from 'react';
import NotificationDetail from '../../../components/NotificationDetail';

export default function NotificationDetailPage() {
  const router = useRouter();
  const { id } = router.query;
  const [notification, setNotification] = useState(null);

  useEffect(() => {
    if (id) {
      async function fetchNotification() {
        const response = await axios.get(`http://localhost:5001/api/PaymentNotifications/${id}`);
        setNotification(response.data);
      }
      fetchNotification();
    }
  }, [id]);

  return (
    <div>
      <h1>Detalle de Notificación</h1>
      {notification ? <NotificationDetail notification={notification} /> : <p>Cargando...</p>}
    </div>
  );
}