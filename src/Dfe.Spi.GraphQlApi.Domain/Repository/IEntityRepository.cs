using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public interface IEntityRepository
    {
        Task<EntityCollection<LearningProvider>> LoadLearningProvidersAsync(LoadLearningProvidersRequest request, CancellationToken cancellationToken);
    }
}