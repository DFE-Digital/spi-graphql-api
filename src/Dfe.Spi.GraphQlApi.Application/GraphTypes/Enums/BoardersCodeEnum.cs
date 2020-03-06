using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums
{
    public class BoardersCodeEnum : EnumerationGraphType
    {
        public BoardersCodeEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "BoardersCode";
            Description = "Boarders Code";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.BoardersCode);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}