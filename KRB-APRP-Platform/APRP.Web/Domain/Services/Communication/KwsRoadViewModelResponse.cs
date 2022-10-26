using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class KwsRoadViewModelResponse : BaseResponse
    {
        public IQueryable<KwsRoadViewModel> Roads { get; set; }
        private KwsRoadViewModelResponse(bool success, string message, IQueryable<KwsRoadViewModel> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KwsRoadViewModelResponse(IQueryable<KwsRoadViewModel> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summaryKwsRoadViewModel
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KwsRoadViewModelResponse(string message) : this(false, message, null)
        { }
    }
}
