using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Http;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.Http.Client;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Newtonsoft.Json;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi
{
    public class RegistryApiRegistryProvider : IRegistryProvider
    {
        private readonly IRestClient _restClient;
        private readonly ISpiExecutionContextManager _executionContextManager;
        private readonly ILoggerWrapper _logger;

        public RegistryApiRegistryProvider(
            IRestClient restClient, 
            RegistryConfiguration configuration, 
            ISpiExecutionContextManager executionContextManager,
            ILoggerWrapper logger)
        {
            _restClient = restClient;
            _executionContextManager = executionContextManager;
            _restClient.BaseUrl = new Uri(configuration.RegistryApiBaseUrl, UriKind.Absolute);
            if (!string.IsNullOrEmpty(configuration.RegistryApiFunctionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.AzureFunctionKeyHeaderName, configuration.RegistryApiFunctionKey,
                    ParameterType.HttpHeader));
            }
            if (!string.IsNullOrEmpty(configuration.RegistryApiSubscriptionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.EapimSubscriptionKeyHeaderName, configuration.RegistryApiSubscriptionKey,
                    ParameterType.HttpHeader));
            }

            _logger = logger;
        }

        public async Task<SearchResultSet> SearchLearningProvidersAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var resource = $"search/learning-providers";
            return await SearchAsync(resource, request, cancellationToken);
        }

        public async Task<SearchResultSet> SearchManagementGroupsAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var resource = $"search/management-group";
            return await SearchAsync(resource, request, cancellationToken);
        }

        public async Task<EntityReference[]> GetSynonymsAsync(string entityType, string sourceSystem, string sourceSystemId,
            CancellationToken cancellationToken)
        {
            var resource = $"{entityType}/{sourceSystem}/{sourceSystemId}/synonyms";
            _logger.Debug($"Looking up synonyms at {resource}");
            
            var httpRequest = new RestRequest(resource, Method.GET);
            httpRequest.AppendContext(_executionContextManager.SpiExecutionContext);
            
            var response = await _restClient.ExecuteTaskAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new EntityReference[0];
                }
                
                throw new RegistryApiException(resource, response.StatusCode, response.Content);
            }
            _logger.Debug($"Synonyms response json from {resource} is ${response.Content}");

            var results = JsonConvert.DeserializeObject<GetSynonymsResult>(response.Content);
            _logger.Debug($"Deserialized response from {resource} to {JsonConvert.SerializeObject(results)}");

            return results.Synonyms;
        }
        public async Task<EntityLinkReference[]> GetLinksAsync(string entityType, string sourceSystem, string sourceSystemId,
            CancellationToken cancellationToken)
        {
            var resource = $"{entityType}/{sourceSystem}/{sourceSystemId}/links";
            _logger.Debug($"Looking up links at {resource}");
            
            var httpRequest = new RestRequest(resource, Method.GET);
            httpRequest.AppendContext(_executionContextManager.SpiExecutionContext);
            
            var response = await _restClient.ExecuteTaskAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new EntityLinkReference[0];
                }
                
                throw new RegistryApiException(resource, response.StatusCode, response.Content);
            }
            _logger.Debug($"Links response json from {resource} is ${response.Content}");

            var results = JsonConvert.DeserializeObject<GetLinksResult>(response.Content);
            _logger.Debug($"Deserialized response from {resource} to {JsonConvert.SerializeObject(results)}");

            return results.Links;
        }


        private async Task<SearchResultSet> SearchAsync(string resource, SearchRequest request, CancellationToken cancellationToken)
        {
            _logger.Debug($"Searching {resource}");

            var json = JsonConvert.SerializeObject(request);
            _logger.Debug($"Search request json going to {resource} is ${json}");
            
            var httpRequest = new RestRequest(resource, Method.POST);
            httpRequest.AppendContext(_executionContextManager.SpiExecutionContext);
            httpRequest.AddParameter(string.Empty, json, "application/json", ParameterType.RequestBody);
            
            var response = await _restClient.ExecuteTaskAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                throw new RegistryApiException(resource, response.StatusCode, response.Content);
            }
            _logger.Debug($"Search response json from {resource} is ${response.Content}");

            var resultset = JsonConvert.DeserializeObject<SearchResultSet>(response.Content);
            _logger.Debug($"Deserialized response from {resource} to {JsonConvert.SerializeObject(resultset)}");

            return resultset;
        }
    }
}