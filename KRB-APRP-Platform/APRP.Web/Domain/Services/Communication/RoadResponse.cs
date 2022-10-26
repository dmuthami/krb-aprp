using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadResponse : BaseResponse
    {
        public Road Road { get; set; }

       
        private RoadResponse(bool success, string message, Road road) : base(success, message)
        {
            Road = road;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadResponse(Road road) : this(true, string.Empty, road)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadResponse(string message) : this(false, message, null)
        { }

    }
}
