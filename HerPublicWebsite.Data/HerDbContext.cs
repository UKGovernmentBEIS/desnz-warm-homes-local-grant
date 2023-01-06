using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public class HerDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<PropertyData> PropertyData { get; set; }
    public DbSet<Epc> Epc { get; set; }
    public DbSet<PropertyRecommendation> PropertyRecommendations { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    
    public HerDbContext(DbContextOptions<HerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SetupPropertyData(modelBuilder);
        SetupPropertyRecommendations(modelBuilder);
        SetupEpc(modelBuilder);
        SetupRelations(modelBuilder);
    }

    private void SetupPropertyData(ModelBuilder modelBuilder)
    {
        // Property data primary key
        modelBuilder.Entity<PropertyData>()
            .Property<int>("PropertyDataId")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<PropertyData>()
            .HasKey("PropertyDataId");
        
        // Property data row versioning
        AddRowVersionColumn(modelBuilder.Entity<PropertyData>());

        modelBuilder.Entity<PropertyData>()
            .HasIndex(p => p.Reference)
            .IsUnique();
    }
    
    private void SetupPropertyRecommendations(ModelBuilder modelBuilder)
    {
        // Property recommendations primary key
        modelBuilder.Entity<PropertyRecommendation>()
            .Property<int>("PropertyRecommendationId")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<PropertyRecommendation>()
            .HasKey("PropertyRecommendationId");
        
        // Property recommendations row versioning
        AddRowVersionColumn(modelBuilder.Entity<PropertyRecommendation>());
    }

    private void SetupEpc(ModelBuilder modelBuilder)
    {
        // Epc primary key
        modelBuilder.Entity<Epc>()
            .Property<int>("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Epc>()
            .HasKey("Id");
        
        // Epc row versioning
        AddRowVersionColumn(modelBuilder.Entity<Epc>());
    }

    private void SetupRelations(ModelBuilder modelBuilder)
    {
        // Set up the PropertyData <-> EPC relationship in the database
        modelBuilder.Entity<Epc>()
            .Property<int>("PropertyDataId");
        
        modelBuilder.Entity<Epc>()
            .HasOne<PropertyData>()
            .WithOne(d => d.Epc)
            .HasForeignKey<Epc>("PropertyDataId")
            .IsRequired();
        
        // Set up the PropertyData <-> UneditedData relationship in the database
        modelBuilder.Entity<PropertyData>()
            .Property<int?>("EditedDataId");

        modelBuilder.Entity<PropertyData>()
            .HasOne<PropertyData>()
            .WithOne(d => d.UneditedData)
            .HasForeignKey<PropertyData>("EditedDataId")
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void AddRowVersionColumn<T>(EntityTypeBuilder<T> builder) where T : class
    {
        // This is a PostgreSQL specific implementation of row versioning
        builder.UseXminAsConcurrencyToken();
    }
}
