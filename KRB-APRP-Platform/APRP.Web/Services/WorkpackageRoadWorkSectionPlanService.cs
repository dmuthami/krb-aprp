using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class WorkpackageRoadWorkSectionPlanService : IWorkpackageRoadWorkSectionPlanService
    {
        private readonly IWorkpackageRoadWorkSectionPlanRepository _workpackageRoadWorkSectionPlanRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkpackageRoadWorkSectionPlanService(IWorkpackageRoadWorkSectionPlanRepository workpackageRoadWorkSectionPlanRepository, IUnitOfWork unitOfWork)
        {
            _workpackageRoadWorkSectionPlanRepository = workpackageRoadWorkSectionPlanRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkpackageRoadWorkSectionPlanResponse> AddAsync(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan)
        {
            try
            {
                await _workpackageRoadWorkSectionPlanRepository.AddAsync(workpackageRoadWorkSectionPlan).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkpackageRoadWorkSectionPlanResponse(workpackageRoadWorkSectionPlan);
            }
            catch (Exception ex)
            {
                return new WorkpackageRoadWorkSectionPlanResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<WorkpackageRoadWorkSectionPlanResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _workpackageRoadWorkSectionPlanRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new WorkpackageRoadWorkSectionPlanResponse("Record Not Found");
                }
                else
                {
                    return new WorkpackageRoadWorkSectionPlanResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new WorkpackageRoadWorkSectionPlanResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<WorkpackageRoadWorkSectionPlanResponse> FindBySectionPlanIdAndWorkPackageId(long sectionPlanId, long workpackageId)
        {
            try
            {
                var existingRecord = await _workpackageRoadWorkSectionPlanRepository.FindBySectionPlanIdAndWorkPackageId(sectionPlanId, workpackageId).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new WorkpackageRoadWorkSectionPlanResponse("Record Not Found");
                }
                else
                {
                    return new WorkpackageRoadWorkSectionPlanResponse(existingRecord);
                }
            }
            catch(Exception ex)
            {
                //logging
                return new WorkpackageRoadWorkSectionPlanResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<WorkpackageRoadWorkSectionPlanResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRecord = await _workpackageRoadWorkSectionPlanRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new WorkpackageRoadWorkSectionPlanResponse("Record Not Found");
                }
                else
                {
                    _workpackageRoadWorkSectionPlanRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkpackageRoadWorkSectionPlanResponse(existingRecord);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new WorkpackageRoadWorkSectionPlanResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<WorkpackageRoadWorkSectionPlanResponse> UpdateAsync(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan)
        {
            try
            {
                var existingRecord = await _workpackageRoadWorkSectionPlanRepository.FindByIdAsync(workpackageRoadWorkSectionPlan.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new WorkpackageRoadWorkSectionPlanResponse("Record Not Found");

                }
                else
                {
                    existingRecord.EngineerRate = workpackageRoadWorkSectionPlan.EngineerRate;
                    existingRecord.PackageAmount = workpackageRoadWorkSectionPlan.PackageAmount;
                    existingRecord.UpdateBy = workpackageRoadWorkSectionPlan.UpdateBy;
                    existingRecord.UpdateDate = DateTime.UtcNow;
                    existingRecord.PackageQuantity = workpackageRoadWorkSectionPlan.PackageQuantity;
               

                    _workpackageRoadWorkSectionPlanRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkpackageRoadWorkSectionPlanResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new WorkpackageRoadWorkSectionPlanResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
