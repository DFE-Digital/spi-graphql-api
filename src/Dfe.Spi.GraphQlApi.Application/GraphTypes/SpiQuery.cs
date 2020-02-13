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
                    new QueryArgument<IntGraphType> {Name = "establishmentNumber"},
                    new QueryArgument<IntGraphType> {Name = "previousEstablishmentNumber"},
                }));

            Field<ListGraphType<LearningProvider>>("learningProviders",
                resolve: learningProvidersResolver.ResolveAsync,
                arguments: new QueryArguments(new QueryArgument[]
                {
                    new QueryArgument<StringGraphType> {Name = "name"},
                    new QueryArgument<StringGraphType> {Name = "type"},
                    new QueryArgument<StringGraphType> {Name = "typeOperator", DefaultValue = "equals"},
                    new QueryArgument<StringGraphType> {Name = "subType"},
                    new QueryArgument<StringGraphType> {Name = "subTypeOperator", DefaultValue = "equals"},
                    new QueryArgument<StringGraphType> {Name = "status"},
                    new QueryArgument<StringGraphType> {Name = "statusOperator", DefaultValue = "equals"},
                    new QueryArgument<StringGraphType> {Name = "openDate"},
                    new QueryArgument<StringGraphType> {Name = "openDateOperator", DefaultValue = "equals"},
                    new QueryArgument<StringGraphType> {Name = "closeDate"},
                    new QueryArgument<StringGraphType> {Name = "closeDateOperator", DefaultValue = "equals"},
                }));
        }
    }
}