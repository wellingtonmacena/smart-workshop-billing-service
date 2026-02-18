namespace SmartWorkshop.Billing.Domain.Events;

public class PaymentFailedEvent : DomainEvent
{
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedDate { get; set; }
}
