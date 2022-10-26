using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ARICSUploadListResponse : BaseResponse
    {
        public IEnumerable<ARICSUpload> ARICSUpload { get; set; }


        private ARICSUploadListResponse(bool success, string message, IEnumerable<ARICSUpload> aRICSUpload) : base(success, message)
        {
            ARICSUpload = aRICSUpload;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ARICSUploadListResponse(IEnumerable<ARICSUpload> aRICSUpload) : this(true, string.Empty, aRICSUpload)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ARICSUploadListResponse(string message) : this(false, message, null)
        { }
    }
}
