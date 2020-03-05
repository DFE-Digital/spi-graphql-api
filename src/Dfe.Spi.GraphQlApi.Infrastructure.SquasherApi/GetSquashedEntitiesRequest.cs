using Dfe.Spi.GraphQlApi.Domain.Repository;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SquasherApi
{
    public class GetSquashedEntitiesRequest
    {
        public string EntityName { get; set; }
        public SquasherEntityReference[] EntityReferences { get; set; }
        public string[] Fields { get; set; }
        public AggregatesRequest AggregatesRequest { get; set; }
    }
}