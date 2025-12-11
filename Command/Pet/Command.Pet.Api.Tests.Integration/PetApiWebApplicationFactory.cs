using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Command.Pet.Api.Tests.Integration;

internal class PetApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            Environment.SetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_PET_EVENTS", "TestPetSitterPetEvents");
        });
    }
}
