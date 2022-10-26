using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class BudgetVoteViewModel
    {
        public BudgetCeilingComputation BudgetCeilingComputation { get; set; }

        public IEnumerable<BudgetCeilingComputation> BudgetCeilingComputations { get; set; }

        public FinancialYear FinancialYear { get; set; }

        public string Referer { get; set; }
    }   
}
