using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLot.Samples.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace AutoLot.Samples
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<BaseEntity>().ToTable("BaseEntities");
            // modelBuilder.Entity<Car>().ToTable("Cars");
            // modelBuilder.Entity<Make>().ToTable("Makes");
            //modelBuilder.Entity<Radio>().ToTable("Radios");
            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("Inventory", "dbo");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.MakeId, "IX_Inventory_MakeId");
                entity.Property(e => e.Color)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Black");
                entity.Property(p => p.FullName)
                .HasComputedColumnSql("[PetName] + ' (' + [Color] + ')'", stored: true);
                entity.Property(e => e.PetName)
                .IsRequired()
                .HasMaxLength(50);
                entity.Property(e => e.TimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken();
                entity.Property(e => e.DateBuilt)
                .HasDefaultValueSql("getdate()");
                entity.Property(e => e.IsDrivable)
                .HasField("_isDrivable")
                .HasDefaultValue(true);
                entity.HasOne(d => d.MakeNavigation)
                .WithMany(p => p.Cars)
                .HasForeignKey(d => d.MakeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Makes_MakeId");
            });

            modelBuilder.Entity<Radio>(entity =>
            {
                entity.HasIndex(e => e.CarId, "IX_Radios_CarId")
                .IsUnique();
                entity.HasOne(d => d.CarNavigation)
                .WithOne(p => p.RadioNavigation)
                .HasForeignKey<Radio>(d => d.CarId);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.OwnsOne(o => o.PersonalInformation,
                pd =>
                {
                    pd.Property<string>(nameof(Person.FirstName))
                    .HasColumnName(nameof(Person.FirstName))
                    .HasColumnType("nvarchar(50)");
                    pd.Property<string>(nameof(Person.LastName))
                    .HasColumnName(nameof(Person.LastName))
                    .HasColumnType("nvarchar(50)");
                });
                entity.Navigation(c => c.PersonalInformation).IsRequired(true);
            });
            // Fluent API calls go here
            //OnModelCreatingPartial(modelBuilder);
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Make> Makes { get; set; }
        public virtual DbSet<Radio> Radios { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        static void SampleSaveChanges()
        {
            //The factory is not meant to be used like this, but it’s demo code :-)
            var context = new ApplicationDbContextFactory().CreateDbContext(null);
            //make some changes
            try
            {
                context.SaveChanges();
            }
            catch (RetryLimitExceededException ex)
            {
                //A retry limit error occurred
                //Should handle intelligently
                Console.WriteLine($"Retry limit exceeded! {ex.Message}");
            }
        }
    }
}
