using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadConditionCodeUnitService : IRoadConditionCodeUnitService
    {
        private readonly IRoadConditionCodeUnitRepository _roadConditionCodeUnitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadConditionCodeUnitService(IRoadConditionCodeUnitRepository roadConditionCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<RoadConditionCodeUnitService> logger)
        {
            _roadConditionCodeUnitRepository = roadConditionCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionCodeUnitResponse> AddAsync(RoadConditionCodeUnit roadConditionCodeUnit)
        {
            try
            {
                await _roadConditionCodeUnitRepository.AddAsync(roadConditionCodeUnit).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadConditionCodeUnitResponse(roadConditionCodeUnit); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionCodeUnitService.AddAsync Error: {Environment.NewLine}");
                return new RoadConditionCodeUnitResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionCodeUnitResponse> FindBySurfaceTypeIdAsync(long SurfaceTypeId)
        {
            try
            {
                var existingAllocationCodeUnit = await _roadConditionCodeUnitRepository.FindBySurfaceTypeIdAsync(SurfaceTypeId).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new RoadConditionCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RoadConditionCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RoadConditionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionCodeUnitResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingAllocationCodeUnit = await _roadConditionCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new RoadConditionCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RoadConditionCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RoadConditionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadConditionCodeUnitResponse> FindByRoadConditionAsync(string RoadCondition)
        {
            try
            {
                var existingAllocationCodeUnit = await _roadConditionCodeUnitRepository.FindByRoadConditionAsync(RoadCondition).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new RoadConditionCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new RoadConditionCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RoadConditionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionCodeUnitListResponse> ListAsync()
        {
            try
            {
                var existingAllocationCodeUnits = await _roadConditionCodeUnitRepository.ListAsync().ConfigureAwait(false);
                if (existingAllocationCodeUnits == null)
                {
                    return new RoadConditionCodeUnitListResponse("Records Not Found");
                }
                else
                {
                    return new RoadConditionCodeUnitListResponse(existingAllocationCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new RoadConditionCodeUnitListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionCodeUnitResponse> RemoveAsync(long ID)
        {
            var existingAllocationCodeUnit = await _roadConditionCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingAllocationCodeUnit == null)
            {
                return new RoadConditionCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadConditionCodeUnitRepository.Remove(existingAllocationCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadConditionCodeUnitResponse(existingAllocationCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadConditionCodeUnitService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadConditionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionCodeUnitResponse> Update(RoadConditionCodeUnit roadConditionCodeUnit)
        {
            var existingAllocationCodeUnit = await _roadConditionCodeUnitRepository.FindByIdAsync(roadConditionCodeUnit.ID).ConfigureAwait(false);
            if (existingAllocationCodeUnit == null)
            {
                return new RoadConditionCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadConditionCodeUnitRepository.Update(roadConditionCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadConditionCodeUnitResponse(roadConditionCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadConditionCodeUnitService.Update Error: {Environment.NewLine}");
                    return new RoadConditionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionCodeUnitResponse> Update(long ID, RoadConditionCodeUnit roadConditionCodeUnit)
        {
            try
            {
                _roadConditionCodeUnitRepository.Update(ID, roadConditionCodeUnit);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadConditionCodeUnitResponse(roadConditionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionCodeUnitService.Update Error: {Environment.NewLine}");
                return new RoadConditionCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
