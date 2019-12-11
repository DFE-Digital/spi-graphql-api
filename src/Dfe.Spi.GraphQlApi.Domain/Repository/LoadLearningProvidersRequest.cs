using Dfe.Spi.GraphQlApi.Domain.Search;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class LoadLearningProvidersRequest : LoadEntitiesRequest<LearningProviderReference>
    {
        public LoadLearningProvidersRequest()
        {
            EntityName = "LearningProvider";
        }
    }
}