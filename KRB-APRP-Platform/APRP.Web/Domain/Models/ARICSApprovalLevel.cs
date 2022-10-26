using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ARICSApprovalLevel
    {
        [Key]
        public long ID { get; set; }
        public int Status { get; set; }

        [StringLength(70), Display(Name = "Approval Level")]
        public string Role { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }
        public int AuthorityType { get; set; }
        public int Order { get; set; }
        public ICollection<ARICSApproval> ARICSApprovals { get; set; }
    }
}
