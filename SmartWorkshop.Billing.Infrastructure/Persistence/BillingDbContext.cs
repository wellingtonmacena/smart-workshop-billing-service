using Microsoft.EntityFrameworkCore;
using SmartWorkshop.Billing.Domain.Entities;

namespace SmartWorkshop.Billing.Infrastructure.Persistence;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Invoice Configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ServiceOrderId).HasColumnName("service_order_id").IsRequired();
            entity.Property(e => e.ClientId).HasColumnName("client_id").IsRequired();
            entity.Property(e => e.ClientName).HasColumnName("client_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.ClientDocument).HasColumnName("client_document").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.IssueDate).HasColumnName("issue_date").IsRequired();
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.PaidDate).HasColumnName("paid_date");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.ServiceOrderId).IsUnique();

            entity.HasMany(e => e.Items)
                .WithOne(i => i.Invoice)
                .HasForeignKey(i => i.InvoiceId);

            entity.HasMany(e => e.Payments)
                .WithOne(p => p.Invoice)
                .HasForeignKey(p => p.InvoiceId);
        });

        // InvoiceItem Configuration
        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.ToTable("invoice_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price").IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(e => e.TotalPrice).HasColumnName("total_price").IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.ItemType).HasColumnName("item_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        // Payment Configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id").IsRequired();
            entity.Property(e => e.Amount).HasColumnName("amount").IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Method).HasColumnName("method").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date").IsRequired();
            entity.Property(e => e.ConfirmedDate).HasColumnName("confirmed_date");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id").HasMaxLength(100);
            entity.Property(e => e.ExternalPaymentId).HasColumnName("external_payment_id").HasMaxLength(100);
            entity.Property(e => e.PaymentProof).HasColumnName("payment_proof").HasMaxLength(500);
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.ExternalPaymentId);
        });
    }
}
