using System.Linq;
using Dfe.Spi.GraphQlApi.Domain.Enumerations;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IEnumerationLoader
    {
        EnumValueDefinition[] GetEnumerationValues(string enumName);
    }

    public class EnumerationLoader : IEnumerationLoader
    {
        private readonly IEnumerationRepository _enumerationRepository;

        public EnumerationLoader(IEnumerationRepository enumerationRepository)
        {
            _enumerationRepository = enumerationRepository;
        }

        public EnumValueDefinition[] GetEnumerationValues(string enumName)
        {
            var values = _enumerationRepository.GetEnumerationValuesAsync(enumName, default).Result;
            return values.Select(v => new EnumValueDefinition
            {
                Name = v.Replace(" ", "")
                        .Replace("-", "to")
                        .Replace("'", "")
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace(",", "")
                        .Replace(".", ""),
                Description = v,
                Value = v,
            }).ToArray();
        }
    }
}