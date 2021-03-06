using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public interface IRegistryProvider
    {
        Task<SearchResultSet> SearchLearningProvidersAsync(SearchRequest request, CancellationToken cancellationToken);
        Task<SearchResultSet> SearchManagementGroupsAsync(SearchRequest request, CancellationToken cancellationToken);
        Task<EntityReference[]> GetSynonymsAsync(string entityType, string sourceSystem, string sourceSystemId, CancellationToken cancellationToken);
        Task<EntityLinkReference[]> GetLinksAsync(string entityType, string sourceSystem, string sourceSystemId, DateTime? pointInTime, CancellationToken cancellationToken);
        Task<EntityLinkBatchResult[]> GetLinksBatchAsync(TypedEntityReference[] entityReferences, DateTime? pointInTime, CancellationToken cancellationToken);
    }
}