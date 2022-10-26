using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class PackageProgressEntryService : IPackageProgressEntryService
    {
        private readonly IPackageProgressEntryRepository _packageProgressEntryyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public PackageProgressEntryService(IPackageProgressEntryRepository packageProgressEntryRepository, 
            IUnitOfWork unitOfWork, 
             ILogger<PackageProgressEntryService> logger)
        {
            _packageProgressEntryyRepository = packageProgressEntryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PackageProgressEntryResponse> AddAsync(PackageProgressEntry packageProgressEntry)
        {
            try
            {
                await _packageProgressEntryyRepository.AddAsync(packageProgressEntry).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new PackageProgressEntryResponse(packageProgressEntry);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.AddAsync Error: {Environment.NewLine}");
                return new PackageProgressEntryResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
       

        public async Task<IEnumerable<PackageProgressEntry>> ListAsync()
        {
            return await _packageProgressEntryyRepository.ListAsync().ConfigureAwait(false);
        }
    }
}
