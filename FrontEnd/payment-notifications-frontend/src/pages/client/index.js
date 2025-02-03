import axios from 'axios';
import { useEffect, useState } from 'react';
import NotificationList from '../../components/NotificationList';

export default function ClientHome() {
  const [notifications, setNotifications] = useState([]);

  useEffect(() => {
    async function fetchNotifications() {
      try {
        const response = await axios.get('http://localhost:5001/api/PaymentNotifications');
        console.log(response.data); // Verificar la respuesta de la API
        setNotifications(response.data.notifications); // Extraer la propiedad notifications
      } catch (error) {
        console.error('Error fetching notifications:', error);
      }
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