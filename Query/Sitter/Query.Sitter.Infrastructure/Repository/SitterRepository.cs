using Microsoft.EntityFrameworkCore;
using Query.Sitter.Domain.Entities;
using Query.Sitter.Infrastructure.DataAccess;

namespace Query.Sitter.Infrastructure.Repository;

public class SitterRepository : ISitterRepository
{
    private readonly IDatabaseContextFactory _databaseContextFactory;

    public SitterRepository(IDatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public async Task CreateAsync(SitterEntity sitter)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        context.Sitters.Add(sitter);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SitterEntity sitter)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        context.Sitters.Update(sitter);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid sitterId)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        var sitter = await GetByIdAsync(sitterId);
        if (sitter != null)
        {
            context.Sitters.Remove(sitter);
            await context.SaveChangesAsync();
        }
    }

    public async Task<SitterEntity> GetByIdAsync(Guid sitterId)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Sitters.SingleAsync(s => s.SitterId == sitterId);
    }

    public async Task<List<SitterEntity>> ListAllAsync()
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Sitters.ToListAsync();
    }

    public async Task<List<SitterEntity>> ListByMemberAsync(Guid memberId)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Sitters.Where(s => s.MemberId == memberId).ToListAsync();
    }
}