using Microsoft.EntityFrameworkCore;

namespace Query.Pet.Infrastructure.DataAccess;

public interface IDatabaseContextFactory: IDbContextFactory<ApplicationDbContext>
{

}
