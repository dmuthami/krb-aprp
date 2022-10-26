using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class PlanActivityPBCService : IPlanActivityPBCService
    {
        private readonly IPlanActivityPBCRepository _planActivityPBCRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoadWorkSectionPlanService _roadWorkSectionPlanService;
        private readonly ILogger _logger;

        public PlanActivityPBCService(IPlanActivityPBCRepository planActivityPBCRepository, 
            IUnitOfWork unitOfWork, IRoadWorkSectionPlanService roadWorkSectionPlanService,
             ILogger<PlanActivityService> logger)
        {
            _planActivityPBCRepository = planActivityPBCRepository;
            _unitOfWork = unitOfWork;
            _roadWorkSectionPlanService = roadWorkSectionPlanService;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityPBCResponse> AddAsync(PlanActivityPBC planActivityPBC)
        {
            try
            {
                await _planActivityPBCRepository.AddAsync(planActivityPBC).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                var existingPlanResponse = await _roadWorkSectionPlanService.FindByIdAsync(planActivityPBC.RoadWorkSectionPlanId).ConfigureAwait(false);
                if (existingPlanResponse.Success)
                {
                    //update the plan total estimate
                    var updatePlan = existingPlanResponse.RoadWorkSectionPlan;
                    updatePlan.TotalEstimateCost += (planActivityPBC.CostPerKMPerMonth * planActivityPBC.PlannedKM);

                    await _roadWorkSectionPlanService.UpdateAsync(updatePlan).ConfigureAwait(false);

                }

                return new PlanActivityPBCResponse(planActivityPBC); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.AddAsync Error: {Environment.NewLine}");
                return new PlanActivityPBCResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityPBCResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _planActivityPBCRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new PlanActivityPBCResponse("Record Not Found");
                }
                else
                {
                    return new PlanActivityPBCResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.FindByIdAsync Error: {Environment.NewLine}");
                return new PlanActivityPBCResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<PlanActivityPBC>> ListAsync()
        {
            return await _planActivityPBCRepository.ListAsync().ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityPBCResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _planActivityPBCRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new PlanActivityPBCResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _planActivityPBCRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    var existingPlanResponse = await _roadWorkSectionPlanService.FindByIdAsync(existingRecord.RoadWorkSectionPlan.ID).ConfigureAwait(false);
                    if (existingPlanResponse.Success)
                    {
                        //update the plan total estimate
                        var updatePlan = existingPlanResponse.RoadWorkSectionPlan;
                        updatePlan.TotalEstimateCost -= (existingRecord.CostPerKMPerMonth * existingRecord.PlannedKM);

                        await _roadWorkSectionPlanService.UpdateAsync(updatePlan).ConfigureAwait(false);

                    }


                    return new PlanActivityPBCResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"PlanActivityService.RemoveAsync Error: {Environment.NewLine}");
                    return new PlanActivityPBCResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PlanActivityPBCResponse> UpdateAsync(PlanActivityPBC planActivityPBC)
        {
            if (planActivityPBC != null)
            {
                var existingRecord = await _planActivityPBCRepository.FindByIdAsync(planActivityPBC.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new PlanActivityPBCResponse("Record Not Found");
                }
                else
                {
                    var oldAmount = (existingRecord.CostPerKMPerMonth * existingRecord.PlannedKM);
                    // var oldAmount = existingRecord.Amount;
                    existingRecord.AchievedKM = planActivityPBC.AchievedKM;
                    existingRecord.CostPerKMPerMonth = planActivityPBC.CostPerKMPerMonth;
                    existingRecord.PlannedKM = planActivityPBC.PlannedKM;
                    existingRecord.TotalAmount = planActivityPBC.CostPerKMPerMonth * planActivityPBC.PlannedKM;
                    //update the workplan value for the section

                    try
                    {
                        _planActivityPBCRepository.Update(existingRecord);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        var existingPlanResponse = await _roadWorkSectionPlanService.FindByIdAsync(existingRecord.RoadWorkSectionPlan.ID).ConfigureAwait(false);
                        if (existingPlanResponse.Success)
                        {
                            //update the plan total estimate
                            var updatePlan = existingPlanResponse.RoadWorkSectionPlan;
                            updatePlan.TotalEstimateCost += ((existingRecord.CostPerKMPerMonth * existingRecord.PlannedKM) - oldAmount);

                            await _roadWorkSectionPlanService.UpdateAsync(updatePlan).ConfigureAwait(false);

                        }

                        return new PlanActivityPBCResponse(existingRecord);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"PlanActivityService.Update Error: {Environment.NewLine}");
                        return new PlanActivityPBCResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            return new PlanActivityPBCResponse("Invalid Record for update");
        }

    }
}
