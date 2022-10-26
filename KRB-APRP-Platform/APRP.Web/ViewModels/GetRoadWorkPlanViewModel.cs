using APRP.Web.Domain.Models;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.ViewModels
{
    public class GetRoadWorkPlanViewModel
    {
        public Road Road { get; set; }
        public RoadWorkSectionPlan RoadWorkSectionPlan { get; set; }
        public IEnumerable<Road> Roads { get; set; }

        public IEnumerable<RoadWorkSectionPlan> RoadWorkSectionPlans { get; set; }

        public IEnumerable<ARICSData> AricsData { get; set; }

        public double budgetPlanTotal { get; set; }
        public double budgetPlanTotalOthers { get; set; }
        public double budgetUtilized { get; set; }

        public double RoadArics { get; set; }

        public RoadWorkBudgetHeader RoadWorkBudgetHeader { get; set; }


        public BudgetCeiling BudgetCeiling { get; set; }

        public Authority Authority { get; set; }
        public WorkplanApprovalBatch WorkplanApprovalBatch { get; set; }
        public string approvalAuthority { get; set; }

        public FinancialYear FinancialYear { get; set; }
        public List<ARICSData> WorkplanRoads { get; set; }

        public List<ARICSData> WorkplanR2000Roads { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public SubmitNewWorkplanModel SubmitNewWorkplanModel { get; set; }
        public int TotalWorkplans { get; set; }
        public int ApprovedWorkplans { get; set; }

    }
}
