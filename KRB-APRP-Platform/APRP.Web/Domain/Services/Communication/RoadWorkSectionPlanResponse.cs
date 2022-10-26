using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadWorkSectionPlanResponse : BaseResponse
    {
        public RoadWorkSectionPlan RoadWorkSectionPlan;
        private RoadWorkSectionPlanResponse(bool success, string message, RoadWorkSectionPlan roadWorkSectionPlan) : base(success, message)
        {
            RoadWorkSectionPlan = roadWorkSectionPlan;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="roadWorkSectionPlan">Saved road work plan for section.</param>
        /// <returns>Response.</returns>
        public RoadWorkSectionPlanResponse(RoadWorkSectionPlan roadWorkSectionPlan) : this(true, string.Empty, roadWorkSectionPlan) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public RoadWorkSectionPlanResponse(string message) : this(false, message, null) { }
    }
}
