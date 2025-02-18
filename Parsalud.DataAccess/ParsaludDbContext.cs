using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Parsalud.DataAccess.Models;

namespace Parsalud.DataAccess;

public class ParsaludDbContext(DbContextOptions<ParsaludDbContext> options) : IdentityDbContext<ParsaludUser, ParsaludRole, Guid>(options)
{
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<Faq> Faqs { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<StyleSheet> StyleSheets { get; set; } = null!;
    public DbSet<PostCategory> PostCategories { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(5), null);
            });
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Post>(x =>
        {
            x.HasOne(x => x.PostCategory)
                .WithMany(x => x.Posts)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<PostCategory>(x =>
        {
            x.HasMany(x => x.Posts)
                .WithOne(x => x.PostCategory)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Section>(x =>
        {
            x.HasIndex(x => x.Code)
                .IsUnique(unique: true);

            x.HasOne(x => x.StyleSheet)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.StyleSheetId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        builder.Entity<StyleSheet>(x =>
        {
            x.HasIndex(x => x.FileName)
                .IsUnique(unique: true);

            x.HasMany(x => x.Sections)
                .WithOne(x => x.StyleSheet)
                .HasForeignKey(x => x.StyleSheetId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
    }
}