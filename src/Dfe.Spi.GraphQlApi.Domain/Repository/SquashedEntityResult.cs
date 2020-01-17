using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class SquashedEntityResult<T> where T : ModelsBase
    {
        public AggregateEntityReference EntityReference { get; set; }
        public T SquashedEntity { get; set; }
        public object[] EntityAdapterErrorDetails { get; set; }
    }
}