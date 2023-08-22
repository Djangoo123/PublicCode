﻿using System;
using System.Collections.Generic;
using CompagnyTools.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompagnyTools.Context;

public partial class EFCoreContext : DbContext
{
    public EFCoreContext()
    {
    }

    public EFCoreContext(DbContextOptions<EFCoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dataoffice> Dataoffices { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=ConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dataoffice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dataoffice_pkey");

            entity.ToTable("dataoffice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Chairdirection)
                .HasMaxLength(50)
                .HasColumnName("chairdirection");
            entity.Property(e => e.X).HasColumnName("x");
            entity.Property(e => e.Y).HasColumnName("y");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("equipments_pkey");

            entity.ToTable("equipments");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DeskId).HasColumnName("desk_id");
            entity.Property(e => e.Specification)
                .HasMaxLength(100)
                .HasColumnName("specification");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.Desk).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.DeskId)
                .HasConstraintName("fk_dataoffice");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
