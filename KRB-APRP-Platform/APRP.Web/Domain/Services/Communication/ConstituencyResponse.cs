using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ConstituencyResponse : BaseResponse
    {
        public Constituency Constituency;
        private ConstituencyResponse(bool success, string message, Constituency constituency) : base(success, message)
        {
            Constituency = constituency;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="constituency">Saved county.</param>
        /// <returns>Response.</returns>
        public ConstituencyResponse(Constituency constituency) : this(true, string.Empty, constituency) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public ConstituencyResponse(string message) : this(false, message, null) { }
    }
}
