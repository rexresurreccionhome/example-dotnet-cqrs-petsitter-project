using Microsoft.AspNetCore.Mvc;
using Cqrs.Infrastructure;
using Cqrs.Command.Sitter;
using Command.Sitter.Api.DTO;

namespace Command.Sitter.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UpdateSitterController : ControllerBase
{
    private readonly ILogger<UpdateSitterController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public UpdateSitterController(ILogger<UpdateSitterController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut(Name = "UpdateSitter")]
    public async Task<ActionResult<SitterResponse>> UpdateSitterAsync(UpdateSitterCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status200OK, new SitterResponse
            {
                Message = "UpdateSitter event successfully sent."
            });
        }
        catch (InvalidOperationException error)
        {
            const string RESPONSE_MESSAGE = "UpdateSitter client input error.";
            _logger.Log(LogLevel.Warning, error, RESPONSE_MESSAGE);

            return StatusCode(StatusCodes.Status400BadRequest, new SitterResponse
            {
                Message = RESPONSE_MESSAGE
            });
        }
        catch (Exception error)
        {
            const string RESPONSE_MESSAGE = "UpdateSitter system error.";
            _logger.Log(LogLevel.Error, error, RESPONSE_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new SitterResponse
            {
                Message = RESPONSE_MESSAGE
            });
        }
    }
}