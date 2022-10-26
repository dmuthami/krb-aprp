using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services.Communication
{
    public class WorkplanUploadResponse : BaseResponse
    {
        public WorkplanUpload WorkplanUpload { get; set; }
        public WorkplanUploadResponse(bool success, string message, WorkplanUpload workplanUpload) : base(success, message)
        {
            WorkplanUpload = workplanUpload;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="workplanUpload">Saved road.</param>
        /// <returns>Response.</returns>
        public WorkplanUploadResponse(WorkplanUpload workplanUpload) : this(true, string.Empty, workplanUpload)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkplanUploadResponse(string message) : this(false, message, null)
        { }
    }
}
