using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services.Communication
{
    public class KwSRoadResponse : BaseResponse
    {
        public KwsRoad KwsRoad { get; set; }

       
        private KwSRoadResponse(bool success, string message, KwsRoad kwsRoad) : base(success, message)
        {
            KwsRoad = kwsRoad;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public KwSRoadResponse(KwsRoad kwsRoad) : this(true, string.Empty, kwsRoad)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KwSRoadResponse(string message) : this(false, message, null)
        { }

    }
}
