using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class WorkplanApprovalBatchService : IWorkplanApprovalBatchService
    {

        private readonly IWorkplanApprovalBatchRepository _workplanApprovalBatchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkplanApprovalBatchService(IWorkplanApprovalBatchRepository workplanApprovalBatchRepository, IUnitOfWork unitOfWork)
        {
            _workplanApprovalBatchRepository= workplanApprovalBatchRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<WorkplanApprovalBatchResponse> AddAsync(WorkplanApprovalBatch workplanApprovalBatch)
        {
            try
            {

                await _workplanApprovalBatchRepository.AddAsync(workplanApprovalBatch).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkplanApprovalBatchResponse(workplanApprovalBatch);
            }
            catch (Exception ex)
            {
                return new WorkplanApprovalBatchResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<WorkplanApprovalBatchResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingWorkplanApprovalBatch = await _workplanApprovalBatchRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingWorkplanApprovalBatch == null)
                {
                    return new WorkplanApprovalBatchResponse("Record Not Found");
                }
                else
                {
                    return new WorkplanApprovalBatchResponse(existingWorkplanApprovalBatch);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new WorkplanApprovalBatchResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<WorkplanApprovalBatchResponse> FindByFinancialYearAsync(long financialYearId, long authorityId)
        {
            try
            {
                var existingWorkplanApprovalBatch = await _workplanApprovalBatchRepository.FindByFinancialYearAsync(financialYearId,  authorityId).ConfigureAwait(false);
                if (existingWorkplanApprovalBatch == null)
                {
                    return new WorkplanApprovalBatchResponse("Record Not Found");
                }
                else
                {
                    return new WorkplanApprovalBatchResponse(existingWorkplanApprovalBatch);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new WorkplanApprovalBatchResponse($"Error occured while getting the record {ex.Message}");
            }
        }


        public async Task<IEnumerable<WorkplanApprovalBatch>> ListAsync()
        {
            try
            {
                return await _workplanApprovalBatchRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<WorkplanApprovalBatch>();
            }
        }

        public async Task<WorkplanApprovalBatchResponse> RemoveAsync(long countyID)
        {
            try
            {
                var existingWorkplanApprovalBatch = await _workplanApprovalBatchRepository.FindByIdAsync(countyID).ConfigureAwait(false);
                if (existingWorkplanApprovalBatch == null)
                {
                    return new WorkplanApprovalBatchResponse("Record Not Found");
                }
                else
                {
                    _workplanApprovalBatchRepository.Remove(existingWorkplanApprovalBatch);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkplanApprovalBatchResponse(existingWorkplanApprovalBatch);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new WorkplanApprovalBatchResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<WorkplanApprovalBatchResponse> UpdateAsync(WorkplanApprovalBatch workplanApprovalBatch)
        {
            try
            {
                var existingWorkplanApprovalBatch = await _workplanApprovalBatchRepository.FindByIdAsync(workplanApprovalBatch.ID).ConfigureAwait(false);
                if (existingWorkplanApprovalBatch == null)
                {
                    return new WorkplanApprovalBatchResponse("Record Not Found");

                }
                else
                {
                    existingWorkplanApprovalBatch.ApprovalStatus = workplanApprovalBatch.ApprovalStatus;
                    existingWorkplanApprovalBatch.isAricsDone = workplanApprovalBatch.isAricsDone;
                    existingWorkplanApprovalBatch.isR2000AspectsDone = workplanApprovalBatch.isR2000AspectsDone;
                    existingWorkplanApprovalBatch.isRoadRelateCourseDone = workplanApprovalBatch.isRoadRelateCourseDone;
                    existingWorkplanApprovalBatch.isRoadSafetyEnvironmentDone= workplanApprovalBatch.isRoadSafetyEnvironmentDone;
                    existingWorkplanApprovalBatch.isUnitRatesEstimateDone= workplanApprovalBatch.isUnitRatesEstimateDone;
                    _workplanApprovalBatchRepository.Update(existingWorkplanApprovalBatch);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkplanApprovalBatchResponse(existingWorkplanApprovalBatch);
                }

            }
            catch (Exception ex)
            {
                //log
                return new WorkplanApprovalBatchResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
