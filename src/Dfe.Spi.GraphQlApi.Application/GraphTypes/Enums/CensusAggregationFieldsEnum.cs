using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums
{
    public class CensusAggregationFieldsEnum : EnumerationGraphType
    {
        public CensusAggregationFieldsEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "CensusAggregationFields";
            Description = "Census Aggregation Fields";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.CensusAggregationFields);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}