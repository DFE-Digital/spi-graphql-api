using System;
using System.Net;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi
{
    public class RegistryApiException : Exception
    {
        public string Resource { get; }
        public HttpStatusCode Status { get; }
        public string Details { get; }

        public RegistryApiException(string resource, HttpStatusCode status, string details)
            : base(GetDefaultMessage(resource, status, details))
        {
            Resource = resource;
            Status = status;
            Details = details;
        }

        private static string GetDefaultMessage(string resource, HttpStatusCode status, string details)
        {
            return $"Error calling Registry API at {resource}. {(int)status} - {details}";
        }
    }
}