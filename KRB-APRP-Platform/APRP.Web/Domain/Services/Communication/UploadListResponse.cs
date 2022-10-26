using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class UploadListResponse : BaseResponse
    {
        public IEnumerable<Upload> Upload { get; set; }


        private UploadListResponse(bool success, string message, IEnumerable<Upload> upload) : base(success, message)
        {
            Upload = upload;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public UploadListResponse(IEnumerable<Upload> upload) : this(true, string.Empty, upload)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public UploadListResponse(string message) : this(false, message, null)
        { }
    }
}
