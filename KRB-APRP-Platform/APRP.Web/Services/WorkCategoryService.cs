using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;

namespace APRP.Web.Services
{
    public class WorkCategoryService : IWorkCategoryService
    {
        private readonly IWorkCategoryRepository _workCategoryRepository;
        private readonly ILogger _logger;

        public WorkCategoryService(IWorkCategoryRepository workCategoryRepository, ILogger<WorkCategoryService> logger)
        {
            _workCategoryRepository = workCategoryRepository;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<WorkCategory>> ListAsync()
        {
            try
            {
                return await _workCategoryRepository.ListAsync().ConfigureAwait(false);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.RemoveAsync Error {Environment.NewLine}");
                return Enumerable.Empty<WorkCategory>();
            }
        }
    }
}
