using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class ReportsController : Controller
    {
        private readonly IWorkPlanPackageService _workPlanPackageService;
        IAuthorityService _authorityService;
        IContractService _contractService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IRegionService _regionService;
        private readonly IUserAccessListService _userAccessListService;
        private readonly IARICSYearService _aRICSYearService;

        public ReportsController(IWorkPlanPackageService workPlanPackageService, 
                                IAuthorityService authorityService,
                                IContractService contractService,
                                IApplicationUsersService applicationUsersService,
                                IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService,
                                IFinancialYearService financialYearService,
                                IRegionService regionService,
                                IUserAccessListService userAccessListService,
                                IARICSYearService aRICSYearService)
        {
            _workPlanPackageService = workPlanPackageService;
            _authorityService = authorityService;
            _contractService = contractService;
            _applicationUsersService = applicationUsersService;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
            _financialYearService = financialYearService;
            _regionService = regionService;
            _userAccessListService = userAccessListService;
            _aRICSYearService = aRICSYearService;
        }
        public async Task<IActionResult> Index()
        {
            ReportsViewModel reportsViewModel = new ReportsViewModel();
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                authority = userAuthority.Authority;
            }

            var fundingSources = await _revenueCollectionCodeUnitService.ListAsync(authority.ID).ConfigureAwait(false);
            var yearList = await _financialYearService.ListAsync().ConfigureAwait(false);
            var authorityList = await _authorityService.ListAsync().ConfigureAwait(false);

            var county = new Authority();
            county.ID = 0;
            county.Code = "Counties";
            county.Name = "Counties";
            county.Type = 10;

            var authList = authorityList.ToList();
            authList.Add(county);

            // authorityList.ToList().Add(allAuthority);

            ViewBag.FundingSourceList = new SelectList(fundingSources.RevenueCollectionCodeUnit, "ID", "RevenueStream");
            if(authority.Type == 0 || authority.Type == 3 || authority.Type == 4) //if KRB or Consultant, all authorities are fetched.
            {
                ViewBag.AuthorityList = new SelectList(authList.Where(f => f.Code != "KRB" && f.Code != "Csltnt" && (f.Type !=2 || f.Type != 4 || f.Type != 0  )).OrderBy(o => o.Type), "ID", "Name");
            }
            else
            {
                ViewBag.AuthorityList = new SelectList(authList.Where(f => f.Code == authority.Code).OrderBy(o => o.Type), "ID", "Name");
            }

            ViewBag.FinancialYearList = new SelectList(yearList, "ID", "Code", yearList.OrderByDescending(o=>o.IsCurrent));
           

            reportsViewModel.Authority = authority;
            //ViewData["AuthorityCode"] = authority.Code;
            //ViewData["AuthorityID"] = authority.Code;

            await PopulateDropDown().ConfigureAwait(false);

            return View(reportsViewModel);
        }

        public async Task<IActionResult> GetARWPForAgency(long authorityId,long financialYearId, long regionId, long countyId)
        {

            return RedirectToAction("GetARWPForAgency", "Workplan", new { authorityId = authorityId, financialYearId= financialYearId, regionId = regionId, countyId = countyId });

        }

        public async Task<IActionResult> GetAPRPForAgency(long authorityId)
        {

            return RedirectToAction("GetAPRPForAgency", "Workplan", new { authorityId = authorityId });

        }


        public async Task<IActionResult> DownloadBidDetails(long workPlanPackageId)

        {
            WorkPlanPackage workPlanPackage = new WorkPlanPackage();
            var workplanPackageResp = await _workPlanPackageService.FindByIdAsync(workPlanPackageId).ConfigureAwait(false);
            if (workplanPackageResp.Success)
                workPlanPackage = workplanPackageResp.WorkPlanPackage;

            return RedirectToAction("DownloadBidDetails", "WorkPlanPackage", new { workPlanPackage = workPlanPackage });
        }

        public async Task<IActionResult> DownloadBidSummary(long workPlanPackageId)
        {
            WorkPlanPackage workPlanPackage = new WorkPlanPackage();
            var workplanPackageResp = await _workPlanPackageService.FindByIdAsync(workPlanPackageId).ConfigureAwait(false);
            if (workplanPackageResp.Success)
                workPlanPackage = workplanPackageResp.WorkPlanPackage;

            return RedirectToAction("DownloadBidSummary", "WorkPlanPackage", new { workPlanPackage = workPlanPackage });
        }

        public async Task<IActionResult> GetRegions(long authorityId)
        {
            var regions = await _regionService.ListRegionsByAuthority(authorityId).ConfigureAwait(false);

            return Json(new
            {
                Success = true,
                Message = "Success",
                Data = regions
            });
        }

        public async Task<IActionResult> GetCounties()
        {
            var counties = await _authorityService.ListAsync().ConfigureAwait(false);
            var countyLists = counties.Where(c=>c.Type == 2).Select(c => new { c.ID, c.Name, c.Number }).OrderBy(c => c.Number).ToList();

            return Json(new
            {
                Success = true,
                Message = "Success",
                Data = countyLists
            });
        }

        public async Task<IActionResult> GetContracts(long authorityId, long financialYearId)
        {
            if(authorityId != 0)
            {
                var contracts = await _contractService.ListContractsByAgencyByFinancialYear(authorityId, financialYearId).ConfigureAwait(false);
                var contractList = contracts.Select(c => new { c.ID, c.WorkPlanPackage.Name }).OrderBy(c => c.Name).ToList();

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Data = contractList
                });
            }
            return Json(new { Success = false, Message = "Contracts for the selected authority could not be retrieved." });

        }

        public async Task<IActionResult> DownloadProgressReport(long contractId, long reportTypeId,long authorityId, long financialYearId)
        {
            switch (reportTypeId)
            {

                case 1:
                    {
                        // BOQ Summary reports
                        //get the workpackage using contract ID
                        var contractResp = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
                        if (!contractResp.Success)
                            return Json(new { Success = false, Message = "We are not able to retrieve the contract information." });

                        var workPackage = contractResp.Contract.WorkPlanPackage;

                        return RedirectToAction("DownloadBidSummary", "WorkPlanPackage", new { workplanPackageId = contractResp.Contract.WorkPlanPackage.ID });
                    }
                case 2:
                    {
                        // BOD Detail reports
                        //get the workpackage using contract ID
                        var contractResp = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
                        if (!contractResp.Success)
                            return Json(new { Success = false, Message = "We are not able to retrieve the contract information." });
                        return RedirectToAction("DownloadBidDetails", "WorkPlanPackage", new { workplanPackageId = contractResp.Contract.WorkPlanPackage.ID });
                    }
                case 3:
                    {
                        // Monthly Progress reports
                        return RedirectToAction("DownloadMonthlyProgressSummary", "WorkPlanPackage", new { contractId = contractId });
                    }
                case 4:
                    {
                        // Quarterly Progress reports
                        return RedirectToAction("DownloadQuarterlyProgressSummary", "WorkPlanPackage", new { authorityId = authorityId, financialYearId = financialYearId });
                    }
                case 5:
                    {
                        //Employment Summary Report
                        return RedirectToAction("DownloadQuarterlyEmploymentSummary", "WorkPlanPackage", new { contractId = contractId });
                    }
                case 6:
                    {
                        return RedirectToAction("DownloadAPRPImplementationReport", "Workplan", new { authorityId = authorityId, financialYearId = financialYearId });
                    }
                default:
                    {

                        return Json(new { Success = false, Message = "We are not able to generate the download file." });
                    }
                    
            }
        }

        #region Utilities
        private async Task<ApplicationUser> GetLoggedInUser()
        {
            var userResp = await _applicationUsersService.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
            ApplicationUser user = null;
            if (userResp.Success)
            {
                //user is found

                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    user = (ApplicationUser)result2.Value;

                    if (user != null)
                    {
                        var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        user.Authority = userAuthority.Authority;
                    }
                }
            }
            return user;
        }
        private async Task PopulateDropDown()
        {
            var authority = await _authorityService.ListAsync("CG").ConfigureAwait(false);
            ViewData["AuthorityId"] = new SelectList(authority, "ID", "Name");

            //Get logged in user
            ApplicationUser applicationUser = await GetLoggedInUser().ConfigureAwait(false);

            int AuthorityType = 0;
            bool result = int.TryParse(applicationUser.Authority.Type.ToString(), out AuthorityType);

            if (AuthorityType == 2)
            {
                ViewData["InstitutionGroupValue"] = 55;
            } else if (AuthorityType == 2)
            {
                ViewData["InstitutionGroupValue"] = 0;
            } else
            {
                ViewData["InstitutionGroupValue"] = (long)applicationUser.Authority.ID;
            }

            IList<ARICSYear> aRICSYears = null;
            //ARICS Year
            var aricsYearResp = await _aRICSYearService.ListAsync().ConfigureAwait(false);
            if (aricsYearResp.Success)
            {
                var objectResult = (ObjectResult)aricsYearResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        aRICSYears = (IList<ARICSYear>)result2.Value;
                    }
                }
            }
            ViewData["ARICSYearId"] = new SelectList(aRICSYears, "Year", "Year");
        }
        #endregion
    }
}
