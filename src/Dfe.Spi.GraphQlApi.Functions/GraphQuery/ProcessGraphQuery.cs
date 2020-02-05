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

        public ProcessGraphQuery(SpiSchema spiSchema, ILoggerWrapper logger, IHttpSpiExecutionContextManager contextManager)
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
            var startTime = DateTime.Now;

            _contextManager.SetContext(req.Headers);
            _contextManager.SetInternalRequestId(Guid.NewGuid());

            string result;
            if (req.Method.Equals("GET") && req.Query.ContainsKey("schema"))
            {
                result = new SchemaPrinter(_spiSchema).Print();
            }
            else
            {
                var graphRequest = await ExtractGraphRequestAsync(req);
                result = await _spiSchema.ExecuteAsync(graphRequest);
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

    public class AuditedOkObjectResult : OkObjectResult
    {
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public Guid RequestId { get; }
        public string ConsumerRequestId { get; }

        public AuditedOkObjectResult(
            object value, 
            DateTime startTime, 
            DateTime endTime, 
            Guid requestId,
            string consumerRequestId)
            : base(value)
        {
            StartTime = startTime;
            EndTime = endTime;
            RequestId = requestId;
            ConsumerRequestId = consumerRequestId;
        }

        public override void OnFormatting(ActionContext context)
        {
            base.OnFormatting(context);

            context.HttpContext.Response.Headers.Add("X-SPI-Start-Time", StartTime.ToString("O"));
            context.HttpContext.Response.Headers.Add("X-SPI-End-Time", EndTime.ToString("O"));
            context.HttpContext.Response.Headers.Add("X-SPI-Request-Id", RequestId.ToString());
            context.HttpContext.Response.Headers.Add("X-SPI-Consumer-Request-Id", ConsumerRequestId);
        }
    }
}