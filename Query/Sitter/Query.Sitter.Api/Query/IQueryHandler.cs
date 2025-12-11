using Cqrs.Query.Sitter;
using Query.Sitter.Domain.Entities;

namespace Query.Sitter.Api.Query;

public interface IQueryHandler
{
    Task<List<SitterEntity>> HandleAsync(FindAllSittersQuery query);
    Task<List<SitterEntity>> HandleAsync(FindSitterByIdQuery query);
}