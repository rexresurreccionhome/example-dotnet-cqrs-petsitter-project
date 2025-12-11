using Query.Sitter.Domain.Entities;

namespace Query.Sitter.Infrastructure.Repository;

public interface ISitterRepository
{
    Task CreateAsync(SitterEntity sitter);
    Task UpdateAsync(SitterEntity sitter);
    Task DeleteAsync(Guid sitterId);
    Task<SitterEntity> GetByIdAsync(Guid sitterId);
    Task<List<SitterEntity>> ListAllAsync();
    Task<List<SitterEntity>> ListByMemberAsync(Guid memberId);
}