using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums
{
    public class OperatorEnum : EnumerationGraphType<Common.Models.DataOperator>
    {
        public OperatorEnum()
        {
            Name = "Operator";
            Description = "Operator to apply to condition";
        }
    }
}