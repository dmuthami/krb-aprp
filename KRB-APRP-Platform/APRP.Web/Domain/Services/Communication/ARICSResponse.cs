using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ARICSResponse : BaseResponse
    {
        public ARICS ARICS { get; set; }


        private ARICSResponse(bool success, string message, ARICS aRICS) : base(success, message)
        {
            ARICS = aRICS;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="ARICS">Saved ARICS.</param>
        /// <returns>Response.</returns>
        public ARICSResponse(ARICS aRICS) : this(true, string.Empty, aRICS)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ARICSResponse(string message) : this(false, message, null)
        { }
    }
}
