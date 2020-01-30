using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProviderTypeEnum : EnumerationGraphType
    {
        public LearningProviderTypeEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "ProviderType";
            Description = "Type of learning provider";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.ProviderType);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}