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
  Box
} from '@mui/material';
import { CheckCircle, Schedule } from '@mui/icons-material';

function NotificationList({ notifications }) {
  const navigateToDetail = (id) => {
    window.location.href = `/client/notification/${id}`;
  };

  return notifications.map((notification) => (
    <Card 
      key={notification.id}
      sx={{ 
        mb: 2, 
        cursor: 'pointer',
        '&:hover': {
          boxShadow: 6,
          transition: 'box-shadow 0.3s ease-in-out'
        }
      }}
      onClick={() => navigateToDetail(notification.id)}
    >
      <CardContent>
        <Grid container spacing={2} alignItems="center">
          <Grid item>
            {notification.estado === 'succeeded' ? (
              <CheckCircle color="success" sx={{ fontSize: 40 }} />
            ) : (
              <Schedule color="warning" sx={{ fontSize: 40 }} />
            )}
          </Grid>
          <Grid item xs>
            <Typography variant="h6" component="div">
              {notification.transaccionID}
            </Typography>
            <Typography color="text.secondary">
              {notification.banco}
            </Typography>
          </Grid>
          <Grid item>
            <Typography variant="h5" component="div">
              €{notification.monto}
            </Typography>
            <Typography color="text.secondary">
              {new Date(notification.fechaHora).toLocaleDateString()}
            </Typography>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  ));
}

export default function ClientHome() {
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchNotifications() {
      try {
        const response = await axios.get('http://localhost:5001/api/PaymentNotifications');
        console.log(response.data); // Verificar la respuesta de la API
        setNotifications(response.data.notifications);
        setLoading(false);
      } catch (error) {
        setError('Error al cargar las notificaciones');
        setLoading(false);
      }
    }
    fetchNotifications();
  }, []);

  if (error) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Catálogo de Notificaciones de Pagos
      </Typography>
      
      {loading ? (
        <Box display="flex" justifyContent="center" my={4}>
          <CircularProgress />
        </Box>
      ) : (
        <NotificationList notifications={notifications} />
      )}
    </Container>
  );
}