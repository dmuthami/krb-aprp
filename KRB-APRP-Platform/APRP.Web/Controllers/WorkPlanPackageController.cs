using System.Data;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class WorkPlanPackageController : Controller
    {
        private readonly IWorkPlanPackageService _workPlanPackageService;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IRoadWorkSectionPlanService _roadWorkSectionPlanService;
        private readonly IWorkpackageRoadWorkSectionPlanService _workpackageRoadWorkSectionPlanService;
        private readonly IContractService _contractService;
        private readonly IEmploymentDetailService _employmentDetailService;
        private readonly IContractorService _contractorService;
        private readonly IPlanActivityService _planActivityService;
        private readonly IPackageProgressEntryService _packageProgressEntryService;
        private readonly IPaymentCertificateService _paymentCertificateService;
        private readonly IPaymentCertificateActivitiesService _paymentCertificateActivitiesService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IFinancialProgressService _financialProgressService;
        private readonly IMonthCodeService _monthCodeService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly IMemoryCache _cache;

        public WorkPlanPackageController(IWorkPlanPackageService workPlanPackageService,
                                        IAuthorityService authorityService,
                                        IApplicationUsersService applicationUsersService,
                                        IFinancialYearService financialYearService,
                                        IRoadWorkSectionPlanService roadWorkSectionPlanService,
                                        IWorkpackageRoadWorkSectionPlanService workpackageRoadWorkSectionPlanService,
                                        IContractService contractService,
                                        IEmploymentDetailService employmentDetailService,
                                        IContractorService contractorService,
                                        IPlanActivityService planActivityService,
                                        IPackageProgressEntryService packageProgressEntryService,
                                        IPaymentCertificateService paymentCertificateService,
                                        IPaymentCertificateActivitiesService paymentCertificateActivitiesService,
                                        IFinancialProgressService financialProgressService,
                                        IMonthCodeService monthCodeService,
                                        IWebHostEnvironment hostingEnvironment,
                                        IMemoryCache cache)
        {
            _workPlanPackageService = workPlanPackageService;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
            _financialYearService = financialYearService;
            _roadWorkSectionPlanService = roadWorkSectionPlanService;
            _workpackageRoadWorkSectionPlanService = workpackageRoadWorkSectionPlanService;
            _contractService = contractService;
            _employmentDetailService = employmentDetailService;
            _contractorService = contractorService;
            _planActivityService = planActivityService;
            _packageProgressEntryService = packageProgressEntryService;
            _paymentCertificateService = paymentCertificateService;
            _paymentCertificateActivitiesService = paymentCertificateActivitiesService;
            _financialProgressService = financialProgressService;
            _monthCodeService = monthCodeService;
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        #region User Access
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
        private async Task<bool> IsApproved(ApplicationUser applicationUser)
        {
            bool _isApproved = false;

            var appUserResp = await _applicationUsersService.GetRolesAsync(applicationUser).ConfigureAwait(false);

            if (appUserResp.Success)
            {
                var objectResult = (ObjectResult)appUserResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    IList<string> _MyRoles = (IList<string>)result2.Value;

                    if ( _MyRoles.Contains("Administrators"))
                    {
                        _isApproved = true;
                    }
                }
            }

            //Check if in Administrator role
            return _isApproved;
        }
        #endregion

        public async Task<IActionResult> RoadWorkPackaging(long ID)
        {
            //fetch the selected work package
            var viewModel = new WorkPlanPackageViewModel();

            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

           

            var user = await GetLoggedInUser().ConfigureAwait(false);

            if (user != null)
            {
                
                //return packages for the specific authority
                viewModel.WorkPlanPackages = await _workPlanPackageService.ListByFinancialYearAsync(fYearResp.Success ? fYearResp.FinancialYear.ID : 0, user.AuthorityId).ConfigureAwait(false);

            }

            if (ID > 0)
            {
                //get workplans for the finanncial year
                var workPlanPackageResp = await _workPlanPackageService.FindByIdAsync(ID).ConfigureAwait(false);
                if (workPlanPackageResp.Success)
                {
                    viewModel.WorkPlanPackage = workPlanPackageResp.WorkPlanPackage;
                }
                else
                {
                    viewModel.WorkPlanPackage = new WorkPlanPackage();
                }
            }
            else
            {
                viewModel.WorkPlanPackage = new WorkPlanPackage();
            }
            //Set Return URL and store in session
            HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
            ViewBag.Authority = user.Authority;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveWorkplanPackage(WorkPlanPackage workPlanPackage)
        {
            if (workPlanPackage != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var authoriy = new Authority();
                var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

                if (fYearResp.Success)
                {
                    workPlanPackage.FinancialYearId = fYearResp.FinancialYear.ID;
                }


                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    ViewBag.Authority = userAuthority.Authority;
                    authoriy = userAuthority.Authority;

                }
                
                //save the package
                if (workPlanPackage.ID > 0)
                {
                    //check uniquenuess of package name
                    var existingPackackageResp = await _workPlanPackageService.FindByCodeAsync(workPlanPackage.Code).ConfigureAwait(false);

                    if (existingPackackageResp.Success)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "A package with the same package number already exists!"
                        });
                    }
                    //edit the work package
                    var existingRecordResp = await _workPlanPackageService.FindByIdAsync(workPlanPackage.ID).ConfigureAwait(false);
                    if (existingRecordResp.Success)
                    {
                        existingRecordResp.WorkPlanPackage.Name = workPlanPackage.Name;
                        existingRecordResp.WorkPlanPackage.Code = workPlanPackage.Code;
                        existingRecordResp.WorkPlanPackage.Contigencies = workPlanPackage.Contigencies;
                        existingRecordResp.WorkPlanPackage.EngineerEstimate = workPlanPackage.EngineerEstimate;
                        existingRecordResp.WorkPlanPackage.Status = workPlanPackage.Status;
                        existingRecordResp.WorkPlanPackage.UpdateBy = user.UserName;
                        existingRecordResp.WorkPlanPackage.UpdateDate = DateTime.UtcNow;
                        existingRecordResp.WorkPlanPackage.VariationAmount = workPlanPackage.VariationAmount;
                        existingRecordResp.WorkPlanPackage.VariationPercentage = workPlanPackage.VariationPercentage;


                        var updateResp = await _workPlanPackageService.UpdateAsync(existingRecordResp.WorkPlanPackage).ConfigureAwait(false);
                        if (updateResp.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success",
                                Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workPlanPackage.ID })
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Update on work package failed"
                            });
                        }
                    }

                }
                else
                {
                    //check uniquenuess of package name
                    var existingPackackageResp = await _workPlanPackageService.FindByCodeAsync(workPlanPackage.Code).ConfigureAwait(false);

                    if (existingPackackageResp.Success)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "A package with the same package number already exists!"
                        });
                    }
                    //new work package
                    workPlanPackage.CreatedBy = user.UserName;
                    workPlanPackage.AuthorityId = authoriy.ID;
                    var addResp = await _workPlanPackageService.AddAsync(workPlanPackage).ConfigureAwait(false);
                    if (addResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = addResp.WorkPlanPackage.ID })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Adding work package failed"
                        });
                    }
                }

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workPlanPackage.ID })
                });

            }
            return Json(new
            {
                Success = false,
                Message = "Invalid Package details"
            });
        }

        public async Task<IActionResult> EditPackage(long ID)
        {

            var viewModel = await _workPlanPackageService.FindByIdAsync(ID).ConfigureAwait(false);
            return PartialView("WorkPackageEdit", viewModel.WorkPlanPackage);
        }

        [HttpPost]
        public async Task<IActionResult> EditPackage(WorkPlanPackage workPlanPackage)
        {
            if (workPlanPackage != null)
            {
                if (workPlanPackage.ID > 0)
                {
                    //check uniquenuess of package name
                    var existingPackackageResp = await _workPlanPackageService.FindByCodeAsync(workPlanPackage.Code).ConfigureAwait(false);

                    if (existingPackackageResp.Success)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "A package with the same package number already exists!"
                        });
                    }
                    //retrieve the package and update
                    var existingRecord = await _workPlanPackageService.FindByIdAsync(workPlanPackage.ID).ConfigureAwait(false);
                    if (existingRecord.Success)
                    {
                        var user = await GetLoggedInUser().ConfigureAwait(false);
                        existingRecord.WorkPlanPackage.Code = workPlanPackage.Code;
                        existingRecord.WorkPlanPackage.Name = workPlanPackage.Name;
                        existingRecord.WorkPlanPackage.UpdateBy = user.UserName;
                        existingRecord.WorkPlanPackage.UpdateDate = DateTime.UtcNow;
                        existingRecord.WorkPlanPackage.VariationAmount = workPlanPackage.VariationAmount;
                        existingRecord.WorkPlanPackage.VariationPercentage = workPlanPackage.VariationPercentage;

                        //ssave
                        var updateRecord = await _workPlanPackageService.UpdateAsync(existingRecord.WorkPlanPackage).ConfigureAwait(false);
                        if (updateRecord.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success",
                                Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workPlanPackage.ID })
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Update on work package failed"
                            });
                        }
                    }
                }
            }
            return PartialView("WorkPackageEdit", workPlanPackage);
        }

        public async Task<IActionResult> EditPackageQauntity(long planActivityId, long workpackageId)
        {
            var viewModel = new PackageQuantityEditViewModel();
            if (planActivityId > 0)
            {
                var sectionPlanActivityResponse = await _planActivityService.FindByIdAsync(planActivityId).ConfigureAwait(false);
                if (sectionPlanActivityResponse.Success)
                    viewModel.PlanActivity = sectionPlanActivityResponse.PlanActivity;
            }
            else
            {
                viewModel.PlanActivity = new PlanActivity();
            }
            viewModel.workPackageId = workpackageId;

            return PartialView("WorkPackageQuantityEdit", viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> EditPackageQuantity(PackageQuantityEditViewModel packageQuantityEditViewModel)
        {
            if (packageQuantityEditViewModel != null)
            {
                if (packageQuantityEditViewModel.PlanActivity != null)
                {
                    if (packageQuantityEditViewModel.PlanActivity.ID > 0 && packageQuantityEditViewModel.workPackageId > 0)
                    {
                        var user = await GetLoggedInUser().ConfigureAwait(false);

                        //get the workpackage first
                        var existingPackageSuccess = await _workPlanPackageService.FindByIdAsync(packageQuantityEditViewModel.workPackageId).ConfigureAwait(false);
                        if (existingPackageSuccess.Success)
                        {
                            //retrieve the plan activity and update

                            var existingRecord = await _planActivityService.FindByIdAsync(packageQuantityEditViewModel.PlanActivity.ID).ConfigureAwait(false);
                            if (existingRecord.Success)
                            {
                                //get the old quantity to affect the engineer's estimate
                                var oldPackageQuantity = existingRecord.PlanActivity.PackageQuantity;

                                existingRecord.PlanActivity.PackageQuantity = packageQuantityEditViewModel.PlanActivity.PackageQuantity;
                                existingRecord.PlanActivity.PackageAmount = (double) (packageQuantityEditViewModel.PlanActivity.PackageQuantity * (decimal)(existingRecord.PlanActivity.Rate));
                                existingRecord.PlanActivity.UpdateBy = user.UserName;
                                existingRecord.PlanActivity.UpdateDate = DateTime.UtcNow;


                                //save
                                var updateRecord = await _planActivityService.UpdateAsync(existingRecord.PlanActivity).ConfigureAwait(false);
                                if (updateRecord.Success)
                                {
                                    //reset the engineer's total in the package quantity

                                    var oldValue = Math.Ceiling(( (decimal) (existingRecord.PlanActivity.Rate) * oldPackageQuantity) * (decimal)(100 + existingPackageSuccess.WorkPlanPackage.VAT)) / 100;
                                    var newValue = Math.Ceiling(( existingRecord.PlanActivity.PackageQuantity * (decimal)(existingRecord.PlanActivity.Rate) * (decimal)(100 + existingPackageSuccess.WorkPlanPackage.VAT))) / 100;

                                    existingPackageSuccess.WorkPlanPackage.EngineerEstimate = (existingPackageSuccess.WorkPlanPackage.EngineerEstimate - (double) oldValue + (double)newValue); //new value

                                    //update the package
                                    await _workPlanPackageService.UpdateAsync(existingPackageSuccess.WorkPlanPackage).ConfigureAwait(false);
                                    return Json(new
                                    {
                                        Success = true,
                                        Message = "Success"//,
                                                           //Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workPlanPackage.ID })
                                    });
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Message = "Update on work package failed"
                                    });
                                }
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Invalid package supplied"
                            });
                        }

                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid activity supplied"
                        });
                    }
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid activity supplied"
                });
            }

            return PartialView("WorkPackageQuantityEdit", packageQuantityEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CopyPlannedQuantity(long workpackageId)
        {

            if (workpackageId > 0)
            {
                //retrieve all the work package activities and update the package quantity with the planned quantity value
                var user = await GetLoggedInUser().ConfigureAwait(false);

                var bulkUpdateResp = await _planActivityService.UpdateBulkPackageQuantityByPackageIdAsync(workpackageId, user).ConfigureAwait(false);
                if (bulkUpdateResp.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = bulkUpdateResp.Message
                    });
                }


            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid work package supplied"
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> RemoveWorkPlan(long sectionPlanId, long workpackageId)
        {
            if (sectionPlanId > 0 && workpackageId > 0)
            {
                //remove the work section plan record
                var existingRecord = await _workpackageRoadWorkSectionPlanService.FindBySectionPlanIdAndWorkPackageId(sectionPlanId, workpackageId).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    var roadWorkSectionPlanId = existingRecord.WorkpackageRoadWorkSectionPlan.RoadWorkSectionPlanId;
                    //remove the record.
                    var deleteResp = await _workpackageRoadWorkSectionPlanService.RemoveAsync(existingRecord.WorkpackageRoadWorkSectionPlan.ID).ConfigureAwait(false);
                    if (deleteResp.Success)
                    {
                        //reset the plan activity quantities to restore to unset position for the package quantity
                        var resetResp = await _planActivityService.ResetPlanQuantitiesAsync(workpackageId, roadWorkSectionPlanId).ConfigureAwait(false);
                        if (!resetResp.Success)
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Unable to reset the quantities, please check with the administrator"
                            });
                        }



                        return Json(new
                        {
                            Success = true,
                            Message = "Success"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Unable to remove the record, please check with the administrator"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Record not found, please check with administrator"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid parameters have been supplied"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveWorkPackage(long workpackageId)
        {
            if (workpackageId > 0)
            {
                //remove the work section plan record
                //remove the package.
                var deleteResp = await _workPlanPackageService.RemoveAsync(workpackageId).ConfigureAwait(false);
                if (deleteResp.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = 0 })
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Unable to remove the record, please check with the administrator"
                    });
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid parameters have been supplied"
                });
            }
        }

        //get workplans to add into work package
        public async Task<IActionResult> GetRoadWorkplans()
        {

            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authoriy = new Authority();
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                ViewBag.Authority = userAuthority.Authority;
                authoriy = userAuthority.Authority;

            }
            var sectionPlanIdList = new List<long>();
            var workpackages = await _workPlanPackageService.ListByFinancialYearAsync(fYearResp.FinancialYear.ID, authoriy.ID).ConfigureAwait(false);

            foreach (var package in workpackages)
            {
                if (package.WorkpackageRoadWorkSectionPlans != null)
                {
                    foreach (var plan in package.WorkpackageRoadWorkSectionPlans)
                    {
                        sectionPlanIdList.Add(plan.RoadWorkSectionPlanId);
                    }
                }
            }



            var agencyWorkPlans = await _roadWorkSectionPlanService.ListByAgencyAsync(authoriy.ID, fYearResp.FinancialYear.ID).ConfigureAwait(false);
            var viewModel = agencyWorkPlans.ToList();

            //var viewModel = agencyWorkPlans.Where(x => !sectionPlanIdList.Contains(x.RoadSectionId));
            if (sectionPlanIdList.Count > 0)
            {
                //viewModel = agencyWorkPlans.Where(x => sectionPlanIdList.Any(s => !x.ID.Equals(s)));
                foreach (var workPlan in agencyWorkPlans)
                {
                    if (sectionPlanIdList.Contains(workPlan.ID))
                    {
                        viewModel.Remove(workPlan);
                    }
                }
            }

            return PartialView("RoadWorkplansSelect", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateSelectedWorkplans(long workpackageId, long[] seletectedIds)
        {

            // selectedData: seletectedIds
            if (workpackageId > 0)
            {
                if (seletectedIds != null)
                {
                    WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan;
                    //editing only the specific fields for work planning the record
                    if (seletectedIds.Length > 0)
                    {
                        foreach (long id in seletectedIds)
                        {
                            //create the record in the database for the activity with the plan
                            workpackageRoadWorkSectionPlan = new WorkpackageRoadWorkSectionPlan();
                            workpackageRoadWorkSectionPlan.PackageAmount = 0.0;
                            workpackageRoadWorkSectionPlan.PackageQuantity = 0.0;
                            workpackageRoadWorkSectionPlan.WorkPlanPackageId = workpackageId;
                            workpackageRoadWorkSectionPlan.RoadWorkSectionPlanId = id;

                            await _workpackageRoadWorkSectionPlanService.AddAsync(workpackageRoadWorkSectionPlan).ConfigureAwait(false);
                        }

                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workpackageId })
                        });

                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid Selection"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Please select a workplan to add to package"
                    });
                }
            }
            return RedirectToAction("GetRoadWorkplans");
        }

        [HttpPost]
        public async Task<IActionResult> ContractPackage(long workpackageId)
        {

            // selectedData: seletectedIds
            if (workpackageId > 0)
            {
                var existingRecord = await _workPlanPackageService.FindByIdAsync(workpackageId).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    //check if the workpackage has already been contracted
                    if (existingRecord.WorkPlanPackage.Status == 0)
                    {
                        //set it as contracting
                        var user = await GetLoggedInUser().ConfigureAwait(false);
                        //create the contract record and redirect to the contract page
                        Contract contractAdd = new Contract();
                        contractAdd.WorkPlanPackageId = workpackageId;
                        contractAdd.ContractSumWorkplan = existingRecord.WorkPlanPackage.WorkpackageRoadWorkSectionPlans.Sum(s => s.RoadWorkSectionPlan.PlanActivities.Sum(r => r.Amount));
                        contractAdd.ContractSumPackage = (1.16 * existingRecord.WorkPlanPackage.WorkpackageRoadWorkSectionPlans.Sum(s => s.RoadWorkSectionPlan.PlanActivities.Sum(r => r.Amount)));
                        contractAdd.CreatedBy = user.UserName;
                        contractAdd.CreationDate = DateTime.UtcNow;

                        var addRecord = await _contractService.AddAsync(contractAdd).ConfigureAwait(false);
                        if (addRecord.Success)
                        {
                            existingRecord.WorkPlanPackage.Status = 1;
                            existingRecord.WorkPlanPackage.UpdateBy = user.UserName;
                            existingRecord.WorkPlanPackage.UpdateDate = DateTime.UtcNow;

                            var updateRecord = await _workPlanPackageService.UpdateAsync(existingRecord.WorkPlanPackage).ConfigureAwait(false);
                            if (updateRecord.Success)
                            {
                                // return RedirectToAction("ContractDetails", new { ID = addRecord.Contract.ID });
                                return Json(new
                                {
                                    Success = true,
                                    Message = "Success",
                                    Href = Url.Action("ContractDetails", "WorkPlanPackage", new { ID = addRecord.Contract.ID })
                                });
                            }
                            else
                            {
                                return Json(new
                                {
                                    Success = false,
                                    Message = "Failed to update contract status for the package"
                                });
                            }

                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Failed to contract the package"
                            });
                        }
                    }

                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the ID"
                    });
                }
            }
            return RedirectToAction("GetRoadWorkplans");
        }

        [HttpPost]
        public async Task<IActionResult> SaveContract(Contract contract)
        {
            if (contract != null)
            {

                var existingRecord = await _contractService.FindByIdAsync(contract.ID).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    var user = await GetLoggedInUser().ConfigureAwait(false);
                    //update the existing record
                    existingRecord.Contract.UpdateBy = user.UserName;
                    existingRecord.Contract.UpdateDate = DateTime.UtcNow;
                    //records updated from form
                    existingRecord.Contract.PerformanceBond = contract.PerformanceBond;
                    existingRecord.Contract.PercentageRetention = contract.PercentageRetention;
                    existingRecord.Contract.inPaymentCertificate = contract.inPaymentCertificate;
                    existingRecord.Contract.ContractTaxable = contract.ContractTaxable;
                    existingRecord.Contract.ContractIsSigned = contract.ContractIsSigned;
                    //existingRecord.Contract.ContractSumWorkplan = contract.ContractSumWorkplan;
                    //existingRecord.Contract.ContractSumPackage = contract.ContractSumPackage;
                    existingRecord.Contract.ContractStartDate = contract.ContractStartDate;
                    existingRecord.Contract.ContractEndDate = contract.ContractEndDate;
                    existingRecord.Contract.AdvancePayment = contract.AdvancePayment;
                    existingRecord.Contract.AdvanceClearance = contract.AdvanceClearance;

                    var existingContractorResp = await _contractorService.FindByKraPinAsync(contract.Contractor.KRAPin).ConfigureAwait(false);
                    if (existingContractorResp.Success)
                        existingRecord.Contract.ContractorId = existingContractorResp.Contractor.ID;

                    var updateResp = await _contractService.UpdateAsync(existingRecord.Contract).ConfigureAwait(false);
                    if (updateResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success.",
                            Href = Url.Action("ContractDetails", "WorkPlanPackage", new { ID = contract.ID })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Record update failed, please contact the administrator"
                        });
                    }


                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the ID"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid record submitted for update"
                });
            }
        }

        //Fetch the contract dates edit partial view
        public async Task<IActionResult> EditPackageContractSummaryDatesDetails(long contractId)
        {
            var viewModel = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
            return PartialView("ContractProjectSummaryDatesEdit", viewModel.Contract);
        }

        //Fetch the contract summary edit partial view
        public async Task<IActionResult> EditPackageContractSummaryDetails(long contractId)
        {
            var viewModel = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
            return PartialView("ContractProjectSummaryEdit", viewModel.Contract);
        }

        [HttpPost]
        public async Task<IActionResult> EditPackageContractSummaryDatesDetails(Contract contract)
        {
            if (contract != null)
            {

                var existingRecord = await _contractService.FindByIdAsync(contract.ID).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    var user = await GetLoggedInUser().ConfigureAwait(false);
                    //update the existing record
                    existingRecord.Contract.UpdateBy = user.UserName;
                    existingRecord.Contract.UpdateDate = DateTime.UtcNow;
                    //records updated from form
                    existingRecord.Contract.AwardDate = contract.AwardDate;
                    existingRecord.Contract.TenderDate = contract.TenderDate;
                    existingRecord.Contract.CommencementDate = contract.CommencementDate;
                    existingRecord.Contract.CompletionDate = contract.CompletionDate;

                    var updateResp = await _contractService.UpdateAsync(existingRecord.Contract).ConfigureAwait(false);
                    if (updateResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success.",
                            Href = Url.Action("ContractDetails", "WorkPlanPackage", new { ID = contract.ID })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Record update failed, please contact the administrator"
                        });
                    }


                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the ID"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid record submitted for update"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditPackageContractSummaryDetails(Contract contract)
        {
            if (contract != null)
            {

                var existingRecord = await _contractService.FindByIdAsync(contract.ID).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    var user = await GetLoggedInUser().ConfigureAwait(false);
                    //update the existing record
                    existingRecord.Contract.UpdateBy = user.UserName;
                    existingRecord.Contract.UpdateDate = DateTime.UtcNow;
                    //records updated from form
                    existingRecord.Contract.ProjectTitle = contract.ProjectTitle;
                    existingRecord.Contract.Financier = contract.Financier;
                    existingRecord.Contract.Engineer = contract.Engineer;
                    existingRecord.Contract.EngineerRepresentative= contract.EngineerRepresentative;

                    var updateResp = await _contractService.UpdateAsync(existingRecord.Contract).ConfigureAwait(false);
                    if (updateResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success.",
                            Href = Url.Action("ContractDetails", "WorkPlanPackage", new { ID = contract.ID })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Record update failed, please contact the administrator"
                        });
                    }


                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the ID"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid record submitted for update"
                });
            }
        }
        public async Task<IActionResult> ContractDetails(long ID)
        {
            var viewModel = new WorkPlanPackageContractViewModel();
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);



            var user = await GetLoggedInUser().ConfigureAwait(false);

            if (user != null)
            {

                //return packages for the specific authority
                viewModel.Contracts = await _contractService.ListContractsByAgencyByFinancialYear(user.AuthorityId,fYearResp.Success ? fYearResp.FinancialYear.ID : 0 ).ConfigureAwait(false);

            }

           

            //check if contract is selected
            if (ID > 0)
            {
                var workPlanPackageContractResp = await _contractService.FindByIdAsync(ID).ConfigureAwait(false);
                if (workPlanPackageContractResp.Success)
                {
                    viewModel.Contract = workPlanPackageContractResp.Contract;
                }
            }
            else
            {
                viewModel.Contract = new Contract();
            }
            ViewBag.Authority = user.Authority;

            return View("RoadWorkPackagingContract", viewModel);
        }


        public async Task<IActionResult> PackageContractDetails(long workpackageId)
        {
            if (workpackageId > 0)
            {
                var workPlanPackageContractResp = await _contractService.FindContractByPackageIdAsync(workpackageId).ConfigureAwait(false);
                if (workPlanPackageContractResp.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("ContractDetails", "WorkPlanPackage", new { ID = workPlanPackageContractResp.Contract.ID })
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Contract for the package could not be found"
                    });

                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid package selection"
                });

            }
        }

        public async Task<IActionResult> EditPackageBidRate(long planActivityId, long workpackageId)
        {
            var viewModel = new PackageQuantityEditViewModel();
            if (planActivityId > 0)
            {
                var sectionPlanActivityResponse = await _planActivityService.FindByIdAsync(planActivityId).ConfigureAwait(false);
                if (sectionPlanActivityResponse.Success)
                    viewModel.PlanActivity = sectionPlanActivityResponse.PlanActivity;
            }
            else
            {
                viewModel.PlanActivity = new PlanActivity();
            }
            viewModel.workPackageId = workpackageId;

            return PartialView("WorkPackageBidRateEdit", viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> EditPackageBidRate(PackageQuantityEditViewModel packageQuantityEditViewModel)
        {
            if (packageQuantityEditViewModel != null)
            {
                if (packageQuantityEditViewModel.PlanActivity != null)
                {
                    if (packageQuantityEditViewModel.PlanActivity.ID > 0 && packageQuantityEditViewModel.workPackageId > 0)
                    {
                        var user = await GetLoggedInUser().ConfigureAwait(false);

                        //get the workpackage first
                        var existingPackageSuccess = await _workPlanPackageService.FindByIdAsync(packageQuantityEditViewModel.workPackageId).ConfigureAwait(false);
                        if (existingPackageSuccess.Success)
                        {
                            //retrieve the plan activity and update

                            var existingRecord = await _planActivityService.FindByIdAsync(packageQuantityEditViewModel.PlanActivity.ID).ConfigureAwait(false);
                            if (existingRecord.Success)
                            {
                                //get the old quantity to affect the engineer's estimate
                                var oldPackageBidRate = existingRecord.PlanActivity.BidRate;

                                existingRecord.PlanActivity.BidRate = packageQuantityEditViewModel.PlanActivity.BidRate;
                                existingRecord.PlanActivity.BillItemAmount = (double)(existingRecord.PlanActivity.PackageQuantity * (decimal)packageQuantityEditViewModel.PlanActivity.BidRate);
                                existingRecord.PlanActivity.UpdateBy = user.UserName;
                                existingRecord.PlanActivity.UpdateDate = DateTime.UtcNow;


                                //save
                                var updateRecord = await _planActivityService.UpdateAsync(existingRecord.PlanActivity).ConfigureAwait(false);
                                if (updateRecord.Success)
                                {
                                    //reset the engineer's total in the package quantity
                                    if (existingPackageSuccess.WorkPlanPackage.OriginalContractSum == 0 && oldPackageBidRate == 0)
                                    {
                                        //Removing the old value from contract sum is not required
                                        existingPackageSuccess.WorkPlanPackage.OriginalContractSum =
                                         Math.Ceiling(existingPackageSuccess.WorkPlanPackage.OriginalContractSum + (double)((existingRecord.PlanActivity.PackageQuantity * (decimal)(packageQuantityEditViewModel.PlanActivity.BidRate) * (decimal) (100 + existingPackageSuccess.WorkPlanPackage.VAT)))) / 100;
                                    }
                                    else
                                    {
                                        //remove the old value from the contract sum then add the new value into the contract sum
                                        var oldValue = Math.Ceiling((existingRecord.PlanActivity.PackageQuantity * (decimal)(oldPackageBidRate) * (decimal)(100 + existingPackageSuccess.WorkPlanPackage.VAT))) / 100;
                                        var newValue = Math.Ceiling((existingRecord.PlanActivity.PackageQuantity * (decimal)(packageQuantityEditViewModel.PlanActivity.BidRate) * (decimal)(100 + existingPackageSuccess.WorkPlanPackage.VAT))) / 100;

                                        existingPackageSuccess.WorkPlanPackage.OriginalContractSum = (existingPackageSuccess.WorkPlanPackage.OriginalContractSum - (double)oldValue + (double)newValue); //new value

                                    }

                                    //update the package
                                    await _workPlanPackageService.UpdateAsync(existingPackageSuccess.WorkPlanPackage).ConfigureAwait(false);
                                    return Json(new
                                    {
                                        Success = true,
                                        Message = "Success"//,
                                                           //Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workPlanPackage.ID })
                                    });
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Message = "Update on work package failed"
                                    });
                                }
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Invalid package supplied"
                            });
                        }

                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid activity supplied"
                        });
                    }
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid activity supplied"
                });
            }

            return PartialView("WorkPackageQuantityEdit", packageQuantityEditViewModel);
        }


        public async Task<IActionResult> ProjectProgressDetails(long ID)
        {
            //ID is contract ID
            var viewModel = new WorkPlanPackageContractViewModel();
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);



            var user = await GetLoggedInUser().ConfigureAwait(false);

            if (user != null)
            {

                //return packages for the specific authority
                viewModel.Contracts = await _contractService.ListContractsByAgencyByFinancialYear(user.AuthorityId,fYearResp.Success ? fYearResp.FinancialYear.ID : 0).ConfigureAwait(false);

            }

            viewModel.Authority = user.Authority;

            //check if contract is selected
            if (ID > 0)
            {
                var workPlanPackageContractResp = await _contractService.FindByIdAsync(ID).ConfigureAwait(false);
                if (workPlanPackageContractResp.Success)
                {
                    viewModel.Contract = workPlanPackageContractResp.Contract;
                }
            }
            else
            {
                viewModel.Contract = new Contract();
            }
            ViewBag.Authority = user.Authority;
            return View("RoadWorkProjectProgress", viewModel);
        }

        public async Task<IActionResult> FinancialProgressDetails(long authorityId, long progressEntryId)
        {
            //ID is financial progress ID
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var viewModel = new FinancialProgressViewModel();
            FinancialProgress financialProgress = new FinancialProgress();
            financialProgress.AuthorityId = authorityId;
            financialProgress.FinancialYear = fYearResp.FinancialYear;
            financialProgress.FinancialYearId = fYearResp.FinancialYear.ID;
            financialProgress.Authority = user.Authority;

            viewModel.FinancialProgress = financialProgress;



            //get the supplied financial progress
            if(progressEntryId > 0)
            {
                var existingRecord = await _financialProgressService.FindByIdAsync(progressEntryId).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    viewModel.FinancialProgress = existingRecord.FinancialProgress;
                }
            }

            //return list of existing financial entries for the supplied contract

            viewModel.FinancialProgressList = await _financialProgressService.ListByAuthorityIdAndFinancialYearAsync(authorityId, fYearResp.FinancialYear.ID).ConfigureAwait(false);
            var fYearList = await _financialYearService.ListAsync().ConfigureAwait(false);
            var monthList = await _monthCodeService.ListAsync().ConfigureAwait(false);
            var selectList = new List<MonthCode>();
            selectList = monthList.ToList();
            if (viewModel.FinancialProgressList.Any())
            {
                foreach (var selectListMonth in monthList)
                {
                    foreach (var monthProgress in viewModel.FinancialProgressList)
                    {
                        if(monthProgress.MonthCodeId == selectListMonth.ID)
                        {
                            selectList.Remove(selectListMonth);
                        }
                    }
                }
            }
            

            //remove the quarter handled


            ViewBag.YearList = new SelectList(fYearList.OrderBy(f=>f.IsCurrent), "ID", "Code");
                        
            ViewBag.MonthCodeList = new SelectList(selectList.OrderBy(q => q.ID), "ID", "Description");
            ViewBag.Authority = user.Authority;

            return View("RoadWorkPackagingFinancialProgress", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkplanPackageProgress([FromForm] FinancialProgressViewModel financialProgressViewModel)
        {
            if (financialProgressViewModel != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);

                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    ViewBag.Authority = userAuthority.Authority;
                }
                string uploadFileName = null;
                if (financialProgressViewModel.BankReconFile != null)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "reportUploadSupportingDocuments");
                    uploadFileName = Guid.NewGuid().ToString() + "_" + financialProgressViewModel.BankReconFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uploadFileName);

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    //copy contents of the uploaded file to stream
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await financialProgressViewModel.BankReconFile.CopyToAsync(stream).ConfigureAwait(false);
                }


                //save the package
                if (financialProgressViewModel.FinancialProgress.ID > 0)
                {
                    //check uniquenuess of package name
                    var existingRecordResp = await _financialProgressService.FindByIdAsync(financialProgressViewModel.FinancialProgress.ID).ConfigureAwait(false);
                    if (existingRecordResp.Success)
                    {
                        var updateRecord = existingRecordResp.FinancialProgress;
                        updateRecord.OpeningBalance = financialProgressViewModel.FinancialProgress.OpeningBalance;
                        updateRecord.FinancialReceipts= financialProgressViewModel.FinancialProgress.FinancialReceipts;
                        updateRecord.FinancialExpenditure= financialProgressViewModel.FinancialProgress.FinancialExpenditure;
                        updateRecord.ClosingBalance= financialProgressViewModel.FinancialProgress.ClosingBalance;
                        updateRecord.BankReconFileName = uploadFileName;
                        //updateRecord.QuarterCodeListId = financialProgressViewModel.FinancialProgress.QuarterCodeListId;
                        updateRecord.MonthCodeId= financialProgressViewModel.FinancialProgress.MonthCodeId;
                        
                      
                        var updateResp = await _financialProgressService.UpdateAsync(updateRecord).ConfigureAwait(false);
                        if (updateResp.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success",
                                Href = Url.Action("FinancialProgressDetails", "WorkPlanPackage", new { authorityId = updateRecord.Authority.ID })
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Creation of progress record failed"
                            });
                        }
                    }

                }
                else
                {
                    //check uniquenuess of package name
                    var newRecord =  new FinancialProgress();
                    newRecord.OpeningBalance = financialProgressViewModel.FinancialProgress.OpeningBalance;
                    newRecord.FinancialReceipts = financialProgressViewModel.FinancialProgress.FinancialReceipts;
                    newRecord.FinancialExpenditure = financialProgressViewModel.FinancialProgress.FinancialExpenditure;
                    newRecord.ClosingBalance = financialProgressViewModel.FinancialProgress.ClosingBalance;
                    newRecord.BankReconFileName = uploadFileName;
                    newRecord.Authority = user.Authority;
                    newRecord.FinancialYearId = financialProgressViewModel.FinancialProgress.FinancialYearId;
                    //newRecord.QuarterCodeListId = financialProgressViewModel.FinancialProgress.QuarterCodeListId;
                    newRecord.MonthCodeId = financialProgressViewModel.FinancialProgress.MonthCodeId;

                    var newResp = await _financialProgressService.AddAsync(newRecord).ConfigureAwait(false);
                    if (newResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("FinancialProgressDetails", "WorkPlanPackage", new { authorityId = newRecord.Authority.ID })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Update on work financial progress record failed. Please check with the administrator."
                        });
                    }
                }

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                   // Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workPlanPackage.ID })
                });

            }
            return Json(new
            {
                Success = false,
                Message = "Invalid Package details"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkplanPackageProgress(long financialProgressID)
        {
            if(financialProgressID > 0)
            {
                var financialProgress = await _financialProgressService.RemoveAsync(financialProgressID).ConfigureAwait(false);
                if (financialProgress.Success)
                {
                    return Json(new { Success = true, Message = "Record successfully removed.", Href = Url.Action("FinancialProgressDetails", "WorkPlanPackage", new { authorityId = financialProgress.FinancialProgress.AuthorityId }) }); 
                }
            }

            return Json(new { Success = false, Message = "Invalid entry details" });
        }

        public ActionResult DownLoadProgressBankDocument(string bankFileName)
        {
            return RedirectToAction("DownloadSupportingDocs", "WorkPlanPackage", new { filename = bankFileName, folder = "reportUploadSupportingDocuments" });
            //(string filename, string folder)
        }

        public ActionResult DownLoadProgressGrantDocument(string bankFileName)
        {
            return RedirectToAction("DownloadSupportingDocs", "WorkPlanPackage", new { filename = bankFileName, folder = "reportUploadSupportingDocuments" });
            //(string filename, string folder)
        }

        public async Task<IActionResult> DownloadSupportingDocs(string filename, string folder)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           _hostingEnvironment.WebRootPath, "uploads",
                           folder, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(false);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        //returns the contract payment certificates
        public async Task<IActionResult> ProjectPaymentCertificates(long ID)
        {
            var viewModel = new Contract();
            if (ID > 0)
            {
                var workPlanPackageContractResp = await _contractService.FindByIdAsync(ID).ConfigureAwait(false);
                if (workPlanPackageContractResp.Success)
                {
                    viewModel = workPlanPackageContractResp.Contract;
                }
            }
            var user = await GetLoggedInUser().ConfigureAwait(false);
            ViewBag.Authority = user.Authority;
            return View("RoadWorkPackagingPaymentCertificates", viewModel);
        }

        //returns the payment certificate activities
        public async Task<IActionResult> ProjectPaymentCertificateActivities(long ID)
        {
            var viewModel = new PaymentCertificate();
            if (ID > 0)
            {
                var workPlanPackageContractResp = await _paymentCertificateService.FindByIdAsync(ID).ConfigureAwait(false);
                if (workPlanPackageContractResp.Success)
                {
                    viewModel = workPlanPackageContractResp.PaymentCertificate;
                }
            }
            var user = await GetLoggedInUser().ConfigureAwait(false);
            ViewBag.Authority = user.Authority;
            return View("RoadWorkPackagingPaymentCertificateActivities", viewModel);
        }

        //get contract activities for selection to include in certificate
        public async Task<IActionResult> GetContractActivities(long contractId)
        {
            var viewModel = new List<PlanActivity>();
            if(contractId > 0)
            {
                var contractActivities = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
                if (contractActivities.Success)
                {
                    //retrieve the plan activities
                    var activities = contractActivities.Contract.WorkPlanPackage.WorkpackageRoadWorkSectionPlans.Select(a => a.RoadWorkSectionPlan.PlanActivities);
                    foreach(var actv in activities)
                    {
                        foreach(var act in actv)
                        {
                            viewModel.Add(act);
                        }
                    }

                }
            }
            
            return PartialView("ContractActivitySelect", viewModel);
        }


        //Update the payment certificate with the selected contract activities
        [HttpPost]
        public async Task<IActionResult> UpdateContractPaymentCertificateActivities(long paymentCertificateId, long[] seletectedIds)
        {

            // selectedData: seletectedIds
            if (paymentCertificateId > 0)
            {
                if (seletectedIds != null)
                {
                    PaymentCertificateActivity paymentCertificateActivity;
                    //editing only the specific fields for work planning the record
                    if (seletectedIds.Length > 0)
                    {
                        var user = await GetLoggedInUser().ConfigureAwait(false);
                        foreach (long id in seletectedIds)
                        {
                            //create the record in the database for the activity with the plan
                            paymentCertificateActivity = new PaymentCertificateActivity();
                            paymentCertificateActivity.PlanActivityId= id;
                            paymentCertificateActivity.PaymentCertificateId = paymentCertificateId;
                            paymentCertificateActivity.CreatedBy = user.UserName;
                            paymentCertificateActivity.CreationDate = DateTime.UtcNow;

                            await _paymentCertificateActivitiesService.AddAsync(paymentCertificateActivity).ConfigureAwait(false);
                        }

                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("ProjectPaymentCertificateActivities", "WorkPlanPackage", new { id = paymentCertificateId })
                        });

                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid Selection"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Please select atleast one activity to add to the certificate."
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Please select the correct payment certificate."
                });
            }
        }

        //Remove a payment certificate ID
        [HttpPost]
        public async Task<IActionResult> RemveCertificateActivity(long certificateActivityId)
        {

            // selectedData: seletectedIds
            if (certificateActivityId > 0)
            {

                var removeResp = await _paymentCertificateActivitiesService.RemoveAsync(certificateActivityId).ConfigureAwait(false);
                if (removeResp.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("ProjectPaymentCertificateActivities", "WorkPlanPackage", new { id = removeResp.PaymentCertificateActivity.PaymentCertificateId })
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = removeResp.Message
                    });
                }
                

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Please select the correct payment certificate activity."
                });
            }
        }

        //Get the partial view to edit the contract certificate activity quantity
        public async Task<IActionResult> GetCertificateActivityEditForm(long certificateActivityId)
        {
            var viewModel = new PaymentCertificateActivity();
            if (certificateActivityId > 0)
            {
                var certActivityResp = await _paymentCertificateActivitiesService.FindByIdAsync(certificateActivityId).ConfigureAwait(false);
                if (certActivityResp.Success)
                {
                    //retrieve the certificate activity
                    viewModel = certActivityResp.PaymentCertificateActivity;

                }
            }

            return PartialView("ContractCertificateActivityEditPartialView", viewModel);
        }

        /// <summary>
        /// Handles submitted form data for budgetline creation or modification
        /// </summary>
        /// <param name="roadSectionEditModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCertificateActivity(PaymentCertificateActivity paymentCertificateActivity)
        {
            if (paymentCertificateActivity != null)
            {
                //editing the record
                var updateResponse = await _paymentCertificateActivitiesService.UpdateAsync(paymentCertificateActivity).ConfigureAwait(false);
                if (updateResponse.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("ProjectPaymentCertificateActivities", "WorkPlanPackage", new { id = updateResponse.PaymentCertificateActivity.PaymentCertificateId })
                    });
                }
                else
                {
                    return PartialView("SectionWorkplanActivityEditPartialView", paymentCertificateActivity);
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Submitted record is null"
                });
            }
        }

        public async Task<IActionResult> ProjectPaymentCertificate(long ID)
        {
            var viewModel = new PaymentCertificate();
            if (ID > 0)
            {
                var workPlanPackageContractResp = await _paymentCertificateService.FindByIdAsync(ID).ConfigureAwait(false);
                if (workPlanPackageContractResp.Success)
                {
                    viewModel = workPlanPackageContractResp.PaymentCertificate;
                }
            }
            var user = await GetLoggedInUser().ConfigureAwait(false);
            ViewBag.Authority = user.Authority;

            return View("RoadWorkPackagingPaymentCertificate", viewModel);
        }
        //This function creates a new payment certificate for a contract
        [HttpPost]
        public async Task<IActionResult> GeneratePaymentCertificate(Contract contract)
        {
            if (contract != null)
            {
                var existingRecord = await _contractService.FindByIdAsync(contract.ID).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    if (existingRecord.Contract.ContractStartDate != null)
                    {
                        var prevCertDate = new DateTime();
                        var latestCertDate = new DateTime();
                        if (contract.LatestCertificateDate == DateTime.Today)
                        {
                            latestCertDate = (DateTime)contract.LatestCertificateDate;
                            //latestCertDate = latestCertDate.AddDays(-1);
                        }
                        else
                        {
                            latestCertDate = (DateTime)contract.LatestCertificateDate;
                        }

                        if (contract.PreviousCertificateDate != null)
                        {
                            prevCertDate = (DateTime)contract.PreviousCertificateDate;
                        }
                        else
                        {
                            prevCertDate = (DateTime)existingRecord.Contract.ContractStartDate;
                        }

                        var user = await GetLoggedInUser().ConfigureAwait(false);

                        //compute the 'this' certificate values
                        var thisTotalWorkDone = 0.0;

                        //create the certificates
                        var addRecord = new PaymentCertificate();
                        addRecord.CertificateNo = existingRecord.Contract.CurrentCertificateNo + 1;
                        addRecord.CertificateContractSum = existingRecord.Contract.ContractSumPackage;
                        addRecord.CertificateReviseContractSum = existingRecord.Contract.ContractRevisionSum;
                        addRecord.CertificateValuationAsAt = 0.0;

                        var packagePlans = existingRecord.Contract.WorkPlanPackage.WorkpackageRoadWorkSectionPlans;
                        //var thisProgressEntryTotals = packagePlans.Sum(i => i.RoadWorkSectionPlan.PlanActivities.Sum(s => s.PackageProgressEntries.Where(e => e.CreationDate >= prevCertDate && e.CreationDate <= latestCertDate).Sum(e => e.Quantity * e.Rate)));
                        var thisProgressEntryTotals = packagePlans.Sum(i => i.RoadWorkSectionPlan.PlanActivities.Sum(s => s.PackageProgressEntries.Sum(e => e.Quantity * e.Rate)));

                        if (thisProgressEntryTotals > 0)
                        {
                            thisTotalWorkDone = thisProgressEntryTotals;
                            //check if there was a previous certificate existing, and pull it's records
                            if (existingRecord.Contract.CurrentCertificateNo > 0)
                            {


                                //there is a certificate and set the previous certificate values
                                var previousCertificate = await _paymentCertificateService.FindByContractIdAndCertificateNo(contract.ID, existingRecord.Contract.CurrentCertificateNo).ConfigureAwait(false);
                                if (previousCertificate.Success)
                                {

                                    addRecord.CertificateTotalWorkDonePrevious = previousCertificate.PaymentCertificate.CertificateTotalWorkDoneThis;
                                    addRecord.CertificateTotalWorkDoneThis = thisTotalWorkDone;
                                    addRecord.CertificateTotalWorkDoneTotals = (previousCertificate.PaymentCertificate.CertificateTotalWorkDoneTotals + thisTotalWorkDone);

                                    addRecord.CertificateMaterialOnSitePrevious = previousCertificate.PaymentCertificate.CertificateMaterialOnSitePrevious;
                                    addRecord.CertificateMaterialOnSiteThis = 0.0; // no previous certificate
                                    addRecord.CertificateMaterialOnSiteTotals = previousCertificate.PaymentCertificate.CertificateMaterialOnSiteTotals;

                                    addRecord.CertificateVariationOfPricePrevious = previousCertificate.PaymentCertificate.CertificateVariationOfPricePrevious;
                                    addRecord.CertificateVariationOfPriceThis = 0.0; // no previous certificate
                                    addRecord.CertificateVariationOfPriceTotals = previousCertificate.PaymentCertificate.CertificateVariationOfPriceTotals;
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Message = "Error occured retrieving the previous certificate. Please contact system administrator"
                                    });
                                }
                            }
                            else
                            {
                                addRecord.CertificateTotalWorkDonePrevious = 0.0; // no previous certificate
                                addRecord.CertificateTotalWorkDoneThis = thisTotalWorkDone;
                                addRecord.CertificateTotalWorkDoneTotals = thisTotalWorkDone; // current certificate value

                                addRecord.CertificateMaterialOnSitePrevious = 0.0; // no previous certificate
                                addRecord.CertificateMaterialOnSiteThis = 0.0; // no previous certificate
                                addRecord.CertificateMaterialOnSiteTotals = 0.0; // current certificate value

                                addRecord.CertificateVariationOfPricePrevious = 0.0; // no previous certificate
                                addRecord.CertificateVariationOfPriceThis = 0.0; // no previous certificate
                                addRecord.CertificateVariationOfPriceTotals = 0.0; // current certificate value
                            }

                            addRecord.VAT = 16F;
                            addRecord.LessVAT = 6F;
                            addRecord.Retention = existingRecord.Contract.PercentageRetention;
                            addRecord.WithholdingTax = 3F;

                            addRecord.AdvanceRecovery = 0.0;
                            addRecord.AdvanceBalance = 0.0;
                            addRecord.LateRepaymentsInterest = 0.0;
                            addRecord.LiquidatedDamages = 0.0;

                            addRecord.NetPaymentPrevious = 0.0;
                            addRecord.NetPaymentThis = 0.0;
                            addRecord.NetPaymentTotals = 0.0;

                            addRecord.CreatedBy = user.UserName;
                            addRecord.CreationDate = DateTime.UtcNow;

                            addRecord.ContractId = existingRecord.Contract.ID;

                            var certificateAdd = await _paymentCertificateService.AddAsync(addRecord).ConfigureAwait(false);
                            if (certificateAdd.Success)
                            {
                                //update the contract certificate dates

                                //existingRecord.Contract.LatestCertificateDate = latestCertDate;
                                existingRecord.Contract.PreviousCertificateDate = latestCertDate;
                                existingRecord.Contract.CurrentCertificateNo += 1;

                                //update the current contract with new values
                                var updateRecord = await _contractService.UpdateAsync(existingRecord.Contract).ConfigureAwait(false);
                                if (updateRecord.Success)
                                {
                                    return Json(new
                                    {
                                        Success = true,
                                        Message = "Success",
                                        Href = Url.Action("ProjectPaymentCertificates", "WorkPlanPackage", new { ID = contract.ID })
                                    });
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Message = "Update of the contract data was unsuccessful. Please contact system administrator."
                                    });
                                }
                            }
                            else
                            {
                                return Json(new
                                {
                                    Success = false,
                                    Message = "Failed to create the payment certificate. Please contact system administrator."
                                });
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "No value entries set for the period selected. Value sum for the period is KSH. 0.0. No certificate has been generated."
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Previous certificate date or contract start date has not been set."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Contract record could not be found. Please contact system administrator."
                    });
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid activity supplied"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkCertificateComplete(long paymentCertificateId)
        {
            var updatePaymentCertResp = await _paymentCertificateService.FindByIdAsync(paymentCertificateId).ConfigureAwait(false);
            if (updatePaymentCertResp.Success)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var updateRecord = updatePaymentCertResp.PaymentCertificate;
                updateRecord.CertificateStatus = 1;
                updateRecord.UpdateBy = user.UserName;
                updateRecord.UpdateDate = DateTime.UtcNow;

                var updateResp = await _paymentCertificateService.UpdateAsync(updateRecord).ConfigureAwait(false);
                if (updateResp.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("ProjectPaymentCertificateActivities", "WorkPlanPackage", new { id = updateResp.PaymentCertificate.ID})
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Update of the payment certificate data was unsuccessful. Please contact system administrator."
                    });
                }
            }
            return Json(new
            {
                Success = false,
                Message = "Invalid payment certificate. Please check with the Administrator!"
            });
        }
        public async Task<IActionResult> Contractor(string kraPin)
        {
            var viewModel = new Contractor();

            //retrieve contract
            var contractResp = await _contractorService.FindByKraPinAsync(kraPin).ConfigureAwait(false);

            if (contractResp.Success)
            {
                viewModel = contractResp.Contractor;
            }
            else
            {
                viewModel.KRAPin = kraPin;
            }

            return PartialView("ContractorBankDetailsPartialView", viewModel);
        }

        public async Task<IActionResult> DownloadBidDetails(long workplanPackageId, int emptyRequest)
        {
            try
            {
               
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                //retrieve the full workpackage details from database
                var workpackageResp = await _workPlanPackageService.FindByIdAsync(workplanPackageId).ConfigureAwait(false);
                var workPlanPackage = workpackageResp.WorkPlanPackage;

                //get the current financial year
                var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (financialYearResp.Success)
                    printData.FinancialYear = financialYearResp.FinancialYear;

                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(workPlanPackage.AuthorityId).ConfigureAwait(false);
                var authority = authorityResp.Authority;
                printData.Authority = authority;


                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    var sheetcreateSummary = excel.Workbook.Worksheets.Add("BOQ Summary");
                    var sheetcreate = excel.Workbook.Worksheets.Add("BOQ Details");

                    //BEGIN BOQ SUMMARY WORKSHEET
                    //print the column headers
                    //First Row cONTRACTOR
                    sheetcreateSummary.Cells[1, 1].Value = "Road Code";
                    sheetcreateSummary.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[1, 2].Value = workPlanPackage.WorkpackageRoadWorkSectionPlans != null ? workPlanPackage.WorkpackageRoadWorkSectionPlans.FirstOrDefault().RoadWorkSectionPlan.Road.RoadNumber : "";
                    sheetcreateSummary.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //First Row cONTRACTOR
                    sheetcreateSummary.Cells[2, 1].Value = "Section Name";
                    sheetcreateSummary.Cells[2, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[2, 2].Value = workPlanPackage.WorkpackageRoadWorkSectionPlans != null ? workPlanPackage.WorkpackageRoadWorkSectionPlans.FirstOrDefault().RoadWorkSectionPlan.RoadSection.SectionName : "";
                    sheetcreateSummary.Cells[2, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[3, 1].Value = "Package Name";
                    sheetcreateSummary.Cells[3, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[3, 2].Value = workPlanPackage.Name;
                    sheetcreateSummary.Cells[3, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[4, 1].Value = "Contractor";
                    sheetcreateSummary.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var contract = await _contractService.FindContractByPackageIdAsync(workPlanPackage.ID).ConfigureAwait(false);
                    if (contract.Success)
                    {

                        sheetcreateSummary.Cells[4, 2].Value = (contract.Contract.Contractor != null ? contract.Contract.Contractor.Name : "");
                        sheetcreateSummary.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {

                        sheetcreateSummary.Cells[4, 2].Value = "";
                        sheetcreateSummary.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    sheetcreateSummary.Cells[1, 3].Value = "";
                    sheetcreateSummary.Cells[1, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[1, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[2, 3].Value = "";
                    sheetcreateSummary.Cells[2, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[3, 3].Value = "";
                    sheetcreateSummary.Cells[3, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[4, 3].Value = "";
                    sheetcreateSummary.Cells[4, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //First Row
                    sheetcreateSummary.Cells[5, 1].Value = "BILL OF QUANTITIES";
                    sheetcreateSummary.Cells[5, 1].Style.Font.Bold = true;
                    // sheetcreate.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    //Header for the line entries
                    sheetcreateSummary.Cells[6, 2].Value = "Summary";
                    sheetcreateSummary.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[6, 3].Value = "Project : " + workPlanPackage.Name;
                    sheetcreateSummary.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //create now the record items
                    //print headers
                    sheetcreateSummary.Cells[5, 1].Value = "BILL NO.";
                    sheetcreateSummary.Cells[5, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[5, 2].Value = "DESCRIPTION";
                    sheetcreateSummary.Cells[5, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateSummary.Cells[5, 3].Value = "AMOUNT (KSH)";
                    sheetcreateSummary.Cells[5, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    var subTotal = 0.0;
                    int row = 6;
                    var billGroupSummaryList = new List<BillGroupSummary>();

                    if (workPlanPackage.WorkpackageRoadWorkSectionPlans != null)
                    {
                        foreach (var roadSectionPlan in workPlanPackage.WorkpackageRoadWorkSectionPlans)
                        {
                            if (roadSectionPlan.RoadWorkSectionPlan != null)
                            {
                                foreach (var plan in roadSectionPlan.RoadWorkSectionPlan.PlanActivities)
                                {

                                    billGroupSummaryList.Add(
                                        new BillGroupSummary()
                                        {
                                            BillNumber = plan.ItemActivityUnitCost.ItemActivityGroup.BillNumber,
                                            Description = plan.ItemActivityUnitCost.ItemActivityGroup.Description,
                                            Amount = plan.PackageAmount
                                        }
                                    );
                                }
                            }
                        }
                    }

                    var groupedSummary = billGroupSummaryList.GroupBy(x => new { x.BillNumber, x.Description }).Select(x => new
                    {
                        BillNumber = x.Key.BillNumber,
                        Description = x.Key.Description,
                        SumAmount = x.Sum(y => y.Amount)
                    }).ToList();

                    foreach (var plan in groupedSummary.OrderBy(g=>g.BillNumber))
                    {

                        sheetcreateSummary.Cells[row, 1].Value = plan.BillNumber;
                        //sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheetcreateSummary.Cells[row, 2].Value = plan.Description;
                        //sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                        if(emptyRequest == 1)
                        {
                            sheetcreateSummary.Cells[row, 3].Value = (plan.SumAmount * 0).ToString("N");
                            // sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                            sheetcreateSummary.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            sheetcreateSummary.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        else
                        {
                            sheetcreateSummary.Cells[row, 3].Value = plan.SumAmount.ToString("N");
                            // sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                            sheetcreateSummary.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            sheetcreateSummary.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        

                        row++;

                        subTotal += plan.SumAmount;

                    }

                    if(emptyRequest == 1)
                    {
                        subTotal *= 0;
                    }


                    sheetcreateSummary.Cells[row + 2, 2].Value = "Sub Total";
                    sheetcreateSummary.Cells[row + 2, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 2, 3].Value = subTotal.ToString("N");
                    sheetcreateSummary.Cells[row + 2, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreateSummary.Cells[row + 4, 2].Value = "VAT @ 16 % ";
                    //sheetcreate.Cells[row + 4, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 4, 3].Value = (subTotal * 0.16).ToString("N");
                    // sheetcreate.Cells[row + 4, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 5, 2].Value = "Total ";
                    sheetcreateSummary.Cells[row + 5, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 5, 3].Value = (subTotal * 1.16).ToString("N");
                    sheetcreateSummary.Cells[row + 5, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreateSummary.Cells[row + 6, 2].Value = "Contigencies ( @ 0% ) ";
                    //sheetcreate.Cells[row + 6, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 6, 3].Value = 0.0;
                    sheetcreateSummary.Cells[row + 6, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 6, 2].Value = "Carried to page on the form of Tender ";
                    sheetcreateSummary.Cells[row + 6, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells.AutoFitColumns();
                    //END BOQ SUMMARY DETAILS

                    //BEGIN BID DETAILS ENTRIES
                    //print the column headers
                    //First Row
                    sheetcreate.Cells[1, 1].Value = "CONTRACT NAME ";
                    sheetcreate.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[1, 2].Value = workPlanPackage.Name;
                    sheetcreate.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //REGION
                    sheetcreate.Cells[1, 7].Value = "REGION ";
                    sheetcreate.Cells[1, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[1, 8].Value = "";
                    sheetcreate.Cells[1, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //FINANCIAL YEAR

                    sheetcreate.Cells[2, 7].Value = "FINANCIAL YEAR";
                    sheetcreate.Cells[2, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[2, 8].Value = printData.FinancialYear.Code;
                    sheetcreate.Cells[2, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    //second row
                    sheetcreate.Cells[2, 1].Value = "BILL OF QUANTITIES ";
                    sheetcreate.Cells[2, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[2, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[3, 1].Value = "ACTIVITY GROUP TITLE ";
                    sheetcreate.Cells[3, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[3, 2].Value = "";
                    sheetcreate.Cells[3, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreate.Cells[4, 1].Value = "CONTRACTOR ";
                    sheetcreate.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    if (contract.Success)
                    {
                        sheetcreate.Cells[4, 2].Value = contract.Contract.Contractor != null ? contract.Contract.Contractor.Name : "";
                        sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {
                        sheetcreate.Cells[4, 2].Value = "";
                        sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }


                    //create now the record items
                    //print headers
                    sheetcreate.Cells[6, 1].Value = "BILL NO.";
                    sheetcreate.Cells[6, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 2].Value = "ITEM CODE";
                    sheetcreate.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 3].Value = "DESCRIPTION";
                    sheetcreate.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 4].Value = "QUANTITY";
                    sheetcreate.Cells[6, 4].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 5].Value = "UNIT";
                    sheetcreate.Cells[6, 5].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 6].Value = "UNIT BID RATE";
                    sheetcreate.Cells[6, 6].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 7].Value = "BILL ITEM COST WITHOUT VAT";
                    sheetcreate.Cells[6, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 8].Value = "TECHNOLOGY";
                    sheetcreate.Cells[6, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int rowDetails = 7;


                    if (workPlanPackage.WorkpackageRoadWorkSectionPlans != null)
                    {
                        foreach (var roadSectionPlan in workPlanPackage.WorkpackageRoadWorkSectionPlans)
                        {
                            if (roadSectionPlan.RoadWorkSectionPlan != null)
                            {

                                foreach (var plan in roadSectionPlan.RoadWorkSectionPlan.PlanActivities.OrderBy(i=>i.ItemActivityUnitCost.ItemActivityGroup.BillNumber))
                                {
                                    sheetcreate.Cells[rowDetails, 1].Value = plan.ItemActivityUnitCost.ItemActivityGroup.BillNumber;
                                    sheetcreate.Cells[rowDetails, 1].Style.Font.Bold = true;
                                    sheetcreate.Cells[rowDetails, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[rowDetails, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[rowDetails, 2].Value = (plan.ItemActivityUnitCost.ItemCode + "-" + plan.ItemActivityUnitCost.SubItemCode + "-" + plan.ItemActivityUnitCost.SubSubItemCode);
                                    sheetcreate.Cells[rowDetails, 2].Style.Font.Bold = true;
                                    sheetcreate.Cells[rowDetails, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[rowDetails, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[rowDetails, 3].Value = plan.ItemActivityUnitCost.Description;
                                    sheetcreate.Cells[rowDetails, 3].Style.Font.Bold = true;
                                    sheetcreate.Cells[rowDetails, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[rowDetails, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                                    sheetcreate.Cells[rowDetails, 4].Value = plan.PackageQuantity;
                                    sheetcreate.Cells[rowDetails, 4].Style.Font.Bold = true;
                                    sheetcreate.Cells[rowDetails, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[rowDetails, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[rowDetails, 5].Value = plan.ItemActivityUnitCost.UnitMeasure;
                                    sheetcreate.Cells[rowDetails, 5].Style.Font.Bold = true;
                                    sheetcreate.Cells[rowDetails, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[rowDetails, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    if(emptyRequest == 0)
                                    {
                                        sheetcreate.Cells[rowDetails, 6].Value = plan.BidRate;
                                        sheetcreate.Cells[rowDetails, 6].Style.Font.Bold = true;
                                        sheetcreate.Cells[rowDetails, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[rowDetails, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                        sheetcreate.Cells[rowDetails, 7].Value = plan.PackageAmount.ToString("N");
                                        sheetcreate.Cells[rowDetails, 7].Style.Font.Bold = true;
                                        sheetcreate.Cells[rowDetails, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[rowDetails, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                    else
                                    {
                                        sheetcreate.Cells[rowDetails, 6].Value = (plan.BidRate * 0);
                                        sheetcreate.Cells[rowDetails, 6].Style.Font.Bold = true;
                                        sheetcreate.Cells[rowDetails, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[rowDetails, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                        sheetcreate.Cells[rowDetails, 7].Value = (plan.PackageAmount * 0).ToString("N");
                                        sheetcreate.Cells[rowDetails, 7].Style.Font.Bold = true;
                                        sheetcreate.Cells[rowDetails, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[rowDetails, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }

                                    sheetcreate.Cells[rowDetails, 8].Value = plan.Technology.Code;
                                    sheetcreate.Cells[rowDetails, 8].Style.Font.Bold = true;
                                    sheetcreate.Cells[rowDetails, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[rowDetails, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    rowDetails++;

                                }
                                sheetcreate.Cells.AutoFitColumns();
                            }
                        }
                    }

                    //END BID DETAILS ENTRIES

                    //sheetcreate.Cells.AutoFitColumns();

                    excel.Workbook.Properties.Title = "Attempts";
                    // excel.Save();
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "WorkPlanPackage", new { fileGuid = handle, FileName = "BOQ.xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> DownloadBidSummary(long workplanPackageId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                //retrieve the full workpackage details from database
                var workpackageResp = await _workPlanPackageService.FindByIdAsync(workplanPackageId).ConfigureAwait(false);
                var workPlanPackage = workpackageResp.WorkPlanPackage;

                //get the current financial year
                var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (financialYearResp.Success)
                    printData.FinancialYear = financialYearResp.FinancialYear;

                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(workPlanPackage.AuthorityId).ConfigureAwait(false);
                var authority = authorityResp.Authority;
                printData.Authority = authority;


                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    //BEGIN BOQ SUMMARY WORKSHEET
                    var sheetcreateSummary = excel.Workbook.Worksheets.Add("BOQ Summary");
                    //print the column headers
                    //First Row cONTRACTOR
                    sheetcreateSummary.Cells[1, 1].Value = "Road Code";
                    sheetcreateSummary.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[1, 2].Value = workPlanPackage.WorkpackageRoadWorkSectionPlans != null ? workPlanPackage.WorkpackageRoadWorkSectionPlans.FirstOrDefault().RoadWorkSectionPlan.Road.RoadNumber : "";
                    sheetcreateSummary.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //First Row cONTRACTOR
                    sheetcreateSummary.Cells[2, 1].Value = "Section Name";
                    sheetcreateSummary.Cells[2, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[2, 2].Value = workPlanPackage.WorkpackageRoadWorkSectionPlans != null ? workPlanPackage.WorkpackageRoadWorkSectionPlans.FirstOrDefault().RoadWorkSectionPlan.RoadSection.SectionName : "";
                    sheetcreateSummary.Cells[2, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[3, 1].Value = "Package Name";
                    sheetcreateSummary.Cells[3, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[3, 2].Value = workPlanPackage.Name;
                    sheetcreateSummary.Cells[3, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[4, 1].Value = "Contractor";
                    sheetcreateSummary.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var contract = await _contractService.FindContractByPackageIdAsync(workPlanPackage.ID).ConfigureAwait(false);
                    if (contract.Success)
                    {

                        sheetcreateSummary.Cells[4, 2].Value = contract.Contract.Contractor.Name;
                        sheetcreateSummary.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {

                        sheetcreateSummary.Cells[4, 2].Value = "";
                        sheetcreateSummary.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    sheetcreateSummary.Cells[1, 3].Value = "";
                    sheetcreateSummary.Cells[1, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[1, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[2, 3].Value = "";
                    sheetcreateSummary.Cells[2, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[3, 3].Value = "";
                    sheetcreateSummary.Cells[3, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[4, 3].Value = "";
                    sheetcreateSummary.Cells[4, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //First Row
                    sheetcreateSummary.Cells[5, 1].Value = "BILL OF QUANTITIES";
                    sheetcreateSummary.Cells[5, 1].Style.Font.Bold = true;
                    // sheetcreate.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    //Header for the line entries
                    sheetcreateSummary.Cells[6, 2].Value = "Summary";
                    sheetcreateSummary.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[6, 3].Value = "Project : " + workPlanPackage.Name;
                    sheetcreateSummary.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //create now the record items
                    //print headers
                    sheetcreateSummary.Cells[5, 1].Value = "ITEM NO.";
                    sheetcreateSummary.Cells[5, 1].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[5, 2].Value = "DESCRIPTION";
                    sheetcreateSummary.Cells[5, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateSummary.Cells[5, 3].Value = "AMOUNT (KSH)";
                    sheetcreateSummary.Cells[5, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    var subTotal = 0.0;
                    int row = 6;
                    var billGroupSummaryList = new List<BillGroupSummary>();

                    if (workPlanPackage.WorkpackageRoadWorkSectionPlans != null)
                    {
                        foreach (var roadSectionPlan in workPlanPackage.WorkpackageRoadWorkSectionPlans)
                        {
                            if (roadSectionPlan.RoadWorkSectionPlan != null)
                            {
                                foreach (var plan in roadSectionPlan.RoadWorkSectionPlan.PlanActivities)
                                {

                                    billGroupSummaryList.Add(
                                        new BillGroupSummary()
                                        {
                                            BillNumber = plan.ItemActivityUnitCost.ItemActivityGroup.BillNumber,
                                            Description = plan.ItemActivityUnitCost.ItemActivityGroup.Description,
                                            Amount = plan.PackageAmount
                                        }
                                    );
                                }
                            }
                        }
                    }

                    var groupedSummary = billGroupSummaryList.GroupBy(x => new { x.BillNumber, x.Description }).Select(x => new
                    {
                        BillNumber = x.Key.BillNumber,
                        Description = x.Key.Description,
                        SumAmount = x.Sum(y => y.Amount)
                    }).ToList();

                    foreach (var plan in groupedSummary)
                    {

                        sheetcreateSummary.Cells[row, 1].Value = plan.BillNumber;
                        //sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheetcreateSummary.Cells[row, 2].Value = plan.Description;
                        //sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;



                        sheetcreateSummary.Cells[row, 3].Value = plan.SumAmount.ToString("N");
                        // sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateSummary.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateSummary.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        row++;

                        subTotal += plan.SumAmount;

                    }


                    sheetcreateSummary.Cells[row + 2, 2].Value = "Sub Total";
                    sheetcreateSummary.Cells[row + 2, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 2, 3].Value = subTotal.ToString("N");
                    sheetcreateSummary.Cells[row + 2, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreateSummary.Cells[row + 4, 2].Value = "VAT @ 16 % ";
                    //sheetcreate.Cells[row + 4, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 4, 3].Value = (subTotal * 0.16).ToString("N");
                    // sheetcreate.Cells[row + 4, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 5, 2].Value = "Total ";
                    sheetcreateSummary.Cells[row + 5, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 5, 3].Value = (subTotal * 1.16).ToString("N");
                    sheetcreateSummary.Cells[row + 5, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreateSummary.Cells[row + 6, 2].Value = "Contigencies ( @ 0% ) ";
                    //sheetcreate.Cells[row + 6, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 6, 3].Value = 0.0;
                    sheetcreateSummary.Cells[row + 6, 3].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells[row + 6, 2].Value = "Carried to page on the form of Tender ";
                    sheetcreateSummary.Cells[row + 6, 2].Style.Font.Bold = true;
                    sheetcreateSummary.Cells[row + 6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateSummary.Cells[row + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreateSummary.Cells.AutoFitColumns();
                    //END BOQ SUMMARY DETAILS



                    excel.Workbook.Properties.Title = "Attempts";
                    // excel.Save();
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "WorkPlanPackage", new { fileGuid = handle, FileName = "BOQ.xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> DownloadContractBidDetails(long workpackageId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                //retrieve the full workpackage details from database
                var workpackageResp = await _workPlanPackageService.FindByIdAsync(workpackageId).ConfigureAwait(false);
                var workPlanPackage = workpackageResp.WorkPlanPackage;

                //get the current financial year
                var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (financialYearResp.Success)
                    printData.FinancialYear = financialYearResp.FinancialYear;

                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(workPlanPackage.AuthorityId).ConfigureAwait(false);
                var authority = authorityResp.Authority;
                printData.Authority = authority;


                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    var sheetcreate = excel.Workbook.Worksheets.Add("BOQ");
                    //print the column headers
                    //First Row
                    sheetcreate.Cells[1, 1].Value = "CONTRACT NAME ";
                    sheetcreate.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[1, 2].Value = workPlanPackage.Name;
                    sheetcreate.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //REGION
                    sheetcreate.Cells[1, 7].Value = "REGION ";
                    sheetcreate.Cells[1, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[1, 8].Value = "";
                    sheetcreate.Cells[1, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //FINANCIAL YEAR

                    sheetcreate.Cells[2, 7].Value = "FINANCIAL YEAR";
                    sheetcreate.Cells[2, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[2, 8].Value = printData.FinancialYear.Code;
                    sheetcreate.Cells[2, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    //second row
                    sheetcreate.Cells[2, 1].Value = "BILL OF QUANTITIES ";
                    sheetcreate.Cells[2, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    //third row
                    sheetcreate.Cells[3, 1].Value = "ACTIVITY GROUP TITLE ";
                    sheetcreate.Cells[3, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[3, 2].Value = "";
                    sheetcreate.Cells[3, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreate.Cells[4, 1].Value = "CONTRACTOR ";
                    sheetcreate.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var contract = await _contractService.FindContractByPackageIdAsync(workPlanPackage.ID).ConfigureAwait(false);
                    if (contract.Success)
                    {
                        sheetcreate.Cells[4, 2].Value = contract.Contract.Contractor.Name;
                        sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {
                        sheetcreate.Cells[4, 2].Value = "";
                        sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }



                    //create now the record items
                    //print headers
                    sheetcreate.Cells[6, 1].Value = "BILL NO.";
                    sheetcreate.Cells[6, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 2].Value = "ITEM CODE";
                    sheetcreate.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 3].Value = "DESCRIPTION";
                    sheetcreate.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 4].Value = "QUANTITY";
                    sheetcreate.Cells[6, 4].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 5].Value = "UNIT";
                    sheetcreate.Cells[6, 5].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 6].Value = "UNIT BID RATE";
                    sheetcreate.Cells[6, 6].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 7].Value = "BILL ITEM COST WITHOUT VAT";
                    sheetcreate.Cells[6, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 8].Value = "TECHNOLOGY";
                    sheetcreate.Cells[6, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int row = 7;


                    if (workPlanPackage.WorkpackageRoadWorkSectionPlans != null)
                    {
                        foreach (var roadSectionPlan in workPlanPackage.WorkpackageRoadWorkSectionPlans)
                        {
                            if (roadSectionPlan.RoadWorkSectionPlan != null)
                            {

                                foreach (var plan in roadSectionPlan.RoadWorkSectionPlan.PlanActivities)
                                {
                                    sheetcreate.Cells[row, 1].Value = plan.ItemActivityUnitCost.ItemActivityGroup.BillNumber;
                                    //sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[row, 2].Value = plan.ItemActivityUnitCost.ItemCode;
                                    //sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[row, 3].Value = plan.ItemActivityUnitCost.Description;
                                    //sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                                    sheetcreate.Cells[row, 4].Value = plan.PackageQuantity;
                                    //sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.UnitMeasure;
                                    //sheetcreate.Cells[row, 5].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                                    sheetcreate.Cells[row, 6].Value = plan.BidRate;
                                    //sheetcreate.Cells[row, 6].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[row, 7].Value = plan.BillItemAmount.ToString("N");
                                    //sheetcreate.Cells[row, 7].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    sheetcreate.Cells[row, 8].Value = plan.Technology.Code;
                                    //sheetcreate.Cells[row, 8].Style.Font.Bold = true;
                                    sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    row++;

                                }
                                sheetcreate.Cells.AutoFitColumns();
                            }
                        }
                    }

                    //sheetcreate.Cells.AutoFitColumns();

                    excel.Workbook.Properties.Title = "Attempts";
                    // excel.Save();
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "WorkPlanPackage", new { fileGuid = handle, FileName = "BOQ.xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> DownloadContractBidSummary(long workpackageId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                //retrieve the full workpackage details from database
                var workpackageResp = await _workPlanPackageService.FindByIdAsync(workpackageId).ConfigureAwait(false);
                var workPlanPackage = workpackageResp.WorkPlanPackage;

                //get the current financial year
                var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (financialYearResp.Success)
                    printData.FinancialYear = financialYearResp.FinancialYear;

                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(workPlanPackage.AuthorityId).ConfigureAwait(false);
                var authority = authorityResp.Authority;
                printData.Authority = authority;


                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    var sheetcreate = excel.Workbook.Worksheets.Add("BOQ Summary");
                    //print the column headers
                    //First Row cONTRACTOR
                    sheetcreate.Cells[1, 1].Value = "Road Code";
                    sheetcreate.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[1, 2].Value = workPlanPackage.WorkpackageRoadWorkSectionPlans.SingleOrDefault().RoadWorkSectionPlan.Road.RoadNumber;
                    sheetcreate.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //First Row cONTRACTOR
                    sheetcreate.Cells[2, 1].Value = "Section Name";
                    sheetcreate.Cells[2, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[2, 2].Value = workPlanPackage.WorkpackageRoadWorkSectionPlans.SingleOrDefault().RoadWorkSectionPlan.RoadSection.SectionName;
                    sheetcreate.Cells[2, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[3, 1].Value = "Package Name";
                    sheetcreate.Cells[3, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[3, 2].Value = workPlanPackage.Name;
                    sheetcreate.Cells[3, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[4, 1].Value = "Contractor";
                    sheetcreate.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var contract = await _contractService.FindContractByPackageIdAsync(workPlanPackage.ID).ConfigureAwait(false);
                    if (contract.Success)
                    {

                        sheetcreate.Cells[4, 2].Value = contract.Contract.Contractor.Name;
                        sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {

                        sheetcreate.Cells[4, 2].Value = "";
                        sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    sheetcreate.Cells[1, 3].Value = "";
                    sheetcreate.Cells[1, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[2, 3].Value = "";
                    sheetcreate.Cells[2, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[3, 3].Value = "";
                    sheetcreate.Cells[3, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[4, 3].Value = "";
                    sheetcreate.Cells[4, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //First Row
                    sheetcreate.Cells[5, 1].Value = "BILL OF QUANTITIES";
                    sheetcreate.Cells[5, 1].Style.Font.Bold = true;
                    // sheetcreate.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    //Header for the line entries
                    sheetcreate.Cells[6, 2].Value = "Summary";
                    sheetcreate.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 3].Value = "Project : " + workPlanPackage.Name;
                    sheetcreate.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //create now the record items
                    //print headers
                    sheetcreate.Cells[5, 1].Value = "ITEM NO.";
                    sheetcreate.Cells[5, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[5, 2].Value = "DESCRIPTION";
                    sheetcreate.Cells[5, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[5, 3].Value = "AMOUNT (KSH)";
                    sheetcreate.Cells[5, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    var subTotal = 0.0;
                    int row = 6;
                    var billGroupSummaryList = new List<BillGroupSummary>();

                    if (workPlanPackage.WorkpackageRoadWorkSectionPlans != null)
                    {
                        foreach (var roadSectionPlan in workPlanPackage.WorkpackageRoadWorkSectionPlans)
                        {
                            if (roadSectionPlan.RoadWorkSectionPlan != null)
                            {
                                foreach (var plan in roadSectionPlan.RoadWorkSectionPlan.PlanActivities)
                                {

                                    billGroupSummaryList.Add(
                                        new BillGroupSummary()
                                        {
                                            BillNumber = plan.ItemActivityUnitCost.ItemActivityGroup.BillNumber,
                                            Description = plan.ItemActivityUnitCost.ItemActivityGroup.Description,
                                            Amount = plan.BillItemAmount
                                        }
                                    );
                                }
                            }
                        }
                    }

                    var groupedSummary = billGroupSummaryList.GroupBy(x => new { x.BillNumber, x.Description }).Select(x => new
                    {
                        BillNumber = x.Key.BillNumber,
                        Description = x.Key.Description,
                        SumAmount = x.Sum(y => y.Amount)
                    }).ToList();

                    foreach (var plan in groupedSummary)
                    {

                        sheetcreate.Cells[row, 1].Value = plan.BillNumber;
                        //sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheetcreate.Cells[row, 2].Value = plan.Description;
                        //sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;



                        sheetcreate.Cells[row, 3].Value = plan.SumAmount.ToString("N");
                        // sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        row++;

                        subTotal += plan.SumAmount;

                    }


                    sheetcreate.Cells[row + 2, 2].Value = "Sub Total";
                    sheetcreate.Cells[row + 2, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[row + 2, 3].Value = subTotal.ToString("N");
                    sheetcreate.Cells[row + 2, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreate.Cells[row + 4, 2].Value = "VAT @ 16 % ";
                    //sheetcreate.Cells[row + 4, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[row + 4, 3].Value = (subTotal * 0.16).ToString("N");
                    // sheetcreate.Cells[row + 4, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[row + 5, 2].Value = "Total ";
                    sheetcreate.Cells[row + 5, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[row + 5, 3].Value = (subTotal * 1.16).ToString("N");
                    //sheetcreate.Cells[row + 5, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    sheetcreate.Cells[row + 6, 2].Value = "Contigencies ( @ 0% ) ";
                    //sheetcreate.Cells[row + 6, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[row + 6, 3].Value = 0.0;
                    //sheetcreate.Cells[row + 6, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[row + 6, 2].Value = "Carried to page on the form of Tender ";
                    sheetcreate.Cells[row + 6, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells.AutoFitColumns();


                    excel.Workbook.Properties.Title = "Attempts";
                    // excel.Save();
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "WorkPlanPackage", new { fileGuid = handle, FileName = "BOQ.xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

   
        public ActionResult Download(string fileGuid, string fileName)
        {

            if (_cache.Get<byte[]>(fileGuid) != null)
            {
                byte[] data = _cache.Get<byte[]>(fileGuid);
                _cache.Remove(fileGuid); //cleanup here as we don't need it in cache anymore
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Something has gone wrong...
                return View("Error"); // or whatever/wherever you want to return the user
            }
        }

        //employement details
        public async Task<IActionResult> EditEmploymentDetails(long contractId, long employmentId)
        {
            var viewModel = new EmploymentDetail();
            if(employmentId > 0)
            {//is an edit request
                //retrieve the record
                var employmentRecordRes = await _employmentDetailService.FindByIdAsync(employmentId).ConfigureAwait(false);
                if(employmentRecordRes.Success)
                {
                    viewModel = employmentRecordRes.EmploymentDetail;
                }
            }
            else
            {//is a new request
                var contractResp = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
                if (contractResp.Success)
                {
                    //get the most recent quarter reported.
                    var employmentReportingCount = contractResp.Contract.EmploymentDetails.Count();
                    viewModel.Period = employmentReportingCount;
                    viewModel.Contract = contractResp.Contract;
                    if(viewModel.Period == 4)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "All the 4 reporting periods have been reported."
                        });
                    }
                }

            }

            return PartialView("ContractEmploymentDetailsEdit", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditEmploymentDetails(EmploymentDetail employmentDetail)
        {
            var user = await GetLoggedInUser().ConfigureAwait(false);

            if (employmentDetail != null)
            {
                if(employmentDetail.ID > 0)
                {
                    //old record, edit
                    employmentDetail.UpdateBy = user.UserName;
                    employmentDetail.UpdateDate = DateTime.UtcNow;

                    var updateResp = await _employmentDetailService.UpdateAsync(employmentDetail).ConfigureAwait(false);
                    if (updateResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success.",
                            Href = Url.Action("ProjectProgressDetails", "WorkPlanPackage", new { ID = updateResp.EmploymentDetail.ContractId })
                        });
                    }
                    return Json(new
                    {
                        Success = false,
                        Message = "Record update failed, please contact the administrator"
                    });
                }
                else
                {
                    //new record
                    
                    EmploymentDetail employmentDetailNew = new EmploymentDetail
                    {
                        MaleCount = employmentDetail.MaleCount,
                        FemaleCount = employmentDetail.FemaleCount,
                        MaleMandays = employmentDetail.MaleMandays,
                        FemaleMandays = employmentDetail.FemaleMandays,
                        ContractId = employmentDetail.Contract.ID,
                        Period = (employmentDetail.Period + 1),
                        PreviousPeriod = employmentDetail.Period,
                        CreatedBy = user.UserName,
                        CreationDate = DateTime.UtcNow
                    };
                      
                    var addRecordResp = await _employmentDetailService.AddAsync(employmentDetailNew).ConfigureAwait(false);
                    if (addRecordResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success.",
                            Href = Url.Action("ProjectProgressDetails", "WorkPlanPackage", new { ID = addRecordResp.EmploymentDetail.ContractId})
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Record update failed, please contact the administrator"
                        });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid record submitted for update"
                });
            }
        }
        //Contract details
        public async Task<IActionResult> EditPackageContractDetails(long contractId)
        {
            var viewModel = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
            return PartialView("ContractDetailsEdit", viewModel.Contract);
        }

        [HttpPost]
        public async Task<IActionResult> EditPackageContractDetails(Contract contract)
        {

            if (contract != null)
            {

                var existingRecord = await _contractService.FindByIdAsync(contract.ID).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    var user = await GetLoggedInUser().ConfigureAwait(false);
                    //update the existing record
                    existingRecord.Contract.UpdateBy = user.UserName;
                    existingRecord.Contract.UpdateDate = DateTime.UtcNow;
                    //records updated from form
                    existingRecord.Contract.PerformanceBond = contract.PerformanceBond;
                    existingRecord.Contract.PercentageRetention = contract.PercentageRetention;
                    existingRecord.Contract.AdvancePayment = contract.AdvancePayment;
                    existingRecord.Contract.AdvanceClearance = contract.AdvanceClearance;
                    existingRecord.Contract.inPaymentCertificate = contract.inPaymentCertificate;
                    existingRecord.Contract.ContractTaxable = contract.ContractTaxable;
                    existingRecord.Contract.ContractIsSigned = contract.ContractIsSigned;
                    existingRecord.Contract.ContractStartDate = contract.ContractStartDate;
                    existingRecord.Contract.ContractEndDate = contract.ContractEndDate;

                    var updateResp = await _contractService.UpdateAsync(existingRecord.Contract).ConfigureAwait(false);
                    if (updateResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success.",
                            Href = Url.Action("ContractDetails", "WorkPlanPackage", new { ID = contract.ID })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Record update failed, please contact the administrator"
                        });
                    }


                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the ID"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid record submitted for update"
                });
            }
        }

        //Contract contractor details
        public async Task<IActionResult> EditPackageContractorDetails(long contractId)
        {
            var viewModel = new WorkpackageContractorSelectList();
            viewModel.contractId = contractId;
            viewModel.Contractors = await _contractorService.ListAsync().ConfigureAwait(false);
            return PartialView("ContractorListPartialViewSelect", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPackageContractorDetails(long contractorId, long contractId)
        {

            if (contractorId > 0 && contractId > 0)
            {

                var existingRecord = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    var existingContractor = await _contractorService.FindByIdAsync(contractorId).ConfigureAwait(false);
                    if (existingContractor.Success)
                    {
                        var user = await GetLoggedInUser().ConfigureAwait(false);
                        //update the existing record
                        existingRecord.Contract.UpdateBy = user.UserName;
                        existingRecord.Contract.UpdateDate = DateTime.UtcNow;
                        //records updated from form
                        existingRecord.Contract.ContractorId = contractorId;

                        var updateResp = await _contractService.UpdateAsync(existingRecord.Contract).ConfigureAwait(false);
                        if (updateResp.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success.",
                                Href = Url.Action("ContractDetails", "WorkPlanPackage", new { ID = contractId })
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Record update failed, please contact the administrator"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "No record for the supplied contractor, please contact the administrator"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the contract ID"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid record submitted for update"
                });
            }
        }

        //Progress activity reporting on quantity
        public async Task<IActionResult> EditProgressQuantity(long activityId, long workpackageId, int editFlag)
        {
            var viewModel = new PackageQuantityEditViewModel();
            if (activityId > 0)
            {
                var sectionPlanActivityResponse = await _planActivityService.FindByIdAsync(activityId).ConfigureAwait(false);
                if (sectionPlanActivityResponse.Success)
                    viewModel.PlanActivity = sectionPlanActivityResponse.PlanActivity;
            }
            else
            {
                viewModel.PlanActivity = new PlanActivity();
            }
            viewModel.workPackageId = workpackageId;
            viewModel.EditFlag = editFlag;

            return PartialView("WorkPackageProgressQuantityEdit", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditProgressQuantity(PackageQuantityEditViewModel packageQuantityEditViewModel)
        {
            if (packageQuantityEditViewModel != null)
            {
                if (packageQuantityEditViewModel.PlanActivity != null)
                {
                    if (packageQuantityEditViewModel.PlanActivity.ID > 0 && packageQuantityEditViewModel.workPackageId > 0)
                    {
                        var user = await GetLoggedInUser().ConfigureAwait(false);

                        //get the workpackage first
                        var existingPackageSuccess = await _workPlanPackageService.FindByIdAsync(packageQuantityEditViewModel.workPackageId).ConfigureAwait(false);
                        if (existingPackageSuccess.Success)
                        {
                            //retrieve the plan activity and update

                            var existingRecord = await _planActivityService.FindByIdAsync(packageQuantityEditViewModel.PlanActivity.ID).ConfigureAwait(false);
                            if (existingRecord.Success)
                            {
                                //check if is editing or new record
                                if (packageQuantityEditViewModel.EditFlag == 0) // add
                                {
                                    var thisMonth = DateTime.Now.Month;
                                    var previousMonth = DateTime.Now.AddMonths(-1).Month;

                                    //make an entry of progress report into the database.
                                    PackageProgressEntry packageProgressEntry = new PackageProgressEntry();
                                    packageProgressEntry.Quantity = packageQuantityEditViewModel.ProgressQuantity;
                                    packageProgressEntry.PlanActivityId = packageQuantityEditViewModel.PlanActivity.ID;
                                    packageProgressEntry.Rate = existingRecord.PlanActivity.BidRate;
                                    packageProgressEntry.ReportingMonth = thisMonth;
                                    packageProgressEntry.CreationDate = DateTime.UtcNow;
                                    packageProgressEntry.CreatedBy = user.UserName;

                                    await _packageProgressEntryService.AddAsync(packageProgressEntry).ConfigureAwait(false);

                                    //previous quantity is all the entries entered in the previous month for the plan item

                                    var previousActivities = existingRecord.PlanActivity.PackageProgressEntries.Where(d => (d.CreationDate.Month == previousMonth));
                                    var previousActivitiesQty = previousActivities.Sum(s => s.Quantity);


                                    var thisMonthActivities = existingRecord.PlanActivity.PackageProgressEntries.Where(d => (d.CreationDate.Month == thisMonth));
                                    var thisMonthActivitiesQty = thisMonthActivities.Sum(s => s.Quantity);

                                    var toDateQty = existingRecord.PlanActivity.PackageProgressEntries.Sum(s => s.Quantity);

                                    existingRecord.PlanActivity.ThisQuantity = thisMonthActivitiesQty;
                                    existingRecord.PlanActivity.PreviousQuantity = previousActivitiesQty;
                                    existingRecord.PlanActivity.DoneTodateQuantity = toDateQty;

                                }
                                else
                                {
                                    var variance = (existingRecord.PlanActivity.PreviousQuantity - packageQuantityEditViewModel.ProgressQuantity);
                                    existingRecord.PlanActivity.ThisQuantity = 0;
                                    existingRecord.PlanActivity.PreviousQuantity = packageQuantityEditViewModel.ProgressQuantity;
                                    existingRecord.PlanActivity.DoneTodateQuantity -= variance;

                                }


                                existingRecord.PlanActivity.UpdateBy = user.UserName;
                                existingRecord.PlanActivity.UpdateDate = DateTime.UtcNow;


                                //save
                                var updateRecord = await _planActivityService.UpdateAsync(existingRecord.PlanActivity).ConfigureAwait(false);
                                if (updateRecord.Success)
                                {
                                    //reset the engineer's total in the package quantity

                                    return Json(new
                                    {
                                        Success = true,
                                        Message = "Success"//,
                                                           //Href = Url.Action("RoadWorkPackaging", "WorkPlanPackage", new { id = workPlanPackage.ID })
                                    });
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Message = "Update on work package failed"
                                    });
                                }
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Invalid package supplied"
                            });
                        }

                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid activity supplied"
                        });
                    }
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid activity supplied"
                });
            }

            return PartialView("WorkPackageQuantityEdit", packageQuantityEditViewModel);
        }

        public async Task<IActionResult> DownloadMonthlyProgressSummary(long contractId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                //retrieve the full workpackage details from database
                var workpackageResp = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
                var workPlanPackage = workpackageResp.Contract.WorkPlanPackage;

                //get the current financial year
                var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (financialYearResp.Success)
                    printData.FinancialYear = financialYearResp.FinancialYear;

                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(workPlanPackage.AuthorityId).ConfigureAwait(false);
                var authority = authorityResp.Authority;
                printData.Authority = authority;


                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    var sheetcreate = excel.Workbook.Worksheets.Add("MonthlyProgress");
                    //print the column headers
                    //First Row
                    sheetcreate.Cells[2, 2].Value = "Details of Monthly Progress Report With Road Activities ";
                    sheetcreate.Cells["B2:T2"].Merge = true;
                    sheetcreate.Cells["B2:T2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheetcreate.Cells["B2:T2"].Style.Font.Bold = true;
                    // sheetcreate.Cells["B2:T2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);



                    //REGION
                    sheetcreate.Cells[4, 2].Value = "REGION ";
                    sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //FINANCIAL YEAR

                    sheetcreate.Cells[4, 9].Value = "FISCAL YEAR";
                    sheetcreate.Cells[4, 9].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[4, 10].Value = printData.FinancialYear.Code;
                    sheetcreate.Cells[4, 10].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //REPORTING MONTH

                    sheetcreate.Cells[4, 18].Value = "REPORTING MONTH";
                    sheetcreate.Cells["R18:S19"].Merge = true;
                    sheetcreate.Cells["R18:S19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreate.Cells["R18:S19"].Style.Font.Bold = true;

                    sheetcreate.Cells[4, 20].Value = DateTime.UtcNow.Month;
                    sheetcreate.Cells[4, 20].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    //Table row headers
                    sheetcreate.Cells[5, 2].Value = "Package No ";
                    sheetcreate.Cells["B5:B6"].Merge = true;
                    sheetcreate.Cells["B5:B6"].Style.Font.Bold = true;
                    sheetcreate.Cells["B5:B6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["B5:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    //third row
                    sheetcreate.Cells[5, 3].Value = "Link ID ";
                    sheetcreate.Cells["C5:C6"].Merge = true;
                    sheetcreate.Cells["C5:C6"].Style.Font.Bold = true;
                    sheetcreate.Cells["C5:C6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["C5:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 4].Value = "Road Code ";
                    sheetcreate.Cells["D5:D6"].Merge = true;
                    sheetcreate.Cells["D5:D6"].Style.Font.Bold = true;
                    sheetcreate.Cells["D5:D6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["D5:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 5].Value = "Section Length";

                    sheetcreate.Cells["E5:E6"].Merge = true;
                    sheetcreate.Cells["E5:E6"].Style.Font.Bold = true;
                    sheetcreate.Cells["E5:E6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["E5:E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 6].Value = "Item Code";
                    sheetcreate.Cells["F5:F6"].Merge = true;
                    sheetcreate.Cells["F5:F6"].Style.Font.Bold = true;
                    sheetcreate.Cells["F5:F6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["F5:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 7].Value = "Description ";
                    sheetcreate.Cells["G5:G6"].Merge = true;
                    sheetcreate.Cells["G5:G6"].Style.Font.Bold = true;
                    sheetcreate.Cells["G5:G6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["G5:G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 8].Value = "Start Chainage";
                    sheetcreate.Cells["H5:H6"].Merge = true;
                    sheetcreate.Cells["H5:H6"].Style.Font.Bold = true;
                    sheetcreate.Cells["H5:H6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["H5:H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 9].Value = "End Chainage";
                    sheetcreate.Cells["I5:I6"].Merge = true;
                    sheetcreate.Cells["I5:I6"].Style.Font.Bold = true;
                    sheetcreate.Cells["I5:I6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["I5:I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 10].Value = "Unit";
                    sheetcreate.Cells["J5:J6"].Merge = true;
                    sheetcreate.Cells["J5:J6"].Style.Font.Bold = true;
                    sheetcreate.Cells["J5:J6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["J5:J6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 11].Value = "Quantity";
                    sheetcreate.Cells["K5:K6"].Merge = true;
                    sheetcreate.Cells["K5:K6"].Style.Font.Bold = true;
                    sheetcreate.Cells["K5:K6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["K5:K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;             //third row

                    sheetcreate.Cells[5, 12].Value = "Bid Rate";
                    sheetcreate.Cells["L5:L6"].Merge = true;
                    sheetcreate.Cells["L5:L6"].Style.Font.Bold = true;
                    sheetcreate.Cells["L5:L6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["L5:L6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 13].Value = "Bill Item Amount (Kshs) Without VAT";
                    sheetcreate.Cells["M5:M6"].Merge = true;
                    sheetcreate.Cells["M5:M6"].Style.Font.Bold = true;
                    sheetcreate.Cells["M5:M6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["M5:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 14].Value = "Till Previous Month";
                    sheetcreate.Cells["N5:O5"].Merge = true;
                    sheetcreate.Cells["N5:O5"].Style.Font.Bold = true;
                    sheetcreate.Cells["N5:O5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["N5:O5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 14].Value = "Quantity";
                    sheetcreate.Cells[6, 14].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[6, 15].Value = "%";
                    sheetcreate.Cells[6, 15].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 16].Value = "This Month";
                    sheetcreate.Cells["P5:Q5"].Merge = true;
                    sheetcreate.Cells["P5:Q5"].Style.Font.Bold = true;
                    sheetcreate.Cells["P5:Q5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["P5:Q5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 16].Value = "Quantity";
                    sheetcreate.Cells[6, 16].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[6, 17].Value = "%";
                    sheetcreate.Cells[6, 17].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 18].Value = "Done To Date";
                    sheetcreate.Cells["R5:S5"].Merge = true;
                    sheetcreate.Cells["R5:S5"].Style.Font.Bold = true;
                    sheetcreate.Cells["R5:S5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["R5:S5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetcreate.Cells[6, 18].Value = "Quantity";
                    sheetcreate.Cells[6, 18].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[6, 19].Value = "%";
                    sheetcreate.Cells[6, 19].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 19].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //third row
                    sheetcreate.Cells[5, 20].Value = "UptoDateValue Of Work Without VAT";
                    sheetcreate.Cells["T5:T6"].Merge = true;
                    sheetcreate.Cells["T5:T6"].Style.Font.Bold = true;
                    sheetcreate.Cells["T5:T6"].Style.WrapText = true;
                    sheetcreate.Cells["T5:T6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells["T5:T6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;



                    int row = 7;

                    sheetcreate.Cells[row, 2].Value = workPlanPackage.Code;
                    sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    if (workPlanPackage.WorkpackageRoadWorkSectionPlans != null)
                    {
                        foreach (var roadSectionPlan in workPlanPackage.WorkpackageRoadWorkSectionPlans)
                        {

                            sheetcreate.Cells[row, 3].Value = roadSectionPlan.RoadWorkSectionPlan.RoadSection.SectionName;
                            sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                            sheetcreate.Cells[row, 4].Value = roadSectionPlan.RoadWorkSectionPlan.Road.RoadName;
                            sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            sheetcreate.Cells[row, 5].Value = roadSectionPlan.RoadWorkSectionPlan.RoadSection.Length.ToString("N");
                            sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            if (roadSectionPlan.RoadWorkSectionPlan != null)
                            {

                                foreach (var plan in roadSectionPlan.RoadWorkSectionPlan.PlanActivities)
                                {
                                    sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.ItemCode;
                                    sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 7].Value = plan.ItemActivityUnitCost.Description;
                                    sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 8].Value = plan.StartChainage.ToString("N");
                                    sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 9].Value = plan.EndChainage.ToString("N");
                                    sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 10].Value = plan.ItemActivityUnitCost.UnitCode;
                                    sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 11].Value = plan.PackageQuantity.ToString("N");
                                    sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 12].Value = plan.BidRate;
                                    sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 13].Value = plan.BillItemAmount;
                                    sheetcreate.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 14].Value = plan.PreviousQuantity.ToString("N");
                                    sheetcreate.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 15].Value = (plan.PreviousQuantity * 100 / plan.PackageQuantity).ToString("N");
                                    sheetcreate.Cells[row, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 16].Value = plan.ThisQuantity.ToString("N");
                                    sheetcreate.Cells[row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 17].Value = (plan.ThisQuantity * 100 / plan.PackageQuantity).ToString("N");
                                    sheetcreate.Cells[row, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 18].Value = plan.DoneTodateQuantity.ToString("N");
                                    sheetcreate.Cells[row, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 19].Value = (plan.DoneTodateQuantity * 100 / plan.PackageQuantity).ToString("N");
                                    sheetcreate.Cells[row, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells[row, 20].Value = (plan.DoneTodateQuantity * plan.BidRate).ToString("N");
                                    sheetcreate.Cells[row, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    sheetcreate.Cells.AutoFitColumns();

                                    row++;

                                }
                            }
                            sheetcreate.Cells.AutoFitColumns();
                        }
                    }

                    sheetcreate.Cells.AutoFitColumns();

                    excel.Workbook.Properties.Title = "Attempts";
                    // excel.Save();
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "WorkPlanPackage", new { fileGuid = handle, FileName = "MonthlyProgress.xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> DownloadQuarterlyProgressSummary(long authorityId, long financialYearId)
        {
            try
            {
                //retrieve the records to be printed
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                //retrieve the full workpackage details from database

                //get the current financial year
                if(financialYearId == 0)
                {
                    var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (!financialYearResp.Success)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Financial year supplied is invalid."
                        });
                    }
                    printData.FinancialYear = financialYearResp.FinancialYear;
                }
                else
                {
                    var financialYearResp = await _financialYearService.FindByIdAsync(financialYearId).ConfigureAwait(false);
                    if (!financialYearResp.Success)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Financial year supplied is invalid."
                        });
                    }
                    printData.FinancialYear = financialYearResp.FinancialYear;
                }
               
              

                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(authorityId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Authority supplied is invalid."
                    });
                }

                var authority = authorityResp.Authority;
                printData.Authority = authority;

                //retrieve the report data
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(printData.Authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                if (!authorityWorkplans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "There are no records for the selected county / agency in the selected financial year that could be retrieved."
                    });
                }
                printData.RoadWorkSectionPlans = authorityWorkplans.ToList();

                var agenctContracts = await _contractService.ListContractsByAgencyByFinancialYear(printData.Authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                if (!agenctContracts.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "There are no contract records for the selected county / agency in the selected financial year that could be retrieved."
                    });
                }

                //get the agency expenditure entries
                var agancyProgress = await _financialProgressService.ListByAuthorityIdAndFinancialYearAsync(printData.Authority.ID, printData.FinancialYear.ID);

                //generate the file.
                string fileName = printData.Authority.Code.ToUpper() + "_" + printData.FinancialYear.Code + "_QuarterlyProgress.xlsx";
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    var reportFormSheet = excel.Workbook.Worksheets.Add("PROGRESS REPORT FORM");
                    var financialSummartSheet = excel.Workbook.Worksheets.Add("FINACIAL SUMMARY");
                    //REPORT FORM BEGINS HERE
                    reportFormSheet.Cells["A1:G1"].Value = "QUARTERLY ROAD WORKS PROGRESS REPORT ";
                    reportFormSheet.Cells["A1:G1"].Merge = true;
                    reportFormSheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    reportFormSheet.Cells["A1:G1"].Style.Font.Bold = true;
                    reportFormSheet.Cells["A1:G1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if(printData.Authority.Type == 2)
                    {
                        reportFormSheet.Cells["A2: C2"].Value = "NAME OF COUNTY GOVERNMENT";
                    }
                    else
                    {
                        reportFormSheet.Cells["A2:C2"].Value = "NAME OF ROAD AGENCY ";
                    }
                    reportFormSheet.Cells["A2:C2"].Merge = true;
                    reportFormSheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    reportFormSheet.Cells["A2:C2"].Style.Font.Bold = true;
                    reportFormSheet.Cells["A2:C2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["D2:G2"].Value = printData.Authority.Name.ToUpper();
                    reportFormSheet.Cells["D2:G2"].Merge = true;
                    reportFormSheet.Cells["D2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    reportFormSheet.Cells["D2:G2"].Style.Font.Bold = true;
                    reportFormSheet.Cells["D2:G2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if (printData.Authority.Type == 2)
                    {
                        reportFormSheet.Cells["A3: C3"].Value = "COUNTY CODE";
                    }
                    else
                    {
                        reportFormSheet.Cells["A3:C3"].Value = "ROAD AGENCY CODE";
                    }
                    reportFormSheet.Cells["A3:C3"].Merge = true;
                    reportFormSheet.Cells["A3:C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    reportFormSheet.Cells["A3:C3"].Style.Font.Bold = true;
                    reportFormSheet.Cells["A3:C3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["D3:G3"].Value = printData.Authority.Code.ToUpper();
                    reportFormSheet.Cells["D3:G3"].Merge = true;
                    reportFormSheet.Cells["D3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    reportFormSheet.Cells["D3:G3"].Style.Font.Bold = true;
                    reportFormSheet.Cells["D3:G3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["A4:C4"].Value = "BUDGET (KSHS)";
                    reportFormSheet.Cells["A4:C4"].Merge = true;
                    reportFormSheet.Cells["A4:C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    reportFormSheet.Cells["A4:C4"].Style.Font.Bold = true;
                    reportFormSheet.Cells["A4:C4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["D4:G4"].Value = "0.00";
                    reportFormSheet.Cells["D4:G4"].Merge = true;
                    reportFormSheet.Cells["D4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    reportFormSheet.Cells["D4:G4"].Style.Font.Bold = true;
                    reportFormSheet.Cells["D4:G4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    //Output the report road list

                    reportFormSheet.Cells["A6:R6"].Value = "ROAD AGENCY  PROGRESS REPORT SHEET.";
                    reportFormSheet.Cells["A6:R6"].Merge = true;
                    reportFormSheet.Cells["A6:R6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["A6:R6"].Style.Font.Bold = true;
                    reportFormSheet.Cells["A6:R6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["A7:H8"].Value = "PLANNED WORKS.";
                    reportFormSheet.Cells["A7:H8"].Merge = true;
                    reportFormSheet.Cells["A7:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["A7:H8"].Style.Font.Bold = true;
                    reportFormSheet.Cells["A7:H8"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["I7:P7"].Value = "ACHIEVED WORKS.";
                    reportFormSheet.Cells["I7:R7"].Merge = true;
                    reportFormSheet.Cells["I7:R7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["I7:R7"].Style.Font.Bold = true;
                    reportFormSheet.Cells["I7:R7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["I8:J8"].Value = "QTR1";
                    reportFormSheet.Cells["I8:J8"].Merge = true;
                    reportFormSheet.Cells["I8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["I8:J8"].Style.Font.Bold = true;
                    reportFormSheet.Cells["I8:J8"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["K8:L8"].Value = "QTR2";
                    reportFormSheet.Cells["K8:L8"].Merge = true;
                    reportFormSheet.Cells["K8:L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["K8:L8"].Style.Font.Bold = true;
                    reportFormSheet.Cells["K8:L8"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["M8:N8"].Value = "QTR3";
                    reportFormSheet.Cells["M8:N8"].Merge = true;
                    reportFormSheet.Cells["M8:N8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["M8:N8"].Style.Font.Bold = true;
                    reportFormSheet.Cells["M8:N8"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["O8:P8"].Value = "QTR4";
                    reportFormSheet.Cells["O8:P8"].Merge = true;
                    reportFormSheet.Cells["O8:P8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["O8:P8"].Style.Font.Bold = true;
                    reportFormSheet.Cells["O8:P8"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells["Q8:R8"].Value = "TOTAL CUMULATIVE";
                    reportFormSheet.Cells["Q8:R8"].Merge = true;
                    reportFormSheet.Cells["Q8:R8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    reportFormSheet.Cells["Q8:R8"].Style.Font.Bold = true;
                    reportFormSheet.Cells["Q8:R8"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    reportFormSheet.Cells[9, 1].Value = "ITEM NUMBER";
                    reportFormSheet.Cells[9, 1].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 2].Value = "ROAD No.";
                    reportFormSheet.Cells[9, 2].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 3].Value = "ROAD NAME";
                    reportFormSheet.Cells[9, 3].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 4].Value = "WORKS CATEGORY";
                    reportFormSheet.Cells[9, 4].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 5].Value = "ST";
                    reportFormSheet.Cells[9, 5].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 6].Value = "KMs";
                    reportFormSheet.Cells[9, 6].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 7].Value = "AMOUNT PLANNED";
                    reportFormSheet.Cells[9, 7].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 8].Value = "CONTRACTOR NAME";
                    reportFormSheet.Cells[9, 8].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 9].Value = "KMs";
                    reportFormSheet.Cells[9, 9].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 10].Value = "EXP (KSH)";
                    reportFormSheet.Cells[9, 10].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 11].Value = "KMs";
                    reportFormSheet.Cells[9, 11].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 12].Value = "EXP (KSH)";
                    reportFormSheet.Cells[9, 12].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 13].Value = "KMs";
                    reportFormSheet.Cells[9, 13].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 14].Value = "EXP (KSH)";
                    reportFormSheet.Cells[9, 14].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 15].Value = "KMs";
                    reportFormSheet.Cells[9, 15].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 16].Value = "EXP (KSH)";
                    reportFormSheet.Cells[9, 16].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 17].Value = "KMs";
                    reportFormSheet.Cells[9, 17].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    reportFormSheet.Cells[9, 18].Value = "EXP (KSH)";
                    reportFormSheet.Cells[9, 18].Style.Font.Bold = true;
                    reportFormSheet.Cells[9, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    reportFormSheet.Cells[9, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    int row = 10;
                    foreach(var contract in agenctContracts)
                    {
                        foreach (var packageSectionPlan in contract.WorkPlanPackage.WorkpackageRoadWorkSectionPlans)
                        {
                            reportFormSheet.Cells[row, 1].Value = (row - 9);
                            reportFormSheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 2].Value = packageSectionPlan.RoadWorkSectionPlan.Road.RoadNumber;
                            reportFormSheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 3].Value = packageSectionPlan.RoadWorkSectionPlan.RoadSection.SectionName;
                            reportFormSheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 4].Value = packageSectionPlan.RoadWorkSectionPlan.WorkCategory.Code;
                            reportFormSheet.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 5].Value = packageSectionPlan.RoadWorkSectionPlan.RoadSection.SurfaceType.Code;
                            reportFormSheet.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 6].Value = packageSectionPlan.RoadWorkSectionPlan.PlannedLength.ToString("N");
                            reportFormSheet.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 7].Value = packageSectionPlan.RoadWorkSectionPlan.TotalEstimateCost.ToString("N");
                            reportFormSheet.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 8].Value = (contract.Contractor != null? contract.Contractor.Name.ToUpper() : "");
                            reportFormSheet.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 9].Value = packageSectionPlan.RoadWorkSectionPlan.AchievedLength.ToString("N");
                            reportFormSheet.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 10].Value = agancyProgress.Where(p=>((p.MonthCode.Code.ToUpper() == "JUL") || (p.MonthCode.Code.ToUpper() == "AUG") || (p.MonthCode.Code.ToUpper() == "SEP"))).Sum(p=>p.FinancialExpenditure).ToString("N");
                            reportFormSheet.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 11].Value = packageSectionPlan.RoadWorkSectionPlan.AchievedLength.ToString("N");
                            reportFormSheet.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 12].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "OCT") || (p.MonthCode.Code.ToUpper() == "NOV") || (p.MonthCode.Code.ToUpper() == "DEC"))).Sum(p => p.FinancialExpenditure).ToString("N");
                            reportFormSheet.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 13].Value = packageSectionPlan.RoadWorkSectionPlan.AchievedLength.ToString("N");
                            reportFormSheet.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 14].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JAN") || (p.MonthCode.Code.ToUpper() == "FEB") || (p.MonthCode.Code.ToUpper() == "MAR"))).Sum(p => p.FinancialExpenditure).ToString("N");
                            reportFormSheet.Cells[row, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 15].Value = packageSectionPlan.RoadWorkSectionPlan.AchievedLength.ToString("N");
                            reportFormSheet.Cells[row, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 16].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "APR") || (p.MonthCode.Code.ToUpper() == "MAY") || (p.MonthCode.Code.ToUpper() == "JUN"))).Sum(p => p.FinancialExpenditure).ToString("N");
                            reportFormSheet.Cells[row, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 17].Value = packageSectionPlan.RoadWorkSectionPlan.AchievedLength.ToString("N");
                            reportFormSheet.Cells[row, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            reportFormSheet.Cells[row, 18].Value = agancyProgress.Sum(p => p.FinancialExpenditure);
                            reportFormSheet.Cells[row, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            reportFormSheet.Cells[row, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            row++;

                        }
                    }

                    //SUMMARY REPORT BEGINS HERE
                    financialSummartSheet.Cells["A1:D1"].Value = "ANNUAL ROAD WORKS PROGRAMME FINANCIAL REPORT  ";
                    financialSummartSheet.Cells["A1:D1"].Merge = true;
                    financialSummartSheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["A1:D1"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["A1:D1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    financialSummartSheet.Cells["E1:F1"].Value = "FY " + printData.FinancialYear.Code;
                    financialSummartSheet.Cells["E1:F1"].Merge = true;
                    financialSummartSheet.Cells["E1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["E1:F1"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["E1:F1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if (printData.Authority.Type == 2)
                    {
                        financialSummartSheet.Cells["A2: C2"].Value = "NAME OF COUNTY GOVERNMENT";
                    }
                    else
                    {
                        financialSummartSheet.Cells["A2:C2"].Value = "NAME OF ROAD AGENCY ";
                    }
                    financialSummartSheet.Cells["A2:C2"].Merge = true;
                    financialSummartSheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["A2:C2"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["A2:C2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    financialSummartSheet.Cells["D2:F2"].Value = printData.Authority.Name.ToUpper();
                    financialSummartSheet.Cells["D2:F2"].Merge = true;
                    financialSummartSheet.Cells["D2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["D2:F2"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["D2:F2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if (printData.Authority.Type == 2)
                    {
                        financialSummartSheet.Cells["A3: C3"].Value = "COUNTY CODE";
                    }
                    else
                    {
                        financialSummartSheet.Cells["A3:C3"].Value = "ROAD AGENCY CODE";
                    }
                    financialSummartSheet.Cells["A3:C3"].Merge = true;
                    financialSummartSheet.Cells["A3:C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["A3:C3"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["A3:C3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    financialSummartSheet.Cells["D3:F3"].Value = printData.Authority.Code.ToUpper();
                    financialSummartSheet.Cells["D3:F3"].Merge = true;
                    financialSummartSheet.Cells["D3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["D3:F3"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["D3:F3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    financialSummartSheet.Cells["A4:C4"].Value = "BUDGET (KSHS)";
                    financialSummartSheet.Cells["A4:C4"].Merge = true;
                    financialSummartSheet.Cells["A4:C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["A4:C4"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["A4:C4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    financialSummartSheet.Cells["D4:F4"].Value = "0.00";
                    financialSummartSheet.Cells["D4:F4"].Merge = true;
                    financialSummartSheet.Cells["D4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells["D4:F4"].Style.Font.Bold = true;
                    financialSummartSheet.Cells["D4:F4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    financialSummartSheet.Cells[6, 1].Value = "QUARTERLY FINANCIAL REPORT";
                    financialSummartSheet.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[6, 2].Value = "QUARTER 1";
                    financialSummartSheet.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[6, 3].Value = "QUARTER 2";
                    financialSummartSheet.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[6, 4].Value = "QUARTER 3";
                    financialSummartSheet.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[6, 5].Value = "QUARTER 4";
                    financialSummartSheet.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[6, 6].Value = "TOTAL";
                    financialSummartSheet.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[7, 1].Value = "";
                    financialSummartSheet.Cells[7, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[7, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[7, 2].Value = "KSH";
                    financialSummartSheet.Cells[7, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[7, 3].Value = "KSH";
                    financialSummartSheet.Cells[7, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[7, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[7, 4].Value = "KSH";
                    financialSummartSheet.Cells[7, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[7, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[7, 5].Value = "KSH";
                    financialSummartSheet.Cells[7, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[7, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[7, 6].Value = "KSH";
                    financialSummartSheet.Cells[7, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[7, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[8, 1].Value = "Opening Balance as at 30 Sep";
                    financialSummartSheet.Cells[8, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[8, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[8, 2].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JUL") || (p.MonthCode.Code.ToUpper() == "AUG") || (p.MonthCode.Code.ToUpper() == "SEP"))).Sum(p => p.OpeningBalance).ToString("N");
                    financialSummartSheet.Cells[8, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[8, 3].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "OCT") || (p.MonthCode.Code.ToUpper() == "NOV") || (p.MonthCode.Code.ToUpper() == "DEC"))).Sum(p => p.OpeningBalance).ToString("N");
                    financialSummartSheet.Cells[8, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[8, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[8, 4].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JAN") || (p.MonthCode.Code.ToUpper() == "FEB") || (p.MonthCode.Code.ToUpper() == "MAR"))).Sum(p => p.OpeningBalance).ToString("N");
                    financialSummartSheet.Cells[8, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[8, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[8, 5].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "APR") || (p.MonthCode.Code.ToUpper() == "MAY") || (p.MonthCode.Code.ToUpper() == "JUN"))).Sum(p => p.OpeningBalance).ToString("N");
                    financialSummartSheet.Cells[8, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[8, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[8, 6].Value = agancyProgress.Sum(p => p.OpeningBalance).ToString("N");
                    financialSummartSheet.Cells[8, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[8, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    financialSummartSheet.Cells[9, 1].Value = "Receipts";
                    financialSummartSheet.Cells[9, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[9, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[9, 2].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JUL") || (p.MonthCode.Code.ToUpper() == "AUG") || (p.MonthCode.Code.ToUpper() == "SEP"))).Sum(p => p.FinancialReceipts).ToString("N");
                    financialSummartSheet.Cells[9, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[9, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[9, 3].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "OCT") || (p.MonthCode.Code.ToUpper() == "NOV") || (p.MonthCode.Code.ToUpper() == "DEC"))).Sum(p => p.FinancialReceipts).ToString("N");
                    financialSummartSheet.Cells[9, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[9, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[9, 4].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JAN") || (p.MonthCode.Code.ToUpper() == "FEB") || (p.MonthCode.Code.ToUpper() == "MAR"))).Sum(p => p.FinancialReceipts).ToString("N");
                    financialSummartSheet.Cells[9, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[9, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[9, 5].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "APR") || (p.MonthCode.Code.ToUpper() == "MAY") || (p.MonthCode.Code.ToUpper() == "JUN"))).Sum(p => p.FinancialReceipts).ToString("N");
                    financialSummartSheet.Cells[9, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[9, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[9, 6].Value = agancyProgress.Sum(p => p.FinancialReceipts).ToString("N");
                    financialSummartSheet.Cells[9, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[9, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    financialSummartSheet.Cells[10, 1].Value = "Expenditure";
                    financialSummartSheet.Cells[10, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[10, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[10, 2].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JUL") || (p.MonthCode.Code.ToUpper() == "AUG") || (p.MonthCode.Code.ToUpper() == "SEP"))).Sum(p => p.FinancialExpenditure).ToString("N");
                    financialSummartSheet.Cells[10, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[10, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[10, 3].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "OCT") || (p.MonthCode.Code.ToUpper() == "NOV") || (p.MonthCode.Code.ToUpper() == "DEC"))).Sum(p => p.FinancialExpenditure).ToString("N");
                    financialSummartSheet.Cells[10, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[10, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[10, 4].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JAN") || (p.MonthCode.Code.ToUpper() == "FEB") || (p.MonthCode.Code.ToUpper() == "MAR"))).Sum(p => p.FinancialExpenditure).ToString("N");
                    financialSummartSheet.Cells[10, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[10, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[10, 5].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "APR") || (p.MonthCode.Code.ToUpper() == "MAY") || (p.MonthCode.Code.ToUpper() == "JUN"))).Sum(p => p.FinancialExpenditure).ToString("N");
                    financialSummartSheet.Cells[10, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[10, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[10, 6].Value = agancyProgress.Sum(p => p.FinancialExpenditure).ToString("N");
                    financialSummartSheet.Cells[10, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[10, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    financialSummartSheet.Cells[11, 1].Value = "Closing Balance as at.. ";
                    financialSummartSheet.Cells[11, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[11, 1].Style.Font.Bold = true;
                    financialSummartSheet.Cells[11, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[11, 2].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JUL") || (p.MonthCode.Code.ToUpper() == "AUG") || (p.MonthCode.Code.ToUpper() == "SEP"))).Sum(p => p.ClosingBalance).ToString("N");
                    financialSummartSheet.Cells[11, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[11, 2].Style.Font.Bold = true;
                    financialSummartSheet.Cells[11, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    financialSummartSheet.Cells[11, 3].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "OCT") || (p.MonthCode.Code.ToUpper() == "NOV") || (p.MonthCode.Code.ToUpper() == "DEC"))).Sum(p => p.ClosingBalance).ToString("N");
                    financialSummartSheet.Cells[11, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[11, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[11, 3].Style.Font.Bold = true;

                    financialSummartSheet.Cells[11, 4].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "JAN") || (p.MonthCode.Code.ToUpper() == "FEB") || (p.MonthCode.Code.ToUpper() == "MAR"))).Sum(p => p.ClosingBalance).ToString("N");
                    financialSummartSheet.Cells[11, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[11, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[11, 4].Style.Font.Bold = true;

                    financialSummartSheet.Cells[11, 5].Value = agancyProgress.Where(p => ((p.MonthCode.Code.ToUpper() == "APR") || (p.MonthCode.Code.ToUpper() == "MAY") || (p.MonthCode.Code.ToUpper() == "JUN"))).Sum(p => p.ClosingBalance).ToString("N");
                    financialSummartSheet.Cells[11, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[11, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[11, 5].Style.Font.Bold = true;

                    financialSummartSheet.Cells[11, 6].Value = agancyProgress.Sum(p => p.ClosingBalance).ToString("N");
                    financialSummartSheet.Cells[11, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[11, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[11, 6].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 1].Value = "Notes";
                    financialSummartSheet.Cells[13, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 1].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 2].Value = " ";
                    financialSummartSheet.Cells[13, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 2].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 3].Value = " ";
                    financialSummartSheet.Cells[13, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 3].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 4].Value = " ";
                    financialSummartSheet.Cells[13, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 4].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 5].Value = " ";
                    financialSummartSheet.Cells[13, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 5].Style.Font.Bold = true;
                    
                    financialSummartSheet.Cells[13, 6].Value = " ";
                    financialSummartSheet.Cells[13, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 6].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 1].Value = "(Attach a copy of the bank reconciliation as at end of quarter - September, December, March and June)";
                    financialSummartSheet.Cells[13, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 1].Style.WrapText = true;

                    financialSummartSheet.Cells[13, 2].Value = " ";
                    financialSummartSheet.Cells[13, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 2].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 3].Value = " ";
                    financialSummartSheet.Cells[13, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 3].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 4].Value = " ";
                    financialSummartSheet.Cells[13, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 4].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 5].Value = " ";
                    financialSummartSheet.Cells[13, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 5].Style.Font.Bold = true;

                    financialSummartSheet.Cells[13, 6].Value = " ";
                    financialSummartSheet.Cells[13, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    financialSummartSheet.Cells[13, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    financialSummartSheet.Cells[13, 6].Style.Font.Bold = true;

                    int printRow = 15;
                    if(printData.Authority.Type == 2)
                    {
                        financialSummartSheet.Cells[printRow, 1].Value = "(Attach a copy of the bank reconciliation as at end of quarter - September, December, March and June)";
                        financialSummartSheet.Cells[printRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        financialSummartSheet.Cells[printRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        financialSummartSheet.Cells[printRow, 1].Style.WrapText = true;

                        financialSummartSheet.Cells[printRow, 2].Value = " ";
                        financialSummartSheet.Cells[printRow, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        financialSummartSheet.Cells[printRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        financialSummartSheet.Cells[printRow, 2].Style.Font.Bold = true;

                        financialSummartSheet.Cells[printRow, 3].Value = " ";
                        financialSummartSheet.Cells[printRow, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        financialSummartSheet.Cells[printRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        financialSummartSheet.Cells[printRow, 3].Style.Font.Bold = true;

                        financialSummartSheet.Cells[printRow, 4].Value = " ";
                        financialSummartSheet.Cells[printRow, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        financialSummartSheet.Cells[printRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        financialSummartSheet.Cells[printRow, 4].Style.Font.Bold = true;

                        financialSummartSheet.Cells[printRow, 5].Value = " ";
                        financialSummartSheet.Cells[printRow, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        financialSummartSheet.Cells[printRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        financialSummartSheet.Cells[printRow, 5].Style.Font.Bold = true;

                        financialSummartSheet.Cells[printRow, 6].Value = " ";
                        financialSummartSheet.Cells[printRow, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        financialSummartSheet.Cells[printRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        financialSummartSheet.Cells[printRow, 6].Style.Font.Bold = true;

                        printRow++;
                    }

                    reportFormSheet.Cells.AutoFitColumns();
                    financialSummartSheet.Cells.AutoFitColumns();

                    excel.Workbook.Properties.Title = "Attempts";
                    // excel.Save();
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "WorkPlanPackage", new { fileGuid = handle, FileName = fileName })
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }
        }

        public async Task<IActionResult> DownloadQuarterlyEmploymentSummary(long contractId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new Contract();
                //retrieve the full workpackage details from database
                var contractResponse = await _contractService.FindByIdAsync(contractId).ConfigureAwait(false);
                if (contractResponse.Success)
                {
                    printData = contractResponse.Contract;    
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Contract Information could not be found."
                    });
                }


                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    var sheetcreate = excel.Workbook.Worksheets.Add("QuartelyEmploymentProgress");
              
                    sheetcreate.Cells[2, 2].Value = "EMPLOYMENT REPORT ";
                    sheetcreate.Cells["B2:G2"].Merge = true;
                    sheetcreate.Cells["B2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreate.Cells["B2:G2"].Style.Font.Bold = true;
                    sheetcreate.Cells["B2:G2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    sheetcreate.Cells[3, 2].Value = "FIRST QUARTERLY REPORT ";
                    sheetcreate.Cells["B3:G3"].Merge = true;
                    sheetcreate.Cells["B3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreate.Cells["B3:G3"].Style.Font.Bold = true;
                    sheetcreate.Cells["B3:G3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    sheetcreate.Cells[3, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[4, 2].Value = "AUTHORITY";
                    sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreate.Cells[4, 2, 4, 3].Merge = true;

                    sheetcreate.Cells[4, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    
                    sheetcreate.Cells[5, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);


                    sheetcreate.Cells[4, 4].Value = "KENHA";
                    sheetcreate.Cells[4, 4].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[5, 2].Value = "FINANCIAL YEAR";
                    sheetcreate.Cells[5, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreate.Cells[5, 2, 5, 3].Merge = true;

                    sheetcreate.Cells[5, 4].Value = "2016-17";
                    sheetcreate.Cells[5, 4].Style.Font.Bold = true;
                    sheetcreate.Cells[5, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[5, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //PRINT HEADERS
                    sheetcreate.Cells[7, 2, 8, 2].Value = "Package Number";
                    sheetcreate.Cells[7, 2, 8, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[7, 2, 8, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[7, 2, 8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheetcreate.Cells[7, 2, 8, 2].Merge = true;

                    sheetcreate.Cells[7, 3, 8, 3].Value = "Road No.";
                    sheetcreate.Cells[7, 3, 8, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[7, 3, 8, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[7, 3, 8, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheetcreate.Cells[7, 3, 8, 3].Merge = true;

                    sheetcreate.Cells[7, 4, 8, 4].Value = "Section Name";
                    sheetcreate.Cells[7, 4, 8, 4].Style.Font.Bold = true;
                    sheetcreate.Cells[7, 4, 8, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[7, 4, 8, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheetcreate.Cells[7, 4, 8, 4].Merge = true;

                    sheetcreate.Cells[7, 5, 8, 5].Value = "Length (KM)";
                    sheetcreate.Cells[7, 5, 8, 5].Style.Font.Bold = true;
                    sheetcreate.Cells[7, 5, 8, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[7, 5, 8, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheetcreate.Cells[7, 5, 8, 5].Merge = true;

                    sheetcreate.Cells[7, 6, 7, 11].Value = "Current Quarter Employment";
                    sheetcreate.Cells[7, 6, 7, 11].Style.Font.Bold = true;
                    sheetcreate.Cells[7, 6, 7, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[7, 6, 7, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheetcreate.Cells[7, 6, 7, 11].Merge = true;

                    sheetcreate.Cells[8, 6].Value = "No. Of Men Employed";
                    sheetcreate.Cells[8, 6].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 7].Value = "No. Of Women Employed";
                    sheetcreate.Cells[8, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 8].Value = "Total No. Persons Employed";
                    sheetcreate.Cells[8, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 9].Value = "Men Man Days";
                    sheetcreate.Cells[8, 9].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 10].Value = "Women Mandays";
                    sheetcreate.Cells[8, 10].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 11].Value = "Total Mandays";
                    sheetcreate.Cells[8, 11].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //cummulative
                    sheetcreate.Cells[7, 12, 7, 17].Value = "Upto Date Employment";
                    sheetcreate.Cells[7, 12, 7, 17].Style.Font.Bold = true;
                    sheetcreate.Cells[7, 12, 7, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[7, 12, 7, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheetcreate.Cells[7, 12, 7, 17].Merge = true;

                    sheetcreate.Cells[8, 12].Value = "No. Of Men Employed";
                    sheetcreate.Cells[8, 12].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 13].Value = "No. Of Women Employed";
                    sheetcreate.Cells[8, 13].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 14].Value = "Total No. Persons Employed";
                    sheetcreate.Cells[8, 14].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 15].Value = "Men Man Days";
                    sheetcreate.Cells[8, 15].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 16].Value = "Women Mandays";
                    sheetcreate.Cells[8, 16].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[8, 17].Value = "Total Mandays";
                    sheetcreate.Cells[8, 17].Style.Font.Bold = true;
                    sheetcreate.Cells[8, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[8, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    int row = 9;
                    int itemNo = 1;

                    foreach(var workplan in printData.WorkPlanPackage.WorkpackageRoadWorkSectionPlans)
                    {
                        sheetcreate.Cells[row, 2].Value = printData.WorkPlanPackage.Code;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 3].Value = workplan.RoadWorkSectionPlan.Road.RoadNumber;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 4].Value = workplan.RoadWorkSectionPlan.RoadSection.SectionName;
                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 5].Value = workplan.RoadWorkSectionPlan.RoadSection.Length.ToString("N");
                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //get the employment details
                        var employmentDetails = printData.EmploymentDetails;
                        if(employmentDetails == null)
                        {
                            return Json(new { Success = false, Message = "There are no employment records recorded for the contract for download." });
                        }
                        var currentManCount = employmentDetails != null?  employmentDetails.OrderByDescending(id => id.ID).FirstOrDefault().MaleCount : 0;
                        var currentFemaleCount = employmentDetails != null ? employmentDetails.OrderByDescending(id => id.ID).FirstOrDefault().FemaleCount : 0;
                        var currentManDay = employmentDetails != null ? employmentDetails.OrderByDescending(id => id.ID).FirstOrDefault().MaleMandays : 0;
                        var currentFemaleManDays = employmentDetails != null ? employmentDetails.OrderByDescending(id => id.ID).FirstOrDefault().FemaleMandays : 0;

                        var cummulativeManCount = employmentDetails != null ? employmentDetails.Sum(i=>i.MaleCount) : 0;
                        var cummulativeFemaleCount = employmentDetails != null ? employmentDetails.Sum(i => i.FemaleCount) : 0;
                        var cummulativeManDay = employmentDetails != null ? employmentDetails.Sum(i => i.MaleMandays) : 0;
                        var cummulativeFemaleManDays = employmentDetails != null ? employmentDetails.Sum(i => i.FemaleMandays) : 0;


                        sheetcreate.Cells[row, 6].Value = currentManCount.ToString("N");
                        sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 7].Value = currentFemaleCount.ToString("N");
                        sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 8].Value = (currentManCount + currentFemaleCount).ToString("N");
                        sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 9].Value = currentManDay.ToString("N");
                        sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 10].Value = currentFemaleManDays.ToString("N");
                        sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 11].Value = (currentManDay + currentFemaleManDays).ToString("N");
                        sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //cummulative
                        sheetcreate.Cells[row, 12].Value = cummulativeManCount.ToString("N");
                        sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 13].Value = cummulativeFemaleCount.ToString("N");
                        sheetcreate.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 14].Value = (cummulativeManCount + cummulativeFemaleCount).ToString("N");
                        sheetcreate.Cells[row, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 15].Value = cummulativeManDay.ToString("N");
                        sheetcreate.Cells[row, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 16].Value = cummulativeFemaleManDays.ToString("N");
                        sheetcreate.Cells[row, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 17].Value = (cummulativeFemaleManDays + cummulativeFemaleManDays).ToString("N");
                        sheetcreate.Cells[row, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        itemNo++;

                        row++;

                    }



                    sheetcreate.Cells.AutoFitColumns();

                    excel.Workbook.Properties.Title = "Attempts";
                    // excel.Save();
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "WorkPlanPackage", new { fileGuid = handle, FileName = "MonthlyeEmploymentProgress.xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }
    }
}