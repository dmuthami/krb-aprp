using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadWorkOperationalActivitiesBudgetService : IRoadWorkOperationalActivitiesBudgetService
    {
        private readonly IRoadWorkOperationalActivitiesBudgetRepository _roadWorkOperationalActivitiesBudgetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadWorkOperationalActivitiesBudgetService(IRoadWorkOperationalActivitiesBudgetRepository roadWorkOperationalActivitiesBudgetRepository, IUnitOfWork unitOfWork,
            ILogger<RoadWorkOperationalActivitiesBudgetService> logger)
        {
            _roadWorkOperationalActivitiesBudgetRepository = roadWorkOperationalActivitiesBudgetRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkOperationalActivitiesBudgetResponse> AddAsync(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget)
        {
            try
            {
                await _roadWorkOperationalActivitiesBudgetRepository.AddAsync(roadWorkOperationalActivitiesBudget).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadWorkOperationalActivitiesBudgetResponse(roadWorkOperationalActivitiesBudget); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkOperationalActivitiesBudgetService.AddAsync Error: {Environment.NewLine}");
                return new RoadWorkOperationalActivitiesBudgetResponse($"Error occured while saving the budget activity line record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkOperationalActivitiesBudgetResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingroadWorkOperationalActivitiesBudget = await _roadWorkOperationalActivitiesBudgetRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingroadWorkOperationalActivitiesBudget == null)
                {
                    return new RoadWorkOperationalActivitiesBudgetResponse("Record Not Found");
                }
                else
                {
                    return new RoadWorkOperationalActivitiesBudgetResponse(existingroadWorkOperationalActivitiesBudget);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkOperationalActivitiesBudgetService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadWorkOperationalActivitiesBudgetResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<RoadWorkOperationalActivitiesBudget>> ListAsync(long headerId)
        {
            try
            {
                return await _roadWorkOperationalActivitiesBudgetRepository.ListAsync(headerId).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkOperationalActivitiesBudgetService.ListAsync Error: {Environment.NewLine}");
                return Enumerable.Empty<RoadWorkOperationalActivitiesBudget>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkOperationalActivitiesBudgetResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingroadWorkOperationalActivitiesBudget = await _roadWorkOperationalActivitiesBudgetRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingroadWorkOperationalActivitiesBudget == null)
                {
                    return new RoadWorkOperationalActivitiesBudgetResponse("Record Not Found");
                }
                else
                {
                    try
                    {
                        _roadWorkOperationalActivitiesBudgetRepository.Remove(existingroadWorkOperationalActivitiesBudget);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                        return new RoadWorkOperationalActivitiesBudgetResponse(existingroadWorkOperationalActivitiesBudget);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"RoadWorkOperationalActivitiesBudgetService.RemoveAsync Error: {Environment.NewLine}");
                        return new RoadWorkOperationalActivitiesBudgetResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkOperationalActivitiesBudgetService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadWorkOperationalActivitiesBudgetResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkOperationalActivitiesBudgetResponse> UpdateAsync(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget)
        {
            try
            {
                if (roadWorkOperationalActivitiesBudget == null)
                {
                    throw new NullReferenceException("roadWorkOperationalActivitiesBudget object is null");
                }
                var existingroadWorkOperationalActivitiesBudget = await _roadWorkOperationalActivitiesBudgetRepository.FindByIdAsync(roadWorkOperationalActivitiesBudget.ID).ConfigureAwait(false);
                if (existingroadWorkOperationalActivitiesBudget == null)
                {
                    return new RoadWorkOperationalActivitiesBudgetResponse("Record Not Found");
                }
                else
                {
                    existingroadWorkOperationalActivitiesBudget.FundingSourceId = roadWorkOperationalActivitiesBudget.FundingSourceId;
                    existingroadWorkOperationalActivitiesBudget.FundTypeId = roadWorkOperationalActivitiesBudget.FundTypeId;
                    existingroadWorkOperationalActivitiesBudget.OverHeadBudget = roadWorkOperationalActivitiesBudget.OverHeadBudget;
                    existingroadWorkOperationalActivitiesBudget.UpdateDate = DateTime.UtcNow;

                    try
                    {
                        _roadWorkOperationalActivitiesBudgetRepository.Update(existingroadWorkOperationalActivitiesBudget);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        return new RoadWorkOperationalActivitiesBudgetResponse(existingroadWorkOperationalActivitiesBudget);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"RoadWorkOperationalActivitiesBudgetService.UpdateAsync Error: {Environment.NewLine}");
                        return new RoadWorkOperationalActivitiesBudgetResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkOperationalActivitiesBudgetService.UpdateAsync Error: {Environment.NewLine}");
                return new RoadWorkOperationalActivitiesBudgetResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
