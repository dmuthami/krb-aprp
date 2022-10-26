using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ItemActivityGroupService : IItemActivityGroupService
    {
        private readonly IItemActivityGroupRepository _itemActivityGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ItemActivityGroupService(IItemActivityGroupRepository itemActivityGroupRepository, IUnitOfWork unitOfWork, ILogger<ItemActivityGroupService> logger)
        {
            _itemActivityGroupRepository = itemActivityGroupRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ItemActivityGroupResponse> AddAsync(ItemActivityGroup itemActivityGroup)
        {
            try
            {
                await _itemActivityGroupRepository.AddAsync(itemActivityGroup).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ItemActivityGroupResponse(itemActivityGroup); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityGroupService.AddAsync Error: {Environment.NewLine}");
                return new ItemActivityGroupResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        public async Task<ItemActivityGroupResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _itemActivityGroupRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ItemActivityGroupResponse("Record Not Found");
                }
                else
                {
                    return new ItemActivityGroupResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityGroupService.FindByIdAsync Error: {Environment.NewLine}");
                return new ItemActivityGroupResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<ItemActivityGroup>> ListAsync()
        {
            return await _itemActivityGroupRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<ItemActivityGroupResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _itemActivityGroupRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new ItemActivityGroupResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _itemActivityGroupRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ItemActivityGroupResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ItemActivityGroupService.RemoveAsync Error: {Environment.NewLine}");
                    return new ItemActivityGroupResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        public async Task<ItemActivityGroupResponse> Update(ItemActivityGroup itemActivityGroup)
        {
            if(itemActivityGroup != null)
            {
                var existingRecord = await _itemActivityGroupRepository.FindByIdAsync(itemActivityGroup.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ItemActivityGroupResponse("Record Not Found");
                }
                else
                {
                    existingRecord.BillNumber = itemActivityGroup.BillNumber;
                    existingRecord.Description = itemActivityGroup.Description;

                    try
                    {
                        _itemActivityGroupRepository.Update(existingRecord);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        return new ItemActivityGroupResponse(existingRecord);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"ItemActivityGroupService.Update Error: {Environment.NewLine}");
                        return new ItemActivityGroupResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            return new ItemActivityGroupResponse("Record Not Found");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ItemActivityGroupResponse> Update(long ID, ItemActivityGroup itemActivityGroup)
        {
            try
            {
                _itemActivityGroupRepository.Update(ID, itemActivityGroup);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ItemActivityGroupResponse(itemActivityGroup);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.Update Error: {Environment.NewLine}");
                return new ItemActivityGroupResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
