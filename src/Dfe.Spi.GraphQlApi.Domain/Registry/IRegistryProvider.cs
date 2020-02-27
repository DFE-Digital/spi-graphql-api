using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public interface IRegistryProvider
    {
        Task<EntityReference[]> GetSynonymsAsync(string entityType, string sourceSystem, string sourceSystemId, CancellationToken cancellationToken);
        Task<EntityLinkReference[]> GetLinksAsync(string entityType, string sourceSystem, string sourceSystemId, CancellationToken cancellationToken);
    }
}