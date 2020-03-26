namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public class SearchGroup
    {
        public SearchFilter[] Filter { get; set; }
        public string CombinationOperator { get; set; }
    }
}