using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSheetListResponse : BaseResponse
    {
        public IEnumerable<RoadSheet> RoadSheets { get; set; }

       
        private RoadSheetListResponse(bool success, string message, IEnumerable<RoadSheet> roadSheets) : base(success, message)
        {
            RoadSheets = roadSheets;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadSheetListResponse(IEnumerable<RoadSheet> roadSheets) : this(true, string.Empty, roadSheets)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSheetListResponse(string message) : this(false, message, null)
        { }

    }
}
