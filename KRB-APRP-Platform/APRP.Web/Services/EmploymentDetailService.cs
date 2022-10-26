using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class EmploymentDetailService : IEmploymentDetailService
    {

        private readonly IEmploymentDetailRepository _employmentDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public EmploymentDetailService(IEmploymentDetailRepository  employmentDetailRepository, IUnitOfWork unitOfWork, ILogger<ItemActivityUnitCostService> logger)
        {
            _employmentDetailRepository = employmentDetailRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<EmploymentDetailResponse> AddAsync(EmploymentDetail employmentDetail)
        {
            try
            {
                await _employmentDetailRepository.AddAsync(employmentDetail).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new EmploymentDetailResponse(employmentDetail); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"EmploymentDetailService.AddAsync Error: {Environment.NewLine}");
                return new EmploymentDetailResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        public async Task<EmploymentDetailResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _employmentDetailRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new EmploymentDetailResponse("Record Not Found");
                }
                else
                {
                    return new EmploymentDetailResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"EmploymentDetailService.FindByIdAsync Error: {Environment.NewLine}");
                return new EmploymentDetailResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<EmploymentDetail>> ListAsync()
        {
            return await _employmentDetailRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<EmploymentDetailResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _employmentDetailRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new EmploymentDetailResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _employmentDetailRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new EmploymentDetailResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"EmploymentDetailService.RemoveAsync Error: {Environment.NewLine}");
                    return new EmploymentDetailResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<EmploymentDetailResponse> UpdateAsync(EmploymentDetail employmentDetail)
        {
            var existingRecord = await _employmentDetailRepository.FindByIdAsync(employmentDetail.ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new EmploymentDetailResponse("Record Not Found");
            }
            else
            {
                existingRecord.MaleCount = employmentDetail.MaleCount;
                existingRecord.FemaleCount= employmentDetail.FemaleCount;
                existingRecord.MaleMandays= employmentDetail.MaleMandays;
                existingRecord.FemaleMandays= employmentDetail.FemaleMandays;
                existingRecord.Period =  employmentDetail.Period;
                existingRecord.UpdateDate = employmentDetail.UpdateDate;
                existingRecord.UpdateBy = employmentDetail.UpdateBy;

                try
                {
                    _employmentDetailRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new EmploymentDetailResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"EmploymentDetailService.UpdateAsync Error: {Environment.NewLine}");
                    return new EmploymentDetailResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
