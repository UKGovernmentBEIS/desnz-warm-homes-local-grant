using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public class HerDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<ReferralRequest> ReferralRequests { get; set; }
    public DbSet<ContactDetails> ContactDetails { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    
    public HerDbContext(DbContextOptions<HerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SetupReferralRequests(modelBuilder);
        SetupContactDetails(modelBuilder);
        SetupRelations(modelBuilder);
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
        modelBuilder.Entity<ContactDetails>()
            .Property<int>("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ContactDetails>()
            .HasKey("Id");
        
        // Contact details row versioning
        AddRowVersionColumn(modelBuilder.Entity<ContactDetails>());
    }

    private void SetupRelations(ModelBuilder modelBuilder)
    {
        // Set up the ReferralRequest -> ContactDetails relationship in the database
        modelBuilder.Entity<ReferralRequest>()
            .HasOne(rr => rr.ContactDetails)
            .WithMany()
            .HasForeignKey("ContactDetailsId")
            .IsRequired();
    }

    private void AddRowVersionColumn<T>(EntityTypeBuilder<T> builder) where T : class
    {
        // This is a PostgreSQL specific implementation of row versioning
        builder.UseXminAsConcurrencyToken();
    }
}
