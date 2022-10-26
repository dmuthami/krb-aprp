using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadClassificationListResponse : BaseResponse
    {
        public IEnumerable<RoadClassification> RoadClassification { get; set; }


        private RoadClassificationListResponse(bool success, string message, IEnumerable<RoadClassification> roadClassification) : base(success, message)
        {
            RoadClassification =roadClassification;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadClassificationListResponse(IEnumerable<RoadClassification> roadClassification) : this(true, string.Empty, roadClassification)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadClassificationListResponse(string message) : this(false, message, null)
        { }
    }
}
