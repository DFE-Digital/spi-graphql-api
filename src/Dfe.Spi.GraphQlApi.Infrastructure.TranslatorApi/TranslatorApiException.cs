using System;
using System.Net;

namespace Dfe.Spi.GraphQlApi.Infrastructure.TranslatorApi
{
    public class TranslatorApiException : Exception
    {
        public TranslatorApiException(string resource, HttpStatusCode statusCode, string details)
            : base($"Error calling {resource} on translator. Status {(int)statusCode} - {details}")
        {
            StatusCode = statusCode;
            Details = details;
        }

        public HttpStatusCode StatusCode { get; }
        public string Details { get; }
    }
}