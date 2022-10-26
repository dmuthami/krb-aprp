using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class BudgetCeilingService : IBudgetCeilingService
    {
        private readonly IBudgetCeilingRepository _budgetCeilingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public BudgetCeilingService(IBudgetCeilingRepository budgetCeilingRepository, IUnitOfWork unitOfWork, ILogger<BudgetCeilingService> logger)
        {
            _budgetCeilingRepository = budgetCeilingRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingResponse> AddAsync(BudgetCeiling budgetCeiling)
        {
            try
            {
                await _budgetCeilingRepository.AddAsync(budgetCeiling).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new BudgetCeilingResponse(budgetCeiling); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingService.AddAsync Error: {Environment.NewLine}");
                return new BudgetCeilingResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingResponse> FindApprovedByAuthorityForCurrentYear(long financialYear, long authorityID)
        {
            try
            {
                var existingRecord = await _budgetCeilingRepository.FindApprovedByAuthorityForCurrentYear(financialYear,authorityID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new BudgetCeilingResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingService.FindApprovedAuthorityForCurrentYear Error: {Environment.NewLine}");
                return new BudgetCeilingResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingResponse> FindByAuthorityIDAndFinancialYearID(long AuthorityID, long FinancialYearID)
        {
            try
            {
                var existingRecord = await _budgetCeilingRepository.FindByAuthorityIDAndFinancialYearID(AuthorityID, FinancialYearID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new BudgetCeilingResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingService.FindApprovedAuthorityForCurrentYear Error: {Environment.NewLine}");
                return new BudgetCeilingResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<BudgetCeilingResponse> FindByBudgetHeaderIDAndAuthority(long budgetHeaderID, long authorityID)
        {
            try
            {
                var existingRecord = await _budgetCeilingRepository.FindByBudgetHeaderIDAndAuthority(budgetHeaderID, authorityID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new BudgetCeilingResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingService.FindApprovedAuthorityForCurrentYear Error: {Environment.NewLine}");
                return new BudgetCeilingResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _budgetCeilingRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new BudgetCeilingResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingService.AddAsync Error: {Environment.NewLine}");
                return new BudgetCeilingResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<BudgetCeiling>> ListAsync()
        {
            return await _budgetCeilingRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<BudgetCeiling>> ListAsync(string AuthorityType, long FinancialYearID)
        {
            return await _budgetCeilingRepository.ListAsync(AuthorityType, FinancialYearID).ConfigureAwait(false);
        }

        public async Task<BudgetCeilingListResponse> RemoveAllAsync(long? BudgetCeilingHeaderID)
        {
            try
            {
                var existingBudgetCeilingList= await _budgetCeilingRepository.RemoveAllAsync(BudgetCeilingHeaderID).ConfigureAwait(false);
                if (existingBudgetCeilingList == null)
                {
                    return new BudgetCeilingListResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingListResponse(existingBudgetCeilingList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingService.AddAsync Error: {Environment.NewLine}");
                return new BudgetCeilingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _budgetCeilingRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new BudgetCeilingResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _budgetCeilingRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new BudgetCeilingResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"BudgetCeilingService.RemoveAsync Error: {Environment.NewLine}");
                    return new BudgetCeilingResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingResponse> Update(BudgetCeiling budgetCeiling)
        {
            var existingRecord = await _budgetCeilingRepository.FindByIdAsync(budgetCeiling.ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new BudgetCeilingResponse("Record Not Found");
            }
            else
            {
                existingRecord.Amount = budgetCeiling.Amount;
                existingRecord.AdditionalInfo = budgetCeiling.AdditionalInfo;
                existingRecord.AuthorityId = budgetCeiling.AuthorityId;
                existingRecord.UpdatedBy = "Kazi";
                existingRecord.UpdateDate = DateTime.UtcNow;

                try
                {
                    _budgetCeilingRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new BudgetCeilingResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"BudgetCeilingService.FindByAsync Error: {Environment.NewLine}");
                    return new BudgetCeilingResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        public Task<BudgetCeilingResponse> Update(FinancialYear financialYear)
        {
            throw new NotImplementedException();
        }

        public async Task<BudgetCeilingResponse> Update(long ID, BudgetCeiling budgetCeiling)
        {
            try
            {
                _budgetCeilingRepository.Update(ID, budgetCeiling);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new BudgetCeilingResponse(budgetCeiling);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.Update Error: {Environment.NewLine}");
                return new BudgetCeilingResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
