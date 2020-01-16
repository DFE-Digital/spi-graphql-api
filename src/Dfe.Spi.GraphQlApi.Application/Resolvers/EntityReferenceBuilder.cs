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

            var tasks = searchResults.Documents.Select(source =>
                GetSynonyms(source, cancellationToken));

            var references = await Task.WhenAll(tasks);

            return references;
        }

        private async Task<AggregateEntityReference> GetSynonyms(EntityReference source, CancellationToken cancellationToken)
        {
            var synonyms = await _getSynonymsFunc(
                source.SourceSystemName, source.SourceSystemId, cancellationToken);

            return new AggregateEntityReference
            {
                AdapterRecordReferences = new[] {source}.Concat(synonyms).ToArray(),
            };
        }
    }
}