using APRP.Web.Domain.Models;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Complaint> Complaints { get;set; }

        public IEnumerable<Road> Roads { get; set; }

        public IEnumerable<ARICSData> aRICSDatas { get; set; }

        public BudgetCeilingHeader BudgetCeilingHeader { get; set; }

        public double BudgetCeilingAmount { get; set; }

        public long ARICEDRoadsCount { get; set; }

        public long AllRoadsCount { get; set; }

        public long AllRoadsWithoutARICS { get; set; }

        public Dictionary<string,string> KenhaStatistics { get; set; }

        public Dictionary<string, string> KeRRaStatistics { get; set; }

        public Dictionary<string, string> KuRAStatistics { get; set; }

        public Dictionary<string, string> CountiesStatistics { get; set; }

        public Dictionary<string, string> KRBRoadRequestsStatistics { get; set; }

        public long KRBRoadRequestsCount { get; set; }

        public BudgetCeilingComputation BudgetCeilingComputation { get; set; }
        public double TotalCGCeilingBudget { get; set; } = 0d;
        public double TotalRACeilingBudget { get; set; } = 0d;

        public double TotalDisbursedFunds { get; set; } = 0d;

        public double TotalDisbursementBalance { get; set; } = 0d;

        public double TotalAuthorityCeilingBudget { get; set; } = 0d;

        public double TotalAuthorityDisbursedFunds { get; set; } = 0d;

        public double TotalAuthorityBudgetBalance { get; set; } = 0d;

        public FinancialYear FinancialYear { get; set; }

    }
}
