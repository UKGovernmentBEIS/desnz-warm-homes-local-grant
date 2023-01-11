using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public class HerDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<Questionnaire> Questionnaires { get; set; }
    public DbSet<ContactDetails> ContactDetails { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    
    public HerDbContext(DbContextOptions<HerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SetupQuestionnaires(modelBuilder);
        SetupContactDetails(modelBuilder);
        SetupRelations(modelBuilder);
    }

    private void SetupQuestionnaires(ModelBuilder modelBuilder)
    {
        // Questionnaire primary key
        modelBuilder.Entity<Questionnaire>()
            .Property<int>("QuestionnaireId")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Questionnaire>()
            .HasKey("QuestionnaireId");
        
        // Questionnaire row versioning
        AddRowVersionColumn(modelBuilder.Entity<Questionnaire>());
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
        // Set up the Questionnaire <-> ContactDetails relationship in the database
        modelBuilder.Entity<ContactDetails>()
            .Property<int>("QuestionnaireId");
        
        modelBuilder.Entity<ContactDetails>()
            .HasOne<Questionnaire>()
            .WithOne(d => d.ContactDetails)
            .HasForeignKey<ContactDetails>("QuestionnaireId")
            .IsRequired();
    }

    private void AddRowVersionColumn<T>(EntityTypeBuilder<T> builder) where T : class
    {
        // This is a PostgreSQL specific implementation of row versioning
        builder.UseXminAsConcurrencyToken();
    }
}
