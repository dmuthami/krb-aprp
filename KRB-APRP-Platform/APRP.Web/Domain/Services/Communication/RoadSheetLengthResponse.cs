using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSheetLengthResponse : BaseResponse
    {
        public RoadSheetLength RoadSheetLength { get; set; }


        private RoadSheetLengthResponse(bool success, string message, RoadSheetLength roadSheetLength) : base(success, message)
        {
            RoadSheetLength = roadSheetLength;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RoadSheetLength">Saved RoadSheetLength.</param>
        /// <returns>Response.</returns>
        public RoadSheetLengthResponse(RoadSheetLength roadSheetLength) : this(true, string.Empty, roadSheetLength)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSheetLengthResponse(string message) : this(false, message, null)
        { }
    }
}
