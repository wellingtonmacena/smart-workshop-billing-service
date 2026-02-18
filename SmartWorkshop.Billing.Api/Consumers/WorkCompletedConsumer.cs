using MassTransit;
using SmartWorkshop.Shared.IntegrationEvents.Production;

namespace SmartWorkshop.Billing.Api.Consumers;

/// <summary>
/// Consumes WorkCompletedIntegrationEvent from Workshop Service
/// Triggers invoice generation and finalization
/// </summary>
public class WorkCompletedConsumer : IConsumer<WorkCompletedIntegrationEvent>
{
    private readonly ILogger<WorkCompletedConsumer> _logger;
    
    public WorkCompletedConsumer(ILogger<WorkCompletedConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<WorkCompletedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Work completed - WorkItemId: {WorkItemId}, ServiceOrderId: {ServiceOrderId}, Duration: {Duration} min",
            message.WorkItemId,
            message.ServiceOrderId,
            message.TotalDurationMinutes);
        
        try
        {
            // TODO: Implement business logic:
            // 1. Retrieve quote for this service order
            // 2. Calculate final amounts based on actual work
            // 3. Generate invoice (NF-e if required)
            // 4. Update invoice with labor hours and parts used
            // 5. Send invoice to customer
            // 6. Prepare payment options (Mercado Pago integration)
            
            _logger.LogInformation("Invoice generated for ServiceOrder {ServiceOrderId}", message.ServiceOrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating invoice for ServiceOrderId: {ServiceOrderId}", message.ServiceOrderId);
            throw; // MassTransit will handle retry policy
        }
    }
}
