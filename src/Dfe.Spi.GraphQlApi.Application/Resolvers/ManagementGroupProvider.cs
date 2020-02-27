using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.Models.Entities;
using GraphQL.Introspection;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IManagementGroupProvider : IResolver<Models.Entities.ManagementGroup>
    {
    }
    public class ManagementGroupProvider : IManagementGroupProvider
    {
        private readonly IRegistryProvider _registryProvider;

        public ManagementGroupProvider(
            IRegistryProvider registryProvider)
        {
            _registryProvider = registryProvider;
        }
        
        public async Task<Models.Entities.ManagementGroup> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            if (context.Source is Models.Entities.LearningProvider learningProvider)
            {
                var managementGroupReference = await GetEntityReferenceAsync(learningProvider, context.CancellationToken);
                if (managementGroupReference == null)
                {
                    return null;
                }
                
                return new Models.Entities.ManagementGroup
                {
                    Type = "Stub",
                    Identifier = "test001",
                    Name = "Test One",
                    Code = "STUB-TEST001",
                    CompaniesHouseNumber = "36259712",
                };
            }
            
            return null;
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
    }
}