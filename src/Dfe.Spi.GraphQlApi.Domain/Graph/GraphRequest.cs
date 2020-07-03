using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dfe.Spi.GraphQlApi.Domain.Graph
{
    public class GraphRequest
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public JObject Variables { get; set; }
        // public Dictionary<string, object> Variables { get; set; }

        public static GraphRequest Parse(string value, string valueMimeType = "application/json")
        {
            if (valueMimeType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
            {
                return JsonConvert.DeserializeObject<GraphRequest>(value);
            }

            if (valueMimeType.Equals("application/graphql", StringComparison.InvariantCultureIgnoreCase))
            {
                return new GraphRequest
                {
                    Query = value,
                };
            }

            throw new ArgumentOutOfRangeException(nameof(valueMimeType),
                $"Unsupported mime type {valueMimeType}. Supported types are application/json and application/graphql");
        }
    }
}