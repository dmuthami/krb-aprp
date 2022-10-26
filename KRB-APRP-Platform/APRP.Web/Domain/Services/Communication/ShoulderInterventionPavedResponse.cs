using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ShoulderInterventionPavedResponse : BaseResponse
    {
        public ShoulderInterventionPaved ShoulderInterventionPaved { get; set; }


        private ShoulderInterventionPavedResponse(bool success, string message, ShoulderInterventionPaved shoulderInterventionPaved) : base(success, message)
        {
            ShoulderInterventionPaved = shoulderInterventionPaved;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ShoulderInterventionPavedResponse(ShoulderInterventionPaved shoulderInterventionPaved) : this(true, string.Empty, shoulderInterventionPaved)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ShoulderInterventionPavedResponse(string message) : this(false, message, null)
        { }
    }
}
