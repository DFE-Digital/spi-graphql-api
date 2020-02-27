using System.Threading.Tasks;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IManagementGroupProvider : IResolver<Models.Entities.ManagementGroup>
    {
    }
    public class ManagementGroupProvider : IManagementGroupProvider
    {
        public async Task<Models.Entities.ManagementGroup> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            if (context.Source is Models.Entities.LearningProvider learningProvider)
            {
                return new Models.Entities.ManagementGroup
                {
                    Type = "Stub",
                    Identifier = "test001",
                    Name = "Test One",
                    Code = "STUB-TEST001",
                    CompaniesHouseNumber = "36259712",
                };
            }
            
            return null;
        }
    }
}