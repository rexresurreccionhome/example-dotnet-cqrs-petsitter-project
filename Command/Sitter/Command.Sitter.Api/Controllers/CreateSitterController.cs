using Microsoft.AspNetCore.Mvc;
using Cqrs.Infrastructure;
using Cqrs.Command.Sitter;
using Command.Sitter.Api.DTO;

namespace Command.Sitter.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CreateSitterController : ControllerBase
{
    private readonly ILogger<CreateSitterController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public CreateSitterController(ILogger<CreateSitterController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost(Name = "CreateSitter")]
    public async Task<ActionResult<CreateSitterResponse>> CreateSitterAsync(CreateSitterCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new CreateSitterResponse()
            {
                SitterId = command.SitterId,
                Message = "CreateSitter event successfully sent.",
            });
        }
        catch (InvalidOperationException error)
        {
            const string RESPONSE_MESSAGE = "CreateSitter client input error.";
            _logger.Log(LogLevel.Warning, error, RESPONSE_MESSAGE);

            return StatusCode(StatusCodes.Status400BadRequest, new CreateSitterResponse()
            {
                SitterId = command.SitterId,
                Message = RESPONSE_MESSAGE
            });
        }
        catch (Exception error)
        {
            const string RESPONSE_MESSAGE = "CreateSitter system error.";
            _logger.Log(LogLevel.Error, error, RESPONSE_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new CreateSitterResponse()
            {
                SitterId = command.SitterId,
                Message = RESPONSE_MESSAGE
            });
        }
    }
}