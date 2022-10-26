using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class WorkPlanPackageService : IWorkPlanPackageService
    {
        private readonly IWorkPlanPackageRepository _workPlanPackageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkPlanPackageService(IWorkPlanPackageRepository workPlanPackageRepository, IUnitOfWork unitOfWork)
        {
            _workPlanPackageRepository = workPlanPackageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkPlanPackageResponse> AddAsync(WorkPlanPackage workPlanPackage)
        {
            try
            {
                workPlanPackage.VAT = 16F;
                await _workPlanPackageRepository.AddAsync(workPlanPackage).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkPlanPackageResponse(workPlanPackage);
            }
            catch (Exception ex)
            {
                return new WorkPlanPackageResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<WorkPlanPackageResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingWorkPlanPackage = await _workPlanPackageRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingWorkPlanPackage == null)
                {
                    return new WorkPlanPackageResponse("Record Not Found");
                }
                else
                {
                    return new WorkPlanPackageResponse(existingWorkPlanPackage);
                }
            }
            catch(Exception ex)
            {
                //logging
                return new WorkPlanPackageResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<WorkPlanPackageResponse> FindByCodeAsync(string code)
        {
            try
            {
                var existingWorkPlanPackage = await _workPlanPackageRepository.FindByCodeAsync(code).ConfigureAwait(false);
                if (existingWorkPlanPackage == null)
                {
                    return new WorkPlanPackageResponse("Record Not Found");
                }
                else
                {
                    return new WorkPlanPackageResponse(existingWorkPlanPackage);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new WorkPlanPackageResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<WorkPlanPackage>> ListAsync()
        {
            try
            {
                return await _workPlanPackageRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<WorkPlanPackage>();
            }
        }
        public async Task<IEnumerable<WorkPlanPackage>> ListByFinancialYearAsync(long financialYearId, long authorityID)
        {
            try
            {
                return await _workPlanPackageRepository.ListByFinancialYearAsync(financialYearId,authorityID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<WorkPlanPackage>();
            }
        }
        public async Task<WorkPlanPackageResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRecord = await _workPlanPackageRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new WorkPlanPackageResponse("Record Not Found");
                }
                else
                {
                    _workPlanPackageRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkPlanPackageResponse(existingRecord);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new WorkPlanPackageResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<WorkPlanPackageResponse> UpdateAsync(WorkPlanPackage workPlanPackage)
        {
            try
            {
                var existingRecord = await _workPlanPackageRepository.FindByIdAsync(workPlanPackage.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new WorkPlanPackageResponse("Record Not Found");

                }
                else
                {
                    existingRecord.Code = workPlanPackage.Code;
                    existingRecord.Name = workPlanPackage.Name;
                    existingRecord.UpdateBy = workPlanPackage.UpdateBy;
                    existingRecord.Status = workPlanPackage.Status;
                    existingRecord.UpdateDate = workPlanPackage.UpdateDate;
                    existingRecord.FinancialYearId = workPlanPackage.FinancialYearId;
                    existingRecord.EngineerEstimate = workPlanPackage.EngineerEstimate;
                    existingRecord.Contigencies = workPlanPackage.Contigencies;
                    existingRecord.VariationPercentage = existingRecord.VariationPercentage;
                    existingRecord.VariationAmount = workPlanPackage.VariationAmount;

                    _workPlanPackageRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkPlanPackageResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new WorkPlanPackageResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
