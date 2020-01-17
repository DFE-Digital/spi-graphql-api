using System.Collections.Generic;
using System.Threading;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests
{
    public static class TestHelper
    {
        public static ResolveFieldContext<T> BuildResolveFieldContext<T>(
            Dictionary<string, object> arguments = null,
            string[] fields = null)
        {
            var context = new ResolveFieldContext<T>
            {
                CancellationToken = new CancellationToken(),
                Arguments =  arguments ?? new Dictionary<string, object>(),
                FieldAst = new Field
                {
                    SelectionSet = new SelectionSet(),
                },
            };

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    context.FieldAst.SelectionSet.Add(
                        new Field(null, new NameNode(field)));
                }
            }

            return context;
        }
    }
}