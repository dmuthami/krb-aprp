using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RevenueCollectionCodeUnitService : IRevenueCollectionCodeUnitService
    {
        private readonly IRevenueCollectionCodeUnitRepository _revenueCollectionCodeUnitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RevenueCollectionCodeUnitService(IRevenueCollectionCodeUnitRepository revenueCollectionCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<RevenueCollectionCodeUnitService> logger)
        {
            _revenueCollectionCodeUnitRepository = revenueCollectionCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitResponse> AddAsync(RevenueCollectionCodeUnit revenueCollectionCodeUnit)
        {
            try
            {
                await _revenueCollectionCodeUnitRepository.AddAsync(revenueCollectionCodeUnit).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RevenueCollectionCodeUnitResponse(revenueCollectionCodeUnit); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.AddAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RevenueCollectionCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitResponse> FindByNameAsync(RevenueStream RevenueStream)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitRepository.FindByNameAsync(RevenueStream).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RevenueCollectionCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitResponse> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitRepository.FindByNameAsync(RevenueStream, FinancialYearId).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RevenueCollectionCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RevenueCollectionCodeUnitResponse> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId, long AuthorityId)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitRepository.FindByNameAsync(RevenueStream, FinancialYearId
                    , AuthorityId).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RevenueCollectionCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitListResponse> ListAsync(long? AuthorityId)
        {
            try
            {
                var existingRevenueCollectionCodeUnits = await _revenueCollectionCodeUnitRepository.ListAsync(AuthorityId).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnits == null)
                {
                    return new RevenueCollectionCodeUnitListResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitListResponse(existingRevenueCollectionCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitListResponse> ListAsync(long FinancialYearId, string Type)
        {
            try
            {
                var existingRevenueCollectionCodeUnits = await _revenueCollectionCodeUnitRepository.ListAsync(FinancialYearId, Type).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnits == null)
                {
                    return new RevenueCollectionCodeUnitListResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitListResponse(existingRevenueCollectionCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitResponse> RemoveAsync(long ID)
        {
            var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RevenueCollectionCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _revenueCollectionCodeUnitRepository.Remove(existingRevenueCollectionCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RevenueCollectionCodeUnitResponse(existingRevenueCollectionCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.RemoveAsync Error: {Environment.NewLine}");
                    return new RevenueCollectionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitResponse> Update(RevenueCollectionCodeUnit revenueCollectionCodeUnit)
        {
            var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitRepository.FindByIdAsync(revenueCollectionCodeUnit.ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RevenueCollectionCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _revenueCollectionCodeUnitRepository.Update(revenueCollectionCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RevenueCollectionCodeUnitResponse(revenueCollectionCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.Update Error: {Environment.NewLine}");
                    return new RevenueCollectionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitResponse> Update(long ID, RevenueCollectionCodeUnit revenueCollectionCodeUnit)
        {
            try
            {
                _revenueCollectionCodeUnitRepository.Update(ID, revenueCollectionCodeUnit);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RevenueCollectionCodeUnitResponse(revenueCollectionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitService.Update Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
