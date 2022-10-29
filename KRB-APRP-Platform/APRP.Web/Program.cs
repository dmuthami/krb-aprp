using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Extensions;
using APRP.Web.Persistence.Contexts;
using APRP.Web.Persistence.Repositories;
using APRP.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ApplicationUser = APRP.Web.Domain.Models.ApplicationUser;
using APRP.Web.Extensions.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using APRP.Web.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using System.Data.SqlClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Framework Provided services to the container.
        #region Framework Provided Services

        var services = builder.Services;
        services.AddControllersWithViews();
        //builder.Services.AddIdentity<ApplicationUser, IdentityRole>();

        #region Database
        var whichDBMS = builder.Configuration.GetConnectionString("DBMS"); //Read Config File
        if (whichDBMS == "PSQL")
        {
            var psqlConnectionString = builder.Configuration.GetConnectionString("WebLegacyDatabase");
        services.AddDbContextPool<AppDbContext>(options =>
            options.UseNpgsql(psqlConnectionString, o => o.UseNetTopologySuite()));
        }else
        {
            var sqlConnectionString = builder.Configuration.GetConnectionString("AppConnectionString");
            SqlConnectionStringBuilder builderr = new SqlConnectionStringBuilder();
            builderr.ConnectionString = sqlConnectionString;
            services.AddDbContextPool<AppDbContext>(options =>
                    options.UseSqlServer(/*
                        sqlConnectionString
                        builder.ConnectionString
                        */builderr.ConnectionString,
                    x => x.UseNetTopologySuite()
                ));
        }


        #endregion

        #region ASP.NET Identity

        services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
        {
            //config.SignIn.RequireConfirmedEmail = true;
        })
        //.AddDefaultUI(UIFramework.Bootstrap4)
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
        #endregion

        #region File Upload & Download
        string path = Path.Combine(builder.Environment.WebRootPath, "uploads");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        services.AddSingleton<IFileProvider>(
        new PhysicalFileProvider(
            Path.Combine(builder.Environment.WebRootPath, "uploads")));
        #endregion

        #region Reguire Authenticated Users
        services.AddScoped<SessionTimeoutAttribute>();
        #endregion

        #region Cookie policy
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        #endregion

        #region Email
        // requires
        // using Microsoft.AspNetCore.Identity.UI.Services;
        services.AddSingleton<IEmailSender, EmailSender>();
        services.Configure<AuthMessageSenderOptions>(builder.Configuration);
        #endregion

        #region SMS
        services.AddTransient<ISmsSender, AuthMessageSender>();
        services.Configure<SMSoptions>(builder.Configuration);
        #endregion

        #region Razor pages
        services.AddRazorPages()
            .AddRazorPagesOptions(options =>
            {
                options.RootDirectory = "/MyPages";
            });
        #endregion

        #region Session Management
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        #endregion

        #region Localization
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        #endregion

        #region MVC
        services.AddMvc()
            .AddRazorRuntimeCompilation()
            .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddSessionStateTempDataProvider()
            .AddDataAnnotationsLocalization()
            //.AddRazorPagesOptions(options =>
            //{
            //    options.Conventions.AddPageRoute("/Home/Index", "");
            //})
            ;

        #endregion

        #region Cookie
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();
        #endregion

        #region Determine Current Culture
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US")
                        //,new CultureInfo("fr")
                        //,new CultureInfo("en-GB")
                        //,new CultureInfo("en")
                        //,new CultureInfo("fr-FR")
                        //,new CultureInfo("fr")
                    };

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
        #endregion

        #region HTTP
        services.AddHttpContextAccessor();
        #endregion

        #region Permission Based authorization
        //Register the handler in Startup.ConfigureServices:
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        //We will create a custom policy provider that will dynamically
        //create a policy with the appropriate requirement as it's needed during runtime.
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        #endregion

        #endregion

        // Add Custom services to the container.
        #region Custom Services
        // After configuring the database context, we also bind our service and repository to the respective classes.
        //we also use a scoped lifetime because these classes internally have to use the database context class

        services.AddScoped<IAdminOperationalActivityRepository, AdminOperationalActivityRepository>();
        services.AddScoped<IApplicationRolesRepository, ApplicationRolesRepository>();
        services.AddScoped<IApplicationUsersRepository, ApplicationUsersRepository>();
        services.AddScoped<IARICSRepository, ARICSRepository>();
        services.AddScoped<IARICSUploadRepository, ARICSUploadRepository>();
        services.AddScoped<IAuthenticateRepository, AuthenticateRepository>();
        services.AddScoped<IAuthorityRepository, AuthorityRepository>();
        services.AddScoped<IBudgetCeilingHeaderRepository, BudgetCeilingHeaderRepository>();
        services.AddScoped<IBudgetCeilingRepository, BudgetCeilingRepository>();
        services.AddScoped<ICommunicationRepository, CommunicationRepository>();
        services.AddScoped<IComplaintTypeRepository, ComplaintTypeRepository>();
        services.AddScoped<IComplaintRepository, ComplaintRepository>();
        services.AddScoped<IConstituencyRepository, ConstituencyRepository>();
        services.AddScoped<ICountyRepository, CountyRepository>();
        services.AddScoped<ICOSTESRespository, COSTESRepository>();
        services.AddScoped<ICountiesRoadRepository, CountiesRoadRepository>();
        services.AddScoped<IExecutionMethodRepository, ExecutionMethodRepository>();
        services.AddScoped<IFinancialYearRepository, FinancialYearRepository>();
        services.AddScoped<IFundingSourceRepository, FundingSourceRepository>();
        services.AddScoped<IFundTypeRepository, FundTypeRepository>();
        services.AddScoped<IGISRoadRepository, GISRoadRepository>();
        services.AddScoped<IIMRepository, IMRepository>();
        services.AddScoped<IItemActivityGroupRepository, ItemActivityGroupRepository>();
        services.AddScoped<IItemActivityUnitCostRepository, ItemActivityUnitCostRepository>();
        services.AddScoped<IKenHARoadRepository, KenHARoadRepository>();
        services.AddScoped<IKeRRARoadRepository, KeRRARoadRepository>();
        services.AddScoped<IKuRARoadRepository, kuRARoadRepository>();
        services.AddScoped<IKwSRoadRepository, kwSRoadRepository>();
        services.AddScoped<IManageRepository, ManageRepository>();
        services.AddScoped<IMessageOutRepository, MessageOutRepository>();
        services.AddScoped<IPlanActivityRepository, PlanActivityRepository>();
        services.AddScoped<IPlanActivityPBCRepository, PlanActivityPBCRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IRoadConditionRepository, RoadConditionRepository>();
        services.AddScoped<IRegisterRepository, RegisterRepository>();
        services.AddScoped<IRoadSectionRepository, RoadSectionRepository>();
        services.AddScoped<IRoadRepository, RoadRepository>();
        services.AddScoped<IRoadSheetRepository, RoadSheetRepository>();
        services.AddScoped<IRoadWorkBudgetHeaderRepository, RoadWorkBudgetHeaderRepository>();
        services.AddScoped<IRoadWorkBudgetLineRepository, RoadWorkBudgetLineRepository>();
        services.AddScoped<IRoadWorkOperationalActivitiesBudgetRepository, RoadWorkOperationalActivitiesBudgetRepository>();
        services.AddScoped<IRoadWorkSectionPlanRepository, RoadWorkSectionPlanRepository>();
        services.AddScoped<IShoulderInterventionPavedRepository, ShoulderInterventionPavedRepository>();
        services.AddScoped<IShoulderSurfaceTypePavedRepository, ShoulderSurfaceTypePavedRepository>();
        services.AddScoped<ISurfaceTypeRepository, SurfaceTypeRepository>();
        services.AddScoped<ISurfaceTypeUnPavedRepository, SurfaceTypeUnPavedRepository>();
        services.AddScoped<ITechnologyRepository, TechnologyRepository>();
        services.AddScoped<ITerrainTypeRepository, TerrainTypeRepository>();
        services.AddScoped<IUserAccessListRepository, UserAccessListRepository>();
        services.AddScoped<IWorkCategoryRepository, WorkCategoryRepository>();
        services.AddScoped<IWorkplanApprovalBatchRepository, WorkplanApprovalBatchRepository>();
        services.AddScoped<IWorkplanApprovalHistoryRepository, WorkplanApprovalHistoryRepository>();
        services.AddScoped<IWorkplanUploadRepository, WorkplanUploadRepository>();
        services.AddScoped<IWorkPlanPackageRepository, WorkPlanPackageRepository>();
        services.AddScoped<IWorkpackageRoadWorkSectionPlanRepository, WorkpackageRoadWorkSectionPlanRepository>();
        services.AddScoped<IContractorRepository, ContractorRepository>();
        services.AddScoped<IDirectorRepository, DirectorRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IPackageProgressEntryRepository, PackageProgressEntryRepository>();
        services.AddScoped<IFinancialProgressRepository, FinancialProgressRepository>();
        services.AddScoped<IPaymentCertificateRepository, PaymentCertificateRepository>();
        services.AddScoped<IPaymentCertificateActivitiesRepository, PaymentCertificateActivitiesRepository>();
        services.AddScoped<IEmploymentDetailRepository, EmploymentDetailRepository>();
        services.AddScoped<IRevenueCollectionRepository, RevenueCollectionRepository>();
        services.AddScoped<IRevenueCollectionCodeUnitRepository, RevenueCollectionCodeUnitRepository>();
        services.AddScoped<IRevenueCollectionCodeUnitTypeRepository, RevenueCollectionCodeUnitTypeRepository>();
        services.AddScoped<IDisbursementRepository, DisbursementRepository>();
        services.AddScoped<IAllocationRepository, AllocationRepository>();
        services.AddScoped<IAllocationCodeUnitRepository, AllocationCodeUnitRepository>();
        services.AddScoped<IWorkCategoryFundingMatrixRepository, WorkCategoryFundingMatrixRepository>();
        services.AddScoped<IOtherFundItemRepository, OtherFundItemRepository>();
        services.AddScoped<IQuarterCodeListRepository, QuarterCodeListRepository>();
        services.AddScoped<IQuarterCodeUnitRepository, QuarterCodeUnitRepository>();
        services.AddScoped<IMonthCodeRepository, MonthCodeRepository>();
        services.AddScoped<ITrainingRepository, TrainingRepository>();
        services.AddScoped<IRoadClassCodeUnitRepository, RoadClassCodeUnitRepository>();
        services.AddScoped<IServiceLevelItemRepository, ServiceLevelItemRepository>();
        services.AddScoped<IItemActivityPBCRepository, ItemActivityPBCRepository>();
        services.AddScoped<IRoadConditionCodeUnitRepository, RoadConditionCodeUnitRepository>();
        services.AddScoped<IWorkCategoryAllocationMatrixRepository, WorkCategoryAllocationMatrixRepository>();
        services.AddScoped<IUploadRepository, UploadRepository>();
        services.AddScoped<IRoadPrioritizationRepository, RoadPrioritizationRepository>();
        services.AddScoped<IRoadClassificationRepository, RoadClassificationRepository>();
        services.AddScoped<IRoadSheetLengthRepository, RoadSheetLengthRepository>();
        services.AddScoped<IRoadSheetIntervalRepository, RoadSheetIntervalRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ITrainingCourseRepository, TrainingCourseRepository>();
        services.AddScoped<IARICSYearRepository, ARICSYearRepository>();
        services.AddScoped<IAfricastingRepository, AfricastingRepository>();
        services.AddScoped<IItemActivityUnitCostRateRepository, ItemActivityUnitCostRateRepository>();
        services.AddScoped<ICostesRegionRepository, CostesRegionRepository>();
        services.AddScoped<IGravelRequiredRepository, GravelRequiredRepository>();
        services.AddScoped<IFundingSourceSubCodeRepository, FundingSourceSubCodeRepository>();
        services.AddScoped<IBudgetCeilingComputationRepository, BudgetCeilingComputationRepository>();
        services.AddScoped<IDisbursementTrancheRepository, DisbursementTrancheRepository>();
        services.AddScoped<IDisbursementCodeListRepository, DisbursementCodeListRepository>();
        services.AddScoped<ICSAllocationRepository, CSAllocationRepository>();
        services.AddScoped<IReleaseRepository, ReleaseRepository>();
        services.AddScoped<IDisbursementReleaseRepository, DisbursementReleaseRepository>();
        services.AddScoped<IARICSApprovalRepository, ARICSApprovalRepository>();
        services.AddScoped<IARICSApprovalLevelRepository, ARICSApprovalLevelRepository>();
        services.AddScoped<IARICSMasterApprovalRepository, ARICSMasterApprovalRepository>();
        services.AddScoped<IMunicipalityRepository, MunicipalityRepository>();
        services.AddScoped<IKWSParkRepository, KWSParkRepository>();
        services.AddScoped<IARICSBatchRepository, ARICSBatchRepository>();

        //The last step before testing our API is to bind the unit of work interface to its respective class.

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAdminOperationalActivityService, AdminOperationalActivityService>();
        services.AddScoped<IApplicationRolesService, ApplicationRolesService>();
        services.AddScoped<IApplicationUsersService, ApplicationUsersService>();
        services.AddScoped<IARICSService, ARICSService>();
        services.AddScoped<IARICSUploadService, ARICSUploadService>();
        services.AddScoped<IAuthenticateService, AuthenticateService>();
        services.AddScoped<IAuthorityService, AuthorityService>();
        services.AddScoped<IBudgetCeilingHeaderService, BudgetCeilingHeaderService>();
        services.AddScoped<IBudgetCeilingService, BudgetCeilingService>();
        services.AddScoped<ICommunicationService, CommunicationService>();
        services.AddScoped<IComplaintTypeService, ComplaintTypeService>();//new
        services.AddScoped<IComplaintService, ComplaintService>();
        services.AddScoped<IConstituencyService, ConstituencyService>();
        services.AddScoped<ICOSTESAPIService, COSTESAPIService>();
        services.AddScoped<ICountyService, CountyService>();
        services.AddScoped<ICountiesRoadService, CountiesRoadService>();
        services.AddScoped<IExecutionMethodService, ExecutionMethodService>();
        services.AddScoped<IFinancialYearService, FinancialYearService>();
        services.AddScoped<IFundingSourceService, FundingSourceService>();
        services.AddScoped<IFundTypeService, FundTypeService>();
        services.AddScoped<IGISRoadService, GISRoadService>();
        services.AddScoped<IIMService, IMService>();
        services.AddScoped<IItemActivityGroupService, ItemActivityGroupService>();
        services.AddScoped<IItemActivityUnitCostService, ItemActivityUnitCostService>();
        services.AddScoped<IKenHARoadService, KenHARoadService>();
        services.AddScoped<IKeRRARoadService, KeRRARoadService>();
        services.AddScoped<IKuRARoadService, KuRARoadService>();
        services.AddScoped<IKwSRoadService, KwSRoadService>();
        services.AddScoped<IManageService, ManageService>();
        services.AddScoped<IMessageOutService, MessageOutService>();
        services.AddScoped<IPlanActivityService, PlanActivityService>();
        services.AddScoped<IPlanActivityPBCService, PlanActivityPBCService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<IRoadConditionService, RoadConditionService>();
        services.AddScoped<IRoadSectionService, RoadSectionService>();
        services.AddScoped<IRoadService, RoadService>();
        services.AddScoped<IRoadSheetService, RoadSheetService>();
        services.AddScoped<IRoadWorkBudgetHeaderService, RoadWorkBudgetHeaderService>();
        services.AddScoped<IRoadWorkBudgetLineService, RoadWorkBudgetLineService>();
        services.AddScoped<IRoadWorkOperationalActivitiesBudgetService, RoadWorkOperationalActivitiesBudgetService>();
        services.AddScoped<IRoadWorkSectionPlanService, RoadWorkSectionPlanService>();
        services.AddScoped<IShoulderInterventionPavedService, ShoulderInterventionPavedService>();
        services.AddScoped<IShoulderSurfaceTypePavedService, ShoulderSurfaceTypePavedService>();
        services.AddScoped<ISurfaceTypeService, SurfaceTypeService>();
        services.AddScoped<ISurfaceTypeUnPavedService, SurfaceTypeUnPavedService>();
        services.AddScoped<ITechnologyService, TechnologyService>();
        services.AddScoped<ITerrainTypeService, TerrainTypeService>();
        services.AddScoped<IUserAccessListService, UserAccessListService>();
        services.AddScoped<IWorkCategoryService, WorkCategoryService>();
        services.AddScoped<IWorkplanApprovalBatchService, WorkplanApprovalBatchService>();
        services.AddScoped<IWorkplanApprovalHistoryService, WorkplanApprovalHistoryService>();
        services.AddScoped<IWorkplanUploadService, WorkplanUploadService>();
        services.AddScoped<IWorkPlanPackageService, WorkPlanPackageService>();
        services.AddScoped<IWorkpackageRoadWorkSectionPlanService, WorkpackageRoadWorkSectionPlanService>();
        services.AddScoped<IContractorService, ContractorService>();
        services.AddScoped<IDirectorService, DirectorService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IPackageProgressEntryService, PackageProgressEntryService>();
        services.AddScoped<IFinancialProgressService, FinancialProgressService>();
        services.AddScoped<IPaymentCertificateService, PaymentCertificateService>();
        services.AddScoped<IPaymentCertificateActivitiesService, PaymentCertificateActivitiesService>();
        services.AddScoped<IEmploymentDetailService, EmploymentDetailService>();
        services.AddScoped<IRevenueCollectionService, RevenueCollectionService>();
        services.AddScoped<IRevenueCollectionCodeUnitService, RevenueCollectionCodeUnitService>();
        services.AddScoped<IRevenueCollectionCodeUnitTypeService, RevenueCollectionCodeUnitTypeService>();
        services.AddScoped<IDisbursementService, DisbursementService>();
        services.AddScoped<IAllocationService, AllocationService>();
        services.AddScoped<IAllocationCodeUnitService, AllocationCodeUnitService>();
        services.AddScoped<IWorkCategoryFundingMatrixService, WorkCategoryFundingMatrixService>();
        services.AddScoped<IOtherFundItemService, OtherFundItemService>();
        services.AddScoped<IQuarterCodeListService, QuarterCodeListService>();
        services.AddScoped<IQuarterCodeUnitService, QuarterCodeUnitService>();
        services.AddScoped<IMonthCodeService, MonthCodeService>();
        services.AddScoped<ITrainingService, TrainingService>();
        services.AddScoped<IRoadClassCodeUnitService, RoadClassCodeUnitService>();
        services.AddScoped<IServiceLevelItemService, ServiceLevelItemService>();
        services.AddScoped<IItemActivityPBCService, ItemActivityPBCService>();
        services.AddScoped<IRoadConditionCodeUnitService, RoadConditionCodeUnitService>();
        services.AddScoped<IWorkCategoryAllocationMatrixService, WorkCategoryAllocationMatrixService>();
        services.AddScoped<IUploadService, UploadService>();
        services.AddScoped<IRoadPrioritizationService, RoadPrioritizationService>();
        services.AddScoped<IRoadClassificationService, RoadClassificationService>();
        services.AddScoped<IRoadSheetLengthService, RoadSheetLengthService>();
        services.AddScoped<IRoadSheetIntervalService, RoadSheetIntervalService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ITrainingCourseService, TrainingCourseService>();
        services.AddScoped<IARICSYearService, ARICSYearService>();
        services.AddScoped<IAfricastingService, AfricastingService>();
        services.AddScoped<IItemActivityUnitCostRateService, ItemActivityUnitCostRateService>();
        services.AddScoped<ICostesRegionService, CostesRegionService>();
        services.AddScoped<IGravelRequiredService, GravelRequiredService>();
        services.AddScoped<IFundingSourceSubCodeService, FundingSourceSubCodeService>();
        services.AddScoped<IBudgetCeilingComputationService, BudgetCeilingComputationService>();
        services.AddScoped<IDisbursementTrancheService, DisbursementTrancheService>();
        services.AddScoped<IDisbursementCodeListService, DisbursementCodeListService>();
        services.AddScoped<ICSAllocationService, CSAllocationService>();
        services.AddScoped<IReleaseService, ReleaseService>();
        services.AddScoped<IDisbursementReleaseService, DisbursementReleaseService>();
        services.AddScoped<IARICSApprovalService, ARICSApprovalService>();
        services.AddScoped<IARICSApprovalLevelService, ARICSApprovalLevelService>();
        services.AddScoped<IARICSMasterApprovalService, ARICSMasterApprovalService>();
        services.AddScoped<IMunicipalityService, MunicipalityService>();
        services.AddScoped<IKWSParkService, KWSParkService>();
        services.AddScoped<IARICSBatchService, ARICSBatchService>();

        #endregion

        //services.AddScoped<IDbInitializer, DbInitializer>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseAuthentication();
        app.UseAuthorization();

        // If the app uses session state, call Session Middleware after Cookie 
        // Policy Middleware and before MVC Middleware.
        app.UseSession();

        #region Logging
        //loggerFactory.AddSerilog();

        #endregion

        app.UseEndpoints(endpoints =>
        {

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapRazorPages();

        });

        #region Seed the roles
        //Seed the roles
        using (var scope = app.Services.CreateScope())
        {
            var srvice = scope.ServiceProvider;
            var context = srvice.GetRequiredService<AppDbContext>();

            try
            {
                SeedRoles.Initialize(srvice).Wait();
            }
            catch (Exception Ex)
            {
                //var logger = services.GetRequiredService<ILogger<Program>>();
                //logger.LogError(Ex, $"An error occurred while seeding the database. {Environment.NewLine}");
            }
        }
        #endregion

        app.Run();
    }
}


