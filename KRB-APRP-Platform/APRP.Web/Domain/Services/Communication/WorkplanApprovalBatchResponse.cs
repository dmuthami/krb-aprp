using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkplanApprovalBatchResponse : BaseResponse
    {
        public WorkplanApprovalBatch WorkplanApprovalBatch { get; set; }
        public WorkplanApprovalBatchResponse(bool success, string message, WorkplanApprovalBatch workplanApprovalBatch) : base(success, message)
        {
            WorkplanApprovalBatch = workplanApprovalBatch;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="workplanApprovalBatch">Saved road.</param>
        /// <returns>Response.</returns>
        public WorkplanApprovalBatchResponse(WorkplanApprovalBatch workplanApprovalBatch) : this(true, string.Empty, workplanApprovalBatch)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkplanApprovalBatchResponse(string message) : this(false, message, null) 
        { }
    }
}
