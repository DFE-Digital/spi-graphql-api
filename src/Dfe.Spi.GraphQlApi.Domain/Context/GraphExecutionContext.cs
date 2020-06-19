using Dfe.Spi.Common.Context.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Context
{
    public class GraphExecutionContext : SpiExecutionContext
    {
        public bool QueryLive { get; set; }
    }
}