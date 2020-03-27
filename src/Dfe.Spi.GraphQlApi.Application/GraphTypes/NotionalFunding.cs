using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class NotionalFunding : ObjectGraphType<Models.RatesModels.LearningProviderModels.NotionalFunding>
    {
        public NotionalFunding()
        {
            Field(x => x.TotalNffFunding, nullable: true)
                .Name("totalNffFunding")
                .Description("Total NFF funding");
            
            Field(x => x.PercentageChangeComparedToBaseline, nullable: true)
                .Name("percentageChangeComparedToBaseline")
                .Description("Percentage change compared to baseline");
            
            Field(x => x.PercentageChangeInPupilLedFunding, nullable: true)
                .Name("percentageChangeInPupilLedFunding")
                .Description("Percentage change in pupil led funding");
            
            Field(x => x.PupilCount, nullable: true)
                .Name("pupilCount")
                .Description("Pupil count");
            
            Field(x => x.TotalNffFundingPerPupil, nullable: true)
                .Name("totalNffFundingPerPupil")
                .Description("Total NFF funding per pupil");
            
            Field(x => x.PercentageChangeInPupilLedFundingPerPupil, nullable: true)
                .Name("percentageChangeInPupilLedFundingPerPupil")
                .Description("Percentage change in pupil led funding per pupil");
        }
    }
}