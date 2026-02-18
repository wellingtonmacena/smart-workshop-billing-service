namespace SmartWorkshop.Billing.Domain.Events;

public class InvoicePaidEvent : DomainEvent
{
    public Guid InvoiceId { get; set; }
    public Guid ServiceOrderId { get; set; }
    public Guid ClientId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PaidDate { get; set; }
}
