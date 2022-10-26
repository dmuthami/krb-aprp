using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class AdminOperationalActivityResponse : BaseResponse
    {
        public AdminOperationalActivity AdminOperationalActivity { get; set; }
        private AdminOperationalActivityResponse(bool success, string message, AdminOperationalActivity adminOperationalActivity) : base(success, message)
        {
            AdminOperationalActivity = adminOperationalActivity;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="adminOperationalActivity">Saved AdminOperationalActivity.</param>
        /// <returns>Response.</returns>
        public AdminOperationalActivityResponse(AdminOperationalActivity adminOperationalActivity) : this(true, string.Empty, adminOperationalActivity) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public AdminOperationalActivityResponse(string message) : this(false, message, null) { }
    }

}
