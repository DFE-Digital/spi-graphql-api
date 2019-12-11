using System.Threading.Tasks;
using Dfe.Spi.Models;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class SpiQuery : ObjectGraphType
    {
        public SpiQuery()
        {
            Field<ListGraphType<LearningProviderType>>("learningProviders",
                resolve: ResolveLearningProvidersAsync,
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "name"}));
        }

        private async Task<LearningProvider[]> ResolveLearningProvidersAsync(ResolveFieldContext<object> context)
        {
            return new[]
            {
                new LearningProvider {Name = "Provider 1"},
                new LearningProvider {Name = "Provider 2"},
            };
        }
    }
}