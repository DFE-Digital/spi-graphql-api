using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums
{
    public class LearningProviderStatusEnum : EnumerationGraphType
    {
        public LearningProviderStatusEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "ProviderStatus";
            Description = "Status of learning provider";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.ProviderStatus);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}