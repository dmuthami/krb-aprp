using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class AWRPViewModel
    {
        public FinancialYear FinancialYear { get; set; }
        public Authority Authority { get; set; }
        public BudgetCeilingHeader BudgetCeilingHeader { get; set; }
        public BudgetCeiling BudgetCeiling { get; set; }

        public List<RoadWorkSectionPlan> RoadWorkSectionPlans { get; set; }
        public List<AdminOperationalActivity> AdminOperationalActivities { get; set; }

        public List<Contract> Contracts { get; set; }
    }
}
