using APIApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Context;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relationship between Users and Invoice
        modelBuilder.Entity<Users>()
            .HasMany(x => x.Invoices)
            .WithOne(x => x.Users)
            .HasForeignKey(x => x.UserId);

        // Relationship between Invoice and InvoiceDetail
        modelBuilder.Entity<Invoice>()
            .HasMany(x => x.InvoiceDetails)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId);

        // Relationship between Product and InvoiceDetail
        modelBuilder.Entity<Product>()
            .HasMany(x => x.InvoiceDetails)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);
    }
}