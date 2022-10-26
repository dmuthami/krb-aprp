using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class UserAccessListDataResponse : BaseResponse
    {
        public IEnumerable<UserAccessList> UserAccessList { get; set; }


        private UserAccessListDataResponse(bool success, string message, IEnumerable<UserAccessList> userAccessList) : base(success, message)
        {
            UserAccessList =userAccessList;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public UserAccessListDataResponse(IEnumerable<UserAccessList> userAccessList) : this(true, string.Empty, userAccessList)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public UserAccessListDataResponse(string message) : this(false, message, null)
        { }
    }
}
