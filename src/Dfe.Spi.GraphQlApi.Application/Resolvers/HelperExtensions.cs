using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public static class HelperExtensions
    {
        internal static DateTime? GetPointInTimeArgument<T>(this ResolveFieldContext<T> context)
        {
            return context.Arguments.GetPointInTimeArgument();
        }

        internal static DateTime? GetPointInTimeArgument(this Arguments arguments)
        {
            var argumentsDictionary = arguments.ToDictionary(
                a => a.Name,
                a => a.Value.Value);
            return argumentsDictionary.GetPointInTimeArgument();
        }

        internal static DateTime? GetPointInTimeArgument(this Dictionary<string, object> arguments)
        {
            const string argumentName = "pointInTime";
            
            if (!arguments.ContainsKey(argumentName))
            {
                return null;
            }

            var value = arguments[argumentName];
            if (value is DateTime || value is DateTime?)
            {
                return value as DateTime?;
            }

            return DateTime.Parse(value.ToString());
        }

        internal static Field GetParentOf(this Document document, Field child)
        {
            foreach (var operation in document.Operations)
            {
                var parent = GetParentOf(child, operation);
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }
        
        private static Field GetParentOf(Field child, Operation operation)
        {
            foreach (Field childField in operation.SelectionSet.Children)
            {
                var childFromTree = GetParentOf(child, childField);
                if (childField != null)
                {
                    return childField;
                }
            }

            return null;
        }
        private static Field GetParentOf(Field child, Field parentField)
        {
            foreach (Field childField in parentField.SelectionSet.Children)
            {
                if (childField.SourceLocation == child.SourceLocation)
                {
                    return parentField;
                }

                var childFromTree = GetParentOf(child, childField);
                if (childFromTree != null)
                {
                    return childFromTree;
                }
            }
            return null;
        }
    }
}