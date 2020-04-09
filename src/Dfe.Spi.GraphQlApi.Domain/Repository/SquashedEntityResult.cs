using Dfe.Spi.GraphQlApi.Domain.Common;
using ModelsBase = Dfe.Spi.Models.ModelsBase;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class SquashedEntityResult<T> where T : ModelsBase
    {
        public AggregateEntityReference EntityReference { get; set; }
        public T SquashedEntity { get; set; }
        public EntityAdapterError[] EntityAdapterErrorDetails { get; set; }
    }
}