using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Http.Server.Definitions;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Domain.Graph;
using GraphQL.Utilities;
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
        private readonly IHttpSpiExecutionContextManager _contextManager;

        public ProcessGraphQuery(
            SpiSchema spiSchema, 
            ILoggerWrapper logger, 
            IHttpSpiExecutionContextManager contextManager)
        {
            _spiSchema = spiSchema;
            _logger = logger;
            _contextManager = contextManager;
        }

        [FunctionName(FunctionName)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "graphql")]
            HttpRequest req,
            CancellationToken cancellationToken)
        {
            _logger.Info("Entering function call");
            var startTime = DateTime.Now;

            _contextManager.SetContext(req.Headers);
            _contextManager.SetInternalRequestId(Guid.NewGuid());
            _logger.Info("Set context");

            string result;
            if (req.Method.Equals("GET") && req.Query.ContainsKey("schema"))
            {
                _logger.Info("Sending schema via GET request");
                result = new SchemaPrinter(_spiSchema).Print();
            }
            else
            {
                _logger.Info("POST request detected, attempting graph QL query");
                var graphRequest = await ExtractGraphRequestAsync(req);
                _logger.Info($"Graph request extracted: {graphRequest}");
                result = await _spiSchema.ExecuteAsync(graphRequest);
                _logger.Info($"Graph result fetched: {result}");
            }

            var endTime = DateTime.Now;
            return new AuditedOkObjectResult(
                result, 
                startTime, 
                endTime, 
                _contextManager.SpiExecutionContext.InternalRequestId.Value,
                _contextManager.SpiExecutionContext.ExternalRequestId);
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