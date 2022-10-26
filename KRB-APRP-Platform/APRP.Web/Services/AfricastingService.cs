using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.DTO;

namespace APRP.Web.Services
{
    public class AfricastingService : IAfricastingService
    {
        public IConfiguration Configuration { get; }
        private readonly IAfricastingRepository _africastingRepository;
        private readonly ILogger _logger;

        public AfricastingService(IConfiguration configuration, IAfricastingRepository africastingRepository,
            ILogger<COSTESAPIService> logger)
        {
            Configuration = configuration;
            _africastingRepository = africastingRepository;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> SendSMSToAfricasting(Africasting africasting)
        {
            try
            {
                var iActionResult = await _africastingRepository.SendSMSToAfricasting(africasting).ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"COSTESAPIService.GetInstructutedWorkItemsAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> SendSMSViaMobileSasa(MobileSasa mobileSasa)
        {
            try
            {
                var iActionResult = await _africastingRepository.SendSMSViaMobileSasa(mobileSasa).ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"COSTESAPIService.GetInstructutedWorkItemsAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}
