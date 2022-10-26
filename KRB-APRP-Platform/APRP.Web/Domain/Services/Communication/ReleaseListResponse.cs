using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ReleaseListResponse : BaseResponse
    {
        public IEnumerable<Release> Release { get; set; }


        private ReleaseListResponse(bool success, string message, IEnumerable<Release> release) : base(success, message)
        {
            Release =release;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ReleaseListResponse(IEnumerable<Release> release) : this(true, string.Empty, release)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ReleaseListResponse(string message) : this(false, message, null)
        { }
    }
}
