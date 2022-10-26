using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSheetIntervalResponse : BaseResponse
    {
        public RoadSheetInterval RoadSheetInterval { get; set; }


        private RoadSheetIntervalResponse(bool success, string message, RoadSheetInterval roadSheetInterval) : base(success, message)
        {
            RoadSheetInterval = roadSheetInterval;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RoadSheetInterval">Saved RoadSheetInterval.</param>
        /// <returns>Response.</returns>
        public RoadSheetIntervalResponse(RoadSheetInterval roadSheetInterval) : this(true, string.Empty, roadSheetInterval)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSheetIntervalResponse(string message) : this(false, message, null)
        { }
    }
}
