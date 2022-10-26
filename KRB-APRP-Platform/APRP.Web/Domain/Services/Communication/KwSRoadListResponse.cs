using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services.Communication
{
    public class KwSRoadListResponse : BaseResponse
    {
        public IEnumerable<KwsRoad> KwsRoads { get; set; }
        private KwSRoadListResponse(bool success, string message, IEnumerable<KwsRoad> kwsRoads) : base(success, message)
        {
            KwsRoads = kwsRoads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KwSRoadListResponse(IEnumerable<KwsRoad> kwsRoads) : this(true, string.Empty, kwsRoads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KwSRoadListResponse(string message) : this(false, message, null)
        { }
    }
}
