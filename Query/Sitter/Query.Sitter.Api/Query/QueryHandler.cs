using Cqrs.Query.Sitter;
using Query.Sitter.Domain.Entities;
using Query.Sitter.Infrastructure.Repository;

namespace Query.Sitter.Api.Query;

public class QueryHandler : IQueryHandler
{
    private readonly ISitterRepository _sitterRepository;

    public QueryHandler(ISitterRepository sitterRepository)
    {
        _sitterRepository = sitterRepository;
    }

    public async Task<List<SitterEntity>> HandleAsync(FindAllSittersQuery query)
        => await _sitterRepository.ListAllAsync();

    public async Task<List<SitterEntity>> HandleAsync(FindSitterByIdQuery query)
    {
        var sitter = await _sitterRepository.GetByIdAsync(query.SitterId);
        return [sitter];
    }
}