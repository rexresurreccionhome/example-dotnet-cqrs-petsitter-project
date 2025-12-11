using Microsoft.EntityFrameworkCore;
using Query.Pet.Domain.Entities;
using Query.Pet.Infrastructure.DataAccess;

namespace Query.Pet.Infrastructure.Repository;

public class PetRepository : IPetRepository
{
    private readonly IDatabaseContextFactory _databaseContextFactory;

    public PetRepository(IDatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public async Task CreateAsync(PetEntity pet)
    {
        using ApplicationDbContext context = _databaseContextFactory.CreateDbContext();
        context.Pets.Add(pet);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid petId)
    {
        using ApplicationDbContext context = _databaseContextFactory.CreateDbContext();
        PetEntity? pet = await GetByPetIdAsync(petId);
        if (pet is not null)
        {
            context.Pets.Remove(pet);
            await context.SaveChangesAsync();
        }
    }

    public async Task<PetEntity> GetByPetIdAsync(Guid petId)
    {
        using ApplicationDbContext context = _databaseContextFactory.CreateDbContext();
        return await context.Pets.SingleAsync(pet => pet.PetId == petId);
    }

    public async Task<List<PetEntity>> ListAllAsync()
    {
        using ApplicationDbContext context = _databaseContextFactory.CreateDbContext();
        return await context.Pets.AsNoTracking().ToListAsync();
    }

    public async Task<List<PetEntity>> ListByMemberAsync(Guid memberId)
    {
        using ApplicationDbContext context = _databaseContextFactory.CreateDbContext();
        return await context.Pets
            .AsNoTracking().Where(pet => pet.MemberId == memberId).ToListAsync();
    }

    public async Task UpdateAsync(PetEntity pet)
    {
        using ApplicationDbContext context = _databaseContextFactory.CreateDbContext();
        context.Pets.Update(pet);
        await context.SaveChangesAsync();
    }
}
