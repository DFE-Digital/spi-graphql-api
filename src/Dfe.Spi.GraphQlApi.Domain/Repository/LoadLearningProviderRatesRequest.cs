using Dfe.Spi.Models.Entities;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class LoadLearningProviderRatesRequest : LoadEntitiesRequest
    {
        public LoadLearningProviderRatesRequest()
        {
            EntityName = nameof(LearningProviderRates);
        }
    }
}