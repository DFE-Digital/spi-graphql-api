using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    internal interface IEntityReferenceBuilder<TEntityReference> where TEntityReference : EntityReference
    {
        Task<AggregateEntityReference<TEntityReference>[]> GetEntityReferences(
            SearchRequest searchRequest, CancellationToken cancellationToken);
    }

    internal class EntityReferenceBuilder<TEntityReference> : IEntityReferenceBuilder<TEntityReference> where TEntityReference : EntityReference
    {
        private readonly Func<SearchRequest, CancellationToken, Task<SearchResultSet<TEntityReference>>> _searchFunc;

        public EntityReferenceBuilder(
            Func<SearchRequest, CancellationToken, Task<SearchResultSet<TEntityReference>>> searchFunc)
        {
            _searchFunc = searchFunc;
        }

        public async Task<AggregateEntityReference<TEntityReference>[]> GetEntityReferences(
            SearchRequest searchRequest, CancellationToken cancellationToken)
        {
            var searchResults = await _searchFunc(searchRequest, cancellationToken);
            
            return searchResults.Documents.Select(r =>
                new AggregateEntityReference<TEntityReference>
                {
                    AdapterRecordReferences = new[] {r},
                }).ToArray();
        }
    }
}