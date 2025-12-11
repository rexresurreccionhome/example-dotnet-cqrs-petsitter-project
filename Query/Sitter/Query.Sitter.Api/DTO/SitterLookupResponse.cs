using Cqrs.Query;
using Query.Sitter.Domain.Entities;

namespace Query.Sitter.Api.DTO;

public class SitterLookupResponse : BaseResponse
{
    public List<SitterEntity> Sitters { get; set; } = new();
    public int Count { get; set; }
}