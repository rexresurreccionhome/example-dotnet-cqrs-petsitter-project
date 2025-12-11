using Microsoft.EntityFrameworkCore;

namespace Query.Pet.Infrastructure.DataAccess;

public class DatabaseContextFactory: IDatabaseContextFactory
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public DatabaseContextFactory(DbContextOptions<ApplicationDbContext> options)
    {
        _options = options;
    }

    public ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(_options);
    }
}
