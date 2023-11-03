using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AdminRazorPageV2.Models;

public partial class HighFlixV4Context : DbContext
{
    public HighFlixV4Context()
    {
    }

    public HighFlixV4Context(DbContextOptions<HighFlixV4Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Episode> Episodes { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieCategory> MovieCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Statistic> Statistics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=192.168.3.61, 1433; database=HighFlixDB_v4;uid=sa;pwd=123456;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.Property(e => e.CommentContent).IsRequired();
            entity.Property(e => e.CommentedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.CommentsNavigation)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_Movie");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<Episode>(entity =>
        {
            entity.ToTable("Episode");

            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.EpisodeName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Movie).WithMany(p => p.Episodes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Episode_Movie");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movie__4BD2941AB864E0F0");

            entity.ToTable("Movie");

            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Director).HasMaxLength(50);
            entity.Property(e => e.MoviePoster).IsRequired();
            entity.Property(e => e.MovieThumnailImage).IsRequired();
            entity.Property(e => e.ReleasedYear)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.PostedByUserNavigation).WithMany(p => p.Movies)
                .HasForeignKey(d => d.PostedByUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movie_User");
        });

        modelBuilder.Entity<MovieCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MovieCategory");

            entity.HasOne(d => d.Category).WithMany()
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovieCategory_Category");

            entity.HasOne(d => d.Movie).WithMany()
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovieCategory_Movie");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A2B6E133E");

            entity.ToTable("Role");

            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.ToTable("Statistic");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.Statistics)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Statistic_Movie");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CD6205E43");

            entity.ToTable("User");

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.RegistedDate).HasColumnType("datetime");
            entity.Property(e => e.Username).IsRequired();

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
