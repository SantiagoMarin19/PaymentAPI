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
  IconButton,
  TextField,
  InputAdornment
} from '@mui/material';
import { CheckCircle, Schedule, Refresh, Home, Dashboard, Search } from '@mui/icons-material';

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
              Banco: {notification.banco}
            </Typography>
            <Typography color="text.secondary">
              Fecha: {new Date(notification.fechaHora).toLocaleDateString()}
            </Typography>
            <Typography color="text.secondary">
              Estado: {notification.estado}
            </Typography>
          </Grid>
          <Grid item>
            <Typography variant="h5" component="div">
              Monto: €{notification.monto}
            </Typography>
          </Grid>
        </Grid>
        <Typography variant="body2" color="text.secondary" mt={2}>
          Para tener más detalle, presione sobre la transacción que quieres ver.
        </Typography>
      </CardContent>
    </Card>
  ));
}

export default function ClientHome() {
  const [notifications, setNotifications] = useState([]);
  const [filteredNotifications, setFilteredNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  const fetchNotifications = async () => {
    try {
      const response = await axios.get('http://localhost:5001/api/PaymentNotifications');
      console.log(response.data); // Verificar la respuesta de la API
      setNotifications(response.data.notifications);
      setFilteredNotifications(response.data.notifications);
      setLoading(false);
    } catch (error) {
      setError('Error al cargar las notificaciones');
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchNotifications();
  }, []);

  useEffect(() => {
    const results = notifications.filter(notification =>
      notification.transaccionID.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredNotifications(results);
  }, [searchTerm, notifications]);

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
          Catálogo de Notificaciones de Pagos
        </Typography>
        <Box>
          <IconButton color="primary" onClick={fetchNotifications}>
            <Refresh />
          </IconButton>
          <IconButton color="primary" onClick={() => window.location.href = '/'}>
            <Home />
          </IconButton>
          <IconButton color="primary" onClick={() => window.location.href = '/admin'}>
            <Dashboard />
          </IconButton>
        </Box>
      </Box>
      
      <TextField
        label="Buscar por ID de Transacción"
        variant="outlined"
        fullWidth
        margin="normal"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        InputProps={{
          endAdornment: (
            <InputAdornment position="end">
              <Search />
            </InputAdornment>
          ),
        }}
        sx={{ mb: 4 }}
      />
      
      {loading ? (
        <Box display="flex" justifyContent="center" my={4}>
          <CircularProgress />
        </Box>
      ) : (
        <Grid container spacing={3}>
          <NotificationList notifications={filteredNotifications} />
        </Grid>
      )}
    </Container>
  );
}