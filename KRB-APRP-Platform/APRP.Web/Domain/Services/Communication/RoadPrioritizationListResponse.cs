using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadPrioritizationListResponse : BaseResponse
    {
        public IEnumerable<RoadPrioritization> RoadPrioritization { get; set; }


        private RoadPrioritizationListResponse(bool success, string message, IEnumerable<RoadPrioritization> roadPrioritization) : base(success, message)
        {
            RoadPrioritization =roadPrioritization;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadPrioritizationListResponse(IEnumerable<RoadPrioritization> roadPrioritization) : this(true, string.Empty, roadPrioritization)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadPrioritizationListResponse(string message) : this(false, message, null)
        { }
    }
}
