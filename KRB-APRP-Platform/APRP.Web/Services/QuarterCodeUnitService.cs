using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class QuarterCodeUnitService : IQuarterCodeUnitService
    {
        private readonly IQuarterCodeUnitRepository _quarterCodeUnitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public QuarterCodeUnitService(IQuarterCodeUnitRepository quarterCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<QuarterCodeUnitService> logger)
        {
            _quarterCodeUnitRepository = quarterCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeUnitResponse> AddAsync(QuarterCodeUnit quarterCodeUnit)
        {
            try
            {
                await _quarterCodeUnitRepository.AddAsync(quarterCodeUnit).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new QuarterCodeUnitResponse(quarterCodeUnit); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitService.AddAsync Error: {Environment.NewLine}");
                return new QuarterCodeUnitResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeUnitResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingQuarterCodeUnit = await _quarterCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingQuarterCodeUnit == null)
                {
                    return new QuarterCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new QuarterCodeUnitResponse(existingQuarterCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new QuarterCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeUnitResponse> FindByQuarterCodeListIdAndFinancialIdAsync(long QuarterCodeListId, long FinancialYearId)
        {
            try
            {
                var existingQuarterCodeUnit = await _quarterCodeUnitRepository.FindByQuarterCodeListIdAndFinancialIdAsync(QuarterCodeListId, FinancialYearId).ConfigureAwait(false);
                if (existingQuarterCodeUnit == null)
                {
                    return new QuarterCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new QuarterCodeUnitResponse(existingQuarterCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new QuarterCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeUnitListResponse> ListAsync()
        {
            try
            {
                var existingQuarterCodeUnits = await _quarterCodeUnitRepository.ListAsync().ConfigureAwait(false);
                if (existingQuarterCodeUnits == null)
                {
                    return new QuarterCodeUnitListResponse("Records Not Found");
                }
                else
                {
                    return new QuarterCodeUnitListResponse(existingQuarterCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new QuarterCodeUnitListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<QuarterCodeUnitListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingQuarterCodeUnits = await _quarterCodeUnitRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingQuarterCodeUnits == null)
                {
                    return new QuarterCodeUnitListResponse("Records Not Found");
                }
                else
                {
                    return new QuarterCodeUnitListResponse(existingQuarterCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new QuarterCodeUnitListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeUnitResponse> RemoveAsync(long ID)
        {
            var existingQuarterCodeUnit = await _quarterCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingQuarterCodeUnit == null)
            {
                return new QuarterCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _quarterCodeUnitRepository.Remove(existingQuarterCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new QuarterCodeUnitResponse(existingQuarterCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"QuarterCodeUnitService.RemoveAsync Error: {Environment.NewLine}");
                    return new QuarterCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeUnitResponse> Update(QuarterCodeUnit quarterCodeUnit)
        {
            var existingQuarterCodeUnit = await _quarterCodeUnitRepository.FindByIdAsync(quarterCodeUnit.ID).ConfigureAwait(false);
            if (existingQuarterCodeUnit == null)
            {
                return new QuarterCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _quarterCodeUnitRepository.Update(quarterCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new QuarterCodeUnitResponse(quarterCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"QuarterCodeUnitService.Update Error: {Environment.NewLine}");
                    return new QuarterCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeUnitResponse> Update(long ID, QuarterCodeUnit quarterCodeUnit)
        {
            try
            {
                _quarterCodeUnitRepository.Update(ID, quarterCodeUnit);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new QuarterCodeUnitResponse(quarterCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitService.Update Error: {Environment.NewLine}");
                return new QuarterCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
