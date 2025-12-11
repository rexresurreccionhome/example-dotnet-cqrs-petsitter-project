namespace Command.Sitter.Api.DTO;

public class CreateSitterResponse: SitterResponse
{
    public required Guid SitterId { get; set; }
}