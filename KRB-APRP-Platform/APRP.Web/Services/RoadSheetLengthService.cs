using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadSheetLengthService : IRoadSheetLengthService
    {
        private readonly IRoadSheetLengthRepository _roadSheetLengthRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadSheetLengthService(IRoadSheetLengthRepository roadSheetLengthRepository, IUnitOfWork unitOfWork
            , ILogger<RoadSheetLengthService> logger)
        {
            _roadSheetLengthRepository = roadSheetLengthRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetLengthResponse> AddAsync(RoadSheetLength roadSheetLength)
        {
            try
            {
                await _roadSheetLengthRepository.AddAsync(roadSheetLength).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSheetLengthResponse(roadSheetLength); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetLengthService.AddAsync Error: {Environment.NewLine}");
                return new RoadSheetLengthResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetLengthResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRevenueCollectionCodeUnit = await _roadSheetLengthRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnit == null)
                {
                    return new RoadSheetLengthResponse("Records Not Found");
                }
                else
                {
                    return new RoadSheetLengthResponse(existingRevenueCollectionCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetLengthService.ListAsync Error: {Environment.NewLine}");
                return new RoadSheetLengthResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetLengthListResponse> ListAsync()
        {
            try
            {
                var existingRevenueCollectionCodeUnits = await _roadSheetLengthRepository.ListAsync().ConfigureAwait(false);
                if (existingRevenueCollectionCodeUnits == null)
                {
                    return new RoadSheetLengthListResponse("Records Not Found");
                }
                else
                {
                    return new RoadSheetLengthListResponse(existingRevenueCollectionCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetLengthService.ListAsync Error: {Environment.NewLine}");
                return new RoadSheetLengthListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetLengthResponse> RemoveAsync(long ID)
        {
            var existingRevenueCollectionCodeUnit = await _roadSheetLengthRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RoadSheetLengthResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadSheetLengthRepository.Remove(existingRevenueCollectionCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadSheetLengthResponse(existingRevenueCollectionCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadSheetLengthService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadSheetLengthResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetLengthResponse> Update(RoadSheetLength roadSheetLength)
        {
            var existingRevenueCollectionCodeUnit = await _roadSheetLengthRepository.FindByIdAsync(roadSheetLength.ID).ConfigureAwait(false);
            if (existingRevenueCollectionCodeUnit == null)
            {
                return new RoadSheetLengthResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadSheetLengthRepository.Update(roadSheetLength);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadSheetLengthResponse(roadSheetLength);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadSheetLengthService.Update Error: {Environment.NewLine}");
                    return new RoadSheetLengthResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetLengthResponse> Update(long ID, RoadSheetLength roadSheetLength)
        {
            try
            {
                _roadSheetLengthRepository.Update(ID, roadSheetLength);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSheetLengthResponse(roadSheetLength);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetLengthService.Update Error: {Environment.NewLine}");
                return new RoadSheetLengthResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
