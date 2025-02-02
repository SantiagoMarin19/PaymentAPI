using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Data;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Configurar Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Registrar los servicios de controladores
builder.Services.AddControllers(); 

// Registrar el contexto de la base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar el servicio de logging
builder.Services.AddLogging();

var app = builder.Build();

// Habilitar el enrutamiento de controladores
app.MapControllers(); 

app.Run();