using Microsoft.AspNetCore.Mvc;
using Cqrs.Infrastructure;
using Cqrs.Command.Pet;
using Command.Pet.Api.DTO;

namespace Command.Pet.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DeletePetController : ControllerBase
{
    private readonly ILogger<DeletePetController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public DeletePetController(ILogger<DeletePetController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost(Name = "DeletePet")]
    public async Task<ActionResult<PetResponse>> DeletePetAsync(DeletePetCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new PetResponse()
            {
                Message = "DeletePet event successfully sent.",
            });
        }
        catch (InvalidOperationException error)
        {
            const string RESPONSE_MESSAGE = "DeletePet client input error.";
            _logger.Log(LogLevel.Warning, error, RESPONSE_MESSAGE);

            return StatusCode(StatusCodes.Status400BadRequest, new PetResponse()
            {
                Message = RESPONSE_MESSAGE
            });
        }
        catch (Exception error)
        {
            const string RESPONSE_MESSAGE = "DeletePet system error.";
            _logger.Log(LogLevel.Error, error, RESPONSE_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new PetResponse()
            {
                Message = RESPONSE_MESSAGE
            });
        }
    }
}
