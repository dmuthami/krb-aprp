using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;

namespace APRP.Web.Services
{
    public class WorkCategoryAllocationMatrixService : IWorkCategoryAllocationMatrixService
    {
        private readonly IWorkCategoryAllocationMatrixRepository _workCategoryAllocationMatrixRepository;
        private readonly IBudgetCeilingService _budgetCeilingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public WorkCategoryAllocationMatrixService(IWorkCategoryAllocationMatrixRepository allocationCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<WorkCategoryAllocationMatrixService> logger,
            IBudgetCeilingService budgetCeilingService)
        {
            _workCategoryAllocationMatrixRepository = allocationCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _budgetCeilingService = budgetCeilingService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixResponse> AddAsync(WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            try
            {
                await _workCategoryAllocationMatrixRepository.AddAsync(workCategoryAllocationMatrix).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkCategoryAllocationMatrixResponse(workCategoryAllocationMatrix); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.AddAsync Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixResponse> FindByAuthorityAndFinancialIdAsync(long AuthorityId, long FinancialYearId, long WorkCategoryId)
        {
            try
            {
                var workCategoryAllocationMatrix = await _workCategoryAllocationMatrixRepository.FindByAuthorityAndFinancialIdAsync(AuthorityId, FinancialYearId, WorkCategoryId).ConfigureAwait(false);
                if (workCategoryAllocationMatrix == null)
                {
                    return new WorkCategoryAllocationMatrixResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryAllocationMatrixResponse(workCategoryAllocationMatrix);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixResponse> FindByIdAsync(long ID)
        {
            try
            {
                var workCategoryAllocationMatrix = await _workCategoryAllocationMatrixRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (workCategoryAllocationMatrix == null)
                {
                    return new WorkCategoryAllocationMatrixResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryAllocationMatrixResponse(workCategoryAllocationMatrix);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<WorkCategoryAllocationMatrixResponse> FindByNameAsync(string Name)
        {
            try
            {
                var workCategoryAllocationMatrix = await _workCategoryAllocationMatrixRepository.FindByNameAsync(Name).ConfigureAwait(false);
                if (workCategoryAllocationMatrix == null)
                {
                    return new WorkCategoryAllocationMatrixResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryAllocationMatrixResponse(workCategoryAllocationMatrix);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixViewModelListResponse> GetAuthorityWorkCategoriesAsync(long AuthorityId, long FinancialYearId)
        {
            try
            {
                var workCategoryAllocationMatrix = await _workCategoryAllocationMatrixRepository.GetAuthorityWorkCategoriesAsync(AuthorityId, FinancialYearId).ConfigureAwait(false);
                IList<WorkCategoryAllocationMatrixViewModel> WorkCategoryAllocationMatrixViewModelEnum = new List<WorkCategoryAllocationMatrixViewModel>();
                foreach (var wrkCategoryAllocationMatrix in workCategoryAllocationMatrix)
                {
                    WorkCategoryAllocationMatrixViewModel workCategoryAllocationMatrixViewModel =
                        new WorkCategoryAllocationMatrixViewModel();
                    workCategoryAllocationMatrixViewModel.FinancialYearId = wrkCategoryAllocationMatrix.FinancialYearId;
                    workCategoryAllocationMatrixViewModel.AuthorityId = wrkCategoryAllocationMatrix.AuthorityId;
                    workCategoryAllocationMatrixViewModel.Percent = wrkCategoryAllocationMatrix.Percent;
                    workCategoryAllocationMatrixViewModel.WorkCategoryId = wrkCategoryAllocationMatrix.WorkCategoryId;

                    //Get budget headr amount
                    var budgetCeilingResp = await _budgetCeilingService.FindByAuthorityIDAndFinancialYearID(
                        wrkCategoryAllocationMatrix.AuthorityId,wrkCategoryAllocationMatrix.FinancialYearId).ConfigureAwait(false);
                    if (budgetCeilingResp.Success)
                    {
                        workCategoryAllocationMatrixViewModel.Amount = budgetCeilingResp.BudgetCeiling.Amount * wrkCategoryAllocationMatrix.Percent;
                    }
                    WorkCategoryAllocationMatrixViewModelEnum.Add(workCategoryAllocationMatrixViewModel);

                }
                if (workCategoryAllocationMatrix == null)
                {
                    return new WorkCategoryAllocationMatrixViewModelListResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryAllocationMatrixViewModelListResponse((IEnumerable<WorkCategoryAllocationMatrixViewModel>)WorkCategoryAllocationMatrixViewModelEnum);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixViewModelListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixListResponse> ListAsync()
        {
            try
            {
                var existingAllocationCodeUnits = await _workCategoryAllocationMatrixRepository.ListAsync().ConfigureAwait(false);
                if (existingAllocationCodeUnits == null)
                {
                    return new WorkCategoryAllocationMatrixListResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryAllocationMatrixListResponse(existingAllocationCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<WorkCategoryAllocationMatrixListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingAllocationCodeUnits = await _workCategoryAllocationMatrixRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingAllocationCodeUnits == null)
                {
                    return new WorkCategoryAllocationMatrixListResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryAllocationMatrixListResponse(existingAllocationCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixResponse> RemoveAsync(long ID)
        {
            var workCategoryAllocationMatrix = await _workCategoryAllocationMatrixRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (workCategoryAllocationMatrix == null)
            {
                return new WorkCategoryAllocationMatrixResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _workCategoryAllocationMatrixRepository.Remove(workCategoryAllocationMatrix);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new WorkCategoryAllocationMatrixResponse(workCategoryAllocationMatrix);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.RemoveAsync Error: {Environment.NewLine}");
                    return new WorkCategoryAllocationMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixResponse> Update(WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            var workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixRepository.FindByIdAsync(workCategoryAllocationMatrix.ID).ConfigureAwait(false);
            if (workCategoryAllocationMatrixResp == null)
            {
                return new WorkCategoryAllocationMatrixResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _workCategoryAllocationMatrixRepository.Update(workCategoryAllocationMatrixResp);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkCategoryAllocationMatrixResponse(workCategoryAllocationMatrixResp);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.Update Error: {Environment.NewLine}");
                    return new WorkCategoryAllocationMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryAllocationMatrixResponse> Update(long ID, WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            try
            {
                _workCategoryAllocationMatrixRepository.Update(ID, workCategoryAllocationMatrix);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkCategoryAllocationMatrixResponse(workCategoryAllocationMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryAllocationMatrixService.Update Error: {Environment.NewLine}");
                return new WorkCategoryAllocationMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
