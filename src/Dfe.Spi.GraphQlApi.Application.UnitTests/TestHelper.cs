using System;
using System.Collections.Generic;
using System.Threading;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests
{
    public static class TestHelper
    {
        private static readonly Random Rng = new Random();

        public static int RandomInt(int min = 0, int max = int.MaxValue)
        {
            return Rng.Next(min, max);
        }
        
        public static ResolveFieldContext<T> BuildResolveFieldContext<T>(
            Dictionary<string, object> arguments = null,
            string[] fields = null,
            T source = default)
        {
            var context = new ResolveFieldContext<T>
            {
                CancellationToken = new CancellationToken(),
                Arguments =  arguments ?? new Dictionary<string, object>(),
                Source = source,
                FieldAst = new Field
                {
                    SelectionSet = new SelectionSet(),
                },
                Errors = new ExecutionErrors(),
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