using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Cqrs.Command.Pet;
using Command.Pet.Api.DTO;

namespace Command.Pet.Api.Tests.Integration.Controllers;

public class CreatePetControllerTests
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory = new PetApiWebApplicationFactory();

    public CreatePetControllerTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Post_CreatePet_Returns201Created()
    {
        // Arrange
        var command = new CreatePetCommand
        {
            PetId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            Name = "Buddy"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/CreatePet", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        CreatePetResponse? createPetResponse = await response.Content.ReadFromJsonAsync<CreatePetResponse>();
        Assert.NotNull(createPetResponse);
        Assert.Equal(command.PetId, createPetResponse.PetId);
        Assert.Equal("CreatePet event successfully sent.", createPetResponse.Message);
    }
}
