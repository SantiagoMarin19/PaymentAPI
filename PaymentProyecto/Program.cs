using Microsoft.EntityFrameworkCore;
using PaymentNotificationsAPI.Data;
using Microsoft.OpenApi.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Agregar Servicios para los contenedores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Habilitar el enrutamiento de controladores
app.MapControllers();

// Usar CORS
app.UseCors("AllowAllOrigins");

app.Run();