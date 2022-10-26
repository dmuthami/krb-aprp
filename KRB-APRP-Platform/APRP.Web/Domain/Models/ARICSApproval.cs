using APRP.Web.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models
{
    public class ARICSApproval : ARICSApprovalAbstract
    {
        [Column("id")]
        public long ID { get; set; }

        [Display(Name = "ARICS Approval ID")]
        [Column("aricsapprovallevelid")]
        public long ARICSApprovalLevelId { get; set; }
        public ARICSApprovalLevel ARICSApprovalLevel { get; set; }
    }
}
