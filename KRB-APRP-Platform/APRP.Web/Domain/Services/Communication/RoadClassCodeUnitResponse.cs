using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadClassCodeUnitResponse : BaseResponse
    {
        public RoadClassCodeUnit RoadClassCodeUnit { get; set; }


        private RoadClassCodeUnitResponse(bool success, string message, RoadClassCodeUnit roadClassCodeUnit) : base(success, message)
        {
            RoadClassCodeUnit = roadClassCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RoadClassCodeUnit">Saved RoadClassCodeUnit.</param>
        /// <returns>Response.</returns>
        public RoadClassCodeUnitResponse(RoadClassCodeUnit roadClassCodeUnit) : this(true, string.Empty, roadClassCodeUnit)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadClassCodeUnitResponse(string message) : this(false, message, null)
        { }
    }
}
