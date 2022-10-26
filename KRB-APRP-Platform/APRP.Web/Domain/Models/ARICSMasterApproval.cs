using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models
{
    public class ARICSMasterApproval
    {
        [Column("id")]
        public long ID { get; set; }

        [Column("batchno")]
        public string BatchNo { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Display(Name = "Authority ID")]
        [Column("authority_id")]
        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }

        [Display(Name = "ARICSYear ID")]
        [Column("arics_year_id")]
        public int ARICSYearId { get; set; }
        public ARICSYear ARICSYear { get; set; }

        public virtual ICollection<ARICSBatch> ARICSBatchs { get; set; }

    }
}
