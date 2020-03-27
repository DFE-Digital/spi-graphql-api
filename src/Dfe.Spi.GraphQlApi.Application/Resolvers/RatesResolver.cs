using System;
using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ILearningProviderRatesResolver : IResolver<Models.Entities.LearningProviderRates>
    {
    }

    public class LearningProviderRatesResolver : ILearningProviderRatesResolver
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILoggerWrapper _logger;

        public LearningProviderRatesResolver(
            IEntityRepository entityRepository,
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _logger = logger;
        }
        
        
        public async Task<LearningProviderRates> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var entityId = BuildEntityId(context);
            
            try
            {
                var request = new LoadLearningProviderRatesRequest
                {
                    EntityReferences = new[]
                    {
                        new AggregateEntityReference
                        {
                            AdapterRecordReferences = new[]
                            {
                                new EntityReference
                                {
                                    SourceSystemId = entityId,
                                    SourceSystemName = "Rates",
                                },
                            }
                        },
                    },
                };
                var rates = await _entityRepository.LoadLearningProviderRatesAsync(request, context.CancellationToken);
                return rates.SquashedEntityResults.FirstOrDefault()?.SquashedEntity;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving rates for {entityId}: {ex.Message}", ex);
                throw;
            }
        }


        private string BuildEntityId<TContext>(ResolveFieldContext<TContext> context)
        {
            var sourceLearningProvider = context.Source as LearningProvider;
            if (!sourceLearningProvider.Urn.HasValue)
            {
                return null;
            }
            
            var year = context.Arguments["year"];

            return $"{year}-{sourceLearningProvider.Urn}";
        }
    }
}