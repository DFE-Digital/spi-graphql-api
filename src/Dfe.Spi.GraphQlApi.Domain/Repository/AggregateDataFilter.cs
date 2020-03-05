using Dfe.Spi.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class AggregateDataFilter
    {
        public string Field { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DataOperator Operator { get; set; }
        public string Value { get; set; }
    }
}