using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi
{
    internal class GetLinksResult
    {
        public EntityLinkReference[] Links { get; set; }
    }
}