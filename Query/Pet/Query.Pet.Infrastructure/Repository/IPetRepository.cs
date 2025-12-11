using Query.Pet.Domain.Entities;

namespace Query.Pet.Infrastructure.Repository;

public interface IPetRepository
{
    public Task CreateAsync(PetEntity pet);
    public Task UpdateAsync(PetEntity pet);
    public Task DeleteAsync(Guid petId);
    public Task<PetEntity> GetByPetIdAsync(Guid petId);
    public Task<List<PetEntity>> ListAllAsync();
    public Task<List<PetEntity>> ListByMemberAsync(Guid memberId);
}
