using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadConditionCodeUnitListResponse : BaseResponse
    {
        public IEnumerable<RoadConditionCodeUnit> RoadConditionCodeUnit { get; set; }


        private RoadConditionCodeUnitListResponse(bool success, string message, IEnumerable<RoadConditionCodeUnit> roadConditionCodeUnit) : base(success, message)
        {
            RoadConditionCodeUnit =roadConditionCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadConditionCodeUnitListResponse(IEnumerable<RoadConditionCodeUnit> roadConditionCodeUnit) : this(true, string.Empty, roadConditionCodeUnit)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadConditionCodeUnitListResponse(string message) : this(false, message, null)
        { }
    }
}
