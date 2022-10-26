using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class WorkplanUploadService : IWorkplanUploadService
    {

        private readonly IWorkplanUploadRepository _workplanUploadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkplanUploadService(IWorkplanUploadRepository workplanUploadRepository, IUnitOfWork unitOfWork)
        {
            _workplanUploadRepository = workplanUploadRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<WorkplanUploadResponse> AddAsync(WorkplanUpload workplanUpload)
        {
            try
            {

                await _workplanUploadRepository.AddAsync(workplanUpload).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkplanUploadResponse(workplanUpload);
            }
            catch (Exception ex)
            {
                return new WorkplanUploadResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<WorkplanUploadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingWorkplanUpload = await _workplanUploadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingWorkplanUpload == null)
                {
                    return new WorkplanUploadResponse("Record Not Found");
                }
                else
                {
                    return new WorkplanUploadResponse(existingWorkplanUpload);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new WorkplanUploadResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<WorkplanUpload>> FindByFinancialYearAsync(long financialYearId, long authorityId)
        {
            try
            {
               return await _workplanUploadRepository.FindByFinancialYearAsync(financialYearId,  authorityId).ConfigureAwait(false);
                
            }
            catch (Exception ex)
            {
                //logging
                return Enumerable.Empty<WorkplanUpload>();
            }
        }


        public async Task<IEnumerable<WorkplanUpload>> ListAsync()
        {
            try
            {
                return await _workplanUploadRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<WorkplanUpload>();
            }
        }

        public async Task<WorkplanUploadResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingWorkplanUpload = await _workplanUploadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingWorkplanUpload == null)
                {
                    return new WorkplanUploadResponse("Record Not Found");
                }
                else
                {
                    _workplanUploadRepository.Remove(existingWorkplanUpload);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkplanUploadResponse(existingWorkplanUpload);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new WorkplanUploadResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<WorkplanUploadResponse> UpdateAsync(WorkplanUpload workplanUpload)
        {
            try
            {
                var existingWorkplanUpload = await _workplanUploadRepository.FindByIdSimpleAsync(workplanUpload.ID).ConfigureAwait(false);
                if (existingWorkplanUpload == null)
                {
                    return new WorkplanUploadResponse("Record Not Found");

                }
                else
                {
                    existingWorkplanUpload.UpdateBy = workplanUpload.UpdateBy;
                    existingWorkplanUpload.UpdateDate = DateTime.UtcNow;
                    existingWorkplanUpload.UploadBudget = workplanUpload.UploadBudget;
                    existingWorkplanUpload.FinancialYearId = workplanUpload.FinancialYearId;
                    existingWorkplanUpload.WorkplansCreated = workplanUpload.WorkplansCreated;
                    existingWorkplanUpload.WorkplansCreatedBy = workplanUpload.WorkplansCreatedBy;
                    existingWorkplanUpload.WorkplansCreatedDate = workplanUpload.WorkplansCreatedDate;
                    
                    _workplanUploadRepository.Update(existingWorkplanUpload);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkplanUploadResponse(existingWorkplanUpload);
                }

            }
            catch (Exception ex)
            {
                //log
                return new WorkplanUploadResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
