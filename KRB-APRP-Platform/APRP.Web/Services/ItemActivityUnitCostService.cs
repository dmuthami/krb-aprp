using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ItemActivityUnitCostService : IItemActivityUnitCostService
    {

        private readonly IItemActivityUnitCostRepository _itemActivityUnitCostRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ItemActivityUnitCostService(IItemActivityUnitCostRepository itemActivityUnitCostRepository, IUnitOfWork unitOfWork, ILogger<ItemActivityUnitCostService> logger)
        {
            _itemActivityUnitCostRepository = itemActivityUnitCostRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ItemActivityUnitCostResponse> AddAsync(ItemActivityUnitCost itemActivityUnitCost)
        {
            try
            {
                await _itemActivityUnitCostRepository.AddAsync(itemActivityUnitCost).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ItemActivityUnitCostResponse(itemActivityUnitCost); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityUnitCostService.AddAsync Error: {Environment.NewLine}");
                return new ItemActivityUnitCostResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        public async Task<ItemActivityUnitCostResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _itemActivityUnitCostRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ItemActivityUnitCostResponse("Record Not Found");
                }
                else
                {
                    return new ItemActivityUnitCostResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityUnitCostService.FindByIdAsync Error: {Environment.NewLine}");
                return new ItemActivityUnitCostResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<ItemActivityUnitCostResponse> FindByActivityCodeAsync(string itemCode)
        {
            try
            {
                var existingRecord = await _itemActivityUnitCostRepository.FindByActivityCodeAsync(itemCode).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ItemActivityUnitCostResponse("Record Not Found");
                }
                else
                {
                    return new ItemActivityUnitCostResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityUnitCostService.FindByIdAsync Error: {Environment.NewLine}");
                return new ItemActivityUnitCostResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<ItemActivityUnitCost>> ListAsync()
        {
            return await _itemActivityUnitCostRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<ItemActivityUnitCostResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _itemActivityUnitCostRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new ItemActivityUnitCostResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _itemActivityUnitCostRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ItemActivityUnitCostResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ItemActivityUnitCostService.RemoveAsync Error: {Environment.NewLine}");
                    return new ItemActivityUnitCostResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ItemActivityUnitCostResponse> Update(ItemActivityUnitCost itemActivityUnitCost)
        {
            var existingRecord = await _itemActivityUnitCostRepository.FindByIdAsync(itemActivityUnitCost.ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new ItemActivityUnitCostResponse("Record Not Found");
            }
            else
            {
                existingRecord.Description = itemActivityUnitCost.Description;
                existingRecord.ItemCode = itemActivityUnitCost.ItemCode;
                existingRecord.SubItemCode = itemActivityUnitCost.SubItemCode;
                existingRecord.SubSubItemCode = itemActivityUnitCost.SubSubItemCode;
                existingRecord.UnitCode =  itemActivityUnitCost.UnitCode;
                existingRecord.UnitDescription = itemActivityUnitCost.UnitDescription;
                existingRecord.UnitMeasure = itemActivityUnitCost.UnitMeasure;
                existingRecord.OverheadRoutineImprovement = existingRecord.OverheadRoutineImprovement;
                existingRecord.ItemActivityGroupId = existingRecord.ItemActivityGroupId;

                try
                {
                    _itemActivityUnitCostRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ItemActivityUnitCostResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ItemActivityUnitCostService.FindByIdAsync Error: {Environment.NewLine}");
                    return new ItemActivityUnitCostResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ItemActivityUnitCostResponse> Update(long ID, ItemActivityUnitCost itemActivityUnitCost)
        {
            try
            {
                _itemActivityUnitCostRepository.Update(ID, itemActivityUnitCost);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ItemActivityUnitCostResponse(itemActivityUnitCost);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.Update Error: {Environment.NewLine}");
                return new ItemActivityUnitCostResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<ItemActivityUnitCostResponse> FindByCodeAsync(string Code)
        {
            try
            {
                var existingRecord = await _itemActivityUnitCostRepository.FindByCodeAsync(Code).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ItemActivityUnitCostResponse("Record Not Found");
                }
                else
                {
                    return new ItemActivityUnitCostResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityUnitCostService.FindByCodeAsync Error: {Environment.NewLine}");
                return new ItemActivityUnitCostResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }
    }
}
