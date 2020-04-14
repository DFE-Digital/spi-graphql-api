using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Http;
using Dfe.Spi.Common.Http.Client;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.Models;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using Newtonsoft.Json;
using RestSharp;
using ModelsBase = Dfe.Spi.Models.ModelsBase;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SquasherApi
{
    public class SquasherEntityRepository : IEntityRepository
    {
        private readonly IRestClient _restClient;
        private readonly ISpiExecutionContextManager _executionContextManager;
        private readonly ILoggerWrapper _logger;

        public SquasherEntityRepository(
            IRestClient restClient, 
            EntityRepositoryConfiguration configuration,
            ISpiExecutionContextManager executionContextManager,
            ILoggerWrapper logger)
        {
            _restClient = restClient;
            _restClient.Timeout = 600 * 1000; // 600s - 10m
            _executionContextManager = executionContextManager;
            _restClient.BaseUrl = new Uri(configuration.SquasherApiBaseUrl, UriKind.Absolute);
            if (!string.IsNullOrEmpty(configuration.SquasherApiFunctionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.AzureFunctionKeyHeaderName, configuration.SquasherApiFunctionKey,
                    ParameterType.HttpHeader));
            }
            if (!string.IsNullOrEmpty(configuration.SquasherApiSubscriptionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.EapimSubscriptionKeyHeaderName, configuration.SquasherApiSubscriptionKey,
                    ParameterType.HttpHeader));
            }

            _logger = logger;
        }

        public async Task<EntityCollection<LearningProvider>> LoadLearningProvidersAsync(
            LoadLearningProvidersRequest request, CancellationToken cancellationToken)
        {
            return await LoadAsync<LearningProvider>(request, cancellationToken);
        }

        public async Task<EntityCollection<ManagementGroup>> LoadManagementGroupsAsync(LoadManagementGroupsRequest request, CancellationToken cancellationToken)
        {
            return await LoadAsync<ManagementGroup>(request, cancellationToken);
        }

        public async Task<EntityCollection<Census>> LoadCensusAsync(LoadCensusRequest request, CancellationToken cancellationToken)
        {
            return await LoadAsync<Census>(request, cancellationToken);
        }

        public async Task<EntityCollection<LearningProviderRates>> LoadLearningProviderRatesAsync(LoadLearningProviderRatesRequest request, CancellationToken cancellationToken)
        {
            return await LoadAsync<LearningProviderRates>(request, cancellationToken);
        }

        public async Task<EntityCollection<ManagementGroupRates>> LoadManagementGroupRatesAsync(LoadManagementGroupRatesRequest request, CancellationToken cancellationToken)
        {
            return await LoadAsync<ManagementGroupRates>(request, cancellationToken);
        }


        private async Task<EntityCollection<T>> LoadAsync<T>(LoadEntitiesRequest request, CancellationToken cancellationToken) where T : ModelsBase
        {
            const string resource = "get-squashed-entity";

            var squasherRequest = new GetSquashedEntitiesRequest
            {
                EntityName = request.EntityName,
                EntityReferences = request.EntityReferences?.Select(er =>
                    new SquasherEntityReference
                    {
                        AdapterRecordReferences = er.AdapterRecordReferences.Select(arr =>
                            new SquasherAdapterReference
                            {
                                Source = arr.SourceSystemName,
                                Id = arr.SourceSystemId,
                            }).ToArray(),
                    })?.ToArray(),
                Fields = request.Fields,
                AggregatesRequest = request.AggregatesRequest,
            };
            var json = JsonConvert.SerializeObject(squasherRequest);
            _logger.Debug($"Search request going to {resource} is {json}");

            var httpRequest = new RestRequest(resource, Method.POST, DataFormat.Json);
            httpRequest.AddParameter("", json, ParameterType.RequestBody);
            httpRequest.AppendContext(_executionContextManager.SpiExecutionContext);

            var response = await _restClient.ExecuteTaskAsync(httpRequest, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                try
                {
                    var errorBody = JsonConvert.DeserializeObject<HttpErrorBody>(response.Content);
                    if (errorBody.ErrorIdentifier == "SPI-ESQ-6")
                    {
                        return new EntityCollection<T>
                        {
                            SquashedEntityResults = request.EntityReferences?.Select(x =>
                                new SquashedEntityResult<T>()).ToArray(),
                        };
                    }
                    
                    _logger.Warning($"Received 404, but ErrorIdentifier was not expected. Expected SPI-ESQ-6, Received {errorBody.ErrorIdentifier} (message: {errorBody.Message}");
                }
                catch(Exception ex)
                {
                    _logger.Warning($"Received 404, but was unable to process error body - {ex.Message}", ex);
                }
            }
            if (!response.IsSuccessful)
            {
                throw new SquasherApiException(resource, response.StatusCode, response.Content);
            }

            _logger.Debug($"Search response json from {resource} is ${response.Content}");

            var results = JsonConvert.DeserializeObject<EntityCollection<T>>(response.Content);
            _logger.Debug($"Deserialized response from {resource} to {JsonConvert.SerializeObject(results)}");

            return results;
        }
    }
}