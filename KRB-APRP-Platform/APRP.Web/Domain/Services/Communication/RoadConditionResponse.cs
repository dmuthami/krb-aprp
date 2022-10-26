using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadCondtionResponse : BaseResponse
    {
        public RoadCondition RoadCondtion { get; set; }


        private RoadCondtionResponse(bool success, string message, RoadCondition roadCondtion) : base(success, message)
        {
            RoadCondtion = roadCondtion;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RoadCondtion">Saved RoadCondtion.</param>
        /// <returns>Response.</returns>
        public RoadCondtionResponse(RoadCondition roadCondtion) : this(true, string.Empty, roadCondtion)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadCondtionResponse(string message) : this(false, message, null)
        { }
    }
}
