using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ItemActivityUnitCost
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(10)]
        public string ItemCode { get; set; }

        [Required]
        public string SubItemCode { get; set; }

        [Required]
        public string SubSubItemCode { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string OverheadRoutineImprovement { get; set; }

        [Required]
        public string UnitCode { get; set; }

        [Required]
        public string UnitMeasure { get; set; }

        [Required]
        public string UnitDescription { get; set; }

        public double PlannedCost { get; set; }

        [Required]
        public long ItemActivityGroupId { get; set; }
        public ItemActivityGroup ItemActivityGroup { get; set; }

        public ICollection<ItemActivityUnitCostRate> ItemActivityUnitCostRates { get; set; }

    }
}
