using Microsoft.EntityFrameworkCore;
using SmartWorkshop.Billing.Infrastructure.Persistence;
using SmartWorkshop.Billing.Infrastructure.Repositories;
using SmartWorkshop.Shared.EventBus.MassTransit;
using SmartWorkshop.Billing.Api.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("BillingDatabase")
    ?? "Host=localhost;Port=5432;Database=smart_workshop_billing;Username=postgres;Password=postgres";

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Repositories
builder.Services.AddScoped<InvoiceRepository>();
builder.Services.AddScoped<PaymentRepository>();

// Register MassTransit with RabbitMQ and Consumers
builder.Services.AddMassTransitWithRabbitMQ(builder.Configuration, x =>
{
    // Register consumers for events from Workshop Service
    x.AddConsumer<ServiceOrderCreatedConsumer>();
    x.AddConsumer<WorkCompletedConsumer>();
    
    // Billing Service publishes:
    // - QuoteCreatedIntegrationEvent
    // - QuoteApprovedIntegrationEvent (customer approval)
    // - QuoteRejectedIntegrationEvent
    // - InvoiceIssuedIntegrationEvent
    // - PaymentConfirmedIntegrationEvent
    // - PaymentFailedIntegrationEvent
});

// Add Health Checks
builder.Services.AddHealthChecks();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
