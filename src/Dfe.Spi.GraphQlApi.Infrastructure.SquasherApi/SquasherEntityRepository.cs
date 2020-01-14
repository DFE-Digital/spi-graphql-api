using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SquasherApi
{
    public class SquasherEntityRepository : IEntityRepository
    {
        private readonly IRestClient _restClient;
        private readonly ILoggerWrapper _logger;

        public SquasherEntityRepository(IRestClient restClient, EntityRepositoryConfiguration configuration,
            ILoggerWrapper logger)
        {
            _restClient = restClient;
            _restClient.BaseUrl = new Uri(configuration.SquasherApiBaseUrl, UriKind.Absolute);
            if (!string.IsNullOrEmpty(configuration.SquasherApiFunctionKey))
            {
                _restClient.DefaultParameters.Add(new Parameter("x-functions-key", configuration.SquasherApiFunctionKey,
                    ParameterType.HttpHeader));
            }

            _logger = logger;
        }

        public async Task<EntityCollection<LearningProvider>> LoadLearningProvidersAsync(
            LoadLearningProvidersRequest request, CancellationToken cancellationToken)
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
                Fields = new[] {"Name"},
            };
            var json = JsonConvert.SerializeObject(squasherRequest);
            _logger.Debug($"Search request going to {resource} is {json}");

            var httpRequest = new RestRequest(resource, Method.POST, DataFormat.Json);
            httpRequest.AddParameter("", json, ParameterType.RequestBody);

            var response = await _restClient.ExecuteTaskAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                throw new SquasherApiException(resource, response.StatusCode, response.Content);
            }

            _logger.Debug($"Search response json from {resource} is ${response.Content}");

            var results = JsonConvert.DeserializeObject<EntityCollection<LearningProvider>>(response.Content);
            _logger.Debug($"Deserialized response from {resource} to {JsonConvert.SerializeObject(results)}");

            return results;
        }
    }

    public class GetSquashedEntitiesRequest
    {
        public string EntityName { get; set; }
        public SquasherEntityReference[] EntityReferences { get; set; }
        public string[] Fields { get; set; }
    }

    public class SquasherEntityReference
    {
        public SquasherAdapterReference[] AdapterRecordReferences { get; set; }
    }

    public class SquasherAdapterReference
    {
        public string Source { get; set; }
        public string Id { get; set; }
    }
}