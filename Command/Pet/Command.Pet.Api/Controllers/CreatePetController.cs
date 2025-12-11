using Microsoft.AspNetCore.Mvc;
using Cqrs.Infrastructure;
using Cqrs.Command.Pet;
using Command.Pet.Api.DTO;

namespace Command.Pet.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CreatePetController : ControllerBase
{
    private readonly ILogger<CreatePetController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public CreatePetController(ILogger<CreatePetController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost(Name = "CreatePet")]
    public async Task<ActionResult<CreatePetResponse>> CreatePetAsync(CreatePetCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new CreatePetResponse()
            {
                PetId = command.PetId,
                Message = "CreatePet event successfully sent.",
            });
        }
        catch (InvalidOperationException error)
        {
            const string RESPONSE_MESSAGE = "CreatePet client input error.";
            _logger.Log(LogLevel.Warning, error, RESPONSE_MESSAGE);

            return StatusCode(StatusCodes.Status400BadRequest, new CreatePetResponse()
            {
                PetId = command.PetId,
                Message = RESPONSE_MESSAGE
            });
        }
        catch (Exception error)
        {
            const string RESPONSE_MESSAGE = "CreatePet system error.";
            _logger.Log(LogLevel.Error, error, RESPONSE_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new CreatePetResponse()
            {
                PetId = command.PetId,
                Message = RESPONSE_MESSAGE
            });
        }
    }
}
