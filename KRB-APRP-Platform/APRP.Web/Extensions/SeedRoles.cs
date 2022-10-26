using APRP.Web.Claims;
using APRP.Web.Domain.Models;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APRP.Web.Extensions
{
    public class SeedRoles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {

                var roleManager = serviceProvider.GetService<RoleManager<ApplicationRole>>();

                string[] roles = new string[]
                {
                  "Approver.Approver1","Approver.Approver2","Approver.Approver3","Approver.Approver4","Approver.Approver5","Approver.Approver6","Approver.Approver7",
                  "Administrators"
                };

                foreach (string role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                    {
                        ApplicationRole applicationRole = new ApplicationRole();
                        applicationRole.Name = role;
                        if (role == "Administrators")
                        {
                            applicationRole.RoleType = 1;
                        }
                        var IR = await roleManager.CreateAsync(applicationRole).ConfigureAwait(false);
                    }
                }


                //Add claims to Administrators Role
                await AddAllClaimsToAdminitratorRole(serviceProvider, "Administrators").ConfigureAwait(false);


                //Create Super Admin
                try
                {
                    var adminID = await EnsureUser(serviceProvider, "P@ssw0rd$%", "krb.web.rms@gmail.com").ConfigureAwait(false);

                    //Add Super Admin to Administrators Role
                    try
                    {
                        var result = await EnsureRole(serviceProvider, adminID, "Administrators", "krb.web.rms@gmail.com").ConfigureAwait(false);
                    }
                    catch (Exception Ex)
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<SeedRoles>>();
                        logger.LogError(Ex, $"An error occurred while creating the Super User. {Environment.NewLine}");
                    }

                    //super Admin 2
                    adminID = await EnsureUser(serviceProvider, "P@ssw0rd$%", "krb.web.rms@tutanota.com").ConfigureAwait(false);
                    try
                    {
                        var result = await EnsureRole(serviceProvider, adminID, "Administrators", "krb.web.rms@tutanota.com").ConfigureAwait(false);
                    }
                    catch (Exception Ex)
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<SeedRoles>>();
                        logger.LogError(Ex, $"An error occurred while creating the Super User. {Environment.NewLine}");
                    }

                }
                catch (Exception Ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<SeedRoles>>();
                    logger.LogError(Ex, $"An error occurred while creating the Super User. {Environment.NewLine}");
                }
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            string myid = null; ;
            var user = await userManager.FindByNameAsync(UserName).ConfigureAwait(false);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = UserName,
                    Email = UserName,
                    TwoFactorEnabled = true,
                    PhoneNumber = "+254737550099",//Todo: Hard Coded the Phone Number for the Super User
                    AuthorityId = 8L//Todo: Hard Coded the Authority ID for KRB
                };
                IdentityResult result;
                result = await userManager.CreateAsync(user, testUserPw).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    myid = user.Id;
                }
                else
                {
                    myid = "Error";
                }
            }
            return myid;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                              string uid, string role, string UserName)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<ApplicationRole>>();

            if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
            {
                ApplicationRole applicationRole = new ApplicationRole();
                applicationRole.Name = role;
                IR = await roleManager.CreateAsync(applicationRole).ConfigureAwait(false);
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            if (uid != "Error" && uid != null)
            {
                var user = await userManager.FindByIdAsync(uid).ConfigureAwait(false);

                IR = await userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            }
            else
            {
                //Try searching by username

                var user = await userManager.FindByNameAsync(UserName).ConfigureAwait(false);
                if (user != null)
                {
                    IR = await userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                }
            }

            return IR;
        }

        private static async Task AddClaim(IServiceProvider serviceProvider, string RoleName, string Permission)
        {
            var roleManager = serviceProvider.GetService<RoleManager<ApplicationRole>>();
            var adminRole = await roleManager.FindByNameAsync(RoleName).ConfigureAwait(false);

            var claimsList = await roleManager.GetClaimsAsync(adminRole).ConfigureAwait(false);
            var claim = claimsList.Where(s => s.Value == Permission).FirstOrDefault();
            if (claim == null)
            {
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, Permission)).ConfigureAwait(false);
            }
        }

        private static async Task AddAllClaimsToAdminitratorRole(IServiceProvider serviceProvider, string RoleName)
        {
            await AddClaim(serviceProvider, RoleName, Permission.KENHA.GIS_View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KENHA.GIS_Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KENHA.GIS_Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KENHA.GIS_Change).ConfigureAwait(false);

            await AddClaim(serviceProvider, RoleName, Permission.KERRA.GIS_View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KERRA.GIS_Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KERRA.GIS_Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KERRA.GIS_Change).ConfigureAwait(false);

            await AddClaim(serviceProvider, RoleName, Permission.KURA.GIS_View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KURA.GIS_Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KURA.GIS_Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KURA.GIS_Change).ConfigureAwait(false);


            await AddClaim(serviceProvider, RoleName, Permission.KWS.GIS_View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KWS.GIS_Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KWS.GIS_Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.KWS.GIS_Change).ConfigureAwait(false);

            await AddClaim(serviceProvider, RoleName, Permission.Counties.GIS_View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Counties.GIS_Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Counties.GIS_Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Counties.GIS_Change).ConfigureAwait(false);

            await AddClaim(serviceProvider, RoleName, Permission.Administrator.Role_View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Administrator.Role_Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Administrator.Role_Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Administrator.Role_Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Administrator.ActivateOrDeactivate).ConfigureAwait(false);
            //GIS
            await AddClaim(serviceProvider, RoleName, Permission.GIS.View).ConfigureAwait(false);

            //ARICS
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.ConductARICS).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.PreparerOrSubmitter).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.FirstReviewer).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.SecondReviewer).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.InternalApprover).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.Approver).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.ARICSYear_View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.ARICSYear_Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.ARICSYear_Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ARICS.ARICSYear_Delete).ConfigureAwait(false);

            //Finance
            await AddClaim(serviceProvider, RoleName, Permission.Finance.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Finance.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Finance.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Finance.Change).ConfigureAwait(false);

            //FinancialYear 
            await AddClaim(serviceProvider, RoleName, Permission.FinancialYear.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FinancialYear.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FinancialYear.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FinancialYear.Change).ConfigureAwait(false);

            //Allocation 
            await AddClaim(serviceProvider, RoleName, Permission.Allocation.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Allocation.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Allocation.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Allocation.Change).ConfigureAwait(false);

            //AllocationCodeUnit 
            await AddClaim(serviceProvider, RoleName, Permission.AllocationCodeUnit.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.AllocationCodeUnit.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.AllocationCodeUnit.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.AllocationCodeUnit.Change).ConfigureAwait(false);

            //Disbursement 
            await AddClaim(serviceProvider, RoleName, Permission.Disbursement.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Disbursement.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Disbursement.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Disbursement.Change).ConfigureAwait(false);

            //Release 
            await AddClaim(serviceProvider, RoleName, Permission.Release.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Release.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Release.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Release.Change).ConfigureAwait(false);

            //CSAllocation 
            await AddClaim(serviceProvider, RoleName, Permission.CSAllocation.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.CSAllocation.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.CSAllocation.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.CSAllocation.Change).ConfigureAwait(false);

            //RevenueCollection 
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollection.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollection.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollection.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollection.Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollection.SubmitBudget).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollection.ApproveBudget).ConfigureAwait(false);

            //RevenueCollectionCodeUnit 
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnit.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnit.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnit.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnit.Change).ConfigureAwait(false);

            //RevenueCollectionCodeUnitType 
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnitType.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnitType.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnitType.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RevenueCollectionCodeUnitType.Change).ConfigureAwait(false);

            //WorkCategoryFundingMatrix 
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryFundingMatrix.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryFundingMatrix.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryFundingMatrix.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryFundingMatrix.Change).ConfigureAwait(false);

            //WorkCategoryAllocationMatrix 
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryAllocationMatrix.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryAllocationMatrix.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryAllocationMatrix.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkCategoryAllocationMatrix.Change).ConfigureAwait(false);

            //Other fund Items 
            await AddClaim(serviceProvider, RoleName, Permission.OtherFundItem.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.OtherFundItem.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.OtherFundItem.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.OtherFundItem.Change).ConfigureAwait(false);

            //Training 
            await AddClaim(serviceProvider, RoleName, Permission.Training.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Training.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Training.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Training.Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Training.Report).ConfigureAwait(false);

            //TrainingCourse 
            await AddClaim(serviceProvider, RoleName, Permission.TrainingCourse.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.TrainingCourse.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.TrainingCourse.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.TrainingCourse.Change).ConfigureAwait(false);

            //QuarterCodeList 
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeList.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeList.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeList.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeList.Change).ConfigureAwait(false);

            //QuarterCodeUnit 
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeUnit.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeUnit.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeUnit.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.QuarterCodeUnit.Change).ConfigureAwait(false);

            //RoadClassification 
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassification.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassification.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassification.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassification.Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassification.Submit).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassification.Approve).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassification.AddRoadtoGIS).ConfigureAwait(false);

            //Fund Type 
            await AddClaim(serviceProvider, RoleName, Permission.FundType.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FundType.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FundType.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FundType.Change).ConfigureAwait(false);

            //FundingSource
            await AddClaim(serviceProvider, RoleName, Permission.FundingSource.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FundingSource.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FundingSource.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.FundingSource.Change).ConfigureAwait(false);

            //ActivityGroup
            await AddClaim(serviceProvider, RoleName, Permission.ActivityGroup.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ActivityGroup.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ActivityGroup.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ActivityGroup.Change).ConfigureAwait(false);

            //ActivityItem
            await AddClaim(serviceProvider, RoleName, Permission.ActivityItem.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ActivityItem.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ActivityItem.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.ActivityItem.Change).ConfigureAwait(false);

            //Authority
            await AddClaim(serviceProvider, RoleName, Permission.Authority.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Authority.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Authority.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Authority.Change).ConfigureAwait(false);

            //Contractor
            await AddClaim(serviceProvider, RoleName, Permission.Contractor.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Contractor.Save).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Contractor.Delete).ConfigureAwait(false);

            //RoadClassCodeUnit
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassCodeUnit.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassCodeUnit.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassCodeUnit.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadClassCodeUnit.Change).ConfigureAwait(false);

            //RoadConditionCodeUnit
            await AddClaim(serviceProvider, RoleName, Permission.RoadConditionCodeUnit.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadConditionCodeUnit.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadConditionCodeUnit.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadConditionCodeUnit.Change).ConfigureAwait(false);

            //RoadPrioritization
            await AddClaim(serviceProvider, RoleName, Permission.RoadPrioritization.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadPrioritization.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadPrioritization.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadPrioritization.Change).ConfigureAwait(false);

            //BudgetCeiling
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.Upload).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.Download).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.DeleteLetter).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeiling.Report).ConfigureAwait(false);

            //BudgetCeilingComputation
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeilingComputation.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeilingComputation.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeilingComputation.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeilingComputation.Change).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeilingComputation.Upload).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeilingComputation.Download).ConfigureAwait(false);           
            await AddClaim(serviceProvider, RoleName, Permission.BudgetCeilingComputation.Report).ConfigureAwait(false);

            //workplan roles
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Upload).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Approver1).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Approver2).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Approver3).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Approver4).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Approver5).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Approver6).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Edit).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.WorkplanPermissions.Delete).ConfigureAwait(false);

            //Costes
            await AddClaim(serviceProvider, RoleName, Permission.COSTES.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.COSTES.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.COSTES.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.COSTES.Change).ConfigureAwait(false);

            //Road
            await AddClaim(serviceProvider, RoleName, Permission.Road.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Road.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Road.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.Road.Change).ConfigureAwait(false);

            //Road Section
            await AddClaim(serviceProvider, RoleName, Permission.RoadSection.View).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadSection.Add).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadSection.Delete).ConfigureAwait(false);
            await AddClaim(serviceProvider, RoleName, Permission.RoadSection.Change).ConfigureAwait(false);
        }
    }
}
