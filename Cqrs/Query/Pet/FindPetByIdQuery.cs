namespace Cqrs.Query.Pet;

public class FindPetByIdQuery: BaseQuery
{
    public Guid PetId { get; set; }
}
