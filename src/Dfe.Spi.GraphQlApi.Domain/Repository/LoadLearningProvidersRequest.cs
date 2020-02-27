using Dfe.Spi.Models.Entities;

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