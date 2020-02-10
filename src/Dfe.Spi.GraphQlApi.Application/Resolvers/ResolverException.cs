using System;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public class ResolverException : Exception
    {
        public ResolverException(string message)
            : base(message)
        {
        }
    }
}