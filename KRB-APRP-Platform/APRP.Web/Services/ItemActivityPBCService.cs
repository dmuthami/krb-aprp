using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;

namespace APRP.Web.Services
{
    public class ItemActivityPBCService : IItemActivityPBCService
    {
        private readonly IItemActivityPBCRepository _itemActivityPBCRepository;
        private readonly ILogger _logger;

        public ItemActivityPBCService(IItemActivityPBCRepository itemActivityPBCRepository, ILogger<WorkCategoryService> logger)
        {
            _itemActivityPBCRepository = itemActivityPBCRepository;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<ItemActivityPBC>> ListAsync()
        {
            try
            {
                return await _itemActivityPBCRepository.ListAsync().ConfigureAwait(false);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.RemoveAsync Error {Environment.NewLine}");
                return Enumerable.Empty<ItemActivityPBC>();
            }
        }
    }
}
