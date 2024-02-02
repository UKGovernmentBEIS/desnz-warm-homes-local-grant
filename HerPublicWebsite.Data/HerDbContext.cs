using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public class HerDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<ReferralRequest> ReferralRequests { get; set; }
    public DbSet<NotificationDetails> NotificationDetails { get; set; }
    public DbSet<AnonymisedReport> AnonymisedReports { get; set; }
    public DbSet<PerReferralReport> PerReferralReports { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public DbSet<ReferralRequestFollowUp> ReferralRequestFollowUps { get; set; }

    public HerDbContext(DbContextOptions<HerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SetupReferralRequests(modelBuilder);
        SetupContactDetails(modelBuilder);
        SetupAnonymisedReports(modelBuilder);
        SetupPerReferralReports(modelBuilder);
        SetupReferralRequestFollowUps(modelBuilder);
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
    }

    private void SetupAnonymisedReports(ModelBuilder modelBuilder)
    {
        // Contact details primary key
        modelBuilder.Entity<AnonymisedReport>()
            .Property<int>("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<AnonymisedReport>()
            .HasKey("Id");
        modelBuilder.Entity<AnonymisedReport>()
            .Property(rr => rr.EpcLodgementDate)
            .HasColumnType("timestamp without time zone");
        modelBuilder.Entity<AnonymisedReport>()
            .Property(rr => rr.SubmissionDate)
            .HasColumnType("timestamp without time zone");

        // Anonymised reports row versioning
        AddRowVersionColumn(modelBuilder.Entity<AnonymisedReport>());
    }

    private void SetupPerReferralReports(ModelBuilder modelBuilder)
    {
        // Contact details primary key
        modelBuilder.Entity<PerReferralReport>()
            .Property<int>("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<PerReferralReport>()
            .HasKey("Id");
        modelBuilder.Entity<PerReferralReport>()
            .Property(rr => rr.ApplicationDate)
            .HasColumnType("timestamp without time zone");

        // Per referral reports row versioning
        AddRowVersionColumn(modelBuilder.Entity<PerReferralReport>());
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

    private void AddRowVersionColumn<T>(EntityTypeBuilder<T> builder) where T : class
    {
        // This is a PostgreSQL specific implementation of row versioning
        builder.UseXminAsConcurrencyToken();
    }
}
