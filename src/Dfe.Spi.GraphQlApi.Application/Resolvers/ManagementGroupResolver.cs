using System;
using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Loaders;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Context;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL;
using GraphQL.DataLoader;
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
        private readonly IDataLoaderContextAccessor _accessor;
        private readonly ILoader<LearningProviderPointer, ManagementGroup> _loader;
        private readonly IGraphExecutionContextManager _executionContextManager;
        private readonly ILoggerWrapper _logger;

        public ManagementGroupResolver(
            IRegistryProvider registryProvider,
            IEntityRepository entityRepository,
            IDataLoaderContextAccessor accessor, 
            ILoader<LearningProviderPointer, ManagementGroup> loader,
            IGraphExecutionContextManager executionContextManager,
            ILoggerWrapper logger)
        {
            _registryProvider = registryProvider;
            _entityRepository = entityRepository;
            _accessor = accessor;
            _loader = loader;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }
        
        public async Task<Models.Entities.ManagementGroup> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            try
            {
                if (context.Source is Models.Entities.LearningProvider learningProvider)
                {
                    if (_accessor == null)
                    {
                        throw new Exception("_accessor null");
                    }
                    if (_loader == null)
                    {
                        throw new Exception("_loader null");
                    }
                    
                    LearningProviderPointer pointer;
                    if (learningProvider.Urn.HasValue)
                    {
                        pointer = new LearningProviderPointer
                        {
                            SourceSystemName = SourceSystemNames.GetInformationAboutSchools,
                            SourceSystemId = learningProvider.Urn.Value.ToString(),
                        };
                    }
                    else if (learningProvider.Ukprn.HasValue)
                    {
                        pointer = new LearningProviderPointer
                        {
                            SourceSystemName = SourceSystemNames.UkRegisterOfLearningProviders,
                            SourceSystemId = learningProvider.Ukprn.Value.ToString(),
                        };
                    }
                    else
                    {
                        return null;
                    }

                    pointer.Fields = GetRequestedFields(context);
                    pointer.PointInTime = GetPointInTime(context);
                    
                    var loader = _accessor.Context.GetOrAddBatchLoader<LearningProviderPointer, ManagementGroup>(
                        "GetLearningProviderManagementGroup", _loader.LoadAsync);
                    return await loader.LoadAsync(pointer);
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

        private DateTime? GetPointInTime<TContext>(ResolveFieldContext<TContext> context)
        {
            if (context.HasArgument("pointInTime"))
            {
                return context.GetPointInTimeArgument();
            }

            var parent = context.Document.GetParentOf(context.FieldAst);
            if (parent == null)
            {
                return null;
            }

            return parent.Arguments.GetPointInTimeArgument();
        }

        private string[] GetRequestedFields<T>(ResolveFieldContext<T> context)
        {
            var selections = context.FieldAst.SelectionSet.Selections.Select(x => ((Field) x).Name);
            return selections.ToArray();
        }
    }
}