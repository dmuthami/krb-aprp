using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class DisbursementViewModel
    {
        public Disbursement Disbursement { get; set; }

        public IEnumerable<Disbursement> Disbursements { get; set; }

        public IList<DisbursementSummaryViewModel> DisbursementSummaryViewModels { get; set; }

        public bool IsDisbursementSummaryViewModels { get; set; }

        public IList<DisbursementSummaryViewModel> DisbursementSummaryByBudgetCeilingViewModels { get; set; }
        public string Referer { get; set; }
        public FinancialYear FinancialYear { get; set; }
        public IEnumerable<AllocationCodeUnit> AllocationCodeUnits { get; set; }
        public IEnumerable<BudgetCeiling> BudgetCeilings { get; set; }

        public IEnumerable<DisbursementCodeList> DisbursementCodeLists { get; set; }

        public DisbursementCodeList DisbursementCodeList { get; set; }


    }
}
