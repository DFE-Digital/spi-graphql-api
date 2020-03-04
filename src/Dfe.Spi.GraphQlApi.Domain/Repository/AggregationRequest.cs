using Dfe.Spi.Common.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class AggregationRequest
    {
        public string Name { get; set; }
        public AggregationRequestCondition[] Conditions { get; set; }
    }
    
    public class AggregationRequestCondition
    {
        public string Field { get; set; }
        public DataOperator Operator { get; set; }
        public string Value { get; set; }
    }
}