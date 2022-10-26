using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class UserAccessListResponse : BaseResponse
    {
        public UserAccessList UserAccessList { get; set; }


        private UserAccessListResponse(bool success, string message, UserAccessList userAccessList) : base(success, message)
        {
            UserAccessList = userAccessList;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="UserAccessList">Saved UserAccessList.</param>
        /// <returns>Response.</returns>
        public UserAccessListResponse(UserAccessList userAccessList) : this(true, string.Empty, userAccessList)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public UserAccessListResponse(string message) : this(false, message, null)
        { }
    }
}
