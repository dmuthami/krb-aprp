namespace APRP.Web.Domain.Models
{
    public class ARICSYear
    {
        public int ID { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ARICSMasterApproval> ARICSMasterApprovals { get; set; }
        public virtual FinancialYear FinancialYear { get; set; }

    }
}
