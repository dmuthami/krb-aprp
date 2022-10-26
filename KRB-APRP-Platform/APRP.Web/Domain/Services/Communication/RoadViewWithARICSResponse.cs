using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadViewWithARICSResponse : BaseResponse
    {
        public IQueryable<RoadViewWithARICSModel> Roads { get; set; }
        private RoadViewWithARICSResponse(bool success, string message, IQueryable<RoadViewWithARICSModel> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public RoadViewWithARICSResponse(IQueryable<RoadViewWithARICSModel> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadViewWithARICSResponse(string message) : this(false, message, null)
        { }
    }
}
