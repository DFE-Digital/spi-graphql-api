using System;

namespace Dfe.Spi.GraphQlApi.Domain.Common
{
    public class InvalidRequestException : Exception
    {
        public string ErrorIdentifier { get; }
        public string[] Details { get; }

        public InvalidRequestException(string message, string errorIdentifier, string[] details)
            : base(message)
        {
            ErrorIdentifier = errorIdentifier;
            Details = details;
        }
    }
}