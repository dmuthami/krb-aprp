using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class DisbursementService :ControllerBase, IDisbursementService
    {
        private readonly IDisbursementRepository _disbursementRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IFinancialYearService _financialYearService;
        private readonly IAllocationCodeUnitService _allocationCodeUnitService;
        private readonly IBudgetCeilingService _budgetCeilingService;

        public DisbursementService(IDisbursementRepository aRICSRepository, IUnitOfWork unitOfWork
            , ILogger<DisbursementService> logger, IFinancialYearService financialYearService,
            IAllocationCodeUnitService allocationCodeUnitService, IBudgetCeilingService budgetCeilingService)
        {
            _disbursementRepository = aRICSRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _financialYearService = financialYearService;
            _allocationCodeUnitService = allocationCodeUnitService;
            _budgetCeilingService = budgetCeilingService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementResponse> AddAsync(Disbursement disbursement)
        {
            try
            {
                await _disbursementRepository.AddAsync(disbursement).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new DisbursementResponse(disbursement); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.AddAsync Error: {Environment.NewLine}");
                return new DisbursementResponse($"Error occured while saving the disbursement record : {Ex.Message}");
            }
        }

        public async Task<DisbursementResponse> DetachFirstEntryAsync(Disbursement disbursement)
        {
            try
            {
                await _disbursementRepository.DetachFirstEntryAsync(disbursement).ConfigureAwait(false);

                return new DisbursementResponse(disbursement); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new DisbursementResponse($"Error occured while saving the disbursement record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<double> DisbursementItemSum(IList<Disbursement> disbursementCollectionList)
        {
            try
            {
                if (disbursementCollectionList == null)
                {
                    return 0d;
                }
                else
                {
                    //Do a computation
                    return await Task.Run(() =>
                    {
                        double sum = 0.0;
                        sum = (disbursementCollectionList.Sum(item => item.Amount));
                        return sum;
                    }).ConfigureAwait(false);

                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.RevenueCollectionSum Error: {Environment.NewLine}");
                return 0d;
            }
        }

        public async Task<GenericResponse> DisbursementSummaryAsync(long FinancialYearId)
        {
            try
            {
                var iActionResult = await _disbursementRepository.DisbursementSummaryAsync(FinancialYearId).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.DisbursementSummaryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursement record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> DisbursementSummaryByBudgetCeilingComputationAsync(long FinancialYearId)
        {
            try
            {
                var iActionResult = await _disbursementRepository.DisbursementSummaryByBudgetCeilingComputationAsync(FinancialYearId).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.DisbursementSummaryByBudgetCeilingComputationAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursement record : {Ex.Message}");
            }
        }

        public async Task<DisbursementResponse> FindByDisbursementEntryAsync(Disbursement disbursement)
        {
            try
            {
                var existingDisbursement = await _disbursementRepository.FindByDisbursementEntryAsync(disbursement).ConfigureAwait(false);
                if (existingDisbursement == null)
                {
                    return new DisbursementResponse("Records Not Found");
                }
                else
                {
                    return new DisbursementResponse(existingDisbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingDisbursement = await _disbursementRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingDisbursement == null)
                {
                    return new DisbursementResponse("Records Not Found");
                }
                else
                {
                    return new DisbursementResponse(existingDisbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementListResponse> ListAsync()
        {
            try
            {
                var existingDisbursements = await _disbursementRepository.ListAsync().ConfigureAwait(false);
                if (existingDisbursements == null)
                {
                    return new DisbursementListResponse("Records Not Found");
                }
                else
                {
                    return new DisbursementListResponse(existingDisbursements);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingDisbursementItem = await _disbursementRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingDisbursementItem == null)
                {
                    return new DisbursementListResponse("Records Not Found");
                }
                else
                {
                    return new DisbursementListResponse(existingDisbursementItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<DisbursementListResponse> ListAsync(long AuthorityId, long FinancialYearId)
        {
            try
            {
                var existingDisbursementItem = await _disbursementRepository.ListAsync(AuthorityId,FinancialYearId).ConfigureAwait(false);
                if (existingDisbursementItem == null)
                {
                    return new DisbursementListResponse("Records Not Found");
                }
                else
                {
                    return new DisbursementListResponse(existingDisbursementItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementResponse> RemoveAsync(long ID)
        {
            var existingDisbursement = await _disbursementRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingDisbursement == null)
            {
                return new DisbursementResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _disbursementRepository.Remove(existingDisbursement);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new DisbursementResponse(existingDisbursement);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"DisbursementService.RemoveAsync Error: {Environment.NewLine}");
                    return new DisbursementResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        public async Task<GenericResponse> SummarizeByFinancialYearIDAndAuthorityIDAsync(long FinancialYearId)
        {
            try
            {
                IActionResult actionResult = null;
                var resp = await DisbursementSummaryAsync(FinancialYearId).ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var anon = result.Value;
                            actionResult = await ReadAnonymousType(anon).ConfigureAwait(false);
                        }
                    }

                }
                if (actionResult==null)
                {
                    return new GenericResponse($"Error occured while retrieving the disbursement record : {actionResult}");
                }
                else
                {
                    return new GenericResponse(actionResult);
                }
             
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception Ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {

                _logger.LogError(Ex, $"DisbursementService.SummarizeByFinancialYearIDAndAuthorityIDAsync Error: {Environment.NewLine}");
                return  new GenericResponse($"Error occured while saving the disbursement record : {Ex.Message}"); 
            }
        }

        private async Task<IActionResult> ReadAnonymousType(dynamic anonymousobject)
        {
            IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModels1 = new List<DisbursementSummaryViewModel>();
            IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)disbursementSummaryViewModels1;

            //Iterate through a list of anonymous objects in C#
            foreach (var obj in anonymousobject)
            {
                DisbursementSummaryViewModel disbursementSummaryViewModel = new DisbursementSummaryViewModel();
                var FinancialYearId = obj.GetType().GetProperty("FinancialYearId").GetValue(obj);
                var AuthorityId = obj.GetType().GetProperty("AuthorityId").GetValue(obj);
                var Count = obj.GetType().GetProperty("Count").GetValue(obj);
                var TotalDisbursement = obj.GetType().GetProperty("TotalDisbursement").GetValue(obj);

                int x;
                bool result = int.TryParse(Count.ToString(), out x);
                disbursementSummaryViewModel.Count = x;

                long y;
                result = long.TryParse(AuthorityId.ToString(), out y);
                disbursementSummaryViewModel.AuthorityId = y;

                result = long.TryParse(FinancialYearId.ToString(), out y);
                disbursementSummaryViewModel.FinancialYearId = y;

                double z;
                result = double.TryParse(TotalDisbursement.ToString(), out z);
                disbursementSummaryViewModel.TotalDisbursement = z;

                //Get finanical year
                var finacialResp = await _financialYearService.FindByIdAsync(disbursementSummaryViewModel.FinancialYearId).ConfigureAwait(false);
                if (finacialResp.Success)
                {
                    disbursementSummaryViewModel.FinancialYear = finacialResp.FinancialYear;
                    //Get percent
                    if (disbursementSummaryViewModel.Authority != null)
                    {

                        var respAllocationcodeUnit = await _allocationCodeUnitService.FindByAuthorityAsync(disbursementSummaryViewModel.Authority.ID).ConfigureAwait(false);
                        if (respAllocationcodeUnit.Success)
                        {
                            disbursementSummaryViewModel.Percent = respAllocationcodeUnit.AllocationCodeUnit.Percent;
                        }
                        //Get Annual ceiling for financial year
                        var respBudgetCeiling = await _budgetCeilingService.FindByAuthorityIDAndFinancialYearID
                            (disbursementSummaryViewModel.Authority.ID, disbursementSummaryViewModel.FinancialYear.ID)
                            .ConfigureAwait(false);
                        if (respBudgetCeiling.Success)
                        {
                            disbursementSummaryViewModel.AnnualCeiling = respBudgetCeiling.BudgetCeiling.Amount;
                            //Compute percent ceiling
                            disbursementSummaryViewModel.PercentOfCeiling = disbursementSummaryViewModel.TotalDisbursement / disbursementSummaryViewModel.AnnualCeiling;
                        }
                    }
                }
                disbursementSummaryViewModels.Add(disbursementSummaryViewModel);
            }
            return Ok(disbursementSummaryViewModels);
        }

        private async Task<IActionResult> ReadAnonymousDisbursementTrancheIDType(dynamic anonymousobject)
        {
            IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModels1 = new List<DisbursementSummaryViewModel>();
            IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)disbursementSummaryViewModels1;

            //Iterate through a list of anonymous objects in C#
            foreach (var obj in anonymousobject)
            {
                DisbursementSummaryViewModel disbursementSummaryViewModel = new DisbursementSummaryViewModel();
                var FinancialYearId = obj.GetType().GetProperty("FinancialYearId").GetValue(obj);
                var AuthorityId = obj.GetType().GetProperty("AuthorityId").GetValue(obj);
                var Count = obj.GetType().GetProperty("Count").GetValue(obj);
                var DisbursementTrancheAmount = obj.GetType().GetProperty("DisbursementTrancheAmount").GetValue(obj);
                var DisbursementTrancheId = obj.GetType().GetProperty("DisbursementTrancheId").GetValue(obj);

                int x;
                bool result = int.TryParse(Count.ToString(), out x);
                disbursementSummaryViewModel.Count = x;

                long y;
                result = long.TryParse(AuthorityId.ToString(), out y);
                disbursementSummaryViewModel.AuthorityId = y;

                result = long.TryParse(FinancialYearId.ToString(), out y);
                disbursementSummaryViewModel.FinancialYearId = y;

                double z;
                result = double.TryParse(DisbursementTrancheAmount.ToString(), out z);
                disbursementSummaryViewModel.DisbursementTrancheAmount = z;

                result = long.TryParse(DisbursementTrancheId.ToString(), out y);
                disbursementSummaryViewModel.DisbursementTrancheId = y;

                //Get finanical year
                var finacialResp = await _financialYearService.FindByIdAsync(disbursementSummaryViewModel.FinancialYearId).ConfigureAwait(false);
                if (finacialResp.Success)
                {
                    disbursementSummaryViewModel.FinancialYear = finacialResp.FinancialYear;
                    //Get percent
                    if (disbursementSummaryViewModel.Authority != null)
                    {

                        var respAllocationcodeUnit = await _allocationCodeUnitService.FindByAuthorityAsync(disbursementSummaryViewModel.Authority.ID).ConfigureAwait(false);
                        if (respAllocationcodeUnit.Success)
                        {
                            disbursementSummaryViewModel.Percent = respAllocationcodeUnit.AllocationCodeUnit.Percent;
                        }
                        //Get Annual ceiling for financial year
                        var respBudgetCeiling = await _budgetCeilingService.FindByAuthorityIDAndFinancialYearID
                            (disbursementSummaryViewModel.Authority.ID, disbursementSummaryViewModel.FinancialYear.ID)
                            .ConfigureAwait(false);
                        if (respBudgetCeiling.Success)
                        {
                            disbursementSummaryViewModel.AnnualCeiling = respBudgetCeiling.BudgetCeiling.Amount;
                            //Compute percent ceiling
                            disbursementSummaryViewModel.PercentOfCeiling = disbursementSummaryViewModel.TotalDisbursement / disbursementSummaryViewModel.AnnualCeiling;
                        }
                    }
                }
                disbursementSummaryViewModels.Add(disbursementSummaryViewModel);
            }
            return Ok(disbursementSummaryViewModels);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementResponse> Update(Disbursement disbursement)
        {
            var existingDisbursement = await _disbursementRepository.FindByIdAsync(disbursement.ID).ConfigureAwait(false);
            if (existingDisbursement == null)
            {
                return new DisbursementResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _disbursementRepository.Update(disbursement);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new DisbursementResponse(disbursement);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"DisbursementService.Update Error: {Environment.NewLine}");
                    return new DisbursementResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementResponse> Update(long ID, Disbursement disbursement)
        {
            try
            {
                _disbursementRepository.Update(ID, disbursement);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new DisbursementResponse(disbursement);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.Update Error: {Environment.NewLine}");
                return new DisbursementResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<DisbursementResponse> FindByAuthorityIdAndFinancialYearIdAsync(long AuthorityId, long FinancialYearId)
        {
            try
            {
                var existingDisbursement = await _disbursementRepository.FindByAuthorityIdAndFinancialYearIdAsync(AuthorityId, FinancialYearId).ConfigureAwait(false);
                if (existingDisbursement == null)
                {
                    return new DisbursementResponse("Records Not Found");
                }
                else
                {
                    return new DisbursementResponse(existingDisbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(long FinancialYearId)
        {
            try
            {
                IActionResult actionResult = null;
                var resp = await DisbursementSummaryByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(FinancialYearId).ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var anon = result.Value;
                            actionResult = await ReadAnonymousDisbursementTrancheIDType(anon).ConfigureAwait(false);
                        }
                    }
                }
                if (actionResult == null)
                {
                    return new GenericResponse($"Error occured while retrieving the disbursement record : {actionResult}");
                }
                else
                {
                    return new GenericResponse(actionResult);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementService.SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursement record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<GenericResponse> DisbursementSummaryByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(long FinancialYearId)
        {
            try
            {
                var iActionResult = await _disbursementRepository
                    .SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(FinancialYearId)
                    .ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.DisbursementSummaryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursement record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<DisbursementListResponse> ListDisbursementByReleaseAsync(long ReleaseId)
        {
            try
            {
                var existingDisbursementItem = await _disbursementRepository
                    .ListDisbursementByReleaseAsync(ReleaseId).ConfigureAwait(false);
                if (existingDisbursementItem == null)
                {
                    return new DisbursementListResponse("Records Not Found");
                }
                else
                {
                    return new DisbursementListResponse(existingDisbursementItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }
    }
}
