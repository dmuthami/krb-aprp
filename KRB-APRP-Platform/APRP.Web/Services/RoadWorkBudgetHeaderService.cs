using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadWorkBudgetHeaderService : IRoadWorkBudgetHeaderService
    {
        private readonly IRoadWorkBudgetHeaderRepository _roadWorkBudgetHeaderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadWorkBudgetHeaderService(IRoadWorkBudgetHeaderRepository roadWorkBudgetHeaderRepository, IUnitOfWork unitOfWork, ILogger<RoadWorkBudgetHeaderService> logger)
        {
            _roadWorkBudgetHeaderRepository = roadWorkBudgetHeaderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetHeaderResponse> AddAsync(RoadWorkBudgetHeader roadWorkBudgetHeader)
        {
            try
            {
                await _roadWorkBudgetHeaderRepository.AddAsync(roadWorkBudgetHeader).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadWorkBudgetHeaderResponse(roadWorkBudgetHeader); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.AddAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetHeaderResponse($"Error occured while saving the budget header record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetHeaderResponse> FindByAuthorityIdForCurrentYear(long yearId, long authorityId)
        {
            try
            {
                var existingRoadWorkBudgetHeader = await _roadWorkBudgetHeaderRepository.FindByAuthorityIdForCurrentYear(yearId,authorityId).ConfigureAwait(false);
                if (existingRoadWorkBudgetHeader == null)
                {
                    return new RoadWorkBudgetHeaderResponse("Record Not Found");
                }
                else
                {
                    return new RoadWorkBudgetHeaderResponse(existingRoadWorkBudgetHeader);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear Error: {Environment.NewLine}");
                return new RoadWorkBudgetHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadWorkBudgetHeaderResponse> FindByFinancialYearIdAndAuthorityID(long financialYearId, long authorityID)
        {
            try
            {
                var existingRoadWorkBudgetHeader = await _roadWorkBudgetHeaderRepository.FindByFinancialYearIdAndAuthorityID(financialYearId, authorityID).ConfigureAwait(false);
                if (existingRoadWorkBudgetHeader == null)
                {
                    return new RoadWorkBudgetHeaderResponse("Record Not Found");
                }
                else
                {
                    return new RoadWorkBudgetHeaderResponse(existingRoadWorkBudgetHeader);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear Error: {Environment.NewLine}");
                return new RoadWorkBudgetHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetHeaderResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoadWorkBudgetHeader = await _roadWorkBudgetHeaderRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadWorkBudgetHeader == null)
                {
                    return new RoadWorkBudgetHeaderResponse("Record Not Found");
                }
                else
                {
                    return new RoadWorkBudgetHeaderResponse(existingRoadWorkBudgetHeader);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<RoadWorkBudgetHeader>> ListAsync()
        {
            try
            {
                return await _roadWorkBudgetHeaderRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.ListAsync Error: {Environment.NewLine}");
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetHeaderResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRoadWorkBudgetHeader = await _roadWorkBudgetHeaderRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadWorkBudgetHeader == null)
                {
                    return new RoadWorkBudgetHeaderResponse("Record Not Found");
                }
                else
                {
                    try
                    {
                        _roadWorkBudgetHeaderRepository.Remove(existingRoadWorkBudgetHeader);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                        return new RoadWorkBudgetHeaderResponse(existingRoadWorkBudgetHeader);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.RemoveAsync Error: {Environment.NewLine}");
                        return new RoadWorkBudgetHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadWorkBudgetHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkBudgetHeaderResponse> Update(RoadWorkBudgetHeader roadWorkBudgetHeader)
        {
            try
            {
                var existingRoadWorkBudgetHeader = await _roadWorkBudgetHeaderRepository.FindByIdAsync(roadWorkBudgetHeader.ID).ConfigureAwait(false);
                if (existingRoadWorkBudgetHeader == null)
                {
                    return new RoadWorkBudgetHeaderResponse("Record Not Found");
                }
                else
                {
                    existingRoadWorkBudgetHeader.Code = roadWorkBudgetHeader.Code;
                    existingRoadWorkBudgetHeader.FinancialYear = roadWorkBudgetHeader.FinancialYear;
                    existingRoadWorkBudgetHeader.Summary = roadWorkBudgetHeader.Summary;
                    try
                    {
                        _roadWorkBudgetHeaderRepository.Update(existingRoadWorkBudgetHeader);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        return new RoadWorkBudgetHeaderResponse(existingRoadWorkBudgetHeader);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.Update Error: {Environment.NewLine}");
                        return new RoadWorkBudgetHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkBudgetHeaderService.Update Error: {Environment.NewLine}");
                return new RoadWorkBudgetHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
