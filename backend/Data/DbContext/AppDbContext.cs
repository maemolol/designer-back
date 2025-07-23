using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<paintings> paintings { get; set; }
    public DbSet<height> height { get; set; }
    public DbSet<width> width { get; set; }
    public DbSet<category> category { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<paintings>().ToTable("paintings");

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<paintings>()
            .HasOne<height>()
            .WithMany()
            .HasForeignKey(r => r.height_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<paintings>()
            .HasOne<width>()
            .WithMany()
            .HasForeignKey(r => r.width_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<paintings>()
            .HasOne<category>()
            .WithMany()
            .HasForeignKey(r => r.category_id)
            .OnDelete(DeleteBehavior.Cascade);

        // INDEXES
        // WHERE, JOIN, EXISTS, ANY, FindAsync = index need

        // Users
        modelBuilder.Entity<category>()
            .HasIndex(c => c.id).IsUnique();

        // Pets

        modelBuilder.Entity<paintings>()
            .HasIndex(p => p.height_id).IsUnique();

        modelBuilder.Entity<paintings>()
            .HasIndex(p => p.width_id).IsUnique();

        modelBuilder.Entity<paintings>()
            .HasIndex(p => p.category_id);
    }
}
