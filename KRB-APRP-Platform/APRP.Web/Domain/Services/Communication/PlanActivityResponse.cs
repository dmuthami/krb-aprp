using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class PlanActivityResponse : BaseResponse
    {
        public PlanActivity PlanActivity { get; set; }
        public PlanActivityResponse(bool success, string message, PlanActivity planActivity) : base(success, message)
        {
            PlanActivity = planActivity;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="planActivity">Saved road.</param>
        /// <returns>Response.</returns>
        public PlanActivityResponse(PlanActivity planActivity) : this(true, string.Empty, planActivity)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public PlanActivityResponse(string message) : this(false, message, null) 
        { }
    }
}
