using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;

namespace Parsalud.BusinessLayer;

public static class Installer
{
    public static void AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<ParsaludDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, options =>
            {
                options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(5), null);
            });
        });

        services.AddTransient<IStyleSheetService, StyleSheetService>();
        services.AddTransient<ISectionService, SectionService>();
        services.AddTransient<IFaqService, FaqService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IPostService, PostService>();
        services.AddTransient<IPostCategoryService, PostCategoryService>();
    }
}