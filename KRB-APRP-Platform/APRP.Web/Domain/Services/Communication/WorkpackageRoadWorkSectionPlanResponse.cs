using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkpackageRoadWorkSectionPlanResponse : BaseResponse
    {
        public WorkpackageRoadWorkSectionPlan WorkpackageRoadWorkSectionPlan { get; set; }


        private WorkpackageRoadWorkSectionPlanResponse(bool success, string message, WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan) : base(success, message)
        {
            WorkpackageRoadWorkSectionPlan = workpackageRoadWorkSectionPlan;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="workpackageRoadWorkSectionPlan">Saved road.</param>
        /// <returns>Response.</returns>
        public WorkpackageRoadWorkSectionPlanResponse(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan) : this(true, string.Empty, workpackageRoadWorkSectionPlan)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkpackageRoadWorkSectionPlanResponse(string message) : this(false, message, null)
        { }
    }
}
