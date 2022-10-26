using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ReleaseResponse : BaseResponse
    {
        public Release Release { get; set; }


        private ReleaseResponse(bool success, string message, Release release) : base(success, message)
        {
            Release = release;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="Release">Saved Release.</param>
        /// <returns>Response.</returns>
        public ReleaseResponse(Release release) : this(true, string.Empty, release)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ReleaseResponse(string message) : this(false, message, null)
        { }
    }
}
