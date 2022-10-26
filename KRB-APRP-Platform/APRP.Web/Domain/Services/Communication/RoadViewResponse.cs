using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadViewResponse : BaseResponse
    {
        public IQueryable<RoadViewModel> Roads { get; set; }
        private RoadViewResponse(bool success, string message, IQueryable<RoadViewModel> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public RoadViewResponse(IQueryable<RoadViewModel> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadViewResponse(string message) : this(false, message, null)
        { }
    }
}
