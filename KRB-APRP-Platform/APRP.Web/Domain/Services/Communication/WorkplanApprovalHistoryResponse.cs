using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkplanApprovalHistoryResponse : BaseResponse
    {
        public WorkplanApprovalHistory WorkplanApprovalHistory;
        private WorkplanApprovalHistoryResponse(bool success, string message, WorkplanApprovalHistory workplanApprovalHistory) : base(success, message)
        {
            WorkplanApprovalHistory = workplanApprovalHistory;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="workplanApprovalHistory">Saved county.</param>
        /// <returns>Response.</returns>
        public WorkplanApprovalHistoryResponse(WorkplanApprovalHistory workplanApprovalHistory) : this(true, string.Empty, workplanApprovalHistory) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public WorkplanApprovalHistoryResponse(string message) : this(false, message, null) { }
    }
}
