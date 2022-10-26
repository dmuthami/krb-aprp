using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class FinancialProgressService : IFinancialProgressService
    {
        private readonly IFinancialProgressRepository _financialProgressRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoadWorkSectionPlanService _roadWorkSectionPlanService;
        private readonly IWorkPlanPackageService _workPlanPackageService;
        private readonly ITechnologyService _technologyService;
        private readonly ILogger _logger;

        public FinancialProgressService(IFinancialProgressRepository financialProgressRepository, 
            IUnitOfWork unitOfWork, 
             ILogger<PlanActivityService> logger)
        {
            _financialProgressRepository = financialProgressRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialProgressResponse> AddAsync(FinancialProgress financialProgress)
        {
            try
            {
                await _financialProgressRepository.AddAsync(financialProgress).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new FinancialProgressResponse(financialProgress); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FinancialProgressService.AddAsync Error: {Environment.NewLine}");
                return new FinancialProgressResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialProgressResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _financialProgressRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new FinancialProgressResponse("Record Not Found");
                }
                else
                {
                    return new FinancialProgressResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FinancialProgressService.FindByIdAsync Error: {Environment.NewLine}");
                return new FinancialProgressResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAsync(long authorityId)
        {
            return await _financialProgressRepository.ListByAuthorityIdAsync(authorityId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAndFinancialYearAsync(long authorityId, long financialYearId)
        {
            return await _financialProgressRepository.ListByAuthorityIdAndFinancialYearAsync(authorityId,financialYearId).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialProgressResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _financialProgressRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new FinancialProgressResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _financialProgressRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new FinancialProgressResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FinancialProgressService.RemoveAsync Error: {Environment.NewLine}");
                    return new FinancialProgressResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialProgressResponse> UpdateAsync(FinancialProgress financialProgress)
        {
            if (financialProgress != null)
            {

                try
                {

                    var existingRecord = await _financialProgressRepository.FindByIdAsync(financialProgress.ID).ConfigureAwait(false);
                    if (existingRecord == null)
                    {
                        return new FinancialProgressResponse("Record Not Found");
                    }
                    else
                    {
                        existingRecord.AnnualFinancialStatementFileName = financialProgress.AnnualFinancialStatementFileName;
                        existingRecord.BankReconFileName = financialProgress.BankReconFileName;
                        existingRecord.ClosingBalance = financialProgress.ClosingBalance;
                        existingRecord.OpeningBalance = financialProgress.OpeningBalance;
                        existingRecord.QuarterCodeListId  = financialProgress.QuarterCodeListId;
                        existingRecord.FinancialExpenditure = financialProgress.FinancialExpenditure;
                        existingRecord.FinancialReceiptReference = financialProgress.FinancialReceiptReference;
                        existingRecord.FinancialReceipts = financialProgress.FinancialReceipts;

                        _financialProgressRepository.Update(existingRecord);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        return new FinancialProgressResponse(existingRecord);

                    }
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FinancialProgressService.Update Error: {Environment.NewLine}");
                    return new FinancialProgressResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
            return new FinancialProgressResponse("Invalid Record for update");
        }
    }
}
