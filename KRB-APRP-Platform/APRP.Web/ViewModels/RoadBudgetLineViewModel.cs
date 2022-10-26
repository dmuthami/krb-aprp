using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class RoadBudgetLineViewModel
    {
        public RoadWorkBudgetHeader RoadWorkBudgetHeader { get; set; }
        public IEnumerable<RoadWorkBudgetLine> RoadWorkBudgetLines { get; set; }

        public BudgetCeiling BudgetCeiling { get; set; }


    }
}
