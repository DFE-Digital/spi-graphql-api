using System;

namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public class SearchRequest
    {
        public SearchGroup[] Groups { get; set; }
        public string CombinationOperator { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public DateTime? PointInTime { get; set; }
    }
}