using SmartWorkshop.Billing.Domain.ValueObjects;

namespace SmartWorkshop.Billing.Domain.Entities;

/// <summary>
/// Represents an invoice/bill for a service order
/// </summary>
public sealed class Invoice : Common.Entity
{
    private Invoice() { }

    public Invoice(Guid serviceOrderId, Guid clientId, string clientName, string clientDocument)
    {
        ServiceOrderId = serviceOrderId;
        ClientId = clientId;
        ClientName = clientName;
        ClientDocument = clientDocument;
        Status = InvoiceStatus.Draft;
        IssueDate = DateTime.UtcNow;
    }

    public Guid ServiceOrderId { get; private set; }
    public Guid ClientId { get; private set; }
    public string ClientName { get; private set; } = string.Empty;
    public string ClientDocument { get; private set; } = string.Empty;
    public InvoiceStatus Status { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal NetAmount => TotalAmount - TaxAmount;
    public string? Notes { get; private set; }

    public ICollection<InvoiceItem> Items { get; private set; } = [];
    public ICollection<Payment> Payments { get; private set; } = [];

    public Invoice AddItem(string description, decimal unitPrice, int quantity, string itemType)
    {
        if (Status != InvoiceStatus.Draft)
        {
            throw new Common.DomainException("Cannot modify an invoice that is not in draft status.");
        }

        var item = new InvoiceItem(Id, description, unitPrice, quantity, itemType);
        Items.Add(item);
        RecalculateTotal();
        MarkAsUpdated();
        return this;
    }

    public Invoice SetDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
        MarkAsUpdated();
        return this;
    }

    public Invoice SetTaxAmount(decimal taxAmount)
    {
        TaxAmount = taxAmount;
        MarkAsUpdated();
        return this;
    }

    public Invoice Issue()
    {
        if (Status != InvoiceStatus.Draft)
        {
            throw new Common.DomainException("Only draft invoices can be issued.");
        }

        if (!Items.Any())
        {
            throw new Common.DomainException("Cannot issue an invoice without items.");
        }

        Status = InvoiceStatus.Issued;
        IssueDate = DateTime.UtcNow;
        MarkAsUpdated();
        return this;
    }

    public Invoice MarkAsPaid(DateTime paidDate)
    {
        if (Status != InvoiceStatus.Issued)
        {
            throw new Common.DomainException("Only issued invoices can be marked as paid.");
        }

        Status = InvoiceStatus.Paid;
        PaidDate = paidDate;
        MarkAsUpdated();
        return this;
    }

    public Invoice Cancel()
    {
        if (Status == InvoiceStatus.Paid)
        {
            throw new Common.DomainException("Cannot cancel a paid invoice.");
        }

        Status = InvoiceStatus.Cancelled;
        MarkAsUpdated();
        return this;
    }

    public Invoice AddPayment(Payment payment)
    {
        Payments.Add(payment);

        // Check if invoice is fully paid
        var totalPaid = Payments
            .Where(p => p.Status == PaymentStatus.Approved)
            .Sum(p => p.Amount);

        if (totalPaid >= TotalAmount)
        {
            MarkAsPaid(DateTime.UtcNow);
        }

        MarkAsUpdated();
        return this;
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.TotalPrice);
    }
}
