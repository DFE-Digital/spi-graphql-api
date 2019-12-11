using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public abstract class LoadEntitiesRequest<T> where T : EntityReference
    {
        public string EntityName { get; protected set; }
        public T[] EntityReferences { get; set; }
    }
}