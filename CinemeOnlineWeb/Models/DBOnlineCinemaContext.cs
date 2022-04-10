using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CinemeOnlineWeb
{
    public partial class DBOnlineCinemaContext : DbContext
    {
        public DBOnlineCinemaContext()
        {
        }

        public DBOnlineCinemaContext(DbContextOptions<DBOnlineCinemaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actor> Actors { get; set; } = null!;
        public virtual DbSet<ActorPlay> ActorPlays { get; set; } = null!;
        public virtual DbSet<Award> Awards { get; set; } = null!;
        public virtual DbSet<CratorsTeam> CratorsTeams { get; set; } = null!;
        public virtual DbSet<Film> Films { get; set; } = null!;
        public virtual DbSet<FilmAward> FilmAwards { get; set; } = null!;
        public virtual DbSet<Response> Responses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server= DESKTOP-1GRC7IR\\SQLEXPRESS;Database=DBOnlineCinema; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>(entity =>
            {
                entity.Property(e => e.ActorId).HasColumnName("ActorID");

                entity.Property(e => e.ActorName).HasMaxLength(50);

                entity.Property(e => e.BirthDate).HasColumnType("date");
            });

            modelBuilder.Entity<ActorPlay>(entity =>
            {
                entity.ToTable("ActorPlay");

                entity.Property(e => e.ActorPlayId).HasColumnName("ActorPlayID");

                entity.Property(e => e.ActorId).HasColumnName("ActorID");

                entity.Property(e => e.FilmId).HasColumnName("FilmID");

                entity.Property(e => e.Role).HasMaxLength(50);

                entity.HasOne(d => d.Actor)
                    .WithMany(p => p.ActorPlays)
                    .HasForeignKey(d => d.ActorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActorPlay_Actors");

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.ActorPlays)
                    .HasForeignKey(d => d.FilmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActorPlay_Films");
            });

            modelBuilder.Entity<Award>(entity =>
            {
                entity.Property(e => e.AwardId).HasColumnName("AwardID");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<CratorsTeam>(entity =>
            {
                entity.HasKey(e => e.CreationTeamId);

                entity.Property(e => e.CreationTeamId).HasColumnName("CreationTeamID");

                entity.Property(e => e.DirectorName).HasMaxLength(50);
            });

            modelBuilder.Entity<Film>(entity =>
            {
                entity.Property(e => e.FilmId).HasColumnName("FilmID");

                entity.Property(e => e.CreationTeamId).HasColumnName("CreationTeamID");

                entity.Property(e => e.FilmName).HasMaxLength(50);

                entity.HasOne(d => d.CreationTeam)
                    .WithMany(p => p.Films)
                    .HasForeignKey(d => d.CreationTeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Films_CratorsTeams1");
            });

            modelBuilder.Entity<FilmAward>(entity =>
            {
                entity.HasKey(e => e.FilmAwardsId);

                entity.Property(e => e.FilmAwardsId).HasColumnName("FilmAwardsID");

                entity.Property(e => e.AwardId).HasColumnName("AwardID");

                entity.Property(e => e.FilmId).HasColumnName("FilmID");

                entity.HasOne(d => d.Award)
                    .WithMany(p => p.FilmAwards)
                    .HasForeignKey(d => d.AwardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FilmAwards_Awards");

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.FilmAwards)
                    .HasForeignKey(d => d.FilmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FilmAwards_Films");
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(e => e.AuthorId);

                entity.Property(e => e.AuthorId).HasColumnName("AuthorID");

                entity.Property(e => e.AuthorName).HasMaxLength(50);

                entity.Property(e => e.DataResponse).HasColumnType("datetime");

                entity.Property(e => e.FilmId).HasColumnName("FilmID");

                entity.Property(e => e.ResponseText).HasColumnType("ntext");

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.Responses)
                    .HasForeignKey(d => d.FilmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Responses_Films");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
