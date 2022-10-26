using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class ARICSDataListResponse : BaseResponse
    {
        public IEnumerable<ARICSData> ARICSData { get; set; }


        private ARICSDataListResponse(bool success, string message, IEnumerable<ARICSData> aRICSData) : base(success, message)
        {
            ARICSData =aRICSData;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ARICSDataListResponse(IEnumerable<ARICSData> aRICSData) : this(true, string.Empty, aRICSData)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ARICSDataListResponse(string message) : this(false, message, null)
        { }
    }
}
