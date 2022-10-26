using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadClassCodeUnitListResponse : BaseResponse
    {
        public IEnumerable<RoadClassCodeUnit> RoadClassCodeUnit { get; set; }


        private RoadClassCodeUnitListResponse(bool success, string message, IEnumerable<RoadClassCodeUnit> roadClassCodeUnit) : base(success, message)
        {
            RoadClassCodeUnit =roadClassCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadClassCodeUnitListResponse(IEnumerable<RoadClassCodeUnit> roadClassCodeUnit) : this(true, string.Empty, roadClassCodeUnit)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadClassCodeUnitListResponse(string message) : this(false, message, null)
        { }
    }
}
