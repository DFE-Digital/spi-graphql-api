using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class SpiQuery : ObjectGraphType
    {
        public SpiQuery(ILearningProviderResolver learningProviderResolver)
        {
            Field<ListGraphType<LearningProviderType>>("learningProviders",
                resolve: learningProviderResolver.ResolveAsync,
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "name"}));
        }
    }
}