using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Domain.Graph;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Dfe.Spi.GraphQlApi.Functions.GraphQuery
{
    public class ProcessGraphQuery
    {
        private const string FunctionName = nameof(ProcessGraphQuery);
        private readonly SpiSchema _spiSchema;
        private readonly ILoggerWrapper _logger;

        public ProcessGraphQuery(SpiSchema spiSchema, ILoggerWrapper logger)
        {
            _spiSchema = spiSchema;
            _logger = logger;
        }

        [FunctionName(FunctionName)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "graphql")]
            HttpRequest req,
            CancellationToken cancellationToken)
        {
            _logger.SetContext(req.Headers);
            _logger.SetInternalRequestId(Guid.NewGuid());

            var graphRequest = await ExtractGraphRequestAsync(req);
            var result = await _spiSchema.ExecuteAsync(graphRequest);
            
            return new OkObjectResult(result);
        }


        private async Task<GraphRequest> ExtractGraphRequestAsync(HttpRequest req)
        {
            if (req.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                return GraphRequest.Parse(req.Query["query"], "application/graphql");
            }
            else if (req.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                var contentType = req.ContentType.Contains(";")
                    ? req.ContentType.Substring(0, req.ContentType.IndexOf(";"))
                    : req.ContentType;
                using (var reader = new StreamReader(req.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    return GraphRequest.Parse(body, contentType);
                }
            }

            return null;
        }
    }
}