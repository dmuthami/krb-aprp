using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class GISRoadResponse : BaseResponse
    {
        public GISRoad GISRoad { get; set; }

       
        private GISRoadResponse(bool success, string message, GISRoad gISRoad) : base(success, message)
        {
            GISRoad = gISRoad;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public GISRoadResponse(GISRoad gISRoad) : this(true, string.Empty, gISRoad)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public GISRoadResponse(string message) : this(false, message, null)
        { }

    }
}
