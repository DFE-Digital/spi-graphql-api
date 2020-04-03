using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ManagementGroupsPagedModel
    {
        public Models.Entities.ManagementGroup[] Results { get; set; }
        public PaginationDetailsModel Pagination { get; set; }
    }

    public class ManagementGroupsPaged : ObjectGraphType<ManagementGroupsPagedModel>
    {
        public ManagementGroupsPaged()
        {
            Field<ListGraphType<ManagementGroup>>("results",
                resolve: ctx => ctx.Source.Results);
            
            Field<PaginationDetails>("_pagination",
                resolve: ctx => ctx.Source.Pagination);
        }
    }
}