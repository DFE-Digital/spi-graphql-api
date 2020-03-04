using Dfe.Spi.Models.Entities;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class LoadCensusRequest : LoadEntitiesRequest
    {
        public LoadCensusRequest()
        {
            EntityName = nameof(Census);
        }
    }
}