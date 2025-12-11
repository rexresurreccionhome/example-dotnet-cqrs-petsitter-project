using Microsoft.AspNetCore.Mvc;
using Cqrs.Infrastructure;
using Cqrs.Query;
using Cqrs.Query.Pet;
using Query.Pet.Api.DTO;
using Query.Pet.Domain.Entities;

namespace Query.Pet.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PetLookupController : ControllerBase
    {
        private readonly ILogger<PetLookupController> _logger;
        private readonly IQueryDispatcher<PetEntity> _queryDispatcher;

        public PetLookupController(ILogger<PetLookupController> logger, IQueryDispatcher<PetEntity> queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetAllPetsAsync()
        {
            try
            {
                List<PetEntity> petEntities = await _queryDispatcher.SendAsync(new FindAllPetsQuery());
                return SuccessResponse(petEntities);
            }
            catch (Exception error)
            {
                return ErrorResponse(error, "Failed to query all pets.");
            }
        }

        [HttpGet("ByMemberId/{memberId}")]
        public async Task<ActionResult<BaseResponse>> GetPetsByMemberAsync(Guid memberId)
        {
            try
            {
                List<PetEntity> petEntities = await _queryDispatcher.SendAsync(new FindPetsByMemberIdQuery(){ MemberId = memberId});
                return SuccessResponse(petEntities);
            }
            catch (Exception error)
            {
                return ErrorResponse(error, "Failed to query pets by member.");
            }
        }

        [HttpGet("ById/{petId}")]
        public async Task<ActionResult<BaseResponse>> GetPetsByIdAsync(Guid petId)
        {
            try
            {
                List<PetEntity> petEntities = await _queryDispatcher.SendAsync(new FindPetByIdQuery(){PetId = petId});
                return SuccessResponse(petEntities);
            }
            catch (Exception error)
            {
                return ErrorResponse(error, "Failed to query pets by Id.");
            }
        }

        private ActionResult<BaseResponse> SuccessResponse(List<PetEntity> petEntities)
        {
            return Ok(
                new PetLookupResponse
                {
                    Pets = petEntities,
                    Count = petEntities.Count,
                    Message = $"Query returned {petEntities.Count} results",
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
}
