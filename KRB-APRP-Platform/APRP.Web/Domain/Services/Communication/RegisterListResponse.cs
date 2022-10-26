using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class RegisterListResponse : BaseResponse
    {
        public IList<UserListViewModel> UserListViewModel { get; set; }


        private RegisterListResponse(bool success, string message, IList<UserListViewModel> userListViewModel) : base(success, message)
        {
            UserListViewModel = userListViewModel;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RegisterListResponse(IList<UserListViewModel> userListViewModel) : this(true, string.Empty, userListViewModel)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RegisterListResponse(string message) : this(false, message, null)
        { }
    }
}
