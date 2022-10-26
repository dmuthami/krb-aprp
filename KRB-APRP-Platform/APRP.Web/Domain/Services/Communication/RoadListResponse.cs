using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadListResponse : BaseResponse
    {
        public IEnumerable<Road> Roads { get; set; }
        private RoadListResponse(bool success, string message, IEnumerable<Road> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public RoadListResponse(IEnumerable<Road> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadListResponse(string message) : this(false, message, null)
        { }
    }
}
