using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ManagementGroupProvisionalFunding : ObjectGraphType<Models.RatesModels.ManagementGroupModels.ProvisionalFunding>
    {
        public ManagementGroupProvisionalFunding()
        {
            Field(x => x.ActualPrimaryUnitOfFunding, nullable: true)
                .Name("actualPrimaryUnitOfFunding")
                .Description("ActualPrimaryUnitOfFunding");
            
            Field(x => x.ActualSecondaryUnitOfFunding, nullable: true)
                .Name("actualSecondaryUnitOfFunding")
                .Description("ActualSecondaryUnitOfFunding");
            
            Field(x => x.PrimaryPupilNumbers, nullable: true)
                .Name("primaryPupilNumbers")
                .Description("PrimaryPupilNumbers");
            
            Field(x => x.SecondaryPupilNumbers, nullable: true)
                .Name("secondaryPupilNumbers")
                .Description("SecondaryPupilNumbers");
            
            Field(x => x.ActualFundingThroughPremesisMobilityGrowthFactors, nullable: true)
                .Name("actualFundingThroughPremesisMobilityGrowthFactors")
                .Description("ActualFundingThroughPremesisMobilityGrowthFactors");
            
            Field(x => x.ActualFundingThroughPremesisMobilityFactors, nullable: true)
                .Name("actualFundingThroughPremesisMobilityFactors")
                .Description("ActualFundingThroughPremesisMobilityFactors");
            
            Field(x => x.ActualFundingThroughPremesisFactors, nullable: true)
                .Name("actualFundingThroughPremesisFactors")
                .Description("ActualFundingThroughPremesisFactors");
            
            Field(x => x.NffSchoolsBlockFunding, nullable: true)
                .Name("nffSchoolsBlockFunding")
                .Description("NffSchoolsBlockFunding");
            
            Field(x => x.IllustrativeGrowthFunding, nullable: true)
                .Name("illustrativeGrowthFunding")
                .Description("IllustrativeGrowthFunding");
            
            Field(x => x.LocalAuthorityProtection, nullable: true)
                .Name("localAuthorityProtection")
                .Description("LocalAuthorityProtection");
            
            Field(x => x.NffSchoolsBlockFundingExcludingFundingThroughGrowthFactor, nullable: true)
                .Name("nffSchoolsBlockFundingExcludingFundingThroughGrowthFactor")
                .Description("NffSchoolsBlockFundingExcludingFundingThroughGrowthFactor");
            
            Field(x => x.ActualHighNeedsNffAllocations, nullable: true)
                .Name("actualHighNeedsNffAllocations")
                .Description("ActualHighNeedsNffAllocations");
            
            Field(x => x.ActualAcaWeightedBasicEntitlementFactorUnitRate, nullable: true)
                .Name("actualAcaWeightedBasicEntitlementFactorUnitRate")
                .Description("ActualAcaWeightedBasicEntitlementFactorUnitRate");
            
            Field(x => x.NumberOfPupilsInSpecialSchoolsAcadamies, nullable: true)
                .Name("numberOfPupilsInSpecialSchoolsAcadamies")
                .Description("NumberOfPupilsInSpecialSchoolsAcadamies");
            
            Field(x => x.AcaWeightedBasicEntitlementUnitRate, nullable: true)
                .Name("acaWeightedBasicEntitlementUnitRate")
                .Description("AcaWeightedBasicEntitlementUnitRate");
            
            Field(x => x.BasicEntitlementFactor, nullable: true)
                .Name("basicEntitlementFactor")
                .Description("BasicEntitlementFactor");
            
            Field(x => x.NumberOfPupilsInSpecialSchoolsAcadamiesIndependentSettings, nullable: true)
                .Name("numberOfPupilsInSpecialSchoolsAcadamiesIndependentSettings")
                .Description("NumberOfPupilsInSpecialSchoolsAcadamiesIndependentSettings");
            
            Field(x => x.ImportExportAdjustmentsIncludingAdjustmentsToNewAndGrowingSpecialFreeSchools, nullable: true)
                .Name("importExportAdjustmentsIncludingAdjustmentsToNewAndGrowingSpecialFreeSchools")
                .Description("ImportExportAdjustmentsIncludingAdjustmentsToNewAndGrowingSpecialFreeSchools");
            
            Field(x => x.HospitalEducationFundingWithEightPercentUplift, nullable: true)
                .Name("hospitalEducationFundingWithEightPercentUplift")
                .Description("HospitalEducationFundingWithEightPercentUplift");
            
            Field(x => x.ActualImportExportAdjustmentUnitRate, nullable: true)
                .Name("actualImportExportAdjustmentUnitRate")
                .Description("ActualImportExportAdjustmentUnitRate");
            
            Field(x => x.NetNumberOfImportedPupils, nullable: true)
                .Name("netNumberOfImportedPupils")
                .Description("NetNumberOfImportedPupils");
            
            Field(x => x.AdditionalFundingForNewAndGrowingSpecialFreeSchools, nullable: true)
                .Name("additionalFundingForNewAndGrowingSpecialFreeSchools")
                .Description("AdditionalFundingForNewAndGrowingSpecialFreeSchools");
            
            Field(x => x.HospitalEducationSpending, nullable: true)
                .Name("hospitalEducationSpending")
                .Description("HospitalEducationSpending");
            
            Field(x => x.NffHighNeedsBlockFunding, nullable: true)
                .Name("nffHighNeedsBlockFunding")
                .Description("NffHighNeedsBlockFunding");
            
            Field(x => x.ActualCssbUnitOfFunding, nullable: true)
                .Name("actualCssbUnitOfFunding")
                .Description("ActualCssbUnitOfFunding");
            
            Field(x => x.ActualCssbUnitOfFundingForOngoingFunctions, nullable: true)
                .Name("actualCssbUnitOfFundingForOngoingFunctions")
                .Description("ActualCssbUnitOfFundingForOngoingFunctions");
            
            Field(x => x.PupilNumbers, nullable: true)
                .Name("pupilNumbers")
                .Description("PupilNumbers");
            
            Field(x => x.PupilNumbersSchoolsBlockDsgDuplicatesApportioned, nullable: true)
                .Name("pupilNumbersSchoolsBlockDsgDuplicatesApportioned")
                .Description("PupilNumbersSchoolsBlockDsgDuplicatesApportioned");
            
            Field(x => x.ActualFundingForHistoricCommitments, nullable: true)
                .Name("actualFundingForHistoricCommitments")
                .Description("ActualFundingForHistoricCommitments");
            
            Field(x => x.NffCssbFunding, nullable: true)
                .Name("nffCssbFunding")
                .Description("NffCssbFunding");
            
            Field(x => x.NffAllocationsForSchoolsHighNeedsAndCentralSchoolServicesBlocks, nullable: true)
                .Name("nffAllocationsForSchoolsHighNeedsAndCentralSchoolServicesBlocks")
                .Description("NffAllocationsForSchoolsHighNeedsAndCentralSchoolServicesBlocks");
        }
    }
}