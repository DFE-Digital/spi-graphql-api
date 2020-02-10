using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class SpiQuery : ObjectGraphType
    {
        public SpiQuery(
            ILearningProviderResolver learningProviderResolver,
            ILearningProvidersResolver learningProvidersResolver)
        {
            Field<LearningProviderType>("learningProvider",
                resolve: learningProviderResolver.ResolveAsync,
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "urn"}));
            
            Field<ListGraphType<LearningProviderType>>("learningProviders",
                resolve: learningProvidersResolver.ResolveAsync,
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "name"}));
        }
    }
}