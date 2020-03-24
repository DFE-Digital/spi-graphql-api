using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Caching.Definitions;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Http;
using Dfe.Spi.Common.Http.Client;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Enumerations;
using Newtonsoft.Json;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.TranslatorApi
{
    public class TranslatorApiEnumerationRepository : IEnumerationRepository
    {
        private readonly EnumerationRepositoryConfiguration _configuration;
        private readonly IRestClient _restClient;
        private readonly ISpiExecutionContextManager _executionContextManager;
        private readonly ICacheProvider _cacheProvider;
        private readonly ILoggerWrapper _logger;

        public TranslatorApiEnumerationRepository(
            EnumerationRepositoryConfiguration configuration,
            IRestClient restClient,
            ISpiExecutionContextManager executionContextManager,
            ICacheProvider cacheProvider,
            ILoggerWrapper logger)
        {
            _configuration = configuration;
            _restClient = restClient;
            _executionContextManager = executionContextManager;
            _cacheProvider = cacheProvider;
            _logger = logger;

            _restClient.BaseUrl = new Uri(configuration.TranslatorApiBaseUrl);
            if (!string.IsNullOrEmpty(configuration.TranslatorApiFunctionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.AzureFunctionKeyHeaderName,
                    configuration.TranslatorApiFunctionKey, ParameterType.HttpHeader));
            }

            if (!string.IsNullOrEmpty(configuration.TranslatorApiSubscriptionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter(CommonHeaderNames.EapimSubscriptionKeyHeaderName,
                    configuration.TranslatorApiSubscriptionKey,
                    ParameterType.HttpHeader));
            }
        }

        public async Task<string[]> GetEnumerationValuesAsync(string enumName, CancellationToken cancellationToken)
        {
            var resource = $"enumerations/{enumName}";

            var cached = (string[]) (await _cacheProvider.GetCacheItemAsync(resource, cancellationToken));
            if (cached != null)
            {
                return cached;
            }

            _logger.Info($"Calling {resource} on translator api");
            var request = new RestRequest(resource, Method.GET);
            request.AppendContext(_executionContextManager.SpiExecutionContext);

            var response = await _restClient.ExecuteTaskAsync(request, cancellationToken);
            if (!response.IsSuccessful)
            {
                throw new TranslatorApiException(resource, response.StatusCode, response.Content);
            }

            _logger.Debug($"Received {response.Content}");
            var result = JsonConvert.DeserializeObject<GetEnumerationValuesResult>(response.Content);

            await _cacheProvider.AddCacheItemAsync(resource, result.EnumerationValuesResult.EnumerationValues,
                new TimeSpan(0, 1, 0), cancellationToken);

            return result.EnumerationValuesResult.EnumerationValues;
        }
    }
}