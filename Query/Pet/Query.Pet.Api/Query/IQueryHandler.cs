using Cqrs.Query.Pet;
using Query.Pet.Domain.Entities;

namespace Query.Pet.Api.Query;

public interface IQueryHandler
{
    public Task<List<PetEntity>> HandleAsync(FindAllPetsQuery query);
    public Task<List<PetEntity>> HandleAsync(FindPetsByMemberIdQuery query);
    public Task<List<PetEntity>> HandleAsync(FindPetByIdQuery query);
}
