namespace Dfe.Spi.GraphQlApi.Domain.Search
{
    public class SearchFilter
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
    }
}