using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class AllocationViewModel
    {
        public BudgetCeiling BudgetCeiling { get; set; }
        public string Referer { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
