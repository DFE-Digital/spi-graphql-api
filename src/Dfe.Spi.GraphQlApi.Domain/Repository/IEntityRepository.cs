using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Models.Entities;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public interface IEntityRepository
    {
        Task<EntityCollection<LearningProvider>> LoadLearningProvidersAsync(LoadLearningProvidersRequest request, CancellationToken cancellationToken);
        Task<EntityCollection<ManagementGroup>> LoadManagementGroupsAsync(LoadManagementGroupsRequest request, CancellationToken cancellationToken);
        Task<EntityCollection<Census>> LoadCensusAsync(LoadCensusRequest request, CancellationToken cancellationToken);
        Task<EntityCollection<LearningProviderRates>> LoadLearningProviderRatesAsync(LoadLearningProviderRatesRequest request, CancellationToken cancellationToken);
        Task<EntityCollection<ManagementGroupRates>> LoadManagementGroupRatesAsync(LoadManagementGroupRatesRequest request, CancellationToken cancellationToken);
    }
}