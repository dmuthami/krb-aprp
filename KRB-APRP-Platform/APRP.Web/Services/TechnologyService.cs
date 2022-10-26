using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;

namespace APRP.Web.Services
{
    public class TechnologyService : ITechnologyService
    {
        private readonly ITechnologyRepository _technologyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TechnologyService(ITechnologyRepository technologyRepository, IUnitOfWork unitOfWork, ILogger<TechnologyService> logger)
        {
            _technologyRepository = technologyRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<Technology>> ListAsync()
        {
            try
            {
                return await _technologyRepository.ListAsync().ConfigureAwait(false);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationController.Register Error: {Environment.NewLine}");
                return Enumerable.Empty<Technology>();
            }
        }
    }
}
