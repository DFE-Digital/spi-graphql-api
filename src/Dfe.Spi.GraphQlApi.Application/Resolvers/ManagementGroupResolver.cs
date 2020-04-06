using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IManagementGroupResolver : IResolver<Models.Entities.ManagementGroup>
    {
    }
    public class ManagementGroupResolver : IManagementGroupResolver
    {
        private readonly IRegistryProvider _registryProvider;
        private readonly IEntityRepository _entityRepository;
        private readonly ILoggerWrapper _logger;

        public ManagementGroupResolver(
            IRegistryProvider registryProvider,
            IEntityRepository entityRepository,
            ILoggerWrapper logger)
        {
            _registryProvider = registryProvider;
            _entityRepository = entityRepository;
            _logger = logger;
        }
        
        public async Task<Models.Entities.ManagementGroup> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            try
            {
                if (context.Source is Models.Entities.LearningProvider learningProvider)
                {
                    var managementGroupReference = await GetEntityReferenceAsync(learningProvider, context.CancellationToken);
                    if (managementGroupReference == null)
                    {
                        return null;
                    }

                    var fields = GetRequestedFields(context);
                    var managementGroup =
                        await GetManagementGroup(managementGroupReference, fields, context.CancellationToken);
                    return managementGroup;
                }

                return null;
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info($"Invalid request when resolving management group - {ex.ErrorIdentifier} - {ex.Message}", ex);
                context.Errors.AddRange(
                    ex.Details.Select(detailsMessage => new ExecutionError(detailsMessage)));
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving management group", ex);
                throw;
            }
        }

        private async Task<EntityReference> GetEntityReferenceAsync(LearningProvider learningProvider, CancellationToken cancellationToken)
        {
            string sourceSystemName;
            string sourceSystemId;
            if (learningProvider.Urn.HasValue)
            {
                sourceSystemName = SourceSystemNames.GetInformationAboutSchools;
                sourceSystemId = learningProvider.Urn.Value.ToString();
            }
            else if (learningProvider.Ukprn.HasValue)
            {
                sourceSystemName = SourceSystemNames.UkRegisterOfLearningProviders;
                sourceSystemId = learningProvider.Ukprn.Value.ToString();
            }
            else
            {
                return null;
            }
            
            var links = await _registryProvider.GetLinksAsync("learning-providers", sourceSystemName, sourceSystemId, cancellationToken);
            var managementGroupLink = links?.FirstOrDefault(l =>
                l.LinkType.Equals("ManagementGroup", StringComparison.InvariantCultureIgnoreCase));
            return managementGroupLink;
        }

        private async Task<ManagementGroup> GetManagementGroup(EntityReference managementGroupReference, string[] fields,
            CancellationToken cancellationToken)
        {
            var request = new LoadManagementGroupsRequest()
            {
                EntityReferences = new[]
                {
                    new AggregateEntityReference
                    {
                        AdapterRecordReferences = new []{managementGroupReference}
                    }, 
                },
                Fields = fields,
            };

            var entityCollection = await _entityRepository.LoadManagementGroupsAsync(request, cancellationToken);
            return entityCollection.SquashedEntityResults.Select(x => x.SquashedEntity).SingleOrDefault();
        }

        private string[] GetRequestedFields<T>(ResolveFieldContext<T> context)
        {
            var selections = context.FieldAst.SelectionSet.Selections.Select(x => ((Field) x).Name);
            return selections.ToArray();
        }
    }
}