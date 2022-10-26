using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class ReleaseService :ControllerBase, IReleaseService
    {
        private readonly IReleaseRepository _releaseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IFinancialYearService _financialYearService;
        private readonly IAllocationCodeUnitService _allocationCodeUnitService;
        private readonly IBudgetCeilingService _budgetCeilingService;

        public ReleaseService(IReleaseRepository releaseRepository, IUnitOfWork unitOfWork
            , ILogger<ReleaseService> logger, IFinancialYearService financialYearService,
            IAllocationCodeUnitService allocationCodeUnitService, IBudgetCeilingService budgetCeilingService)
        {
            _releaseRepository = releaseRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _financialYearService = financialYearService;
            _allocationCodeUnitService = allocationCodeUnitService;
            _budgetCeilingService = budgetCeilingService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ReleaseResponse> AddAsync(Release release)
        {
            try
            {
                await _releaseRepository.AddAsync(release).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ReleaseResponse(release); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.AddAsync Error: {Environment.NewLine}");
                return new ReleaseResponse($"Error occured while saving the release record : {Ex.Message}");
            }
        }

        public async Task<ReleaseResponse> DetachFirstEntryAsync(Release release)
        {
            try
            {
                await _releaseRepository.DetachFirstEntryAsync(release).ConfigureAwait(false);

                return new ReleaseResponse(release); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new ReleaseResponse($"Error occured while saving the release record : {Ex.Message}");
            }
        }

        public async Task<ReleaseResponse> FindByReleaseEntryAsync(Release release)
        {
            try
            {
                var existingDisbursement = await _releaseRepository.FindByReleaseEntryAsync(release).ConfigureAwait(false);
                if (existingDisbursement == null)
                {
                    return new ReleaseResponse("Records Not Found");
                }
                else
                {
                    return new ReleaseResponse(existingDisbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.ListAsync Error: {Environment.NewLine}");
                return new ReleaseResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ReleaseResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingDisbursement = await _releaseRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingDisbursement == null)
                {
                    return new ReleaseResponse("Records Not Found");
                }
                else
                {
                    return new ReleaseResponse(existingDisbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.ListAsync Error: {Environment.NewLine}");
                return new ReleaseResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ReleaseListResponse> ListAsync()
        {
            try
            {
                var existingReleases = await _releaseRepository.ListAsync().ConfigureAwait(false);
                if (existingReleases == null)
                {
                    return new ReleaseListResponse("Records Not Found");
                }
                else
                {
                    return new ReleaseListResponse(existingReleases);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.ListAsync Error: {Environment.NewLine}");
                return new ReleaseListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ReleaseResponse> RemoveAsync(long ID)
        {
            var existingDisbursement = await _releaseRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingDisbursement == null)
            {
                return new ReleaseResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _releaseRepository.Remove(existingDisbursement);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ReleaseResponse(existingDisbursement);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ReleaseService.RemoveAsync Error: {Environment.NewLine}");
                    return new ReleaseResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ReleaseResponse> Update(Release release)
        {
            var existingDisbursement = await _releaseRepository.FindByIdAsync(release.ID).ConfigureAwait(false);
            if (existingDisbursement == null)
            {
                return new ReleaseResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _releaseRepository.Update(release);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ReleaseResponse(release);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ReleaseService.Update Error: {Environment.NewLine}");
                    return new ReleaseResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ReleaseResponse> Update(long ID, Release release)
        {
            try
            {
                _releaseRepository.Update(ID, release);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ReleaseResponse(release);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.Update Error: {Environment.NewLine}");
                return new ReleaseResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<ReleaseListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingReleases = await _releaseRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingReleases == null)
                {
                    return new ReleaseListResponse("Records Not Found");
                }
                else
                {
                    return new ReleaseListResponse(existingReleases);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.ListAsync Error: {Environment.NewLine}");
                return new ReleaseListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<ReleaseListResponse> ListAsync(long FinancialYearId, string Code)
        {
            try
            {
                var existingReleases = await _releaseRepository.ListAsync(FinancialYearId, Code).ConfigureAwait(false);
                if (existingReleases == null)
                {
                    return new ReleaseListResponse("Records Not Found");
                }
                else
                {
                    return new ReleaseListResponse(existingReleases);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.ListAsync Error: {Environment.NewLine}");
                return new ReleaseListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<DisbursementResponse> FindDisbursementByReleaseAsync(Release release)
        {
            try
            {
                var existingDisbursement = await _releaseRepository.FindDisbursementByReleaseAsync(release).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ReleaseService.ListAsync Error: {Environment.NewLine}");
                return new DisbursementResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListDisbursementByReleaseAsync(Release release)
        {
            try
            {
                var iActionResult = await _releaseRepository.ListDisbursementByReleaseAsync(release).ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.ListDisbursementByReleaseAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ReleaseListResponse> ListAsync2(FinancialYear financialYear, string Code)
        {
            try
            {
                var arr = financialYear.Code.Split("/");

                //get start date
                var startDate = new DateTime(Convert.ToInt32(arr[0]),7,1,0,0,0);

                //get end date
                var endDate = new DateTime(Convert.ToInt32(arr[1]), 6, 30, 23, 59, 59);

                var existingReleases = await _releaseRepository.ListAsync2(financialYear.ID, Code, startDate, endDate).ConfigureAwait(false);
                if (existingReleases == null)
                {
                    return new ReleaseListResponse("Records Not Found");
                }
                else
                {
                    return new ReleaseListResponse(existingReleases);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseService.ListAsync Error: {Environment.NewLine}");
                return new ReleaseListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }
    }
}
