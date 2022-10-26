using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ARICSListResponse : BaseResponse
    {
        public IEnumerable<ARICS> ARICS { get; set; }


        private ARICSListResponse(bool success, string message, IEnumerable<ARICS> aRICS) : base(success, message)
        {
            ARICS =aRICS;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ARICSListResponse(IEnumerable<ARICS> aRICS) : this(true, string.Empty, aRICS)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ARICSListResponse(string message) : this(false, message, null)
        { }
    }
}
