using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models
{
    public class ARICSBatch
    {
        [Key]
        [Column("id")]
        public long ID { get; set; }

        [Display(Name = "Road Section ID")]
        [Column("road_section_id")]
        public long RoadSectionId { get; set; }
        public RoadSection RoadSection { get; set; }

        [Display(Name = "ARICS Master Approval ID")]
        [Column("arics_master_approval_id")]
        public long ARICSMasterApprovalId { get; set; }
        public ARICSMasterApproval ARICSMasterApproval { get; set; }
    }
}
