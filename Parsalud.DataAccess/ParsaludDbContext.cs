using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parsalud.DataAccess.Models;

namespace Parsalud.DataAccess;

public class ParsaludDbContext(DbContextOptions<ParsaludDbContext> options) : IdentityDbContext<ParsaludUser, ParsaludRole, Guid>(options)
{
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<PostCategory> PostCategories { get; set; } = null!;

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
    }
}