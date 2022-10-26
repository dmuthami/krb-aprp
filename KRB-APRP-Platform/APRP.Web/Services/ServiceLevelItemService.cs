using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;

namespace APRP.Web.Services
{
    public class ServiceLevelItemService : IServiceLevelItemService
    {
        private readonly IServiceLevelItemRepository _serviceLevelItemRepository;
        private readonly ILogger _logger;

        public ServiceLevelItemService(IServiceLevelItemRepository serviceLevelItemRepository, ILogger<WorkCategoryService> logger)
        {
            _serviceLevelItemRepository = serviceLevelItemRepository;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<ServiceLevelItem>> ListAsync()
        {
            try
            {
                return await _serviceLevelItemRepository.ListAsync().ConfigureAwait(false);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.RemoveAsync Error {Environment.NewLine}");
                return Enumerable.Empty<ServiceLevelItem>();
            }
        }
    }
}
