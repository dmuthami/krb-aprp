using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadSheetIntervalService : IRoadSheetIntervalService
    {
        private readonly IRoadSheetIntervalRepository _roadSheetIntervalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadSheetIntervalService(IRoadSheetIntervalRepository roadSheetIntervalRepository, IUnitOfWork unitOfWork
            , ILogger<RoadSheetIntervalService> logger)
        {
            _roadSheetIntervalRepository = roadSheetIntervalRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetIntervalResponse> AddAsync(RoadSheetInterval roadSheetInterval)
        {
            try
            {
                await _roadSheetIntervalRepository.AddAsync(roadSheetInterval).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSheetIntervalResponse(roadSheetInterval); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetIntervalService.AddAsync Error: {Environment.NewLine}");
                return new RoadSheetIntervalResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetIntervalResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _roadSheetIntervalRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RoadSheetIntervalResponse("Records Not Found");
                }
                else
                {
                    return new RoadSheetIntervalResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetIntervalService.ListAsync Error: {Environment.NewLine}");
                return new RoadSheetIntervalResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetIntervalListResponse> ListAsync()
        {
            try
            {
                var existingRevenueCollectionCodeUnits = await _roadSheetIntervalRepository.ListAsync().ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnits == null)
                {
                    return new RoadSheetIntervalListResponse("Records Not Found");
                }
                else
                {
                    return new RoadSheetIntervalListResponse(existingRevenueCollectionCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetIntervalService.ListAsync Error: {Environment.NewLine}");
                return new RoadSheetIntervalListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetIntervalResponse> RemoveAsync(long ID)
        {
            var existingRevenueCollectionCodeUnit = await _roadSheetIntervalRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RoadSheetIntervalResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadSheetIntervalRepository.Remove(existingRevenueCollectionCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadSheetIntervalResponse(existingRevenueCollectionCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadSheetIntervalService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadSheetIntervalResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetIntervalResponse> Update(RoadSheetInterval roadSheetInterval)
        {
            var existingRevenueCollectionCodeUnit = await _roadSheetIntervalRepository.FindByIdAsync(roadSheetInterval.ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RoadSheetIntervalResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadSheetIntervalRepository.Update(roadSheetInterval);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadSheetIntervalResponse(roadSheetInterval);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadSheetIntervalService.Update Error: {Environment.NewLine}");
                    return new RoadSheetIntervalResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetIntervalResponse> Update(long ID, RoadSheetInterval roadSheetInterval)
        {
            try
            {
                _roadSheetIntervalRepository.Update(ID, roadSheetInterval);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSheetIntervalResponse(roadSheetInterval);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetIntervalService.Update Error: {Environment.NewLine}");
                return new RoadSheetIntervalResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
