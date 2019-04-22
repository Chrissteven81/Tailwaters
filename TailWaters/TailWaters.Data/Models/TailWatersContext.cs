using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TailWaters.Data.Models
{
    public partial class TailWatersContext : DbContext
    {
        public TailWatersContext()
        {
        }

        public TailWatersContext(DbContextOptions<TailWatersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }
        public virtual DbSet<TailWater> TailWaters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=TailWaters;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Operator>(entity =>
            {
                entity.ToTable("Operator");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.HasOne(d => d.TailWater)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.TailWaterId)
                    .HasConstraintName("FK_Schedules_TailWaters");
            });

            modelBuilder.Entity<Subscriber>(entity =>
            {
                entity.ToTable("Subscriber");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<TailWater>(entity =>
            {
                entity.ToTable("TailWater");

                entity.Property(e => e.Acronym)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.TailWaters)
                    .HasForeignKey(d => d.OperatorId)
                    .HasConstraintName("FK_TailWaters_Operators");
            });
        }
    }
}
