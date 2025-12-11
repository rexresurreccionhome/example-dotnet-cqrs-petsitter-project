using Microsoft.AspNetCore.Mvc;
using Cqrs.Command.Pet;
using Cqrs.Infrastructure;
using Command.Pet.Api.DTO;

namespace Command.Pet.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UpdatePetController : ControllerBase
{
    private readonly ILogger<UpdatePetController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public UpdatePetController(ILogger<UpdatePetController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost(Name = "UpdatePet")]
    public async Task<ActionResult<PetResponse>> UpdatePetAsync(UpdatePetCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new PetResponse()
            {
                Message = "UpdatePet event successfully sent.",
            });
        }
        catch (InvalidOperationException error)
        {
            const string RESPONSE_MESSAGE = "UpdatePet client input error";
            _logger.Log(LogLevel.Warning, error, RESPONSE_MESSAGE);

            return StatusCode(StatusCodes.Status400BadRequest, new PetResponse()
            {
                Message = RESPONSE_MESSAGE
            });
        }
        catch (Exception error)
        {
            const string RESPONSE_MESSAGE = "UpdatePet system error.";
            _logger.Log(LogLevel.Error, error, RESPONSE_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new PetResponse()
            {
                Message = RESPONSE_MESSAGE
            });
        }
    }
}
