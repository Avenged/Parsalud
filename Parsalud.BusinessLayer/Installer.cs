﻿using Microsoft.EntityFrameworkCore;
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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddTransient<IPostService, PostService>();
    }
}
