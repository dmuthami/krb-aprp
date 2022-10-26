using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadWorkBudgetLineService : IRoadWorkBudgetLineService
    {
        private readonly IRoadWorkBudgetLineRepository _roadWorkBudgetLineRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadWorkBudgetLineService(IRoadWorkBudgetLineRepository roadWorkBudgetLineRepository, IUnitOfWork unitOfWork, ILogger<RoadWorkBudgetLineService> logger)
        {
            _roadWorkBudgetLineRepository = roadWorkBudgetLineRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetLineResponse> AddAsync(RoadWorkBudgetLine roadWorkBudgetLine)
        {
            try
            {
                await _roadWorkBudgetLineRepository.AddAsync(roadWorkBudgetLine).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadWorkBudgetLineResponse(roadWorkBudgetLine); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetLineService.AddAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetLineResponse($"Error occured while saving the budget line record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetLineResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoadWorkBudgetLine = await _roadWorkBudgetLineRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadWorkBudgetLine == null)
                {
                    return new RoadWorkBudgetLineResponse("Record Not Found");
                }
                else
                {
                    return new RoadWorkBudgetLineResponse(existingRoadWorkBudgetLine);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetLineService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetLineResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadWorkBudgetLineResponse> FindByRoadWorkBudgetHeaderIdAsync(long RoadWorkBudgetHeaderId)
        {
            try
            {
                var existingRoadWorkBudgetLine = await _roadWorkBudgetLineRepository.FindByRoadWorkBudgetHeaderIdAsync(RoadWorkBudgetHeaderId).ConfigureAwait(false);
                if (existingRoadWorkBudgetLine == null)
                {
                    return new RoadWorkBudgetLineResponse("Record Not Found");
                }
                else
                {
                    return new RoadWorkBudgetLineResponse(existingRoadWorkBudgetLine);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetLineService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetLineResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<RoadWorkBudgetLine>> ListAsync(long HeaderId)
        {
            try
            {
                return await _roadWorkBudgetLineRepository.ListAsync(HeaderId).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkBudgetLineService.ListAsync Error: {Environment.NewLine}");
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetLineResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRoadWorkBudgetLine = await _roadWorkBudgetLineRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadWorkBudgetLine == null)
                {
                    return new RoadWorkBudgetLineResponse("Record Not Found");
                }
                else
                {
                    try
                    {
                        _roadWorkBudgetLineRepository.Remove(existingRoadWorkBudgetLine);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                        return new RoadWorkBudgetLineResponse(existingRoadWorkBudgetLine);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"RoadWorkBudgetLineService.RemoveAsync Error: {Environment.NewLine}");
                        return new RoadWorkBudgetLineResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkBudgetLineService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetLineResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetLineResponse> UpdateAsync(RoadWorkBudgetLine roadWorkBudgetLine)
        {
            try
            {
                var existingRoadWorkBudgetLine = await _roadWorkBudgetLineRepository.FindByIdAsync(roadWorkBudgetLine.ID).ConfigureAwait(false);
                if (existingRoadWorkBudgetLine == null)
                {
                    return new RoadWorkBudgetLineResponse("Record Not Found");
                }
                else
                {
                    existingRoadWorkBudgetLine.FundingSourceId = roadWorkBudgetLine.FundingSourceId;
                    existingRoadWorkBudgetLine.FundTypeId = roadWorkBudgetLine.FundTypeId;
                    existingRoadWorkBudgetLine.PeriodicMentanance = roadWorkBudgetLine.PeriodicMentanance;
                    existingRoadWorkBudgetLine.RehabilitationWork = roadWorkBudgetLine.RehabilitationWork;
                    existingRoadWorkBudgetLine.RoutineMaintanance = roadWorkBudgetLine.RoutineMaintanance;
                    existingRoadWorkBudgetLine.SpotImprovement = roadWorkBudgetLine.SpotImprovement;
                    existingRoadWorkBudgetLine.StructureConstruction = roadWorkBudgetLine.StructureConstruction;
                    existingRoadWorkBudgetLine.Total = roadWorkBudgetLine.Total;
                    existingRoadWorkBudgetLine.UpdatedBy = roadWorkBudgetLine.UpdatedBy;

                    try
                    {
                        _roadWorkBudgetLineRepository.Update(existingRoadWorkBudgetLine);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        return new RoadWorkBudgetLineResponse(existingRoadWorkBudgetLine);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"RoadWorkBudgetLineService.UpdateAsync Error: {Environment.NewLine}");
                        return new RoadWorkBudgetLineResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetLineService.UpdateAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetLineResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
