using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadPrioritizationService : IRoadPrioritizationService
    {
        private readonly IRoadPrioritizationRepository _roadPrioritizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadPrioritizationService(IRoadPrioritizationRepository roadPrioritizationRepository, IUnitOfWork unitOfWork
            , ILogger<RoadPrioritizationService> logger)
        {
            _roadPrioritizationRepository = roadPrioritizationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadPrioritizationResponse> AddAsync(RoadPrioritization roadPrioritization)
        {
            try
            {
                await _roadPrioritizationRepository.AddAsync(roadPrioritization).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadPrioritizationResponse(roadPrioritization); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadPrioritizationService.AddAsync Error: {Environment.NewLine}");
                return new RoadPrioritizationResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadPrioritizationResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoadPrioritization = await _roadPrioritizationRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadPrioritization == null)
                {
                    return new RoadPrioritizationResponse("Records Not Found");
                }
                else
                {
                    return new RoadPrioritizationResponse(existingRoadPrioritization);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadPrioritizationService.ListAsync Error: {Environment.NewLine}");
                return new RoadPrioritizationResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadPrioritizationResponse> FindByNameAsync(string Code)
        {
            try
            {
                var existingRoadPrioritization = await _roadPrioritizationRepository.FindByNameAsync(Code).ConfigureAwait(false);
                if (existingRoadPrioritization == null)
                {
                    return new RoadPrioritizationResponse("Records Not Found");
                }
                else
                {
                    return new RoadPrioritizationResponse(existingRoadPrioritization);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadPrioritizationService.ListAsync Error: {Environment.NewLine}");
                return new RoadPrioritizationResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadPrioritizationListResponse> ListAsync()
        {
            try
            {
                var existingRoadPrioritizations = await _roadPrioritizationRepository.ListAsync().ConfigureAwait(false);
                if (existingRoadPrioritizations == null)
                {
                    return new RoadPrioritizationListResponse("Records Not Found");
                }
                else
                {
                    return new RoadPrioritizationListResponse(existingRoadPrioritizations);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadPrioritizationService.ListAsync Error: {Environment.NewLine}");
                return new RoadPrioritizationListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadPrioritizationResponse> RemoveAsync(long ID)
        {
            var existingRoadPrioritization = await _roadPrioritizationRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRoadPrioritization == null)
            {
                return new RoadPrioritizationResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadPrioritizationRepository.Remove(existingRoadPrioritization);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadPrioritizationResponse(existingRoadPrioritization);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadPrioritizationService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadPrioritizationResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadPrioritizationResponse> Update(RoadPrioritization roadPrioritization)
        {
            var existingRoadPrioritization = await _roadPrioritizationRepository.FindByIdAsync(roadPrioritization.ID).ConfigureAwait(false);
            if (existingRoadPrioritization == null)
            {
                return new RoadPrioritizationResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadPrioritizationRepository.Update(roadPrioritization);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadPrioritizationResponse(roadPrioritization);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadPrioritizationService.Update Error: {Environment.NewLine}");
                    return new RoadPrioritizationResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadPrioritizationResponse> Update(long ID, RoadPrioritization roadPrioritization)
        {
            try
            {
                _roadPrioritizationRepository.Update(ID, roadPrioritization);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadPrioritizationResponse(roadPrioritization);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadPrioritizationService.Update Error: {Environment.NewLine}");
                return new RoadPrioritizationResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
