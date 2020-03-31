namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public class SearchResultSet
    {
        public SearchResult[] Results { get; set; }
        public int Skipped { get; set; }
        public int Taken { get; set; }
        public int TotalNumberOfRecords { get; set; }
    }
}