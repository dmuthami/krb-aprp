using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class RoadWorkPlanSummaryViewModel
    {
        public RoadWorkBudgetHeader RoadWorkBudgetHeader { get; set; }
        public List<RoadWorkPlanViewModel> RoadWorkPlanViewModels { get; set; }

        public BudgetCeiling BudgetCeiling { get; set; }


    }
}
