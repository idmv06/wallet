using Microsoft.EntityFrameworkCore;
using SicaiWallet.Web.Models;

namespace SicaiWallet.Web.Data;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<WalletAccount> WalletAccounts => Set<WalletAccount>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>(); 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasIndex(student => student.SicaiStudentId)
            .IsUnique();
            
        modelBuilder.Entity<WalletAccount>()
         .Property(wallet => wallet.Balance)
         .HasPrecision(18, 2);


        modelBuilder.Entity<Student>()
            .HasOne(student => student.WalletAccount)
         .WithOne(wallet => wallet.Student)
         .HasForeignKey<WalletAccount>(wallet => wallet.StudentId);

        modelBuilder.Entity<WalletTransaction>()
            .Property(transaction => transaction.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<WalletAccount>()
          .HasMany(wallet => wallet.Transactions)
         .WithOne(transaction => transaction.WalletAccount)
          .HasForeignKey(transaction => transaction.WalletAccountId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}