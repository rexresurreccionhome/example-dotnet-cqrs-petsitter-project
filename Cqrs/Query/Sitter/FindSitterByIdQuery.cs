namespace Cqrs.Query.Sitter;

public class FindSitterByIdQuery : BaseQuery
{
    public Guid SitterId { get; set; }
}