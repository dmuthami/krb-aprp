using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadClassificationResponse : BaseResponse
    {
        public RoadClassification RoadClassification { get; set; }


        private RoadClassificationResponse(bool success, string message, RoadClassification roadClassification) : base(success, message)
        {
            RoadClassification = roadClassification;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RoadClassification">Saved RoadClassification.</param>
        /// <returns>Response.</returns>
        public RoadClassificationResponse(RoadClassification roadClassification) : this(true, string.Empty, roadClassification)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadClassificationResponse(string message) : this(false, message, null)
        { }
    }
}
