using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public class HerDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<ReferralRequest> ReferralRequests { get; set; }
    public DbSet<NotificationDetails> NotificationDetails { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    
    public HerDbContext(DbContextOptions<HerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SetupReferralRequests(modelBuilder);
        SetupContactDetails(modelBuilder);
    }

    private void SetupReferralRequests(ModelBuilder modelBuilder)
    {
        // Referral request primary key
        modelBuilder.Entity<ReferralRequest>()
            .Property<int>("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ReferralRequest>()
            .HasKey("Id");
        modelBuilder.Entity<ReferralRequest>()
            .Property(rr => rr.RequestDate)
            .HasColumnType("timestamp without time zone");
        
        // Referral request row versioning
        AddRowVersionColumn(modelBuilder.Entity<ReferralRequest>());
    }

    private void SetupContactDetails(ModelBuilder modelBuilder)
    {
        // Contact details primary key
        modelBuilder.Entity<NotificationDetails>()
            .Property<int>("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<NotificationDetails>()
            .HasKey("Id");
        
        // Contact details row versioning
        AddRowVersionColumn(modelBuilder.Entity<NotificationDetails>());
    }

    private void AddRowVersionColumn<T>(EntityTypeBuilder<T> builder) where T : class
    {
        // This is a PostgreSQL specific implementation of row versioning
        builder.UseXminAsConcurrencyToken();
    }
}
