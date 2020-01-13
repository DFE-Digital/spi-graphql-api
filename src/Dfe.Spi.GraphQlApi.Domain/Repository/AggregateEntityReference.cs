using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class AggregateEntityReference<T> where T : EntityReference
    {
        public T[] AdapterRecordReferences { get; set; }
    }
}