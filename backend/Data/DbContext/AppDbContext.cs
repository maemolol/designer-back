using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Paintings> Paintings { get; set; }
    public DbSet<Height> Height { get; set; }
    public DbSet<Width> Width { get; set; }
    public DbSet<Category> Category { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Paintings>().ToTable("paintings");
        modelBuilder.Entity<Height>().ToTable("height");
        modelBuilder.Entity<Width>().ToTable("width");
        modelBuilder.Entity<Category>().ToTable("category");

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Paintings> (entity =>
        {
            entity.Property(p => p.Heightid).HasColumnName("height_id");
            entity.Property(p => p.Widthid).HasColumnName("width_id");
            entity.Property(p => p.Categoryid).HasColumnName("category_id");
            entity.Property(p => p.Imagelink).HasColumnName("image_link");

            entity.HasOne(p => p.Height)
                .WithMany()
                .HasForeignKey(r => r.Heightid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Width)
                .WithMany()
                .HasForeignKey(r => r.Widthid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(r => r.Categoryid)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // INDEXES
        // WHERE, JOIN, EXISTS, ANY, FindAsync = index need

        // Users
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.id).IsUnique();

        // Pets

        modelBuilder.Entity<Paintings>()
            .HasIndex(p => p.Heightid).IsUnique();

        modelBuilder.Entity<Paintings>()
            .HasIndex(p => p.Widthid).IsUnique();

        modelBuilder.Entity<Paintings>()
            .HasIndex(p => p.Categoryid);
    }
}
