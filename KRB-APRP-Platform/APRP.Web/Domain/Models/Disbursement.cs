using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Disbursement
    {
        public long ID { get; set; }

        public double Amount { get; set; }

        [Display(Name = "Financial Year")]
        public long FinancialYearId { get; set; }

        [Display(Name = "Financial Year")]
        public FinancialYear FinancialYear { get; set; }

        [Display(Name = "DisbursementTranche ID")]
        public long DisbursementTrancheId { get; set; }
        public DisbursementTranche DisbursementTranche { get; set; }

        [Display(Name = "Authority ID")]
        public long AuthorityId { get; set; }
        [Display(Name = "Authority ID")]
        public Authority Authority { get; set; }

        [Display(Name = "BudgetCeilingComputation ID")]
        public long BudgetCeilingComputationId { get; set; }
        public BudgetCeilingComputation BudgetCeilingComputation { get; set; }

        //public  ICollection<Release> Releases { get; set; }

        public IList<DisbursementRelease> DisbursementReleases { get; set; }
    }
}
