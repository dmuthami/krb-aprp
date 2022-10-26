using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class AllocationCodeUnitService : IAllocationCodeUnitService
    {
        private readonly IAllocationCodeUnitRepository _allocationCodeUnitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AllocationCodeUnitService(IAllocationCodeUnitRepository allocationCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<AllocationCodeUnitService> logger)
        {
            _allocationCodeUnitRepository = allocationCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationCodeUnitResponse> AddAsync(AllocationCodeUnit allocationCodeUnit)
        {
            try
            {
                await _allocationCodeUnitRepository.AddAsync(allocationCodeUnit).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new AllocationCodeUnitResponse(allocationCodeUnit); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationCodeUnitService.AddAsync Error: {Environment.NewLine}");
                return new AllocationCodeUnitResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationCodeUnitResponse> FindByAuthorityAsync(long AuthorityId)
        {
            try
            {
                var existingAllocationCodeUnit = await _allocationCodeUnitRepository.FindByAuthorityAsync(AuthorityId).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new AllocationCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new AllocationCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new AllocationCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationCodeUnitResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingAllocationCodeUnit = await _allocationCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new AllocationCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new AllocationCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new AllocationCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<AllocationCodeUnitResponse> FindByNameAsync(string Item)
        {
            try
            {
                var existingAllocationCodeUnit = await _allocationCodeUnitRepository.FindByNameAsync(Item).ConfigureAwait(false);
                if (existingAllocationCodeUnit == null)
                {
                    return new AllocationCodeUnitResponse("Records Not Found");
                }
                else
                {
                    return new AllocationCodeUnitResponse(existingAllocationCodeUnit);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new AllocationCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationCodeUnitListResponse> ListAsync(string AuthorityType)
        {
            try
            {
                var existingAllocationCodeUnits = await _allocationCodeUnitRepository.ListAsync( AuthorityType).ConfigureAwait(false);
                if (existingAllocationCodeUnits == null)
                {
                    return new AllocationCodeUnitListResponse("Records Not Found");
                }
                else
                {
                    return new AllocationCodeUnitListResponse(existingAllocationCodeUnits);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationCodeUnitService.ListAsync Error: {Environment.NewLine}");
                return new AllocationCodeUnitListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationCodeUnitResponse> RemoveAsync(long ID)
        {
            var existingAllocationCodeUnit = await _allocationCodeUnitRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingAllocationCodeUnit == null)
            {
                return new AllocationCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _allocationCodeUnitRepository.Remove(existingAllocationCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new AllocationCodeUnitResponse(existingAllocationCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"AllocationCodeUnitService.RemoveAsync Error: {Environment.NewLine}");
                    return new AllocationCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationCodeUnitResponse> Update(AllocationCodeUnit allocationCodeUnit)
        {
            var existingAllocationCodeUnit = await _allocationCodeUnitRepository.FindByIdAsync(allocationCodeUnit.ID).ConfigureAwait(false);
            if (existingAllocationCodeUnit == null)
            {
                return new AllocationCodeUnitResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _allocationCodeUnitRepository.Update(allocationCodeUnit);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new AllocationCodeUnitResponse(allocationCodeUnit);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"AllocationCodeUnitService.Update Error: {Environment.NewLine}");
                    return new AllocationCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AllocationCodeUnitResponse> Update(long ID, AllocationCodeUnit allocationCodeUnit)
        {
            try
            {
                _allocationCodeUnitRepository.Update(ID, allocationCodeUnit);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new AllocationCodeUnitResponse(allocationCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationCodeUnitService.Update Error: {Environment.NewLine}");
                return new AllocationCodeUnitResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
