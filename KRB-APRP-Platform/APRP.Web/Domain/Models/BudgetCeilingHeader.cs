namespace APRP.Web.Domain.Models
{
    public class BudgetCeilingHeader
    {
        public long ID { get; set; }
        public FinancialYear FinancialYear { get; set; }
        public long FinancialYearId { get; set; }
        public double TotalAmount { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public IEnumerable<BudgetCeiling> BudgetCeilings { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmissionDate { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int isCurrent { get; set; }

    }
}
