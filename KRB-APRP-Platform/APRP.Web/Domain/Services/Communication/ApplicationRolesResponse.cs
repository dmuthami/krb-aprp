using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ApplicationRolesResponse : BaseResponse
    {
        public ApplicationRole ApplicationRole { get; set; }


        private ApplicationRolesResponse(bool success, string message, ApplicationRole applicationRole) : base(success, message)
        {
            ApplicationRole = applicationRole;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ApplicationRolesResponse(ApplicationRole applicationRole) : this(true, string.Empty, applicationRole)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ApplicationRolesResponse(string message) : this(false, message, null)
        { }
    }
}
