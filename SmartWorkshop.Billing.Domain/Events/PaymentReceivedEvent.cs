using SmartWorkshop.Billing.Domain.ValueObjects;

namespace SmartWorkshop.Billing.Domain.Events;

public class PaymentReceivedEvent : DomainEvent
{
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaymentDate { get; set; }
}
