using Dfe.Spi.Models.Entities;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class LoadRatesRequest : LoadEntitiesRequest
    {
        public LoadRatesRequest()
        {
            EntityName = nameof(Rates);
        }
    }
}