using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class IllustrativeFunding : ObjectGraphType<Models.RatesModels.IllustrativeFunding>
    {
        public IllustrativeFunding()
        {
            Field(x => x.TotalNffFunding, nullable: true)
                .Name("totalNffFunding")
                .Description("Total NFF funding");
            
            Field(x => x.PercentageChangeComparedToBaseline, nullable: true)
                .Name("percentageChangeComparedToBaseline")
                .Description("Percentage change compared to baseline");
            
            Field(x => x.NewAndGrowingSchoolsTotalNffFundingIfFullyImplemented, nullable: true)
                .Name("newAndGrowingSchoolsTotalNffFundingIfFullyImplemented")
                .Description("New and growing schools total NFF funding if fully implemented");
            
            Field(x => x.TotalNffFundingIfFullyImplemented, nullable: true)
                .Name("totalNffFundingIfFullyImplemented")
                .Description("Total NFF funding uf fully implemented");
            
            Field(x => x.PercentageChangeInPupilLedFunding, nullable: true)
                .Name("percentageChangeInPupilLedFunding")
                .Description("Percentage change in pupil led funding");
            
            Field(x => x.PercentageChangeComparedToBaselineIfFullyImplemented, nullable: true)
                .Name("percentageChangeComparedToBaselineIfFullyImplemented")
                .Description("Percentage change compared to baseline if fully implemented");
            
            Field(x => x.PercentageChangeInPupilLedFundingIfFullyImplemented, nullable: true)
                .Name("percentageChangeInPupilLedFundingIfFullyImplemented")
                .Description("Percentage change in pupil led funding if fully implemented");
            
            Field(x => x.TotalNffFundingIfFullyImplementedPerPupil, nullable: true)
                .Name("totalNffFundingIfFullyImplementedPerPupil")
                .Description("Total NFF funding if fully implemented per pupil");
            
            Field(x => x.PercentageChangeInPupilLedFundingPerPupil, nullable: true)
                .Name("percentageChangeInPupilLedFundingPerPupil")
                .Description("Percentage change in pupil led funding per pupil");
        }
    }
}