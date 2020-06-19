using System;
using Dfe.Spi.Common.Context.Models;
using Dfe.Spi.Common.Http.Server;
using Dfe.Spi.GraphQlApi.Domain.Context;
using Microsoft.AspNetCore.Http;

namespace Dfe.Spi.GraphQlApi.Functions
{
    public class HttpGraphExecutionContextManager : HttpSpiExecutionContextManager, IGraphExecutionContextManager
    {
        public override SpiExecutionContext SpiExecutionContext => GraphExecutionContext;

        public GraphExecutionContext GraphExecutionContext { get; private set; }

        public override void SetContext(IHeaderDictionary headerDictionary)
        {
            var context = new GraphExecutionContext();
            
            base.ReadHeadersIntoContext(headerDictionary, context);

            var queryLiveHeader = headerDictionary.GetHeaderValue("X-QUERY-LIVE") ?? string.Empty;
            context.QueryLive = queryLiveHeader.Equals("YES", StringComparison.InvariantCultureIgnoreCase)
                                || queryLiveHeader.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase)
                                || queryLiveHeader.Equals("1", StringComparison.InvariantCultureIgnoreCase);

            GraphExecutionContext = context;
        }
    }
}