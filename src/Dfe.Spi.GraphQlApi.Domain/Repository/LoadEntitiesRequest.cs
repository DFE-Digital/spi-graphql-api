using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public abstract class LoadEntitiesRequest
    {
        public string EntityName { get; protected set; }
        public AggregateEntityReference[] EntityReferences { get; set; }
    }
}