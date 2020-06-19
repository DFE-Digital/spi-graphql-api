using System;
using Dfe.Spi.Common.Context.Definitions;

namespace Dfe.Spi.GraphQlApi.Domain.Context
{
    public interface IGraphExecutionContextManager : ISpiExecutionContextManager
    {
        GraphExecutionContext GraphExecutionContext { get; }
    }
}