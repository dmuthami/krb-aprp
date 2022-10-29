using APRP.Web.Domain.Models;
using APRP.Web.Domain.Models.Audit;
using APRP.Web.Domain.Models.History;
using APRP.Web.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Policy;

namespace APRP.Web.Persistence.Contexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<ARICS> ARICSS { get; set; }
        public DbSet<ARICSUpload> ARICSUploads { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<Constituency> Constituencies { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<ExecutionMethod> ExecutionMethods { get; set; }
        public DbSet<FinancialYear> FinancialYears { get; set; }
        public DbSet<FundingSource> FundingSources { get; set; }
        public DbSet<FundType> FundTypes { get; set; }
        public DbSet<GISRoad> GISRoads { get; set; }
        public DbSet<ItemActivityGroup> ItemActivityGroups { get; set; }
        public DbSet<ItemActivityUnitCost> ItemActivityUnitCosts { get; set; }
        public DbSet<ItemActivityPBC> ItemActivityPBCs { get; set; }
        public DbSet<ServiceLevelGroup> ServiceLevelGroups { get; set; }
        public DbSet<ServiceLevelItem> ServiceLevelItems { get; set; }
        public DbSet<PlanActivityPBC> PlanActivityPBCs { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Road> Roads { get; set; }
        public DbSet<RoadSection> RoadSections { get; set; }
        public DbSet<RoadSheet> RoadSheets { get; set; }
        public DbSet<RoadWorkBudgetHeader> RoadWorkBudgetHeaders { get; set; }
        public DbSet<RoadWorkBudgetLine> RoadWorkBudgetLines { get; set; }
        public DbSet<RoadWorkOperationalActivitiesBudget> RoadWorkOperationalActivitiesBudgets { get; set; }
        public DbSet<RoadWorkSectionPlan> RoadWorkSectionPlans { get; set; }
        public DbSet<ShoulderInterventionPaved> ShoulderInterventionPaveds { get; set; }
        public DbSet<ShoulderSurfaceTypePaved> ShoulderSurfaceTypePaveds { get; set; }
        public DbSet<SurfaceType> SurfaceTypes { get; set; }
        public DbSet<SurfaceTypeUnPaved> SurfaceTypeUnPaveds { get; set; }
        public DbSet<TerrainType> TerrainTypes { get; set; }
        public DbSet<WorkCategory> WorkCategories { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<MessageOut> MessageOuts { get; set; }
        public DbSet<Authority> Authorities { get; set; }
        public DbSet<PlanActivity> PlanActivities { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<BudgetCeilingHeader> BudgetCeilingHeaders { get; set; }
        public DbSet<BudgetCeiling> BudgetCeilings { get; set; }
        public DbSet<ComplaintType> ComplaintTypes { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<WorkplanApprovalBatch> WorkplanApprovalBatches { get; set; }

        public DbSet<WorkplanApprovalHistory> WorkplanApprovalHistories { get; set; }

        public DbSet<WorkplanUpload> WorkplanUploads { get; set; }

        public DbSet<UserAccessList> UserAccessLists { get; set; }

        public DbSet<RoadCondition> RoadConditions { get; set; }

        public DbSet<WorkPlanPackage> WorkPlanPackages { get; set; }

        public DbSet<WorkpackageRoadWorkSectionPlan> WorkpackageRoadWorkSectionPlans { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<Director> Directors { get; set; }

        public DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<KenhaRoad> KenhaRoads { get; set; }
        public virtual DbSet<KerraRoad> KerraRoads { get; set; }
        public virtual DbSet<CountiesRoad> CountiesRoads { get; set; }
        public virtual DbSet<KuraRoad> KuraRoads { get; set; }
        public virtual DbSet<KwsRoad> KwsRoads { get; set; }

        public DbSet<PackageProgressEntry> PackageProgressEntries { get; set; }
        public DbSet<FinancialProgress> FinancialProgress { get; set; }
        public DbSet<OtherFundItem> OtherFundItems { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<AllocationCodeUnit> AllocationCodeUnits { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        public DbSet<RevenueCollection> RevenueCollections { get; set; }
        public DbSet<RevenueCollectionCodeUnit> RevenueCollectionCodeUnits { get; set; }

        public DbSet<RevenueCollectionCodeUnitType> RevenueCollectionCodeUnitTypes { get; set; }
        public DbSet<WorkCategoryFundingMatrix> WorkCategoryFundingMatrixs { get; set; }
        public DbSet<PaymentCertificate> PaymentCertificates { get; set; }
        public DbSet<PaymentCertificateActivity> PaymentCertificateActivities { get; set; }

        public DbSet<EmploymentDetail> EmploymentDetails { get; set; }
        public DbSet<QuarterCodeList> QuarterCodeLists { get; set; }
        public DbSet<QuarterCodeUnit> QuarterCodeUnits { get; set; }
        public DbSet<MonthCode> MonthCodes { get; set; }
        public DbSet<Training> Trainings { get; set; }

        public DbSet<RoadClassCodeUnit> RoadClassCodeUnits { get; set; }
        public DbSet<RoadClassification> RoadClassifications { get; set; }
        public DbSet<DesignationCodeUnit> DesignationCodeUnits { get; set; }
        public DbSet<RoadConditionCodeUnit> RoadConditionCodeUnits { get; set; }
        public DbSet<RoadConditionCodeUnith> RoadConditionCodeUniths { get; set; }
        public DbSet<RoadConditionCodeUnitAudit> RoadConditionCodeUnitAudits { get; set; }
        public DbSet<WorkCategoryAllocationMatrix> WorkCategoryAllocationMatrixs { get; set; }
        public DbSet<RoadPrioritization> RoadPrioritizations { get; set; }
        public DbSet<RoadSheetLength> RoadSheetLengths { get; set; }
        public DbSet<RoadSheetInterval> RoadSheetIntervals { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TrainingCourse> TrainingCourses { get; set; }
        public DbSet<ItemActivityUnitCostRate> ItemActivityUnitCostRates { get; set; }
        public DbSet<ARICSYear> ARICSYears { get; set; }
        public DbSet<CostesRegion> CostesRegions { get; set; }
        public DbSet<GravelRequired> GravelRequireds { get; set; }

        public DbSet<FundingSourceSubCode> FundingSourceSubCodes { get; set; }
        public DbSet<AdminOperationalActivity> AdminOperationalActivities { get; set; }

        public DbSet<BudgetCeilingComputation> BudgetCeilingComputations { get; set; }

        public DbSet<DisbursementTranche> DisbursementTranches { get; set; }

        public DbSet<DisbursementCodeList> DisbursementCodeLists { get; set; }

        public virtual DbSet<CSAllocation> CSAllocations { get; set; }

        public virtual DbSet<Release> Releases { get; set; }

        public virtual DbSet<DisbursementRelease> DisbursementReleases { get; set; }
        public virtual DbSet<ARICSApproval> ARICSApprovals { get; set; }
        public virtual DbSet<ARICSApprovalLevel> ARICSApprovalLevels { get; set; }
        public virtual DbSet<ARICSApprovalh> ARICSApprovalhs { get; set; }

        public virtual DbSet<ARICSMasterApproval> ARICSMasterApprovals { get; set; }
        public virtual DbSet<KWSPark> KWSParks { get; set; }
        public virtual DbSet<Municipality> Municipalitys { get; set; }

        public virtual DbSet<ARICSBatch> ARICSBatchs { get; set; }
        //seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ARICS>().ToTable("ARICS");
            modelBuilder.Entity<ARICSUpload>().ToTable("ARICSUpload");
            modelBuilder.Entity<Upload>().ToTable("Upload");
            modelBuilder.Entity<Constituency>().ToTable("Constituency");
            modelBuilder.Entity<County>().ToTable("County");
            modelBuilder.Entity<ExecutionMethod>().ToTable("ExecutionMethod");


            modelBuilder.Entity<FinancialYear>().ToTable("FinancialYear");
            modelBuilder.Entity<FundingSource>().ToTable("FundingSource");
            modelBuilder.Entity<FundType>().ToTable("FundType");
            modelBuilder.Entity<GISRoad>().ToTable("GISRoad");
            modelBuilder.Entity<ItemActivityGroup>().ToTable("ItemActivityGroup");

            modelBuilder.Entity<ItemActivityUnitCost>().ToTable("ItemActivityUnitCost");
            modelBuilder.Entity<Region>().ToTable("Region");
            modelBuilder.Entity<Road>().ToTable("Road");
            modelBuilder.Entity<RoadSection>().ToTable("RoadSection");
            modelBuilder.Entity<RoadSheet>().ToTable("RoadSheet");


            modelBuilder.Entity<RoadWorkBudgetHeader>().ToTable("RoadWorkBudgetHeader");
            modelBuilder.Entity<RoadWorkBudgetLine>().ToTable("RoadWorkBudgetLine");
            modelBuilder.Entity<RoadWorkOperationalActivitiesBudget>().ToTable("RoadWorkOperationalActivitiesBudget");
            modelBuilder.Entity<RoadWorkSectionPlan>()
                .ToTable("RoadWorkSectionPlan")
                .HasIndex(p => new { p.RoadSectionId })
                .IsUnique(false);
            modelBuilder.Entity<ShoulderInterventionPaved>().ToTable("ShoulderInterventionPaved");

            modelBuilder.Entity<ShoulderSurfaceTypePaved>().ToTable("ShoulderSurfaceTypePaved");
            modelBuilder.Entity<SurfaceType>().ToTable("SurfaceType");
            modelBuilder.Entity<SurfaceTypeUnPaved>().ToTable("SurfaceTypeUnPaved");
            modelBuilder.Entity<TerrainType>().ToTable("TerrainType");
            modelBuilder.Entity<WorkCategory>().ToTable("WorkCategory");

            modelBuilder.Entity<ApplicationRole>().ToTable("ApplicationRole");
            modelBuilder.Entity<MessageOut>().ToTable("MessageOut");
            modelBuilder.Entity<Authority>().ToTable("Authority");
            modelBuilder.Entity<PlanActivity>().ToTable("PlanActivity");
            modelBuilder.Entity<Technology>().ToTable("Technology");

            modelBuilder.Entity<BudgetCeilingHeader>().ToTable("BudgetCeilingHeader");
            modelBuilder.Entity<BudgetCeiling>().ToTable("BudgetCeiling");
            modelBuilder.Entity<ComplaintType>().ToTable("ComplaintType");
            modelBuilder.Entity<Complaint>().ToTable("Complaint");

            modelBuilder.Entity<UserAccessList>().ToTable("UserAccessList");
            modelBuilder.Entity<RoadCondition>().ToTable("RoadCondition");
            modelBuilder.Entity<WorkPlanPackage>().ToTable("WorkPlanPackage");
            modelBuilder.Entity<WorkpackageRoadWorkSectionPlan>().ToTable("WorkpackageRoadWorkSectionPlan");
            modelBuilder.Entity<Contractor>().ToTable("Contractor");

            modelBuilder.Entity<Director>().ToTable("Director");
            modelBuilder.Entity<Contract>().ToTable("Contract");
            modelBuilder.Entity<KenhaRoad>().ToTable("KenhaRoad");
            modelBuilder.Entity<KerraRoad>().ToTable("KerraRoad");
            modelBuilder.Entity<CountiesRoad>().ToTable("CountiesRoad");

            modelBuilder.Entity<KuraRoad>().ToTable("KuraRoad");
            modelBuilder.Entity<KwsRoad>().ToTable("KwsRoad");

            modelBuilder.Entity<OtherFundItem>().ToTable("OtherFundItem");
            modelBuilder.Entity<Allocation>().ToTable("Allocation");
            modelBuilder.Entity<AllocationCodeUnit>().ToTable("AllocationCodeUnit");
            modelBuilder.Entity<Disbursement>().ToTable("Disbursement");
            modelBuilder.Entity<RevenueCollection>().ToTable("RevenueCollection");
            modelBuilder.Entity<RevenueCollectionCodeUnit>().ToTable("RevenueCollectionCodeUnit");
            modelBuilder.Entity<RevenueCollectionCodeUnitType>().ToTable("RevenueCollectionCodeUnitType");
            modelBuilder.Entity<WorkCategoryFundingMatrix>().ToTable("WorkCategoryFundingMatrix");
            modelBuilder.Entity<QuarterCodeList>().ToTable("QuarterCodeList");
            modelBuilder.Entity<QuarterCodeUnit>().ToTable("QuarterCodeUnit");
            modelBuilder.Entity<Training>().ToTable("Training");

            modelBuilder.Entity<RoadClassCodeUnit>().ToTable("RoadClassCodeUnit");
            modelBuilder.Entity<RoadClassification>().ToTable("RoadClassification");
            modelBuilder.Entity<DesignationCodeUnit>().ToTable("DesignationCodeUnit");
            modelBuilder.Entity<RoadConditionCodeUnit>().ToTable("RoadConditionCodeUnit");
            modelBuilder.Entity<RoadConditionCodeUnith>().ToTable("RoadConditionCodeUnith");
            modelBuilder.Entity<RoadConditionCodeUnitAudit>().ToTable("RoadConditionCodeUnitAudit");
            modelBuilder.Entity<WorkCategoryAllocationMatrix>().ToTable("WorkCategoryAllocationMatrix");
            modelBuilder.Entity<RoadPrioritization>().ToTable("RoadPrioritization");
            modelBuilder.Entity<RoadSheetLength>().ToTable("RoadSheetLength");
            modelBuilder.Entity<RoadSheetInterval>().ToTable("RoadSheetInterval");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<TrainingCourse>().ToTable("TrainingCourse");
            modelBuilder.Entity<ItemActivityUnitCostRate>().ToTable("ItemActivityUnitCostRate");
            modelBuilder.Entity<ARICSYear>().ToTable("ARICSYear");
            modelBuilder.Entity<CostesRegion>().ToTable("CostesRegion");
            modelBuilder.Entity<GravelRequired>().ToTable("GravelRequired");
            modelBuilder.Entity<FundingSourceSubCode>().ToTable("FundingSourceSubCode");
            modelBuilder.Entity<BudgetCeilingComputation>().ToTable("BudgetCeilingComputation");
            modelBuilder.Entity<DisbursementTranche>().ToTable("DisbursementTranche");
            modelBuilder.Entity<DisbursementCodeList>().ToTable("DisbursementCodeList");
            modelBuilder.Entity<CSAllocation>().ToTable("CSAllocation");
            modelBuilder.Entity<Release>().ToTable("Releases");
            modelBuilder.Entity<DisbursementRelease>().ToTable("DisbursementRelease");
            modelBuilder.Entity<ARICSApproval>().ToTable("ARICSApproval");
            modelBuilder.Entity<ARICSApprovalLevel>().ToTable("ARICSApprovalLevel");
            modelBuilder.Entity<ARICSApprovalh>().ToTable("ARICSApprovalh");
            modelBuilder.Entity<ARICSMasterApproval>().ToTable("ARICSMasterApproval");
            modelBuilder.Entity<KWSPark>().ToTable("KWSPark");
            modelBuilder.Entity<Municipality>().ToTable("Municipality");
            modelBuilder.Entity<ARICSBatch>().ToTable("ARICSBatch");

            //one to many relationship between Road and RoadSections
            modelBuilder.Entity<RoadSection>()
            .HasOne<Road>(s => s.Road)
            .WithMany(g => g.RoadSections)
            .HasForeignKey(s => s.RoadId);

            //one to many relationship between RoadSections and RoadSheet
            modelBuilder.Entity<RoadSheet>()
            .HasOne<RoadSection>(s => s.RoadSection)
            .WithMany(g => g.RoadSheets)
            .HasForeignKey(s => s.RoadSectionId);

            //one to many relationship between RoadSheet and ARICS
            modelBuilder.Entity<ARICS>()
            .HasOne<RoadSheet>(s => s.RoadSheet)
            .WithMany(g => g.ARICSS)
            .HasForeignKey(s => s.RoadSheetId);

            //one to many relationship between Terrain Type and Roadsheet
            modelBuilder.Entity<RoadSheet>()
            .HasOne<TerrainType>(s => s.TerrainType)
            .WithMany(g => g.RoadSheets)
            .HasForeignKey(s => s.TerrainTypeId);

            //one to many relationship between Shoulder Surface Type Paved and ARICS
            modelBuilder.Entity<ARICS>()
            .HasOne<ShoulderSurfaceTypePaved>(s => s.ShoulderSurfaceTypePaved)
            .WithMany(g => g.ARICSZ)
            .HasForeignKey(s => s.ShoulderSurfaceTypePavedId);

            //one to many relationship between Shoulder Surface Type Paved and ARICS
            modelBuilder.Entity<ARICS>()
            .HasOne<ShoulderSurfaceTypePaved>(s => s.ShoulderSurfaceTypePaved)
            .WithMany(g => g.ARICSZ)
            .HasForeignKey(s => s.ShoulderSurfaceTypePavedId);

            //one to many relationship between Surface Type UnPaved and ARICS
            modelBuilder.Entity<ARICS>()
            .HasOne<SurfaceTypeUnPaved>(s => s.SurfaceTypeUnPaved)
            .WithMany(g => g.ARICSZ)
            .HasForeignKey(s => s.SurfaceTypeUnPavedId);

            //one to many relationship between Surface Type UnPaved and ARICS
            modelBuilder.Entity<ARICS>()
            .HasOne<GravelRequired>(s => s.GravelRequired)
            .WithMany(g => g.ARICSZ)
            .HasForeignKey(s => s.GravelRequiredId);

            //one to many relationship between Counties  and Constituencies
            modelBuilder.Entity<Constituency>()
            .HasOne<County>(s => s.County)
            .WithMany(g => g.Constituencys)
            .HasForeignKey(s => s.CountyId);

            ////one to many relationship between RoadSections and ARICS Uploads
            modelBuilder.Entity<ARICSUpload>()
            .HasOne<RoadSection>(s => s.RoadSection)
            .WithMany(g => g.ARICSUploads)
            .HasForeignKey(s => s.RoadSectionId);

            ////one to many relationship between Constituencies and RoadSections
            modelBuilder.Entity<RoadSection>()
            .HasOne<Constituency>(s => s.Constituency)
            .WithMany(g => g.RoadSections)
            .HasForeignKey(s => s.ConstituencyId);

            //many to many relationship between Disbursement and releases
            modelBuilder.Entity<DisbursementRelease>().HasKey(sc => new { sc.DisbursementId, sc.ReleaseId });

            //One to zero or one relationship between ARICSYear and Financial Year
            modelBuilder.Entity<ARICSYear>()
                        .HasOne(b => b.FinancialYear)
                        .WithOne(i => i.ARICSYear)
                        .HasForeignKey<FinancialYear>(b => b.ARICSYearId);

            //one to many relationship between SurfaceType and RoadClassifications
            //modelBuilder.Entity<SurfaceType>()
            //.HasMany<RoadClassification>(g => g.RoadClassifications)
            //.WithOne(s => s.SurfaceType)
            //.HasForeignKey(s => s.SurfaceTypeId)
            //.OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Seed();

        }
        //seed data
        public DbSet<APRP.Web.Domain.Models.RoadPrioritization> RoadPrioritization { get; set; }

    }
}
