import axios from 'axios';
import { useEffect, useState } from 'react';
import NotificationList from '../../components/NotificationList';

export default function ClientHome() {
  const [notifications, setNotifications] = useState([]);

  useEffect(() => {
    async function fetchNotifications() {
      const response = await axios.get('http://localhost:5001/api/PaymentNotifications');
      setNotifications(response.data);
    }
    fetchNotifications();
  }, []);

  return (
    <div>
      <h1>Catálogo de Notificaciones de Pagos</h1>
      <NotificationList notifications={notifications} />
    </div>
  );
}