using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkPlanPackageResponse : BaseResponse
    {
        public WorkPlanPackage WorkPlanPackage { get; set; }


        private WorkPlanPackageResponse(bool success, string message, WorkPlanPackage workPlanPackage) : base(success, message)
        {
            WorkPlanPackage = workPlanPackage;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="fundType">Saved road.</param>
        /// <returns>Response.</returns>
        public WorkPlanPackageResponse(WorkPlanPackage workPlanPackage) : this(true, string.Empty, workPlanPackage)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkPlanPackageResponse(string message) : this(false, message, null)
        { }
    }
}
