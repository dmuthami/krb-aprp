using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class CSAllocation
    {
        public long ID { get; set; }

        public double Amount { get; set; }

        [Display(Name = "Authority ID")]
        public long AuthorityId { get; set; }
        [Display(Name = "Authority ID")]
        public Authority Authority { get; set; }

        [Display(Name = "BudgetCeilingComputation ID")]
        public long BudgetCeilingComputationId { get; set; }
        public BudgetCeilingComputation BudgetCeilingComputation { get; set; }
    }
}
