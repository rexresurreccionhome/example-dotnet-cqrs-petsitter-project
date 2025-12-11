using Cqrs.Query;
using Query.Pet.Domain.Entities;

namespace Query.Pet.Api.DTO;

public class PetLookupResponse : BaseResponse
{
    public List<PetEntity> Pets { get; set; } = [];
    public required int Count { get; set; } = 0;
}
