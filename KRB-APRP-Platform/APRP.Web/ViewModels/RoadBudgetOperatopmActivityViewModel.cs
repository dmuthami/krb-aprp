using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class RoadBudgetOperatopmActivityViewModel
    {
        public RoadWorkBudgetHeader RoadWorkBudgetHeader { get; set; }
        public IEnumerable<RoadWorkOperationalActivitiesBudget> RoadWorkOperationalActivitiesBudgets { get; set; }

        public BudgetCeiling BudgetCeiling { get; set; }

    }
}
