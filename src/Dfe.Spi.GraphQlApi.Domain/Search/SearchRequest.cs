namespace Dfe.Spi.GraphQlApi.Domain.Search
{
    public class SearchRequest
    {
        public SearchGroup[] Groups { get; set; }
        public string CombinationOperator { get; set; }
    }
}