using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProviderSubTypeEnum : EnumerationGraphType
    {
        public LearningProviderSubTypeEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "ProviderSubType";
            Description = "Sub-Type of learning provider";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.ProviderSubType);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}