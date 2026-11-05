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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasIndex(student => student.SicaiStudentId)
            .IsUnique();
    }
}