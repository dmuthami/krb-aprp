using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkplanApprovalHistory
    {
        [Key]
        public long ID { get; set; }
        public string ApprovalBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string ApprovalComment { get; set; }
        public int approvalStatus { get; set; }
        public string documentLink { get; set; }
        public long WorkplanApprovalBatchId { get; set; }
        public WorkplanApprovalBatch WorkplanApprovalBatch { get; set; }
    }
}
