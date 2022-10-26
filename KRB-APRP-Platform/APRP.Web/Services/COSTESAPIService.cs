using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class COSTESAPIService : ICOSTESAPIService
    {
        public IConfiguration Configuration { get; }
        private readonly ICOSTESRespository _cOSTESRespository;
        private readonly ILogger _logger;

        public COSTESAPIService(IConfiguration configuration, ICOSTESRespository cOSTESRespository,
            ILogger<COSTESAPIService> logger)
        {
            Configuration = configuration;
            _cOSTESRespository = cOSTESRespository;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> GetInstructutedWorkItemsAsync(string Token,int Year, string RegionCode)
        {
            try
            {
                var iActionResult = await _cOSTESRespository.GetInstructutedWorkItemsAsync(Token, Year, RegionCode).ConfigureAwait(false);


                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"COSTESAPIService.GetInstructutedWorkItemsAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> EncodeTo64Async(string toEncode)
        {
            try
            {
                var iActionResult = await _cOSTESRespository.EncodeTo64Async(toEncode).ConfigureAwait(false);


                return new GenericResponse(iActionResult); //successful
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception Ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _logger.LogError(Ex, $"COSTESAPIService.GetInstructutedWorkItemsAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> GetAccessTokenAsync(string Token)
        {
            try
            {
                var iActionResult = await _cOSTESRespository.GetAccessTokenAsync(Token).ConfigureAwait(false);


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
