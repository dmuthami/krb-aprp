using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class PlanActivityPBCResponse : BaseResponse
    {
        public PlanActivityPBC PlanActivityPBC { get; set; }
        public PlanActivityPBCResponse(bool success, string message, PlanActivityPBC planActivityPBC) : base(success, message)
        {
            PlanActivityPBC = planActivityPBC;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="planActivityPBC">Saved road.</param>
        /// <returns>Response.</returns>
        public PlanActivityPBCResponse(PlanActivityPBC planActivityPBC) : this(true, string.Empty, planActivityPBC)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public PlanActivityPBCResponse(string message) : this(false, message, null) 
        { }
    }
}
