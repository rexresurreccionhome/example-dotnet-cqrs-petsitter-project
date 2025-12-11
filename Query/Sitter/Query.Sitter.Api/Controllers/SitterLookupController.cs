using Microsoft.AspNetCore.Mvc;
using Cqrs.Query;
using Cqrs.Query.Sitter;
using Cqrs.Infrastructure;
using Query.Sitter.Api.DTO;
using Query.Sitter.Domain.Entities;

namespace Query.Sitter.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SitterLookupController : ControllerBase
{
    private readonly ILogger<SitterLookupController> _logger;
    private readonly IQueryDispatcher<SitterEntity> _queryDispatcher;

    public SitterLookupController(ILogger<SitterLookupController> logger, IQueryDispatcher<SitterEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse>> GetAllSittersAsync()
    {
        try
        {
            List<SitterEntity> sitterEntities = await _queryDispatcher.SendAsync(new FindAllSittersQuery());
            return SuccessResponse(sitterEntities);
        }
        catch (Exception error)
        {
            return ErrorResponse(error, "Failed to query all sitters.");
        }
    }

    [HttpGet("ById/{sitterId}")]
    public async Task<ActionResult<BaseResponse>> GetSittersByIdAsync(Guid sitterId)
    {
        try
        {
            List<SitterEntity> sitterEntities = await _queryDispatcher.SendAsync(new FindSitterByIdQuery(){SitterId = sitterId});
            return SuccessResponse(sitterEntities);
        }
        catch (Exception error)
        {
            return ErrorResponse(error, "Failed to query sitters by Id.");
        }
    }

    private ActionResult<BaseResponse> SuccessResponse(List<SitterEntity> sitterEntities)
    {
        return Ok(
            new SitterLookupResponse
            {
                Sitters = sitterEntities,
                Count = sitterEntities.Count,
                Message = $"Query returned {sitterEntities.Count} results",
            }
        );
    }

    private ActionResult<BaseResponse> ErrorResponse(Exception error, string message)
    {
        _logger.LogError(error, message);
        return StatusCode(
            StatusCodes.Status500InternalServerError,
            new BaseResponse
            {
                Message = message,
            }
        );
    }
}