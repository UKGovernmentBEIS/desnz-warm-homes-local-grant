using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.Data;

public class WhlgDbContext(DbContextOptions<WhlgDbContext> options) : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<ReferralRequest> ReferralRequests { get; set; }
    public DbSet<NotificationDetails> NotificationDetails { get; set; }
    public DbSet<ReferralRequestFollowUp> ReferralRequestFollowUps { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SetupReferralRequests(modelBuilder);
        SetupContactDetails(modelBuilder);
        SetupReferralRequestFollowUps(modelBuilder);
        SetupSession(modelBuilder);
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
            .Property(rr => rr.EpcLodgementDate)
            .HasColumnType("timestamp without time zone");
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
        
        // Add the foreign key relationship with SetNull delete behavior
        modelBuilder.Entity<NotificationDetails>()
            .HasOne(rr => rr.ReferralRequest)
            .WithMany()
            .HasForeignKey(details => details.ReferralRequestId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void SetupReferralRequestFollowUps(ModelBuilder modelBuilder)
    {
        // Referral request follow up primary key
        modelBuilder.Entity<ReferralRequestFollowUp>()
            .Property<int>("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ReferralRequestFollowUp>()
            .HasKey("Id");
        modelBuilder.Entity<ReferralRequestFollowUp>()
            .HasIndex(rrfu => rrfu.Token).IsUnique();
        modelBuilder.Entity<ReferralRequestFollowUp>()
            .Property(rrfu => rrfu.DateOfFollowUpResponse)
            .HasColumnType("timestamp without time zone");

        // Referral request follow up row versioning
        AddRowVersionColumn(modelBuilder.Entity<ReferralRequestFollowUp>());
    }

    private void SetupSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>()
            .Property(s => s.Id)
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Session>()
            .HasKey("Id");
        modelBuilder.Entity<Session>()
            .Property(s => s.Timestamp)
            .HasColumnType("timestamp with time zone");

        // Session row versioning
        AddRowVersionColumn(modelBuilder.Entity<Session>());
    }

    private void AddRowVersionColumn<T>(EntityTypeBuilder<T> builder) where T : class
    {
        // This is a PostgreSQL specific implementation of row versioning
        builder.UseXminAsConcurrencyToken();
    }
}