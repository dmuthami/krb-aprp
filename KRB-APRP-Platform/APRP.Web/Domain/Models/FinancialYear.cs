using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models
{
    public enum IsCurrent
    {
        Not_Current = 0,
        Current = 1
    }

    public enum Revision
    {
        Initial = 0,
        Rev_1 = 1,
        Rev_2 = 2,
        Rev_3 = 3,
        Rev_4 = 4,
        Rev_5 = 5,
        Rev_6 = 6,
        Rev_7 = 7,
        Rev_8 = 8,
        Rev_9 = 9,
        Rev_10 = 10
    }

    public class FinancialYear
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(70)]
        public string Summary { get; set; }

        public IsCurrent IsCurrent { get; set; }

        public Revision Revision { get; set; }

        public ICollection<ItemActivityUnitCostRate> ItemActivityUnitCostRates { get; set; }

        public ICollection<BudgetCeilingComputation> BudgetCeilingComputations { get; set; }

        [Display(Name = "Calendar Year")]
        [ForeignKey("ARICSYear")]
        public Nullable<int> ARICSYearId { get; set; }
        public virtual ARICSYear ARICSYear { get; set; }

    }
}

