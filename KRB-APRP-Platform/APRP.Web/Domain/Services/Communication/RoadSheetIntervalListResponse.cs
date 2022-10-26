using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSheetIntervalListResponse : BaseResponse
    {
        public IEnumerable<RoadSheetInterval> RoadSheetInterval { get; set; }


        private RoadSheetIntervalListResponse(bool success, string message, IEnumerable<RoadSheetInterval> roadSheetInterval) : base(success, message)
        {
            RoadSheetInterval =roadSheetInterval;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadSheetIntervalListResponse(IEnumerable<RoadSheetInterval> roadSheetInterval) : this(true, string.Empty, roadSheetInterval)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSheetIntervalListResponse(string message) : this(false, message, null)
        { }
    }
}
