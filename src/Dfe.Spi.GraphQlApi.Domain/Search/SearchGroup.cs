namespace Dfe.Spi.GraphQlApi.Domain.Search
{
    public class SearchGroup
    {
        public SearchFilter[] Filter { get; set; }
        public string CombinationOperator { get; set; }
    }
}