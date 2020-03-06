using Dfe.Spi.Common.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class AggregateQuery
    {
        public DataFilter[] DataFilters { get; set; }
    }
}