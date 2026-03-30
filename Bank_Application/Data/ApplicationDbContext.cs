using Bank_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Application.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for all entities
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Repayment> Repayments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for Account
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            // Configure decimal precision for Transaction
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            // Configure decimal precision for Loan
            modelBuilder.Entity<Loan>()
                .Property(l => l.LoanAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Loan>()
                .Property(l => l.InterestRate)
                .HasPrecision(18, 2);

            // Configure decimal precision for Repayment
            modelBuilder.Entity<Repayment>()
                .Property(r => r.AmountPaid)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Repayment>()
                .Property(r => r.BalanceRemaining)
                .HasPrecision(18, 2);
        }
    }
}
