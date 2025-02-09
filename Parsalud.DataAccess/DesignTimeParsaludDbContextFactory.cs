using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Parsalud.DataAccess;

public class DesignTimeParsaludDbContextFactory : IDesignTimeDbContextFactory<ParsaludDbContext>
{
    public ParsaludDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ParsaludDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=Parsalud;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;");

        return new ParsaludDbContext(optionsBuilder.Options);
    }
}