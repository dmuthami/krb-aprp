using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class DisbursementSummaryViewModel
    {
        public Authority Authority { get; set; }
        public long AuthorityId { get; set; }

        public double Percent { get; set; }

        public double AnnualCeiling { get; set; }

        public FinancialYear FinancialYear { get; set; }
        public long FinancialYearId { get; set; }
        public int Count { get; set; }
        public double TotalDisbursement { get; set; }

        public double DisbursementTrancheAmount { get; set; }

        public long DisbursementTrancheId { get; set; }
        public double PercentOfCeiling { get; set; }

        public long BudgetCeilingComputationId { get; set; }
        public BudgetCeilingComputation BudgetCeilingComputation { get; set; }
    }
}
