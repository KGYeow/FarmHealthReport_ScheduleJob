using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FarmHealthReport_ScheduleJob.Models;

public partial class FarmServerMonitoringDbTestContext : DbContext
{
    public FarmServerMonitoringDbTestContext()
    {
    }

    public FarmServerMonitoringDbTestContext(DbContextOptions<FarmServerMonitoringDbTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Collection> Collections { get; set; }

    public virtual DbSet<CollectionRecord> CollectionRecords { get; set; }

    public virtual DbSet<ConnectionBroker> ConnectionBrokers { get; set; }

    public virtual DbSet<ServerHealthReport> ServerHealthReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=awase1pensql81;Database=FarmServerMonitoringDB_Test;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Collection>(entity =>
        {
            entity.ToTable("Collection");

            entity.Property(e => e.CdriveFreeSpaceAvg)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CDriveFreeSpaceAvg");
            entity.Property(e => e.CpuUsageAvg)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DdriveFreeSpaceAvg)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DDriveFreeSpaceAvg");
            entity.Property(e => e.MemoryUsageAvg)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ReportId).HasMaxLength(100);
            entity.Property(e => e.SessionsActiveAvg)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsActiveSum)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsDiscAvg)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsDiscSum)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsNullAvg)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsNullSum)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsTotalAvg)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsTotalSum)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Report).WithMany(p => p.Collections)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Collection_ServerHealthReport");
        });

        modelBuilder.Entity<CollectionRecord>(entity =>
        {
            entity.ToTable("CollectionRecord");

            entity.Property(e => e.CdriveFreeSpace)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CDriveFreeSpace");
            entity.Property(e => e.CpuUsage)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DdriveFreeSpace)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DDriveFreeSpace");
            entity.Property(e => e.Enabled)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MemoryUsage)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PendingReboot)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ServerName).HasMaxLength(50);
            entity.Property(e => e.SessionsActive)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsDisc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsNull)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SessionsTotal)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Uptime)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Collection).WithMany(p => p.CollectionRecords)
                .HasForeignKey(d => d.CollectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectionRecord_Collection");
        });

        modelBuilder.Entity<ConnectionBroker>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK_ConnectionBroker_1");

            entity.ToTable("ConnectionBroker");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ServerHealthReport>(entity =>
        {
            entity.ToTable("ServerHealthReport");

            entity.Property(e => e.Id).HasMaxLength(100);
            entity.Property(e => e.ScriptEndTime).HasColumnType("datetime");
            entity.Property(e => e.ScriptStartTime).HasColumnType("datetime");

            entity.HasMany(d => d.ConnectionBrokerNames).WithMany(p => p.Reports)
                .UsingEntity<Dictionary<string, object>>(
                    "ConnectionBrokerServerHealthMap",
                    r => r.HasOne<ConnectionBroker>().WithMany()
                        .HasForeignKey("ConnectionBrokerName")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ConnectionBrokerServerHealthMap_ConnectionBroker"),
                    l => l.HasOne<ServerHealthReport>().WithMany()
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ConnectionBrokerServerHealthMap_ServerHealthReport"),
                    j =>
                    {
                        j.HasKey("ReportId", "ConnectionBrokerName");
                        j.ToTable("ConnectionBrokerServerHealthMap");
                        j.IndexerProperty<string>("ReportId").HasMaxLength(100);
                        j.IndexerProperty<string>("ConnectionBrokerName").HasMaxLength(50);
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
