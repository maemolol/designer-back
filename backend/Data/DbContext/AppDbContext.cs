using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
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

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Paintings>()
            .HasOne<Height>()
            .WithMany()
            .HasForeignKey(r => r.height_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Paintings>()
            .HasOne<Width>()
            .WithMany()
            .HasForeignKey(r => r.shelter_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Painting>()
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey(r => r.category_id)
            .OnDelete(DeleteBehavior.Cascade);

        // INDEXES
        // WHERE, JOIN, EXISTS, ANY, FindAsync = index need

        // Users
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.id).IsUnique();

        // Pets
        modelBuilder.Entity<Paintings>()
            .HasIndex(p => p.name).IsUnique();

        modelBuilder.Entity<Paintings>()
            .HasIndex(p => p.painting_id).IsUnique();

        modelBuilder.Entity<Pets>()
            .HasIndex(p => p.category_id);
    }
}
