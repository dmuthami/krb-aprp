using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class PlanActivity
    {
        [Key]
        public long ID { get; set; }
        public long RoadWorkSectionPlanId { get; set; }
        public RoadWorkSectionPlan RoadWorkSectionPlan { get; set; }
        public long ItemActivityUnitCostId { get; set; }
        public ItemActivityUnitCost ItemActivityUnitCost { get; set; }
        [Display(Name = "Start Ch.")]
        public double StartChainage { get; set; }
        [Display(Name = "End Ch.")]
        public double EndChainage { get; set; }

        public long? TechnologyId { get; set; }
        public Technology Technology { get; set; }
        public decimal Quantity { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        [Display(Name = "Package Quantity")]
        public decimal PackageQuantity { get; set; }
        [Display(Name = "Package Amount")]
        public double PackageAmount { get; set; }

        public double BidRate { get; set; }
        public double BillItemAmount { get; set; }

        [Display(Name = "Variation Quantity")]
        public int VariationQuantity { get; set; }
        [Display(Name = "Variation Amount")]
        public int VariationAmount { get; set; }
        [Display(Name = "Labour %")]
        public float  LabourPercent { get; set; }

        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }

        [Display(Name = "Previous Quantity")]
        public int PreviousQuantity { get; set; }
        [Display(Name = "Quantity Done To Date")]
        public int DoneTodateQuantity { get; set; }
        [Display(Name ="This Quantity")]
        public int ThisQuantity { get; set; }
        public IEnumerable<PackageProgressEntry> PackageProgressEntries { get; }

    }
}
