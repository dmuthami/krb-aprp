using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class ARICSDataResponse : BaseResponse
    {
        public ARICSData ARICSData { get; set; }


        private ARICSDataResponse(bool success, string message, ARICSData aRICSData) : base(success, message)
        {
            ARICSData = aRICSData;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="ARICSData">Saved ARICSData.</param>
        /// <returns>Response.</returns>
        public ARICSDataResponse(ARICSData aRICSData) : this(true, string.Empty, aRICSData)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ARICSDataResponse(string message) : this(false, message, null)
        { }
    }
}
