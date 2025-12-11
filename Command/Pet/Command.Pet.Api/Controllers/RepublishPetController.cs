using Microsoft.AspNetCore.Mvc;
using Cqrs.Infrastructure;
using Cqrs.Command.Pet;
using Command.Pet.Api.DTO;

namespace Command.Pet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepublishPetController : ControllerBase
    {
        private readonly ILogger<RepublishPetController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public RepublishPetController(ILogger<RepublishPetController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost(Name = "RepublishPet")]
        public async Task<ActionResult<PetResponse>> RepublishPetAsync()
        {
            try
            {
                await _commandDispatcher.SendAsync(new RepublishPetCommand());
                return StatusCode(StatusCodes.Status201Created, new PetResponse()
                {
                    Message = "RepublishPet event successfully sent.",
                });
            }
            catch (InvalidOperationException error)
            {
                const string RESPONSE_MESSAGE = "RepublishPet client input error.";
                _logger.Log(LogLevel.Warning, error, RESPONSE_MESSAGE);

                return StatusCode(StatusCodes.Status400BadRequest, new PetResponse()
                {
                    Message = RESPONSE_MESSAGE
                });
            }
            catch (Exception error)
            {
                const string RESPONSE_MESSAGE = "RepublishPet system error.";
                _logger.Log(LogLevel.Error, error, RESPONSE_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new PetResponse()
                {
                    Message = RESPONSE_MESSAGE
                });
            }
        }
    }
}
