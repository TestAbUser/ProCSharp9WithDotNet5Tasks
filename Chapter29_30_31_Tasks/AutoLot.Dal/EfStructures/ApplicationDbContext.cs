using System;
using System.Collections.Generic;
using AutoLot.Dal.Exceptions;
using AutoLot.Models.Entities;
using AutoLot.Models.Entities.Owned;
using AutoLot.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace AutoLot.Dal.EfStructures;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        base.SavingChanges += (sender, args) =>
        {
            Console.WriteLine($"Saving changes for {((ApplicationDbContext)sender)!.Database!.GetConnectionString()}");
        };
        base.SavedChanges += (sender, args) =>
        {
            Console.WriteLine($"Saved {args!.EntitiesSavedCount} changes for " +
                $"{((ApplicationDbContext)sender)!.Database!.GetConnectionString()}");
        };
        base.SaveChangesFailed += (sender, args) =>
        {
            Console.WriteLine($"An exception occurred! {args.Exception.Message} entities");
        };

        ChangeTracker.Tracked += ChangeTracker_Tracked;
        ChangeTracker.StateChanged += ChangeTracker_StateChanged;
    }

    private void ChangeTracker_StateChanged(object? sender, EntityStateChangedEventArgs e)
    {
        if (e.Entry.Entity is not Car c)
        {
            return;
        }
        var action = string.Empty;
        Console.WriteLine($"Car {c.PetName} was {e.OldState} before the state changed to {e.NewState}");
        switch (e.NewState)
        {
            case EntityState.Unchanged:
                action = e.OldState switch
                {
                    EntityState.Added => "Added",
                    EntityState.Modified => "Edited",
                    _ => action
                };
                Console.WriteLine($"The object was {action}");
                break;
        }
    }

    private void ChangeTracker_Tracked(object? sender, EntityTrackedEventArgs e)
    {
        var source = (e.FromQuery) ? "Database" : "Code";
        if (e.Entry.Entity is Car c)
        {
            Console.WriteLine($"Car entry {c.PetName} was added from {source}");
        }
    }

    public DbSet<SeriLogEntry>? LogEntries { get; set; }
    public DbSet<CreditRisk>? CreditRisks { get; set; }
    public DbSet<Customer>? Customers { get; set; }
    public DbSet<Make>? Makes { get; set; }
    public DbSet<Car>? Cars { get; set; }
    public DbSet<Order>? Orders { get; set; }
    public virtual DbSet<CustomerOrderViewModel>? CustomerOrderViewModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerOrderViewModel>(entity =>
        {
            entity.HasNoKey().ToView("CustomerOrderView", "dbo");
        });

        modelBuilder.Entity<SeriLogEntry>(entity =>
        {
            entity.Property(e => e.Properties).HasColumnType("Xml");
            entity.Property(e => e.TimeStamp).HasDefaultValueSql("GetDate()");
        });
        modelBuilder.Entity<CreditRisk>(entity =>
        {
            entity.HasIndex(e => e.CustomerId, "IX_CreditRisks_CustomerId");

            // entity.Property(e => e.FirstName).HasMaxLength(50);
            // entity.Property(e => e.LastName).HasMaxLength(50);
            //entity.Property(e => e.TimeStamp)
            //    .IsRowVersion()
            //    .IsConcurrencyToken();

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p!.CreditRisks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_CreditRisks_Customers");

            entity.OwnsOne(o => o.PersonalInformation,
                pd =>
                {
                    pd.Property<string>(nameof(Person.FirstName))
                    .HasColumnName(nameof(Person.FirstName))
                    .HasColumnType("nvarchar(50)");
                    pd.Property<string>(nameof(Person.LastName))
                    .HasColumnName(nameof(Person.LastName))
                    .HasColumnType("nvarchar(50)");
                    pd.Property(p => p.FullName)
                    .HasColumnName(nameof(Person.FullName))
                    .HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
                });
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.OwnsOne(o => o.PersonalInformation,
            pd =>
            {
                pd.Property(p => p.FirstName).HasColumnName(nameof(Person.FirstName));
                pd.Property(p => p.LastName).HasColumnName(nameof(Person.LastName));
                pd.Property(p => p.FullName)
                .HasColumnName(nameof(Person.FullName))
                .HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
            });
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasQueryFilter(c => c.IsDrivable);
            entity.Property(p => p.IsDrivable).HasField("_isDrivable").HasDefaultValue(true);

            entity.ToTable("Inventory");

            entity.HasIndex(e => e.MakeId, "IX_Inventory_MakeId");

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.PetName).HasMaxLength(50);
            //entity.Property(e => e.TimeStamp)
            //    .IsRowVersion()
            //    .IsConcurrencyToken();

            entity.HasOne(d => d.MakeNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.MakeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Make_Inventory");
        });

        modelBuilder.Entity<Make>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(e => e.Cars)
            .WithOne(c => c.MakeNavigation!)
            .HasForeignKey(k => k.MakeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Make_Inventory");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.CarId, "IX_Orders_CarId");

            entity.HasIndex(e => new { e.CustomerId, e.CarId }, "IX_Orders_CustomerId_CarId").IsUnique();

            entity.Property(e => e.TimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CarNavigation)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Inventory");

            entity.HasOne(d => d.CustomerNavigation)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Orders_Customers");
        });

        modelBuilder.Entity<Order>().HasQueryFilter(e => e.CarNavigation!.IsDrivable);

        OnModelCreatingPartial(modelBuilder);
    }

    public override int SaveChanges()
    {
        try
        {
            return base.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            //A concurrency error occurred
            //Should log and handle intelligently
            throw new CustomConcurrencyException("A concurrency error happened.", ex);
        }
        catch (RetryLimitExceededException ex)
        {
            //DbResiliency retry limit exceeded
            //Should log and handle intelligently
            throw new CustomRetryLimitExceededException("There is a problem with SQl Server.", ex);
        }
        catch (DbUpdateException ex)
        {
            //Should log and handle intelligently
            throw new CustomDbUpdateException("An error occurred updating the database", ex);
        }
        catch (Exception ex)
        {
            //Should log and handle intelligently
            throw new CustomException("An error occurred updating the database", ex);
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
