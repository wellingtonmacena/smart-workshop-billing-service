namespace SmartWorkshop.Billing.Domain.Events;

public class PaymentConfirmedEvent : DomainEvent
{
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid ServiceOrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ConfirmedDate { get; set; }
}
