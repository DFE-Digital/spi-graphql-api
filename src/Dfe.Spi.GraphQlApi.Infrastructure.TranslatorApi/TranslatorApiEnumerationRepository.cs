using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ILoggerWrapper _logger;

        public TranslatorApiEnumerationRepository(
            EnumerationRepositoryConfiguration configuration,
            IRestClient restClient,
            ILoggerWrapper logger)
        {
            _configuration = configuration;
            _restClient = restClient;
            _logger = logger;

            _restClient.BaseUrl = new Uri(configuration.TranslatorApiBaseUrl);
            if (!string.IsNullOrEmpty(configuration.TranslatorApiFunctionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter("x-functions-key",
                    configuration.TranslatorApiFunctionKey, ParameterType.HttpHeader));
            }
        }

        public async Task<string[]> GetEnumerationValuesAsync(string enumName, CancellationToken cancellationToken)
        {
            var resource = $"enumerations/{enumName}";
            _logger.Info($"Calling {resource} on translator api");
            var request = new RestRequest(resource, Method.GET);
            var response = await _restClient.ExecuteTaskAsync(request, cancellationToken);
            if (!response.IsSuccessful)
            {
                throw new TranslatorApiException(resource, response.StatusCode, response.Content);
            }

            _logger.Info($"Received {response.Content}");
            var result = JsonConvert.DeserializeObject<GetEnumerationValuesResult>(response.Content);
            return result.EnumerationValuesResult.EnumerationValues;
        }
    }
}