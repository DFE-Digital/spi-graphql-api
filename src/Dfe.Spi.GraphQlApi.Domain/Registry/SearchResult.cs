using System.Collections.Generic;
using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public class SearchResult
    {
        public EntityReference[] Entities { get; set; }
        public Dictionary<string, string> IndexedData { get; set; }
    }
}