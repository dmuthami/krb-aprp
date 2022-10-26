using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ShoulderInterventionPavedListResponse : BaseResponse
    {
        public IEnumerable<ShoulderInterventionPaved> ShoulderInterventionPaved { get; set; }


        private ShoulderInterventionPavedListResponse(bool success, string message, IEnumerable<ShoulderInterventionPaved> shoulderInterventionPaved) : base(success, message)
        {
            ShoulderInterventionPaved = shoulderInterventionPaved;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ShoulderInterventionPavedListResponse(IEnumerable<ShoulderInterventionPaved> shoulderInterventionPaved) : this(true, string.Empty, shoulderInterventionPaved)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ShoulderInterventionPavedListResponse(string message) : this(false, message, null)
        { }
    }
}
