using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class CSAllocationViewModel
    {
        public CSAllocation CSAllocation { get; set; }
        public IEnumerable<CSAllocation> CSAllocations { get; set; }
        public bool IsDisbursementSummaryViewModels { get; set; }
        public string Referer { get; set; }
        public FinancialYear FinancialYear { get; set; }
        public IEnumerable<AllocationCodeUnit> AllocationCodeUnits { get; set; }
        public IEnumerable<BudgetCeiling> BudgetCeilings { get; set; }
    }
}
