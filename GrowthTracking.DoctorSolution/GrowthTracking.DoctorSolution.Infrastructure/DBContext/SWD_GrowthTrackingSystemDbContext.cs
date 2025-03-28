﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using GrowthTracking.DoctorSolution.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrowthTracking.DoctorSolution.Infrastructure.DBContext;

public partial class SWD_GrowthTrackingSystemDbContext : DbContext
{
    public SWD_GrowthTrackingSystemDbContext(DbContextOptions<SWD_GrowthTrackingSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<IdentityDocument> IdentityDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.Property(e => e.DoctorId)
                .ValueGeneratedNever()
                .HasColumnName("doctor_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.AverageRating)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("average_rating");
            entity.Property(e => e.Biography)
                .HasColumnType("text")
                .HasColumnName("biography");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.ProfilePhoto)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("profile_photo");
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("specialization");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Workplace)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("workplace");
        });

        modelBuilder.Entity<IdentityDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Identity__9666E8ACFB5C76CA");

            entity.ToTable("IdentityDocument");

            entity.HasIndex(e => e.DoctorId, "IX_IdentityDocument_DoctorId");

            entity.Property(e => e.DocumentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("document_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.DocumentUrl).HasColumnName("document_url");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Doctor).WithMany(p => p.IdentityDocuments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_IdentityDocument_Doctor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}