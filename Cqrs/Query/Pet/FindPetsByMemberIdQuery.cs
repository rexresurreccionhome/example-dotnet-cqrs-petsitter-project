namespace Cqrs.Query.Pet;

public class FindPetsByMemberIdQuery: BaseQuery
{
    public Guid MemberId { get; set; }
}
