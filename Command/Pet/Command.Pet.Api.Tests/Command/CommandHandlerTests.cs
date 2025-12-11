using Moq;
using Command.Pet.Api.Command;
using Command.Pet.Domain;
using Cqrs.Infrastructure.Handler;
using Cqrs.Command.Pet;

namespace Command.Pet.Api.Tests.Command;

public class CommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatePetCommand_CallsSaveAsync()
    {
        // Arrange
        var mockEventSourcingHandler = new Mock<IEventSourcingHandler<PetAggregate>>();
        var handler = new CommandHandler(mockEventSourcingHandler.Object);
        var command = new CreatePetCommand
        {
            PetId = new Guid("af89ee30-fb08-490b-ad34-a48cdac8d6de"),
            MemberId = new Guid("528a98cd-2f50-4697-a307-98034439a922"),
            Name = "Buddy",
        };

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockEventSourcingHandler.Verify(x => x.SaveAsync(It.IsAny<PetAggregate>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdatePetCommand_CallsSaveAsync()
    {
        // Arrange
        var mockEventSourcingHandler = new Mock<IEventSourcingHandler<PetAggregate>>();
        var petAggregate = new PetAggregate(
                petId: new Guid("af89ee30-fb08-490b-ad34-a48cdac8d6de"),
                memberId: new Guid("528a98cd-2f50-4697-a307-98034439a922"),
                name: "OldName"
            );
        mockEventSourcingHandler
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(petAggregate);
        var handler = new CommandHandler(mockEventSourcingHandler.Object);
        var command = new UpdatePetCommand
        {
            PetId = new Guid("af89ee30-fb08-490b-ad34-a48cdac8d6de"),
            Name = "Buddy",
        };

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockEventSourcingHandler.Verify(x => x.SaveAsync(petAggregate), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeletePetCommand_CallsSaveAsync()
    {
        // Arrange
        var mockEventSourcingHandler = new Mock<IEventSourcingHandler<PetAggregate>>();
        var petAggregate = new PetAggregate(
                petId: new Guid("af89ee30-fb08-490b-ad34-a48cdac8d6de"),
                memberId: new Guid("528a98cd-2f50-4697-a307-98034439a922"),
                name: "Buddy"
            );
        mockEventSourcingHandler
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(petAggregate);
        var handler = new CommandHandler(mockEventSourcingHandler.Object);
        var command = new DeletePetCommand
        {
            PetId = new Guid("af89ee30-fb08-490b-ad34-a48cdac8d6de"),
        };

        // Act
        await handler.HandleAsync(command);

        // Assert
        Assert.False(petAggregate.IsActive, "Pet should be marked as inactive after deletion.");
        mockEventSourcingHandler.Verify(x => x.SaveAsync(petAggregate), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RepublishPetCommand_CallsRepublishEventsAsync()
    {
        // Arrange
        var mockEventSourcingHandler = new Mock<IEventSourcingHandler<PetAggregate>>();
        var handler = new CommandHandler(mockEventSourcingHandler.Object);
        var command = new RepublishPetCommand();

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockEventSourcingHandler.Verify(x => x.RepublishEventsAsync(), Times.Once);
    }
}
