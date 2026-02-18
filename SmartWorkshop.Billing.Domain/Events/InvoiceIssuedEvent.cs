namespace SmartWorkshop.Billing.Domain.Events;

public class InvoiceIssuedEvent : DomainEvent
{
    public Guid InvoiceId { get; set; }
    public Guid ServiceOrderId { get; set; }
    public Guid ClientId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
}
