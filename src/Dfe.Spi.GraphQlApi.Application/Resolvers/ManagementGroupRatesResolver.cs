using System;
using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IManagementGroupRatesResolver : IResolver<Models.Entities.ManagementGroupRates>
    {
    }
    public class ManagementGroupRatesResolver : IManagementGroupRatesResolver
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILoggerWrapper _logger;

        public ManagementGroupRatesResolver(
            IEntityRepository entityRepository,
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _logger = logger;
        }
        
        public async Task<ManagementGroupRates> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var entityId = BuildEntityId(context);
            if (entityId == null)
            {
                return null;
            }
            
            try
            {
                var request = new LoadManagementGroupRatesRequest
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
                                    SourceSystemName = SourceSystemNames.Rates,
                                },
                            }
                        },
                    },
                };
                var rates = await _entityRepository.LoadManagementGroupRatesAsync(request, context.CancellationToken);
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
            var sourceManagementGroup = context.Source as ManagementGroup;
            if (!(sourceManagementGroup?.Code?.StartsWith("LocalAuthority-") ?? false))
            {
                return null;
            }
            
            var year = context.Arguments["year"];
            var laCode = sourceManagementGroup.Code.Substring(15);

            return $"{year}-{laCode}";
        }
    }
}