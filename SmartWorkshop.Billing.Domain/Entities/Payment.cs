using SmartWorkshop.Billing.Domain.ValueObjects;

namespace SmartWorkshop.Billing.Domain.Entities;

public sealed class Payment : Common.Entity
{
    private Payment() { }

    public Payment(Guid invoiceId, decimal amount, PaymentMethod method)
    {
        InvoiceId = invoiceId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        PaymentDate = DateTime.UtcNow;
    }

    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public DateTime? ConfirmedDate { get; private set; }
    public string? TransactionId { get; private set; }
    public string? ExternalPaymentId { get; private set; } // For integrations like Mercado Pago
    public string? PaymentProof { get; private set; }
    public string? Notes { get; private set; }

    public Invoice Invoice { get; private set; } = null!;

    public Payment SetExternalPaymentId(string externalId)
    {
        ExternalPaymentId = externalId;
        MarkAsUpdated();
        return this;
    }

    public Payment SetTransactionId(string transactionId)
    {
        TransactionId = transactionId;
        MarkAsUpdated();
        return this;
    }

    public Payment Approve()
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
        {
            throw new Common.DomainException($"Cannot approve payment with status {Status}.");
        }

        Status = PaymentStatus.Approved;
        ConfirmedDate = DateTime.UtcNow;
        MarkAsUpdated();
        return this;
    }

    public Payment Reject(string? reason = null)
    {
        if (Status == PaymentStatus.Approved)
        {
            throw new Common.DomainException("Cannot reject an approved payment.");
        }

        Status = PaymentStatus.Rejected;
        if (!string.IsNullOrEmpty(reason)) Notes = reason;
        MarkAsUpdated();
        return this;
    }

    public Payment Cancel()
    {
        if (Status == PaymentStatus.Approved)
        {
            throw new Common.DomainException("Cannot cancel an approved payment. Use refund instead.");
        }

        Status = PaymentStatus.Cancelled;
        MarkAsUpdated();
        return this;
    }

    public Payment Refund(string? reason = null)
    {
        if (Status != PaymentStatus.Approved)
        {
            throw new Common.DomainException("Only approved payments can be refunded.");
        }

        Status = PaymentStatus.Refunded;
        if (!string.IsNullOrEmpty(reason)) Notes = reason;
        MarkAsUpdated();
        return this;
    }

    public Payment SetProcessing()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new Common.DomainException("Only pending payments can be set to processing.");
        }

        Status = PaymentStatus.Processing;
        MarkAsUpdated();
        return this;
    }
}
