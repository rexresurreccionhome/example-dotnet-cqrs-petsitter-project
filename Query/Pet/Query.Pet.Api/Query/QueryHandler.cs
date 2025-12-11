using Cqrs.Query.Pet;
using Query.Pet.Domain.Entities;
using Query.Pet.Infrastructure.Repository;

namespace Query.Pet.Api.Query;

public class QueryHandler : IQueryHandler
{
    private readonly IPetRepository _petRepository;

    public QueryHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<List<PetEntity>> HandleAsync(FindAllPetsQuery query)
    {
        return await _petRepository.ListAllAsync();
    }

    public async Task<List<PetEntity>> HandleAsync(FindPetsByMemberIdQuery query)
    {
        return await _petRepository.ListByMemberAsync(query.MemberId);
    }

    public async Task<List<PetEntity>> HandleAsync(FindPetByIdQuery query)
    {
        PetEntity petEntity = await _petRepository.GetByPetIdAsync(query.PetId);
        return [petEntity];
    }
}
