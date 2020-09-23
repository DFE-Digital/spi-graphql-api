using Dfe.Spi.GraphQlApi.Domain.Registry;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi
{
    public class GetLinksBatchResponse
    {
        public EntityLinkBatchResult[] Entities { get; set; }
    }
}