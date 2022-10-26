using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadConditionCodeUnitResponse : BaseResponse
    {
        public RoadConditionCodeUnit RoadConditionCodeUnit { get; set; }


        private RoadConditionCodeUnitResponse(bool success, string message, RoadConditionCodeUnit roadConditionCodeUnit) : base(success, message)
        {
            RoadConditionCodeUnit = roadConditionCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RoadConditionCodeUnit">Saved RoadConditionCodeUnit.</param>
        /// <returns>Response.</returns>
        public RoadConditionCodeUnitResponse(RoadConditionCodeUnit roadConditionCodeUnit) : this(true, string.Empty, roadConditionCodeUnit)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadConditionCodeUnitResponse(string message) : this(false, message, null)
        { }
    }
}
