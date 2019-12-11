using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Search
{
    public class SearchResultSet<T> where T: EntityReference
    {
        public T[] Documents { get; set; }
    }
}