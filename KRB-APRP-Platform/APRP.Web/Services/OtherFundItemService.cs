using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class OtherFundItemService : IOtherFundItemService
    {
        private readonly IOtherFundItemRepository _otherFundItemRepository;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public OtherFundItemService(IOtherFundItemRepository otherFundItemRepository, IUnitOfWork unitOfWork
            , ILogger<OtherFundItemService> logger,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService)
        {
            _otherFundItemRepository = otherFundItemRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<OtherFundItemResponse> AddAsync(OtherFundItem otherFundItem)
        {
            try
            {
                await _otherFundItemRepository.AddAsync(otherFundItem).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new OtherFundItemResponse(otherFundItem); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"OtherFundItemService.AddAsync Error: {Environment.NewLine}");
                return new OtherFundItemResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<OtherFundItemResponse> FindByFinancialIdAsync(long FinancialYearId)
        {
            try
            {
                var existingOtherFundItem = await _otherFundItemRepository.FindByFinancialIdAsync(FinancialYearId).ConfigureAwait(false);
                if (existingOtherFundItem == null)
                {
                    return new OtherFundItemResponse("Records Not Found");
                }
                else
                {
                    return new OtherFundItemResponse(existingOtherFundItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"OtherFundItemService.ListAsync Error: {Environment.NewLine}");
                return new OtherFundItemResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<OtherFundItemResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingOtherFundItem = await _otherFundItemRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingOtherFundItem == null)
                {
                    return new OtherFundItemResponse("Records Not Found");
                }
                else
                {
                    return new OtherFundItemResponse(existingOtherFundItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"OtherFundItemService.ListAsync Error: {Environment.NewLine}");
                return new OtherFundItemResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<OtherFundItemResponse> FindByNameAsync(string Description)
        {
            try
            {
                var existingOtherFundItem = await _otherFundItemRepository.FindByNameAsync(Description).ConfigureAwait(false);
                if (existingOtherFundItem == null)
                {
                    return new OtherFundItemResponse("Records Not Found");
                }
                else
                {
                    return new OtherFundItemResponse(existingOtherFundItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"OtherFundItemService.ListAsync Error: {Environment.NewLine}");
                return new OtherFundItemResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<OtherFundItemListResponse> ListAsync()
        {
            try
            {
                var existingOtherFundItems = await _otherFundItemRepository.ListAsync().ConfigureAwait(false);
                if (existingOtherFundItems == null)
                {
                    return new OtherFundItemListResponse("Records Not Found");
                }
                else
                {
                    return new OtherFundItemListResponse(existingOtherFundItems);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"OtherFundItemService.ListAsync Error: {Environment.NewLine}");
                return new OtherFundItemListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<OtherFundItemListResponse> ListAsync(long FinancialYearId)
        {
            try
            {
                var existingOtherFundItems = await _otherFundItemRepository.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (existingOtherFundItems == null)
                {
                    return new OtherFundItemListResponse("Records Not Found");
                }
                else
                {
                    return new OtherFundItemListResponse(existingOtherFundItems);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"OtherFundItemService.ListAsync Error: {Environment.NewLine}");
                return new OtherFundItemListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<OtherFundItemResponse> RemoveAsync(long ID)
        {
            var existingOtherFundItem = await _otherFundItemRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingOtherFundItem == null)
            {
                return new OtherFundItemResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _otherFundItemRepository.Remove(existingOtherFundItem);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new OtherFundItemResponse(existingOtherFundItem);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"OtherFundItemService.RemoveAsync Error: {Environment.NewLine}");
                    return new OtherFundItemResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<OtherFundItemResponse> Update(OtherFundItem otherFundItem)
        {
            var existingOtherFundItem = await _otherFundItemRepository.FindByIdAsync(otherFundItem.ID).ConfigureAwait(false);
            if (existingOtherFundItem == null)
            {
                return new OtherFundItemResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _otherFundItemRepository.Update(otherFundItem);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new OtherFundItemResponse(otherFundItem);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"OtherFundItemService.Update Error: {Environment.NewLine}");
                    return new OtherFundItemResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<OtherFundItemResponse> Update(long ID, OtherFundItem otherFundItem)
        {
            try
            {
                _otherFundItemRepository.Update(ID, otherFundItem);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new OtherFundItemResponse(otherFundItem);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"OtherFundItemService.Update Error: {Environment.NewLine}");
                return new OtherFundItemResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
