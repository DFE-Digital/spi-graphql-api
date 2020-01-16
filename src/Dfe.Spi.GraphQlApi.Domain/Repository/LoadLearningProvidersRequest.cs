using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class LoadLearningProvidersRequest : LoadEntitiesRequest
    {
        public LoadLearningProvidersRequest()
        {
            EntityName = nameof(LearningProvider);
        }
    }
}