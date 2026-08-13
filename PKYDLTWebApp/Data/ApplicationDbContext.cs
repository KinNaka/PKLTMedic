using Microsoft.EntityFrameworkCore;
using ClinicManagement.Models;

namespace ClinicManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ============ EXISTING TABLES ============
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }


        // ============ NEW TABLES - PRODUCT & SUPPLIER ============
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }


        // ============ INVENTORY MANAGEMENT ============
        public DbSet<Inventory> Inventories { get; set; }


        // ============ IMPORT ORDERS ============
        public DbSet<ImportOrder> ImportOrders { get; set; }
        public DbSet<ImportOrderDetail> ImportOrderDetails { get; set; }


        // ============ SALES & TRANSACTIONS ============
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }


        // ============ INVOICES ============
        public DbSet<Invoice> Invoices { get; set; }


        // ============ PRESCRIPTIONS ============
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }


        // ============ PERMISSIONS ============
        public DbSet<Permission> Permissions { get; set; }


        // ============ DATABASE CONFIGURATION ============
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============ Indexes for Performance ============

            // Product
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductName);

            // Supplier
            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.SupplierCode)
                .IsUnique();

            // Inventory
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.ProductId);

            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.Quantity);

            // ImportOrder
            modelBuilder.Entity<ImportOrder>()
                .HasIndex(io => io.ImportCode)
                .IsUnique();

            modelBuilder.Entity<ImportOrder>()
                .HasIndex(io => io.ImportDate);

            // Sale
            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.SaleCode)
                .IsUnique();

            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.SaleDate);

            // Invoice
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceDate);

            // Prescription
            modelBuilder.Entity<Prescription>()
                .HasIndex(p => p.PrescriptionCode)
                .IsUnique();

            modelBuilder.Entity<Prescription>()
                .HasIndex(p => p.PrescriptionDate);

            // Permission
            modelBuilder.Entity<Permission>()
                .HasIndex(p => new { p.RoleId, p.ModuleName, p.Action })
                .IsUnique();

            // ============ Relationships Foreign Keys ============

            // Product -> Supplier
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            // Inventory -> Product
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ImportOrder -> Supplier
            modelBuilder.Entity<ImportOrder>()
                .HasOne(io => io.Supplier)
                .WithMany(s => s.ImportOrders)
                .HasForeignKey(io => io.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // ImportOrder -> User
            modelBuilder.Entity<ImportOrder>()
                .HasOne(io => io.CreatedByUser)
                .WithMany()
                .HasForeignKey(io => io.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // ImportOrderDetail -> ImportOrder
            modelBuilder.Entity<ImportOrderDetail>()
                .HasOne(iod => iod.ImportOrder)
                .WithMany(io => io.ImportDetails)
                .HasForeignKey(iod => iod.ImportOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ImportOrderDetail -> Product
            modelBuilder.Entity<ImportOrderDetail>()
                .HasOne(iod => iod.Product)
                .WithMany()
                .HasForeignKey(iod => iod.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sale -> Customer
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Sale -> SalesPerson
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.SalesPerson)
                .WithMany()
                .HasForeignKey(s => s.SalesPersonUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // SaleDetail -> Sale
            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Sale)
                .WithMany(s => s.SaleDetails)
                .HasForeignKey(sd => sd.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // SaleDetail -> Product
            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Product)
                .WithMany(p => p.SaleDetails)
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice -> Sale
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Sale)
                .WithMany(s => s.Invoices)
                .HasForeignKey(i => i.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Invoice -> CreatedByUser
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.CreatedByUser)
                .WithMany()
                .HasForeignKey(i => i.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Prescription -> Customer
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Customer)
                .WithMany()
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prescription -> CreatedByUser
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // PrescriptionDetail -> Prescription
            modelBuilder.Entity<PrescriptionDetail>()
                .HasOne(pd => pd.Prescription)
                .WithMany(p => p.PrescriptionDetails)
                .HasForeignKey(pd => pd.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // PrescriptionDetail -> Product
            modelBuilder.Entity<PrescriptionDetail>()
                .HasOne(pd => pd.Product)
                .WithMany(p => p.PrescriptionDetails)
                .HasForeignKey(pd => pd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Permission -> Role
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Role)
                .WithMany()
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}