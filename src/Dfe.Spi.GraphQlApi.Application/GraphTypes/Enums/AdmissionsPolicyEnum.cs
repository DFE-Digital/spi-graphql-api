using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums
{
    public class AdmissionsPolicyEnum : EnumerationGraphType
    {
        public AdmissionsPolicyEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "BoardersCode";
            Description = "Boarders Code";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.AdmissionsPolicy);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}