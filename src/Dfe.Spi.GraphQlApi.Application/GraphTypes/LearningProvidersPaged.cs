using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProvidersPagedModel
    {
        public Models.Entities.LearningProvider[] Results { get; set; }
        public PaginationDetailsModel Pagination { get; set; }
    }

    public class LearningProvidersPaged : ObjectGraphType<LearningProvidersPagedModel>
    {
        public LearningProvidersPaged()
        {
            Field<ListGraphType<LearningProvider>>("results",
                resolve: ctx => ctx.Source.Results);
            
            Field<PaginationDetails>("_pagination",
                resolve: ctx => ctx.Source.Pagination);
        }
    }
}