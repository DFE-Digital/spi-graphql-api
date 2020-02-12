using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LocalAuthorityCodeEnum : EnumerationGraphType
    {
        public LocalAuthorityCodeEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "LocalAuthorityCode";
            Description = "Local Authority Code";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.LocalAuthorityCode);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}