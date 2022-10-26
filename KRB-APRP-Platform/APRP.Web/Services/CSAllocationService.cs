using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class CSAllocationService : ICSAllocationService
    {
        private readonly ICSAllocationRepository _cSAllocationRepository;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CSAllocationService(ICSAllocationRepository aRICSRepository, IUnitOfWork unitOfWork
            , ILogger<CSAllocationService> logger,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService)
        {
            _cSAllocationRepository = aRICSRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CSAllocationtResponse> AddAsync(CSAllocation cSAllocation)
        {
            try
            {
                await _cSAllocationRepository.AddAsync(cSAllocation).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new CSAllocationtResponse(cSAllocation); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.AddAsync Error: {Environment.NewLine}");
                return new CSAllocationtResponse($"Error occured while saving the cSAllocation record : {Ex.Message}");
            }
        }

        public async Task<CSAllocationtResponse> DetachFirstEntryAsync(CSAllocation cSAllocation)
        {
            try
            {
                await _cSAllocationRepository.DetachFirstEntryAsync(cSAllocation).ConfigureAwait(false);

                return new CSAllocationtResponse(cSAllocation); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new CSAllocationtResponse($"Error occured while saving the cSAllocation record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<double> CSAllocationtItemSum(IList<CSAllocation> cSAllocationCollectionList)
        {
            try
            {
                if (cSAllocationCollectionList == null)
                {
                    return 0d;
                }
                else
                {
                    //Do a computation
                    return await Task.Run(() =>
                    {
                        double sum = 0.0;
                        sum = (cSAllocationCollectionList.Sum(item => item.Amount));
                        return sum;
                    }).ConfigureAwait(false);

                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.RevenueCollectionSum Error: {Environment.NewLine}");
                return 0d;
            }
        }

        public async Task<GenericResponse> CSAllocationSummaryAsync(long FinancialYearId)
        {
            try
            {
                var iActionResult = await _cSAllocationRepository.CSAllocationSummaryAsync(FinancialYearId).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.CSAllocationSummaryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the cSAllocation record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> CSAllocationSummaryByBudgetCeilingComputationAsync(long FinancialYearId)
        {
            try
            {
                var iActionResult = await _cSAllocationRepository.CSAllocationSummaryByBudgetCeilingComputationAsync(FinancialYearId).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.CSAllocationSummaryByBudgetCeilingComputationAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the cSAllocation record : {Ex.Message}");
            }
        }

        public async Task<CSAllocationtResponse> FindByCSAllocationEntryAsync(CSAllocation cSAllocation)
        {
            try
            {
                var existingDisbursement = await _cSAllocationRepository.FindByCSAllocationEntryAsync(cSAllocation).ConfigureAwait(false);
                if (existingDisbursement == null)
                {
                    return new CSAllocationtResponse("Records Not Found");
                }
                else
                {
                    return new CSAllocationtResponse(existingDisbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.ListAsync Error: {Environment.NewLine}");
                return new CSAllocationtResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CSAllocationtResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingDisbursement = await _cSAllocationRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingDisbursement == null)
                {
                    return new CSAllocationtResponse("Records Not Found");
                }
                else
                {
                    return new CSAllocationtResponse(existingDisbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.ListAsync Error: {Environment.NewLine}");
                return new CSAllocationtResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CSAllocationtListResponse> ListAsync()
        {
            try
            {
                var existingDisbursements = await _cSAllocationRepository.ListAsync().ConfigureAwait(false);
                if (existingDisbursements == null)
                {
                    return new CSAllocationtListResponse("Records Not Found");
                }
                else
                {
                    return new CSAllocationtListResponse(existingDisbursements);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.ListAsync Error: {Environment.NewLine}");
                return new CSAllocationtListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CSAllocationtListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingDisbursementItem = await _cSAllocationRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingDisbursementItem == null)
                {
                    return new CSAllocationtListResponse("Records Not Found");
                }
                else
                {
                    return new CSAllocationtListResponse(existingDisbursementItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.ListAsync Error: {Environment.NewLine}");
                return new CSAllocationtListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<CSAllocationtListResponse> ListAsync(long AuthorityId, long FinancialYearId)
        {
            try
            {
                var existingDisbursementItem = await _cSAllocationRepository.ListAsync(AuthorityId,FinancialYearId).ConfigureAwait(false);
                if (existingDisbursementItem == null)
                {
                    return new CSAllocationtListResponse("Records Not Found");
                }
                else
                {
                    return new CSAllocationtListResponse(existingDisbursementItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.ListAsync Error: {Environment.NewLine}");
                return new CSAllocationtListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CSAllocationtResponse> RemoveAsync(long ID)
        {
            var existingDisbursement = await _cSAllocationRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingDisbursement == null)
            {
                return new CSAllocationtResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _cSAllocationRepository.Remove(existingDisbursement);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new CSAllocationtResponse(existingDisbursement);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"CSAllocationtService.RemoveAsync Error: {Environment.NewLine}");
                    return new CSAllocationtResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CSAllocationtResponse> Update(CSAllocation cSAllocation)
        {
            var existingDisbursement = await _cSAllocationRepository.FindByIdAsync(cSAllocation.ID).ConfigureAwait(false);
            if (existingDisbursement == null)
            {
                return new CSAllocationtResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _cSAllocationRepository.Update(cSAllocation);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new CSAllocationtResponse(cSAllocation);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"CSAllocationtService.Update Error: {Environment.NewLine}");
                    return new CSAllocationtResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CSAllocationtResponse> Update(long ID, CSAllocation cSAllocation)
        {
            try
            {
                _cSAllocationRepository.Update(ID, cSAllocation);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new CSAllocationtResponse(cSAllocation);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationtService.Update Error: {Environment.NewLine}");
                return new CSAllocationtResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
