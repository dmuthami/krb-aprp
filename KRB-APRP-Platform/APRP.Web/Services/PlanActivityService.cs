using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class PlanActivityService : IPlanActivityService
    {
        private readonly IPlanActivityRepository _planActivityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoadWorkSectionPlanService _roadWorkSectionPlanService;
        private readonly IWorkPlanPackageService _workPlanPackageService;
        private readonly ITechnologyService _technologyService;
        private readonly ILogger _logger;

        public PlanActivityService(IPlanActivityRepository planActivityRepository, 
            IUnitOfWork unitOfWork, 
            IRoadWorkSectionPlanService roadWorkSectionPlanService,
            IWorkPlanPackageService workPlanPackageService,
            ITechnologyService technologyService,
             ILogger<PlanActivityService> logger)
        {
            _planActivityRepository = planActivityRepository;
            _unitOfWork = unitOfWork;
            _roadWorkSectionPlanService = roadWorkSectionPlanService;
            _workPlanPackageService = workPlanPackageService;
            _technologyService = technologyService;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityResponse> AddAsync(PlanActivity planActivity)
        {
            try
            {
                await _planActivityRepository.AddAsync(planActivity).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                var existingPlanResponse = await _roadWorkSectionPlanService.FindByIdAsync(planActivity.RoadWorkSectionPlanId).ConfigureAwait(false);
                if (existingPlanResponse.Success)
                {
                    //update the plan total estimate
                    var updatePlan = existingPlanResponse.RoadWorkSectionPlan;
                    updatePlan.TotalEstimateCost += planActivity.Amount;

                    await _roadWorkSectionPlanService.UpdateAsync(updatePlan).ConfigureAwait(false);

                }

                return new PlanActivityResponse(planActivity); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.AddAsync Error: {Environment.NewLine}");
                return new PlanActivityResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _planActivityRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new PlanActivityResponse("Record Not Found");
                }
                else
                {
                    return new PlanActivityResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.FindByIdAsync Error: {Environment.NewLine}");
                return new PlanActivityResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<PlanActivity>> ListAsync()
        {
            return await _planActivityRepository.ListAsync().ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _planActivityRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new PlanActivityResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _planActivityRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    var existingPlanResponse = await _roadWorkSectionPlanService.FindByIdAsync(existingRecord.RoadWorkSectionPlan.ID).ConfigureAwait(false);
                    if (existingPlanResponse.Success)
                    {
                        //update the plan total estimate
                        var updatePlan = existingPlanResponse.RoadWorkSectionPlan;
                        updatePlan.TotalEstimateCost -= existingRecord.Amount;

                        await _roadWorkSectionPlanService.UpdateAsync(updatePlan).ConfigureAwait(false);

                    }

                    return new PlanActivityResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"PlanActivityService.RemoveAsync Error: {Environment.NewLine}");
                    return new PlanActivityResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        public async Task<PlanActivityResponse> ResetPlanQuantitiesAsync(long workpackageId, long roadWorkSectionPlanId)
        {
            var existingRecord = await _roadWorkSectionPlanService.FindByIdAsync(roadWorkSectionPlanId).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new PlanActivityResponse("Record Not Found");
            }
            else
            {
                try
                {
                    if(existingRecord.RoadWorkSectionPlan.PlanActivities != null)
                    {
                        foreach (var plan in existingRecord.RoadWorkSectionPlan.PlanActivities)
                        {
                            //update the package quantity
                            plan.PackageQuantity = 0.0M;
                            await UpdateAsync(plan).ConfigureAwait(false);
                        }
                    }

                    // reset the workpackage engineer's estimate to 0
                    var existingPackage = await _workPlanPackageService.FindByIdAsync(workpackageId).ConfigureAwait(false);
                    if (existingPackage.Success)
                    {
                        existingPackage.WorkPlanPackage.EngineerEstimate = 0.0;
                        await _workPlanPackageService.UpdateAsync(existingPackage.WorkPlanPackage).ConfigureAwait(false);
                    }
                    //force success
                    return new PlanActivityResponse(true, "Success", new PlanActivity());
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"PlanActivityService.RemoveAsync Error: {Environment.NewLine}");
                    return new PlanActivityResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityResponse> UpdateAsync(PlanActivity planActivity)
        {
            if (planActivity != null)
            {

                try
                {

                    planActivity.Amount = (double)(planActivity.Quantity * (decimal)(planActivity.Rate));
                    var existingRecord = await _planActivityRepository.FindByIdAsync(planActivity.ID).ConfigureAwait(false);
                    if (existingRecord == null)
                    {
                        return new PlanActivityResponse("Record Not Found");
                    }
                    else
                    {
                        var oldAmount = existingRecord.Amount;
                        existingRecord.Amount = planActivity.Amount;
                        existingRecord.Quantity = planActivity.Quantity;
                        existingRecord.Rate = planActivity.Rate;
                        existingRecord.StartChainage = planActivity.StartChainage;
                        existingRecord.EndChainage = planActivity.EndChainage;
                        existingRecord.TechnologyId = planActivity.TechnologyId;


                        var technologyList = await _technologyService.ListAsync().ConfigureAwait(false);
                        var tech = technologyList.Where(id => id.ID == (long)planActivity.TechnologyId).SingleOrDefault();
                        if (tech.Code == "LB")
                        {
                            existingRecord.LabourPercent = 100.00f;
                        }
                        else if (tech.Code == "MB")
                        {
                            existingRecord.LabourPercent = 0.00f;
                        }
                        else
                        {
                            existingRecord.LabourPercent = planActivity.LabourPercent;
                        }
                        //update the workplan value for the section

                        _planActivityRepository.Update(existingRecord);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        var existingPlanResponse = await _roadWorkSectionPlanService.FindByIdAsync(existingRecord.RoadWorkSectionPlan.ID).ConfigureAwait(false);
                        if (existingPlanResponse.Success)
                        {
                            //update the plan total estimate
                            var updatePlan = existingPlanResponse.RoadWorkSectionPlan;
                            updatePlan.TotalEstimateCost += (existingRecord.Amount - oldAmount);

                            await _roadWorkSectionPlanService.UpdateAsync(updatePlan).ConfigureAwait(false);

                        }


                        return new PlanActivityResponse(existingRecord);

                    }
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"PlanActivityService.Update Error: {Environment.NewLine}");
                    return new PlanActivityResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
            return new PlanActivityResponse("Invalid Record for update");
        }

        public async Task<PlanActivityResponse> UpdateBulkPackageQuantityByPackageIdAsync(long workpackageId, ApplicationUser user)
        {
            if(workpackageId > 0)
            {
                try
                {
                    //get package activities
                    var exisitingRecord = await _workPlanPackageService.FindByIdAsync(workpackageId).ConfigureAwait(false);
                    //retrieve all the plan activities
                    if (exisitingRecord.Success)
                    {
                        var workPackage = exisitingRecord.WorkPlanPackage;
                        var totalEngineerEstimate = 0.0;
                        if (workPackage.WorkpackageRoadWorkSectionPlans != null)
                        {
                            foreach (var section in workPackage.WorkpackageRoadWorkSectionPlans)
                            {
                                if (section.RoadWorkSectionPlan.PlanActivities != null)
                                {
                                    foreach (var plan in section.RoadWorkSectionPlan.PlanActivities)
                                    {
                                        //set the package quantity same as the planned quantity
                                        plan.PackageQuantity = plan.Quantity;
                                        plan.PackageAmount = (double)((plan.Quantity * (decimal)plan.Rate));
                                        plan.UpdateBy = user.UserName;
                                        plan.UpdateDate = DateTime.UtcNow;

                                        //update the record
                                        await UpdateAsync(plan).ConfigureAwait(false);

                                        //set the engineer's total
                                        totalEngineerEstimate = (totalEngineerEstimate + (double)(plan.PackageQuantity * (decimal)plan.Rate));
                                    }
                                }
                            }

                            //update the engineer's estimate value
                            exisitingRecord.WorkPlanPackage.EngineerEstimate = Math.Ceiling(totalEngineerEstimate * (100 + exisitingRecord.WorkPlanPackage.VAT)) / 100;
                            await _workPlanPackageService.UpdateAsync(exisitingRecord.WorkPlanPackage).ConfigureAwait(false);

                            return new PlanActivityResponse(true, "Update Successful", null);
                        }
                        else
                        {
                            return new PlanActivityResponse("Work package is missing work plans, please check with the administrator");
                        }
                    }
                    else
                    {
                        return new PlanActivityResponse("Failed to retrieve the work package, please check with the administrator");
                    }

                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"PlanActivityService.Update Error: {Environment.NewLine}");
                    return new PlanActivityResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
            else
            {
                return new PlanActivityResponse("Invalid package record supplied for update");
            }
        }
    }
}
