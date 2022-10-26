using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class AllocationService : IAllocationService
    {
        private readonly IAllocationRepository _allocationRepository;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AllocationService(IAllocationRepository allocationRepository, IUnitOfWork unitOfWork
            , ILogger<AllocationService> logger,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService)
        {
            _allocationRepository = allocationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationResponse> AddAsync(Allocation allocation)
        {
            try
            {
                await _allocationRepository.AddAsync(allocation).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new AllocationResponse(allocation); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.AddAsync Error: {Environment.NewLine}");
                return new AllocationResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingAllocations = await _allocationRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingAllocations == null)
                {
                    return new AllocationResponse("Record Not Found");
                }
                else
                {
                    return new AllocationResponse(existingAllocations);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.FinfByIdAsync Error: {Environment.NewLine}");
                return new AllocationResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationResponse> FindByAllocationCodeUnitIdAsync(long AllocationCodeUnitId)
        {
            try
            {
                var existingAllocations = await _allocationRepository.FindByAllocationCodeUnitIdAsync(AllocationCodeUnitId).ConfigureAwait(false);
                if (existingAllocations == null)
                {
                    return new AllocationResponse("Record Not Found");
                }
                else
                {
                    return new AllocationResponse(existingAllocations);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.FinfByIdAsync Error: {Environment.NewLine}");
                return new AllocationResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationListResponse> ListAsync()
        {
            try
            {
                var existingAllocations = await _allocationRepository.ListAsync().ConfigureAwait(false);
                if (existingAllocations == null)
                {
                    return new AllocationListResponse("Records Not Found");
                }
                else
                {
                    return new AllocationListResponse(existingAllocations);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.ListAsync Error: {Environment.NewLine}");
                return new AllocationListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingAllocationItem = await _allocationRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingAllocationItem == null)
                {
                    return new AllocationListResponse("Records Not Found");
                }
                else
                {
                    return new AllocationListResponse(existingAllocationItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.ListAsync Error: {Environment.NewLine}");
                return new AllocationListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationResponse> RemoveAsync(long ID)
        {
            var existingAllocations = await _allocationRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingAllocations == null)
            {
                return new AllocationResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _allocationRepository.Remove(existingAllocations);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new AllocationResponse(existingAllocations);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"AllocationService.RemoveAsync Error: {Environment.NewLine}");
                    return new AllocationResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationResponse> Update(Allocation allocation)
        {
            var existingAllocations = await _allocationRepository.FindByIdAsync(allocation.ID).ConfigureAwait(false);
            if (existingAllocations == null)
            {
                return new AllocationResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _allocationRepository.Update(allocation);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new AllocationResponse(allocation);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"AllocationService.Update Error: {Environment.NewLine}");
                    return new AllocationResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationResponse> Update(long ID, Allocation allocation)
        {
            try
            {
                _allocationRepository.Update(ID, allocation);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new AllocationResponse(allocation);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.Update Error: {Environment.NewLine}");
                return new AllocationResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
