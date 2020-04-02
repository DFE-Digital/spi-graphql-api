using Dfe.Spi.Models.Entities;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class LoadManagementGroupRatesRequest : LoadEntitiesRequest
    {
        public LoadManagementGroupRatesRequest()
        {
            EntityName = nameof(ManagementGroupRates);
        }
    }
}