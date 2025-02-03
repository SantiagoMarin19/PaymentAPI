import axios from 'axios';
import { useEffect, useState } from 'react';
import {
  Card,
  CardContent,
  Typography,
  Container,
  Grid,
  CircularProgress,
  Alert,
  Box,
  Paper
} from '@mui/material';

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
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Dashboard Administrativo
      </Typography>
      <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6">Transacciones Exitosas</Typography>
        <Typography variant="h4">{stats.successfulTransactions}</Typography>
      </Paper>
      <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6">Monto Total</Typography>
        <Typography variant="h4">€{stats.totalAmount}</Typography>
      </Paper>
      <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6">Métodos de Pago Más Utilizados</Typography>
        <Typography variant="h4">{stats.mostUsedPaymentMethods.join(', ')}</Typography>
      </Paper>
    </Container>
  );
}