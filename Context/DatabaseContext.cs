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
        
        //quan hệ giữa Users và Invoice
        //1 user có nhiều hóa đơn
        //1 hóa đơn chỉ thuộc về 1 user
        modelBuilder.Entity<Users>()
            .HasMany(x => x.Invoices)
            .WithOne(x => x.Users)
            .HasForeignKey(x => x.UserId);
        
        //quan hệ giữa Invoice và InvoiceDetail
        //1 hóa đơn có nhiều chi tiết hóa đơn
        //1 chi tiết hóa đơn chỉ thuộc về 1 hóa đơn
        modelBuilder.Entity<Invoice>()
            .HasMany(x => x.InvoiceDetails)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId);
        
        //quan hệ giữa Product và InvoiceDetail
        //1 sản phẩm có nhiều chi tiết hóa đơn
        //1 chi tiết hóa đơn chỉ thuộc về 1 sản phẩm
        modelBuilder.Entity<Product>()
            .HasMany(x => x.InvoiceDetails)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);
    }
    
}