using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadClassCodeUnitService : IRoadClassCodeUnitService
    {
        private readonly IRoadClassCodeUnitRepository _roadClassCodeUnitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadClassCodeUnitService(IRoadClassCodeUnitRepository allocationCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<RoadClassCodeUnitService> logger)
        {
            _roadClassCodeUnitRepository = allocationCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassCodeUnitResponse> AddAsync(RoadClassCodeUnit roadClassCodeUnit)
        {
            try
            {
                await _roadClassCodeUnitRepository.AddAsync(roadClassCodeUnit).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadClassCodeUnitResponse(roadClassCodeUnit); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassCodeUnitService.AddAsync Error: {Environment.NewLine}");
                return new RoadClassCodeUnitResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassCodeUnitResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingAllocationCodeUnit = await _roadClassCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new RoadClassCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadClassCodeUnitResponse> FindByNameAsync(string RoadClass)
        {
            try
            {
                var existingAllocationCodeUnit = await _roadClassCodeUnitRepository.FindByNameAsync(RoadClass).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new RoadClassCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassCodeUnitListResponse> ListAsync()
        {
            try
            {
                var existingAllocationCodeUnits = await _roadClassCodeUnitRepository.ListAsync().ConfigureAwait(false);
                if (existingAllocationCodeUnits == null)
                {
                    return new RoadClassCodeUnitListResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassCodeUnitListResponse(existingAllocationCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassCodeUnitListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassCodeUnitResponse> RemoveAsync(long ID)
        {
            var existingAllocationCodeUnit = await _roadClassCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingAllocationCodeUnit == null)
            {
                return new RoadClassCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadClassCodeUnitRepository.Remove(existingAllocationCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadClassCodeUnitResponse(existingAllocationCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadClassCodeUnitService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadClassCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassCodeUnitResponse> Update(RoadClassCodeUnit roadClassCodeUnit)
        {
            var existingAllocationCodeUnit = await _roadClassCodeUnitRepository.FindByIdAsync(roadClassCodeUnit.ID).ConfigureAwait(false);
            if (existingAllocationCodeUnit == null)
            {
                return new RoadClassCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadClassCodeUnitRepository.Update(roadClassCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadClassCodeUnitResponse(roadClassCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadClassCodeUnitService.Update Error: {Environment.NewLine}");
                    return new RoadClassCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassCodeUnitResponse> Update(long ID, RoadClassCodeUnit roadClassCodeUnit)
        {
            try
            {
                _roadClassCodeUnitRepository.Update(ID, roadClassCodeUnit);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadClassCodeUnitResponse(roadClassCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassCodeUnitService.Update Error: {Environment.NewLine}");
                return new RoadClassCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
