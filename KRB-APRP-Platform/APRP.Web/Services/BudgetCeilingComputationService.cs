using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class BudgetCeilingComputationService : IBudgetCeilingComputationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IBudgetCeilingComputationRepository _budgetCeilingComputationRepository;
        public BudgetCeilingComputationService(IUnitOfWork unitOfWork, ILogger<BudgetCeilingComputationService> logger,
            IBudgetCeilingComputationRepository budgetCeilingComputationRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _budgetCeilingComputationRepository = budgetCeilingComputationRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository.AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    }
                }
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> DetachFirstEntryAsync(BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository
                    .DetachFirstEntryAsync(budgetCeilingComputation).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the budgetCeilingComputation record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> FindByBudgetVoteEntryAsync(BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository.
                    FindByBudgetVoteEntryAsync(budgetCeilingComputation).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.FindByBudgetVoteListEntryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByCodeAndFinancialYearIdAsync(string Code, long FinancialYearID)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository.FindByCodeAndFinancialYearIdAsync(Code, FinancialYearID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository
                    .ListAsync().
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository
                    .ListAsync(FinancialYearId).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> ListAsync(string Code)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository
                    .ListAsync(Code).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        var iActionResult2 = await _budgetCeilingComputationRepository.Remove(budgetCeilingComputation).ConfigureAwait(false);

                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                            }
                        }
                        return new GenericResponse(iActionResult2); //successful
                    }
                    else
                    {
                        return new GenericResponse(iActionResult); //failure
                    }
                }
                else
                {
                    return new GenericResponse(iActionResult); //failure
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.BudgetCeilingComputation budgetCeilingComputation)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                var iActionResult = await _budgetCeilingComputationRepository.Update(ID, budgetCeilingComputation).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    }
                }
                return new GenericResponse(iActionResult); //failure or success
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}