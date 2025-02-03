import axios from 'axios';
import { useEffect, useState } from 'react';
import {
  Typography,
  Container,
  CircularProgress,
  Alert,
  Box,
  Paper,
  IconButton,
  Grid
} from '@mui/material';
import { Dashboard, People, Euro, Payment, TrendingUp } from '@mui/icons-material';

export default function AdminDashboard() {
  const [stats, setStats] = useState({
    successfulTransactions: 0,
    totalAmount: 0,
    mostUsedPaymentMethods: []
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchStats() {
      try {
        const response = await axios.get('http://localhost:5001/api/PaymentNotifications/stats');
        console.log(response.data); // Verificar la respuesta de la API
        setStats(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching stats:', error);
        setError('Error fetching stats');
        setLoading(false);
      }
    }
    fetchStats();
  }, []);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Typography variant="h4" component="h1" gutterBottom>
          Dashboard Administrativo
        </Typography>
        <Box>
          <IconButton color="primary" onClick={() => window.location.href = '/admin'}>
            <Dashboard />
          </IconButton>
          <IconButton color="primary" onClick={() => window.location.href = '/client'}>
            <People />
          </IconButton>
        </Box>
      </Box>
      <Grid container spacing={3}>
        <Grid item xs={12} sm={6}>
          <Paper elevation={3} sx={{ p: 3, display: 'flex', alignItems: 'center' }}>
            <TrendingUp color="primary" sx={{ fontSize: 40, mr: 2 }} />
            <Box>
              <Typography variant="h6">Transacciones Exitosas</Typography>
              <Typography variant="h4">{stats.successfulTransactions}</Typography>
            </Box>
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Paper elevation={3} sx={{ p: 3, display: 'flex', alignItems: 'center' }}>
            <Euro color="primary" sx={{ fontSize: 40, mr: 2 }} />
            <Box>
              <Typography variant="h6">Monto Total</Typography>
              <Typography variant="h4">€{stats.totalAmount}</Typography>
            </Box>
          </Paper>
        </Grid>
        <Grid item xs={12}>
          <Paper elevation={3} sx={{ p: 3, display: 'flex', alignItems: 'center' }}>
            <Payment color="primary" sx={{ fontSize: 40, mr: 2 }} />
            <Box>
              <Typography variant="h6">Métodos de Pago Más Utilizados</Typography>
              <Typography variant="h4">{stats.mostUsedPaymentMethods.join(', ')}</Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}