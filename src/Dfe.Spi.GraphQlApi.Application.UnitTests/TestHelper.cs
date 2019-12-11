using System.Collections.Generic;
using System.Threading;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests
{
    public static class TestHelper
    {
        public static ResolveFieldContext<T> BuildResolveFieldContext<T>(
            Dictionary<string, object> arguments = null)
        {
            return new ResolveFieldContext<T>
            {
                CancellationToken = new CancellationToken(),
                Arguments =  arguments ?? new Dictionary<string, object>(),
            };
        }
    }
}