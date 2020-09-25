using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Context;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;

namespace Dfe.Spi.GraphQlApi.Application.Loaders
{
    public class LearningProviderManagementGroupLoader : ILoader<LearningProviderPointer, ManagementGroup>
    {
        private readonly IRegistryProvider _registryProvider;
        private readonly IEntityRepository _entityRepository;
        private readonly IGraphExecutionContextManager _executionContextManager;
        private readonly ILoggerWrapper _logger;

        public LearningProviderManagementGroupLoader(
            IRegistryProvider registryProvider,
            IEntityRepository entityRepository,
            IGraphExecutionContextManager executionContextManager,
            ILoggerWrapper logger)
        {
            _registryProvider = registryProvider;
            _entityRepository = entityRepository;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        public async Task<IDictionary<LearningProviderPointer, ManagementGroup>> LoadAsync(
            IEnumerable<LearningProviderPointer> keys,
            CancellationToken cancellationToken)
        {
            var learningProviderPointers = keys.ToArray();
            _logger.Debug($"Looking up management groups for {learningProviderPointers.Length} providers");

            var managementGroupLinks = await GetManagementGroupLinksAsync(learningProviderPointers, cancellationToken);

            var managementGroups = await LoadManagementGroupsAsync(
                managementGroupLinks, 
                learningProviderPointers.FirstOrDefault()?.Fields, 
                learningProviderPointers.FirstOrDefault()?.PointInTime, 
                cancellationToken);

            var results = TransformToDictionary(learningProviderPointers, managementGroupLinks, managementGroups);

            return results;
        }

        private async Task<EntityLinkBatchResult[]> GetManagementGroupLinksAsync(
            LearningProviderPointer[] learningProviderPointers,
            CancellationToken cancellationToken)
        {
            var entityReferences = learningProviderPointers.Select(pointer =>
                new TypedEntityReference
                {
                    EntityType = "learning-provider",
                    SourceSystemName = pointer.SourceSystemName,
                    SourceSystemId = pointer.SourceSystemId,
                }).ToArray();
            var pointInTime = learningProviderPointers.FirstOrDefault()?.PointInTime;
            var entityLinks = await _registryProvider.GetLinksBatchAsync(entityReferences, pointInTime, cancellationToken);
            _logger.Debug($"Found {entityLinks.Length} entity links");

            return entityLinks
                .Select(result =>
                    new EntityLinkBatchResult
                    {
                        Entity = result.Entity,
                        Links = result.Links?.Where(l => l.LinkType == "ManagementGroup").ToArray(),
                    })
                .ToArray();
        }

        private async Task<SquashedEntityResult<ManagementGroup>[]> LoadManagementGroupsAsync(
            EntityLinkBatchResult[] links,
            string[] fields,
            DateTime? pointInTime,
            CancellationToken cancellationToken)
        {
            _logger.Debug($"Loading {links.Length} management groups");

            var managementGroupPointers = new List<AggregateEntityReference>();
            foreach (var batchResult in links)
            {
                if (batchResult.Links == null)
                {
                    continue;
                }

                foreach (var link in batchResult.Links)
                {
                    managementGroupPointers.Add(new AggregateEntityReference
                    {
                        AdapterRecordReferences = new[]
                        {
                            new EntityReference
                            {
                                SourceSystemName = link.SourceSystemName,
                                SourceSystemId = link.SourceSystemId,
                            },
                        },
                    });
                }
            }
            
            var request = new LoadManagementGroupsRequest()
            {
                EntityReferences = managementGroupPointers.ToArray(),
                Fields = fields,
                Live = _executionContextManager.GraphExecutionContext.QueryLive,
                PointInTime = pointInTime,
            };

            var entityCollection = await _entityRepository.LoadManagementGroupsAsync(request, cancellationToken);

            var results = new Dictionary<TypedEntityReference, ManagementGroup>();

            return entityCollection.SquashedEntityResults;
        }

        private IDictionary<LearningProviderPointer, ManagementGroup> TransformToDictionary(
            LearningProviderPointer[] learningProviderPointers,
            EntityLinkBatchResult[] managementGroupLinks,
            SquashedEntityResult<ManagementGroup>[] managementGroups)
        {
            var results = new Dictionary<LearningProviderPointer, ManagementGroup>();

            foreach (var learningProviderPointer in learningProviderPointers)
            {
                var link = managementGroupLinks.SingleOrDefault(l =>
                    l.Entity.SourceSystemName.Equals(learningProviderPointer.SourceSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                    l.Entity.SourceSystemId.Equals(learningProviderPointer.SourceSystemId, StringComparison.InvariantCultureIgnoreCase));
                var managementGroupPointer = link?.Links?.FirstOrDefault();
                var managementGroup = managementGroupPointer != null
                    ? managementGroups.SingleOrDefault(se => se.EntityReference.AdapterRecordReferences.Any(ar =>
                        ar.SourceSystemName.Equals(managementGroupPointer.SourceSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        ar.SourceSystemId.Equals(managementGroupPointer.SourceSystemId, StringComparison.InvariantCultureIgnoreCase)))?.SquashedEntity
                    : null;
                
                results.Add(learningProviderPointer, managementGroup);
            }

            return results;
        }
    }
}