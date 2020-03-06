using System.Collections.Generic;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class AggregatesRequest
    {
        public Dictionary<string, AggregateQuery> AggregateQueries { get; set; } = new Dictionary<string, AggregateQuery>();
    }
}