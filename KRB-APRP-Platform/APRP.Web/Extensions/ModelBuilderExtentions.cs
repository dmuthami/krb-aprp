using APRP.Web.Extensions.SeedData;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions
{
    public static class ModelBuilderExtentions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            //Seed ARICS Year Data
            ARICSYearData.SeedARICSYear(modelBuilder);

            //Seed FundingSource
            //FundingSourceData.SeedFundingSource(modelBuilder);

            //Seed DisbursementTranche
            //FundingSourceSubCodeData.SeedFundingSourceSubCode(modelBuilder);

            //Seed FundType
            FundTypeData.SeedFundType(modelBuilder);

            //Seed Financial Year
            FinancialYearData.SeedFinancialYear(modelBuilder);

            //Seed Surface Type
            SurfaceTypeData.SeedSurfaceType(modelBuilder);
            //Surface Type Unpaved
            SurfaceTypeUnPavedData.SeedSurfaceTypeUnPaved(modelBuilder);

            //Seed Shoulder Surface Type Paved
            ShoulderSurfaceTypePavedData.SeedShoulderSurfaceTypePaved(modelBuilder);

            //Seed Shoulder Intervention  Paved
            ShoulderInterventionPavedData.SeedShoulderInterventionPaved(modelBuilder);

            //Seed Regions
            //RegionData.SeedRegion(modelBuilder);

            //Seed TerrainType
            TerrainTypeData.SeedTerrainType(modelBuilder);

            //Work Category Data
            WorkCategoryData.SeedWorkCategory(modelBuilder);

            //Seed ExecutionMethod
            ExecutionMethodData.SeedExecutionMethod(modelBuilder);

            //Seed ItemActivityGroup
            //ItemActivityGroupData.SeedItemActivityGroup(modelBuilder);

            //Seed Technology
            TechnologyData.SeedTechnology(modelBuilder);

            //Seed  QuarterCodeListData
            QuarterCodeListData.SeedQurterCodeList(modelBuilder);

            //Seed Authority Data
            //AuthorityData.SeedAuthority(modelBuilder);

            //Seed UserAccess List
            //SeedUserAccessListData.SeedUserAccessList(modelBuilder);

            //ComplaintType
            ComplaintTypeData.SeedComplaintType(modelBuilder);

            //Counties
            //CountiesData.SeedCounties(modelBuilder);

            //RoadClassCodeUnit
            RoadClassCodeUnitData.SeedRoadClassCodeUnit(modelBuilder);

            //RoadConditionCodeUnit
            RoadConditionCodeUnitData.SeedRoadConditionCodeUnit(modelBuilder);

            ItemActivityPBCData.SeedItemActivityPBCData(modelBuilder);

            RevenueCollectionCodeUnitTypeData.SeedRevenueCollectionCodeUnitType(modelBuilder);

            // Road Prioritization
            RoadPrioritizationData.SeedRoadPrioritization(modelBuilder);

            //Seed Roadsheet Interval
            RoadSheetIntervalData.SeedRoadSheetInterval(modelBuilder);

            //Seed Roadsheet Length
            RoadSheetLengthData.SeedRoadSheetLength(modelBuilder);

            //Seed Shoulder Required Data
            GravelRequiredData.SeedGravelRequired(modelBuilder);

            //Seed Costes Region Data
            //CostesRegionData.SeedCostesRegion(modelBuilder);

            //Seed Disbursement Tranche Data
            DisbursementTrancheData.SeedDisbursementTranche(modelBuilder);

            //Seed DisbursementCodeList Data
            DisbursementCodeListData.SeedDisbursementCodeList(modelBuilder);

            //Seed DisbursementCodeList Data
            //KWSParkData.SeedKWSPark(modelBuilder);

            //Month code list data
            MonthCodeData.SeedMonthCode(modelBuilder);

            //ARICS Approval Level data
            ARICSApprovalLevelData.SeedARICSApprovalLevel(modelBuilder);

        }

    }
}
