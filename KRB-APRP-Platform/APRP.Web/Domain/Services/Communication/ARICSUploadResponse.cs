using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ARICSUploadResponse : BaseResponse
    {
        public ARICSUpload ARICSUpload { get; set; }


        private ARICSUploadResponse(bool success, string message, ARICSUpload aRICSUpload) : base(success, message)
        {
            ARICSUpload = aRICSUpload;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ARICSUploadResponse(ARICSUpload aRICSUpload) : this(true, string.Empty, aRICSUpload)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ARICSUploadResponse(string message) : this(false, message, null)
        { }
    }
}
