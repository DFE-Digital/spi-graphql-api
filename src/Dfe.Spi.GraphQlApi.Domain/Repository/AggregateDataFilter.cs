using Dfe.Spi.Common.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class AggregateDataFilter
    {
        public string Field { get; set; }
        public DataOperator Operator { get; set; }
        public string Value { get; set; }
    }
}