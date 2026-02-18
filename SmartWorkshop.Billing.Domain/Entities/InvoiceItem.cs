namespace SmartWorkshop.Billing.Domain.Entities;

public sealed class InvoiceItem : Common.Entity
{
    private InvoiceItem() { }

    public InvoiceItem(Guid invoiceId, string description, decimal unitPrice, int quantity, string itemType)
    {
        InvoiceId = invoiceId;
        Description = description;
        UnitPrice = unitPrice;
        Quantity = quantity;
        ItemType = itemType; // "Service" or "Supply"
        TotalPrice = unitPrice * quantity;
    }

    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string ItemType { get; private set; } = string.Empty; // Service, Supply, Labor, etc.

    public Invoice Invoice { get; private set; } = null!;
}
