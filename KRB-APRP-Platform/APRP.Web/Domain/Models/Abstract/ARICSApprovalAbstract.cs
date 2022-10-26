using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models.Abstract
{
    public abstract class ARICSApprovalAbstract
    {
        [Display(Name="User Name")]
        [Column("username")]
        public string UserName { get; set; }

        [Column("comment")]
        public string Comment { get; set; }

        [Display(Name = "ARICS Master Approval ID")]
        [Column("aricsmasterapprovalid")]
        public long ARICSMasterApprovalId { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Display(Name = "Updated By")]
        [Column("updatedby")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Updated Date")]
        [Column("updateddate")]
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Created By")]
        [Column("createdby")]
        public string CreatedBy { get; set; }

        [Display(Name = "Creation Date")]
        [Column("creationdate")]
        public DateTime CreationDate { get; set; }

        [Column("beginlifespan")]
        public Nullable<DateTime> BeginLifeSpan { get; set; }

        [Column("endlifespan")]
        public Nullable<DateTime> EndLifeSpan { get; set; }
    }
}
