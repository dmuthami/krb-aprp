using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class KuraRoadViewModelResponse : BaseResponse
    {
        public IQueryable<KuraRoadViewModel> Roads { get; set; }
        private KuraRoadViewModelResponse(bool success, string message, IQueryable<KuraRoadViewModel> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KuraRoadViewModelResponse(IQueryable<KuraRoadViewModel> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summaryKuraViewModel
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KuraRoadViewModelResponse(string message) : this(false, message, null)
        { }
    }
}
