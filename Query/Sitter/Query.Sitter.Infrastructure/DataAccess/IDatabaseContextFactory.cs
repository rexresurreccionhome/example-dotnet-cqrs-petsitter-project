using Microsoft.EntityFrameworkCore;

namespace Query.Sitter.Infrastructure.DataAccess;

public interface IDatabaseContextFactory: IDbContextFactory<ApplicationDbContext>
{

}
