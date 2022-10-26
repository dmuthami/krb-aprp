using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSheetLengthListResponse : BaseResponse
    {
        public IEnumerable<RoadSheetLength> RoadSheetLength { get; set; }


        private RoadSheetLengthListResponse(bool success, string message, IEnumerable<RoadSheetLength> roadSheetLength) : base(success, message)
        {
            RoadSheetLength =roadSheetLength;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadSheetLengthListResponse(IEnumerable<RoadSheetLength> roadSheetLength) : this(true, string.Empty, roadSheetLength)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSheetLengthListResponse(string message) : this(false, message, null)
        { }
    }
}
