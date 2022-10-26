using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadConditionService : IRoadConditionService
    {
        private readonly IRoadConditionRepository _roadConditionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public RoadConditionService(IUnitOfWork unitOfWork, ILogger<RoadConditionService> logger,
            IRoadConditionRepository roadConditionRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _roadConditionRepository = roadConditionRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadCondtionResponse> AddAsync(RoadCondition roadCondtion)
        {
            try
            {
                await _roadConditionRepository.AddAsync(roadCondtion).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadCondtionResponse(roadCondtion); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.AddAsync Error: {Environment.NewLine}");
                return new RoadCondtionResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadCondtionResponse> DetachFirstEntryAsync(RoadCondition roadCondtion)
        {
            try
            {
                await _roadConditionRepository.DetachFirstEntryAsync(roadCondtion).ConfigureAwait(false);

                return new RoadCondtionResponse(roadCondtion); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new RoadCondtionResponse($"Error occured while saving the cSAllocation record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadCondtionResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoadCondition = await _roadConditionRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadCondition == null)
                {
                    return new RoadCondtionResponse("Record Not Found");
                }
                else
                {
                    return new RoadCondtionResponse(existingRoadCondition);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadCondtionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadCondtionResponse> FindByPriorityRateAsync(long AuthorityID, int? Year, long PriorityRate)
        {
            try
            {
                var existingRoadCondition = await _roadConditionRepository.FindByPriorityRateAsync(AuthorityID, Year,PriorityRate).ConfigureAwait(false);
                if (existingRoadCondition == null)
                {
                    return new RoadCondtionResponse("Record Not Found");
                }
                else
                {
                    return new RoadCondtionResponse(existingRoadCondition);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadCondtionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadCondtionResponse> FindByRoadIdAsync(long RoadID, int? Year)
        {
            try
            {
                var existingRoadCondition = await _roadConditionRepository.FindByRoadIdAsync(RoadID, Year).ConfigureAwait(false);
                if (existingRoadCondition == null)
                {
                    return new RoadCondtionResponse("Record Not Found");
                }
                else
                {
                    return new RoadCondtionResponse(existingRoadCondition);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadCondtionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadCondtionResponse> GetRoadConditionByYear(Road road, int? Year)
        {
            try
            {
                var existingRoadCondition = await _roadConditionRepository.GetRoadConditionByYear(road,Year).ConfigureAwait(false);
                if (existingRoadCondition == null)
                {
                    return new RoadCondtionResponse("Record Not Found");
                }
                else
                {
                    return new RoadCondtionResponse(existingRoadCondition);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadCondtionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionListResponse> ListAsync()
        {
            try
            {
                var roadConditionList = await _roadConditionRepository.ListAsync().ConfigureAwait(false);

                if (roadConditionList == null)
                {
                    return new RoadConditionListResponse("Record Not Found");
                }
                else
                {
                    return new RoadConditionListResponse(roadConditionList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.ListAsync Error: {Environment.NewLine}");
                return new RoadConditionListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionListResponse> ListAsync(int? Year)
        {
            try
            {
                var roadConditionList = await _roadConditionRepository.ListAsync(Year).ConfigureAwait(false);

                if (roadConditionList == null)
                {
                    return new RoadConditionListResponse("Record Not Found");
                }
                else
                {
                    return new RoadConditionListResponse(roadConditionList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.ListAsync Error: {Environment.NewLine}");
                return new RoadConditionListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadConditionListResponse> ListAsync(Authority authority, int? Year)
        {
            try
            {
                var roadConditionList = await _roadConditionRepository.ListAsync(authority,Year).ConfigureAwait(false);

                if (roadConditionList == null)
                {
                    return new RoadConditionListResponse("Record Not Found");
                }
                else
                {
                    return new RoadConditionListResponse(roadConditionList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.ListAsync Error: {Environment.NewLine}");
                return new RoadConditionListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadCondtionResponse> RemoveAsync(long ID)
        {
            var existingRoad = await _roadConditionRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRoad == null)
            {
                return new RoadCondtionResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadConditionRepository.Remove(existingRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadCondtionResponse(existingRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadConditionService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadCondtionResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadCondtionResponse> Update(long ID, RoadCondition roadCondtion)
        {
            try
            {
                _roadConditionRepository.Update(ID, roadCondtion);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadCondtionResponse(roadCondtion);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadConditionService.Update Error: {Environment.NewLine}");
                return new RoadCondtionResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
