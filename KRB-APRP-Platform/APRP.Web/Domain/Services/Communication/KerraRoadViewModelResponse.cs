using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class KerraRoadViewModelResponse : BaseResponse
    {
        public IQueryable<KerraRoadViewModel> Roads { get; set; }
        private KerraRoadViewModelResponse(bool success, string message, IQueryable<KerraRoadViewModel> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KerraRoadViewModelResponse(IQueryable<KerraRoadViewModel> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summaryKerraRoadViewModel
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KerraRoadViewModelResponse(string message) : this(false, message, null)
        { }
    }
}
