import axios from 'axios';
import { useEffect, useState } from 'react';

export default function AdminDashboard() {
  const [stats, setStats] = useState({
    successfulTransactions: 0,
    totalAmount: 0,
    mostUsedPaymentMethods: []
  });

  useEffect(() => {
    async function fetchStats() {
      try {
        const response = await axios.get('http://localhost:5001/api/PaymentNotifications/stats');
        console.log(response.data); // Verificar la respuesta de la API
        setStats(response.data);
      } catch (error) {
        console.error('Error fetching stats:', error);
      }
    }
    fetchStats();
  }, []);

  return (
    <div>
      <h1>Dashboard Administrativo</h1>
      <p>Transacciones Exitosas: {stats.successfulTransactions}</p>
      <p>Monto Total: {stats.totalAmount}</p>
      <p>Métodos de Pago Más Utilizados: {stats.mostUsedPaymentMethods.join(', ')}</p>
    </div>
  );
}