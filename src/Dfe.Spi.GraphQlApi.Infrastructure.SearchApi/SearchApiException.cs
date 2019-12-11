using System;
using System.Net;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SearchApi
{
    public class SearchApiException : Exception
    {
        public string Resource { get; }
        public HttpStatusCode Status { get; }
        public string Details { get; }

        public SearchApiException(string resource, HttpStatusCode status, string details)
            : base(GetDefaultMessage(resource, status, details))
        {
            Resource = resource;
            Status = status;
            Details = details;
        }

        private static string GetDefaultMessage(string resource, HttpStatusCode status, string details)
        {
            return $"Error calling Search API at {resource}. {(int)status} - {details}";
        }
    }
}