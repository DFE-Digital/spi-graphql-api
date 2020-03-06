using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums
{
    public class LearningProviderGenderOfEntryEnum : EnumerationGraphType
    {
        public LearningProviderGenderOfEntryEnum(IEnumerationLoader enumerationLoader)
        {
            Name = "GenderOfEntry";
            Description = "Gender Of Entry or learning provider";

            var values = enumerationLoader.GetEnumerationValues(EnumerationNames.GenderOfEntry);
            foreach (var value in values)
            {
                AddValue(value);
            }
        }
    }
}