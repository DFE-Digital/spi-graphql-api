using System;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Spi.GraphQlApi.Functions.GraphQuery
{
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