using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSheetResponse : BaseResponse
    {
        public RoadSheet RoadSheet { get; set; }

       
        private RoadSheetResponse(bool success, string message, RoadSheet roadSheet) : base(success, message)
        {
            RoadSheet = roadSheet;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadSheetResponse(RoadSheet roadSheet) : this(true, string.Empty, roadSheet)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSheetResponse(string message) : this(false, message, null)
        { }

    }
}
