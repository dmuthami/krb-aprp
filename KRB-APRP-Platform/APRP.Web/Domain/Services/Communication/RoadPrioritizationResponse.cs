using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadPrioritizationResponse : BaseResponse
    {
        public RoadPrioritization RoadPrioritization { get; set; }


        private RoadPrioritizationResponse(bool success, string message, RoadPrioritization roadPrioritization) : base(success, message)
        {
            RoadPrioritization = roadPrioritization;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RoadPrioritization">Saved RoadPrioritization.</param>
        /// <returns>Response.</returns>
        public RoadPrioritizationResponse(RoadPrioritization roadPrioritization) : this(true, string.Empty, roadPrioritization)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadPrioritizationResponse(string message) : this(false, message, null)
        { }
    }
}
