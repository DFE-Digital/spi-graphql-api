using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class PaginationDetailsModel
    {
        public int Skipped { get; set; }
        public int Taken { get; set; }
        public int TotalNumberOfRecords { get; set; }
    }
    
    public class PaginationDetails : ObjectGraphType<PaginationDetailsModel>
    {
        public PaginationDetails()
        {
            Field(x => x.Skipped)
                .Name("skipped")
                .Description("Skipped");
            
            Field(x => x.Taken)
                .Name("taken")
                .Description("Taken");
            
            Field(x => x.TotalNumberOfRecords)
                .Name("totalNumberOfRecords")
                .Description("TotalNumberOfRecords");
        }
    }
}