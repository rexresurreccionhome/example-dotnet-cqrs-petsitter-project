namespace Command.Pet.Api.DTO;

public class CreatePetResponse: PetResponse
{
    public required Guid PetId { get; set; }
}
