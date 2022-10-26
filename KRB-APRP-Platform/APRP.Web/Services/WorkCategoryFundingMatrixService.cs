using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class WorkCategoryFundingMatrixService : IWorkCategoryFundingMatrixService
    {
        private readonly IWorkCategoryFundingMatrixRepository _workCategoryFundingMatrixRepository;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public WorkCategoryFundingMatrixService(IWorkCategoryFundingMatrixRepository workCategoryFundingMatrixRepository, IUnitOfWork unitOfWork
            , ILogger<WorkCategoryFundingMatrixService> logger,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService)
        {
            _workCategoryFundingMatrixRepository = workCategoryFundingMatrixRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryFundingMatrixResponse> AddAsync(WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            try
            {
                await _workCategoryFundingMatrixRepository.AddAsync(workCategoryFundingMatrix).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkCategoryFundingMatrixResponse(workCategoryFundingMatrix); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.AddAsync Error: {Environment.NewLine}");
                return new WorkCategoryFundingMatrixResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryFundingMatrixResponse> FindByAuthorityAndFinancialIdAsync(long AuthorityId, long FinancialYearId)
        {
            try
            {
                var existingWorkCategoryFundingMatrix = await _workCategoryFundingMatrixRepository.FindByAuthorityAndFinancialIdAsync(AuthorityId,FinancialYearId).ConfigureAwait(false);
                if (existingWorkCategoryFundingMatrix == null)
                {
                    return new WorkCategoryFundingMatrixResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryFundingMatrixResponse(existingWorkCategoryFundingMatrix);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryFundingMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryFundingMatrixResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingWorkCategoryFundingMatrix = await _workCategoryFundingMatrixRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingWorkCategoryFundingMatrix == null)
                {
                    return new WorkCategoryFundingMatrixResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryFundingMatrixResponse(existingWorkCategoryFundingMatrix);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryFundingMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryFundingMatrixListResponse> ListAsync()
        {
            try
            {
                var existingWorkCategoryFundingMatrixs = await _workCategoryFundingMatrixRepository.ListAsync().ConfigureAwait(false);
                if (existingWorkCategoryFundingMatrixs == null)
                {
                    return new WorkCategoryFundingMatrixListResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryFundingMatrixListResponse(existingWorkCategoryFundingMatrixs);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryFundingMatrixListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<WorkCategoryFundingMatrixListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingWorkCategoryFundingMatrixs = await _workCategoryFundingMatrixRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingWorkCategoryFundingMatrixs == null)
                {
                    return new WorkCategoryFundingMatrixListResponse("Records Not Found");
                }
                else
                {
                    return new WorkCategoryFundingMatrixListResponse(existingWorkCategoryFundingMatrixs);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.ListAsync Error: {Environment.NewLine}");
                return new WorkCategoryFundingMatrixListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryFundingMatrixResponse> RemoveAsync(long ID)
        {
            var existingWorkCategoryFundingMatrix = await _workCategoryFundingMatrixRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingWorkCategoryFundingMatrix == null)
            {
                return new WorkCategoryFundingMatrixResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _workCategoryFundingMatrixRepository.Remove(existingWorkCategoryFundingMatrix);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new WorkCategoryFundingMatrixResponse(existingWorkCategoryFundingMatrix);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.RemoveAsync Error: {Environment.NewLine}");
                    return new WorkCategoryFundingMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryFundingMatrixResponse> Update(WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            var existingWorkCategoryFundingMatrix = await _workCategoryFundingMatrixRepository.FindByIdAsync(workCategoryFundingMatrix.ID).ConfigureAwait(false);
            if (existingWorkCategoryFundingMatrix == null)
            {
                return new WorkCategoryFundingMatrixResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _workCategoryFundingMatrixRepository.Update(workCategoryFundingMatrix);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new WorkCategoryFundingMatrixResponse(workCategoryFundingMatrix);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.Update Error: {Environment.NewLine}");
                    return new WorkCategoryFundingMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<WorkCategoryFundingMatrixResponse> Update(long ID, WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            try
            {
                _workCategoryFundingMatrixRepository.Update(ID, workCategoryFundingMatrix);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkCategoryFundingMatrixResponse(workCategoryFundingMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryFundingMatrixService.Update Error: {Environment.NewLine}");
                return new WorkCategoryFundingMatrixResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
