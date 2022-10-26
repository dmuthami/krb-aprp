using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Authority
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        [Required]
        [StringLength(70)]
        public string Name { get; set; }

        public int? Type { get; set; }

        public string? Number { get; set; }

        public ICollection<RevenueCollectionCodeUnit> RevenueCollectionCodeUnits { get; set; }

        public ICollection<ItemActivityUnitCostRate> ItemActivityUnitCostRates { get; set; }

        public ICollection<Region> Regions { get; set; }

        public ICollection<BudgetCeilingComputation> BudgetCeilingComputations { get; set; }

        public virtual ICollection<CSAllocation> CSAllocations { get; set; }

        public virtual ICollection<ARICSMasterApproval> ARICSMasterApprovals { get; set; }
    }
}
