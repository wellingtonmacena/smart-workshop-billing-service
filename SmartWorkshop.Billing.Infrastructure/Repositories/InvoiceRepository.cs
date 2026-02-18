using Microsoft.EntityFrameworkCore;
using SmartWorkshop.Billing.Domain.Entities;
using SmartWorkshop.Billing.Domain.ValueObjects;
using SmartWorkshop.Billing.Infrastructure.Persistence;

namespace SmartWorkshop.Billing.Infrastructure.Repositories;

public class InvoiceRepository
{
    private readonly BillingDbContext _context;

    public InvoiceRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .ToListAsync(cancellationToken);
    }

    public async Task<Invoice?> GetByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.ServiceOrderId == serviceOrderId, cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .Where(i => i.CustomerPersonId == customerId)
            .OrderByDescending(i => i.IssuedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .Where(i => i.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Invoices
            .Where(i => i.Status == InvoiceStatus.Issued && i.DueDate < now)
            .ToListAsync(cancellationToken);
    }

    public async Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return invoice;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        invoice.MarkAsUpdated();
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync(cancellationToken);
        return invoice;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices.FindAsync([id], cancellationToken);
        if (invoice != null)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
