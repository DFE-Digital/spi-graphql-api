using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Http;
using Dfe.Spi.Common.Http.Client;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SearchApi
{
    public class SearchApiSearchProvider : ISearchProvider
    {
        private readonly IRestClient _restClient;
        private readonly ISpiExecutionContextManager _executionContextManager;
        private readonly ILoggerWrapper _logger;

        public SearchApiSearchProvider(
            IRestClient restClient, 
            SearchConfiguration configuration, 
            ISpiExecutionContextManager executionContextManager,
            ILoggerWrapper logger)
        {
            _restClient = restClient;
            _executionContextManager = executionContextManager;
            _restClient.BaseUrl = new Uri(configuration.SearchApiBaseUrl, UriKind.Absolute);
            if (!string.IsNullOrEmpty(configuration.SearchApiFunctionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.AzureFunctionKeyHeaderName, configuration.SearchApiFunctionKey,
                    ParameterType.HttpHeader));
            }
            if (!string.IsNullOrEmpty(configuration.SearchApiSubscriptionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.EapimSubscriptionKeyHeaderName, configuration.SearchApiSubscriptionKey,
                    ParameterType.HttpHeader));
            }

            _logger = logger;
        }
        
        public async Task<SearchResultSet<LearningProviderReference>> SearchLearningProvidersAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            return await ExecuteSearch<LearningProviderReference>("learning-providers", request, cancellationToken);
        }

        private async Task<SearchResultSet<TEntity>> ExecuteSearch<TEntity>(string resource, SearchRequest request, CancellationToken cancellationToken)
            where TEntity : EntityReference
        {
            var json = JsonConvert.SerializeObject(request);
            _logger.Debug($"Search request going to {resource} is {json}");
            
            var httpRequest = new RestRequest(resource, Method.POST, DataFormat.Json);
            httpRequest.AppendContext(_executionContextManager.SpiExecutionContext);
            httpRequest.AddParameter("", json, ParameterType.RequestBody);
            
            var response = await _restClient.ExecuteTaskAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                throw new SearchApiException(resource, response.StatusCode, response.Content);
            }
            _logger.Debug($"Search response json from {resource} is ${response.Content}");

            var results = JsonConvert.DeserializeObject<SearchResultSet<TEntity>>(response.Content);
            _logger.Debug($"Deserialized response from {resource} to {JsonConvert.SerializeObject(results)}");

            return results;
        }
    }
}