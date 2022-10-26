using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class UploadResponse : BaseResponse
    {
        public Upload Upload { get; set; }


        private UploadResponse(bool success, string message, Upload upload) : base(success, message)
        {
            Upload = upload;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public UploadResponse(Upload upload) : this(true, string.Empty, upload)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public UploadResponse(string message) : this(false, message, null)
        { }
    }
}
