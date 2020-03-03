using Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs;
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
            Field<LearningProvider>("learningProvider",
                resolve: learningProviderResolver.ResolveAsync,
                arguments: new QueryArguments(new QueryArgument[]
                {
                    new QueryArgument<IntGraphType> {Name = "urn"},
                    new QueryArgument<IntGraphType> {Name = "ukprn"},
                    new QueryArgument<StringGraphType> {Name = "uprn"},
                    new QueryArgument<StringGraphType> {Name = "companiesHouseNumber"},
                    new QueryArgument<StringGraphType> {Name = "charitiesCommissionNumber"},
                    new QueryArgument<StringGraphType> {Name = "dfeNumber"},
                }));

            Field<ListGraphType<LearningProvider>>("learningProviders",
                resolve: learningProvidersResolver.ResolveAsync,
                arguments: new QueryArguments(
                    new QueryArgument<ComplexQuery> { Name = "criteria" }));
        }
    }
}