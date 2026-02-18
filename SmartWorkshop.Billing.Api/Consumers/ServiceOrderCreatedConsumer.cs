using MassTransit;
using SmartWorkshop.Shared.IntegrationEvents.OS;

namespace SmartWorkshop.Billing.Api.Consumers;

/// <summary>
/// Consumes ServiceOrderCreatedIntegrationEvent from Workshop Service
/// Initiates quote generation process
/// </summary>
public class ServiceOrderCreatedConsumer : IConsumer<ServiceOrderCreatedIntegrationEvent>
{
    private readonly ILogger<ServiceOrderCreatedConsumer> _logger;
    
    public ServiceOrderCreatedConsumer(ILogger<ServiceOrderCreatedConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ServiceOrderCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Service order created - ServiceOrderId: {ServiceOrderId}, CustomerId: {CustomerId}, VehicleId: {VehicleId}",
            message.ServiceOrderId,
            message.CustomerPersonId,
            message.VehicleId);
        
        try
        {
            // TODO: Implement business logic:
            // 1. Create initial quote based on service order
            // 2. Estimate costs for services and parts
            // 3. Calculate labor costs
            // 4. Apply discounts/promotions if applicable
            // 5. Send quote to customer for approval
            
            _logger.LogInformation("Quote created for ServiceOrder {ServiceOrderId}", message.ServiceOrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quote for ServiceOrderId: {ServiceOrderId}", message.ServiceOrderId);
            throw; // MassTransit will handle retry policy
        }
    }
}
