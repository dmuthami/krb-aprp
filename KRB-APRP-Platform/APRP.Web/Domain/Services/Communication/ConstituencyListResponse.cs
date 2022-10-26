using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ConstituencyListResponse : BaseResponse
    {
        public IEnumerable<Constituency> ConstituencyList;
        private ConstituencyListResponse(bool success, string message, IEnumerable<Constituency> constituency) : base(success, message)
        {
            ConstituencyList = constituency;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="constituency">Saved county.</param>
        /// <returns>Response.</returns>
        public ConstituencyListResponse(IEnumerable<Constituency> constituency) : this(true, string.Empty, constituency) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public ConstituencyListResponse(string message) : this(false, message, null) { }
    }
}
