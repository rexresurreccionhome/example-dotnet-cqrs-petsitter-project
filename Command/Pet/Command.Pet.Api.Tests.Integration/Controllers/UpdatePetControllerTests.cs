using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Cqrs.Command.Pet;
using Command.Pet.Api.DTO;
using Microsoft.Extensions.DependencyInjection;

namespace Command.Pet.Api.Tests.Integration.Controllers;

public class UpdatePetControllerTests
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory = new PetApiWebApplicationFactory();

    public UpdatePetControllerTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Post_UpdatePet_Returns201Created()
    {
        // Arrange
        var createPetCommand = new CreatePetCommand
        {
            PetId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            Name = "Buddy"
        };
        using (var scope = _factory.Services.CreateScope())
        {
            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler>();
            await handler.HandleAsync(createPetCommand);
        }
        var command = new UpdatePetCommand
        {
            PetId = createPetCommand.PetId,
            Name = "Jimmy"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/UpdatePet", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        PetResponse? petResponse = await response.Content.ReadFromJsonAsync<PetResponse>();
        Assert.NotNull(petResponse);
        Assert.Equal("UpdatePet event successfully sent.", petResponse.Message);
    }
}
