using APRP.Web.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models.History
{
    public class ARICSApprovalh : ARICSApprovalAbstract
    {
        [Column("id")]
        public long ID { get; set; }

        [Column("aricsapprovalid")]
        public long ARICSApprovalId { get; set; }

        [Column("aricsapprovallevelid")]
        public long ARICSApprovalLevelId { get; set; }
        public ARICSApprovalLevel ARICSApprovalLevel { get; set; }
    }
}
