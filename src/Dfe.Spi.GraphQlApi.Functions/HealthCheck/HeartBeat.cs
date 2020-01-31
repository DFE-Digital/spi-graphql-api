using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Dfe.Spi.GraphQlApi.Functions.HealthCheck
{
    public class HeartBeat
    {
        [FunctionName(nameof(HeartBeat))]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "HeartBeat")]
            HttpRequest httpRequest)
        {
            OkResult toReturn = new OkResult();

            // Just needs to return 200/OK.
            return toReturn;
        }
    }
}