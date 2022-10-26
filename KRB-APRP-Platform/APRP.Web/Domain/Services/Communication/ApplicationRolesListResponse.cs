using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ApplicationRolesListResponse : BaseResponse
    {
        public IEnumerable<ApplicationRole> ApplicationRole { get; set; }


        private ApplicationRolesListResponse(bool success, string message, IEnumerable<ApplicationRole> applicationRole) : base(success, message)
        {
            ApplicationRole =applicationRole;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ApplicationRolesListResponse(IEnumerable<ApplicationRole> applicationRole) : this(true, string.Empty, applicationRole)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ApplicationRolesListResponse(string message) : this(false, message, null)
        { }
    }
}
