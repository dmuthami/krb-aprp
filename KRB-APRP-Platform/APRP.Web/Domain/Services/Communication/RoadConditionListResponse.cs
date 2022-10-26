using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadConditionListResponse : BaseResponse
    {
        public IEnumerable<RoadCondition> RoadCondtion { get; set; }


        private RoadConditionListResponse(bool success, string message, IEnumerable<RoadCondition> roadCondtion) : base(success, message)
        {
            RoadCondtion =roadCondtion;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadConditionListResponse(IEnumerable<RoadCondition> roadCondtion) : this(true, string.Empty, roadCondtion)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadConditionListResponse(string message) : this(false, message, null)
        { }
    }
}
