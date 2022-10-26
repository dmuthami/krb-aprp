using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadWorkBudgetHeader
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long FinancialYearId { get; set; }
        public FinancialYear FinancialYear { get; set; }

        public Authority Authority { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        public string Summary { get; set; }

        public IEnumerable<RoadWorkBudgetLine> RoadWorkBudgetLines { get; set; }
        public IEnumerable<RoadWorkOperationalActivitiesBudget> RoadWorkOperationalActivitiesBudgets { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmissionDate { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
