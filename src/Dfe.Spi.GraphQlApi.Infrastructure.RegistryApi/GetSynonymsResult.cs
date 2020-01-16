using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi
{
    internal class GetSynonymsResult
    {
        public EntityReference[] Synonyms { get; set; }
    }
}