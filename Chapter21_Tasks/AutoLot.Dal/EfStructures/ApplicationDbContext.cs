using System;
using System.Collections.Generic;
//using AutoLot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoLot.Dal.EfStructures;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
