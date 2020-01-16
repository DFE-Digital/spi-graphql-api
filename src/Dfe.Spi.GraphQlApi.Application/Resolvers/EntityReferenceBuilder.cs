using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    internal interface IEntityReferenceBuilder
    {
        Task<AggregateEntityReference[]> GetEntityReferences(
            SearchRequest searchRequest, CancellationToken cancellationToken);
    }

    internal class EntityReferenceBuilder<TEntityReference> : IEntityReferenceBuilder
        where TEntityReference : EntityReference
    {
        private readonly Func<SearchRequest, CancellationToken, Task<SearchResultSet<TEntityReference>>> _searchFunc;
        private readonly Func<string, string, CancellationToken, Task<EntityReference[]>> _getSynonymsFunc;

        public EntityReferenceBuilder(
            Func<SearchRequest, CancellationToken, Task<SearchResultSet<TEntityReference>>> searchFunc,
            Func<string, string, CancellationToken, Task<EntityReference[]>> getSynonymsFunc)
        {
            _searchFunc = searchFunc;
            _getSynonymsFunc = getSynonymsFunc;
        }

        public async Task<AggregateEntityReference[]> GetEntityReferences(
            SearchRequest searchRequest, CancellationToken cancellationToken)
        {
            var searchResults = await _searchFunc(searchRequest, cancellationToken);

            var references = new AggregateEntityReference[searchResults.Documents.Length];
            for (var i = 0; i < searchResults.Documents.Length; i++)
            {
                var result = searchResults.Documents[i];
                var synonyms =
                    await _getSynonymsFunc(result.SourceSystemName, result.SourceSystemId, cancellationToken);

                references[i] = new AggregateEntityReference
                {
                    AdapterRecordReferences = new[] {result}.Concat(synonyms).ToArray(),
                };
            }

            return references;
        }
    }
}