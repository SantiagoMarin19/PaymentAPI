import axios from 'axios';
import { useRouter } from 'next/router';
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
  Paper,
  IconButton
} from '@mui/material';
import { CheckCircle, Schedule, Refresh, Home, Dashboard } from '@mui/icons-material';

export default function NotificationDetailPage() {
  const router = useRouter();
  const { id } = router.query;
  const [notification, setNotification] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (id) {
      async function fetchNotification() {
        try {
          console.log(`Fetching notification with id: ${id}`);
          const response = await axios.get(`http://localhost:5001/api/PaymentNotifications/${id}`);
          console.log('Response data:', response.data);
          setNotification(response.data);
          setLoading(false);
        } catch (error) {
          console.error('Error al cargar el detalle de la notificación', error);
          setError('Error al cargar el detalle de la notificación');
          setLoading(false);
        }
      }
      fetchNotification();
    }
  }, [id]);

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

  if (!notification) {
    console.log('No notification data available');
    return null;
  }

  console.log('Notification data:', notification);

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Typography variant="h4" component="h1" gutterBottom>
          Detalle de Notificación
        </Typography>
        <Box>
          <IconButton color="primary" onClick={() => window.location.href = '/'}>
            <Home />
          </IconButton>
          <IconButton color="primary" onClick={() => window.location.href = '/client'}>
            <Dashboard />
          </IconButton>
        </Box>
      </Box>
      <Card>
        <CardContent>
          <Box display="flex" alignItems="center" mb={3}>
            {notification.estado === 'succeeded' ? (
              <CheckCircle color="success" sx={{ mr: 1 }} />
            ) : (
              <Schedule color="warning" sx={{ mr: 1 }} />
            )}
            <Typography variant="h5" component="h1">
              {notification.transaccionID}
            </Typography>
          </Box>

          <Paper elevation={0} sx={{ bgcolor: 'grey.50', p: 3, mb: 3 }}>
            <Grid container spacing={3}>
              <Grid item xs={6}>
                <Typography color="text.secondary">Monto</Typography>
                <Typography variant="h6">€{notification.monto}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography color="text.secondary">Fecha</Typography>
                <Typography variant="h6">{new Date(notification.fechaHora).toLocaleDateString()}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography color="text.secondary">Estado</Typography>
                <Typography variant="h6">{notification.estado}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography color="text.secondary">Método de Pago</Typography>
                <Typography variant="h6">{notification.metodoPago}</Typography>
              </Grid>
            </Grid>
          </Paper>

          <Typography color="text.secondary">
            {notification.descripcion}
          </Typography>
        </CardContent>
      </Card>
    </Container>
  );
}