using Dfe.Spi.Models.Entities;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class LoadManagementGroupsRequest : LoadEntitiesRequest
    {
        public LoadManagementGroupsRequest()
        {
            EntityName = nameof(ManagementGroup);
        }
    }
}