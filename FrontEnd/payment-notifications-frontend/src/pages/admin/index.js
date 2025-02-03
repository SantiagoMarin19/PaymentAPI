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
      const response = await axios.get('http://localhost:5001/api/PaymentNotifications/stats');
      setStats(response.data);
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