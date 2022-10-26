using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RevenueCollectionCodeUnitTypeService : IRevenueCollectionCodeUnitTypeService
    {
        private readonly IRevenueCollectionCodeUnitTypeRepository _revenueCollectionCodeUnitTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RevenueCollectionCodeUnitTypeService(IRevenueCollectionCodeUnitTypeRepository revenueCollectionCodeUnitTypeRepository, IUnitOfWork unitOfWork
            , ILogger<RevenueCollectionCodeUnitTypeService> logger)
        {
            _revenueCollectionCodeUnitTypeRepository = revenueCollectionCodeUnitTypeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitTypeResponse> AddAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            try
            {
                await _revenueCollectionCodeUnitTypeRepository.AddAsync(revenueCollectionCodeUnitType).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RevenueCollectionCodeUnitTypeResponse(revenueCollectionCodeUnitType); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.AddAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitTypeResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitTypeResponse> DetachFirstEntryAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            try
            {
                await _revenueCollectionCodeUnitTypeRepository.DetachFirstEntryAsync(revenueCollectionCodeUnitType).ConfigureAwait(false);                

                return new RevenueCollectionCodeUnitTypeResponse(revenueCollectionCodeUnitType); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.AddAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitTypeResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitTypeResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RevenueCollectionCodeUnitTypeResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitTypeResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitTypeResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RevenueCollectionCodeUnitTypeResponse> FindByNameAsync(string Type)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitTypeRepository.FindByNameAsync(Type).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RevenueCollectionCodeUnitTypeResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitTypeResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitTypeResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitTypeListResponse> ListAsync()
        {
            try
            {
                var existingRevenueCollectionCodeUnits = await _revenueCollectionCodeUnitTypeRepository.ListAsync().ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnits == null)
                {
                    return new RevenueCollectionCodeUnitTypeListResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionCodeUnitTypeListResponse(existingRevenueCollectionCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitTypeListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitTypeResponse> RemoveAsync(long ID)
        {
            var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RevenueCollectionCodeUnitTypeResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _revenueCollectionCodeUnitTypeRepository.Remove(existingRevenueCollectionCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RevenueCollectionCodeUnitTypeResponse(existingRevenueCollectionCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.RemoveAsync Error: {Environment.NewLine}");
                    return new RevenueCollectionCodeUnitTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitTypeResponse> Update(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            var existingRevenueCollectionCodeUnit = await _revenueCollectionCodeUnitTypeRepository.FindByIdAsync(revenueCollectionCodeUnitType.ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RevenueCollectionCodeUnitTypeResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _revenueCollectionCodeUnitTypeRepository.Update(revenueCollectionCodeUnitType);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RevenueCollectionCodeUnitTypeResponse(revenueCollectionCodeUnitType);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.Update Error: {Environment.NewLine}");
                    return new RevenueCollectionCodeUnitTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionCodeUnitTypeResponse> Update(long ID, RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            try
            {
                _revenueCollectionCodeUnitTypeRepository.Update(ID, revenueCollectionCodeUnitType);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RevenueCollectionCodeUnitTypeResponse(revenueCollectionCodeUnitType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitTypeService.Update Error: {Environment.NewLine}");
                return new RevenueCollectionCodeUnitTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
