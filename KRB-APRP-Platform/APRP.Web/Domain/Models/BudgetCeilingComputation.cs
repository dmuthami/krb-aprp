using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class BudgetCeilingComputation
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(10), Display(Name = "Code No.")]
        public string Code { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Budget Ceiling Item")]
        public string Name { get; set; }

        public double Amount { get; set; }

        [Display(Name = "Financial Year ID")]
        public long FinancialYearId { get; set; }
        [Display(Name = "Financial Year")]
        public FinancialYear FinancialYear { get; set; }

        [Display(Name = "Authority ID")]
        public long AuthorityId { get; set; }
        [Display(Name = "Authority")]
        public Authority Authority { get; set; }

        public ICollection<Disbursement> Disbursements { get; set; }

        public virtual ICollection<CSAllocation> CSAllocations { get; set; }
    }
}
