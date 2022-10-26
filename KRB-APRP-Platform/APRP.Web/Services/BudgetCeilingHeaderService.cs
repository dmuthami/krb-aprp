using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class BudgetCeilingHeaderService : IBudgetCeilingHeaderService
    {
        private readonly IBudgetCeilingHeaderRepository _budgetCeilingHeaderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public BudgetCeilingHeaderService(IBudgetCeilingHeaderRepository budgetCeilingHeaderRepository, IUnitOfWork unitOfWork, ILogger<BudgetCeilingHeaderService> logger/**/)
        {
            _budgetCeilingHeaderRepository = budgetCeilingHeaderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingHeaderResponse> AddAsync(BudgetCeilingHeader budgetCeilingHeader)
        {
            try
            {
                await _budgetCeilingHeaderRepository.AddAsync(budgetCeilingHeader).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new BudgetCeilingHeaderResponse(budgetCeilingHeader); //successful
            }
            catch (Exception Ex)
            {
                //exception logging
                _logger.LogError(Ex, $"BudgetCeilingHeaderService.AddAsync Error: {Environment.NewLine}");
                return new BudgetCeilingHeaderResponse($"Error occured while saving the budget ceiling header record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingHeaderResponse> FindByFinancialYearAsync(long FinancialYearID)
        {
            try
            {
                var existingRecord = await _budgetCeilingHeaderRepository.FindByFinancialYearAsync(FinancialYearID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new BudgetCeilingHeaderResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingHeaderResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingHeaderService.FindByIdAsync Error: {Environment.NewLine}");
                //exception logging
                return new BudgetCeilingHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingHeaderResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _budgetCeilingHeaderRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new BudgetCeilingHeaderResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingHeaderResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingHeaderService.FindByIdAsync Error: {Environment.NewLine}");
                //exception logging
                return new BudgetCeilingHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingHeaderResponse> FindCurrentAsync()
        {
            try
            {
                var existingRecord = await _budgetCeilingHeaderRepository.FindCurrentAsync().ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new BudgetCeilingHeaderResponse("Record Not Found");
                }
                else
                {
                    return new BudgetCeilingHeaderResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingHeaderService.FindByCurrentAsync Error: {Environment.NewLine}");
                //exception logging
                return new BudgetCeilingHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<BudgetCeilingHeader>> ListAsync()
        {
            return await _budgetCeilingHeaderRepository.ListAsync().ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingHeaderResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _budgetCeilingHeaderRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new BudgetCeilingHeaderResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _budgetCeilingHeaderRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new BudgetCeilingHeaderResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    //exception logging
                    _logger.LogError(Ex, $"BudgetCeilingHeaderService.RemoveAsync Error: {Environment.NewLine}");
                    return new BudgetCeilingHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingHeaderResponse> Update(BudgetCeilingHeader budgetCeilingHeader)
        {
            var existingRecord = await _budgetCeilingHeaderRepository.FindByIdAsync(budgetCeilingHeader.ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new BudgetCeilingHeaderResponse("Record Not Found");
            }
            else
            {
                existingRecord.ApprovalComment = budgetCeilingHeader.ApprovalComment;
                existingRecord.ApprovalDate = budgetCeilingHeader.ApprovalDate;
                existingRecord.ApprovalStatus = budgetCeilingHeader.ApprovalStatus;
                existingRecord.ApprovedBy = budgetCeilingHeader.ApprovedBy;
                existingRecord.CreatedBy = budgetCeilingHeader.CreatedBy;
                existingRecord.CreationDate = budgetCeilingHeader.CreationDate;
                existingRecord.FinancialYear = budgetCeilingHeader.FinancialYear;
                existingRecord.SubmissionDate = budgetCeilingHeader.SubmissionDate;
                existingRecord.SubmittedBy = budgetCeilingHeader.SubmittedBy;
                existingRecord.TotalAmount = budgetCeilingHeader.TotalAmount;               

                try
                {
                    _budgetCeilingHeaderRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new BudgetCeilingHeaderResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    //exception logging
                    _logger.LogError(Ex, $"BudgetCeilingHeaderService.Update Error: {Environment.NewLine}");
                    return new BudgetCeilingHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<BudgetCeilingHeaderResponse> Update(long ID, BudgetCeilingHeader budgetCeilingHeader)
        {
            try
            {
                _budgetCeilingHeaderRepository.Update(ID, budgetCeilingHeader);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new BudgetCeilingHeaderResponse(budgetCeilingHeader);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.Update Error: {Environment.NewLine}");
                return new BudgetCeilingHeaderResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
