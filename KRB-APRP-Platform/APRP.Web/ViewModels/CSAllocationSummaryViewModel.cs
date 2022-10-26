using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class CSAllocationSummaryViewModel
    {
        public Authority Authority { get; set; }
        public long AuthorityId { get; set; }

        public double Amount { get; set; }

        public FinancialYear FinancialYear { get; set; }
        public long FinancialYearId { get; set; }
        public int Count { get; set; }
        public double TotalDisbursement { get; set; }

        public long BudgetCeilingComputationId { get; set; }
        public BudgetCeilingComputation BudgetCeilingComputation { get; set; }
    }
}
